// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects event type names that do not follow past-tense naming conventions.
/// Domain events represent something that has already happened and should be
/// named accordingly (e.g. "OrderPlaced" instead of "PlaceOrder").
/// <para>
/// The check is a static heuristic: the event name is split on PascalCase word
/// boundaries and the <em>last word</em> is tested for known past-tense suffixes
/// (regular <c>-ed/-ied</c> endings plus a curated list of common irregular verbs
/// used in event-modeling contexts). This cannot replace human judgment, so the
/// rule emits a <see cref="EventModelRecommendationSeverity.Suggestion"/> rather
/// than a warning.
/// </para>
/// </summary>
public partial class EventNamingConventionRule : IEventModelRule
{
    /// <summary>
    /// Well-known irregular past-tense verbs whose base form would otherwise
    /// not satisfy the <c>-ed/-ied</c> suffix check. Add more as needed.
    /// </summary>
    static readonly HashSet<string> _knownIrregularPastTense = new(StringComparer.OrdinalIgnoreCase)
    {
        "built", "bought", "brought", "caught", "done", "drawn", "driven",
        "eaten", "fallen", "felt", "found", "given", "gone", "grown",
        "held", "hit", "kept", "known", "left", "lost", "made", "met",
        "paid", "put", "read", "run", "said", "seen", "sent", "set",
        "shown", "shut", "sold", "spent", "told", "taken", "taught",
        "thought", "thrown", "understood", "won", "written"
    };

    /// <inheritdoc/>
    public IEnumerable<EventModelRecommendation> Evaluate(IEnumerable<Module> modules)
    {
        foreach (var module in modules)
        {
            foreach (var recommendation in EvaluateFeatures(module.Features, module.Name, FeaturePath.Empty))
            {
                yield return recommendation;
            }
        }
    }

    static IEnumerable<EventModelRecommendation> EvaluateFeatures(IEnumerable<Feature> features, string moduleName, FeaturePath path)
    {
        foreach (var feature in features)
        {
            var featurePath = path.Append(feature.Name);
            foreach (var slice in feature.VerticalSlices)
            {
                foreach (var eventType in slice.Events)
                {
                    if (!IsPastTense(eventType.Name))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Suggestion,
                            EventModelRecommendationCategory.Naming,
                            moduleName,
                            featurePath,
                            slice.Name,
                            eventType.Name,
                            $"Event type '{eventType.Name}' does not appear to use past-tense naming.",
                            "Domain events represent facts that have already occurred. Consider renaming using past tense, e.g. 'OrderPlaced' instead of 'PlaceOrder'.");
                    }
                }
            }

            foreach (var recommendation in EvaluateFeatures(feature.Features, moduleName, featurePath))
            {
                yield return recommendation;
            }
        }
    }

    /// <summary>
    /// Checks whether a PascalCase event type name ends in a past-tense word.
    /// The name is split on PascalCase boundaries and the last word is compared
    /// against the regular <c>-ed/-ied</c> endings and a curated set of common
    /// irregular verbs.
    /// </summary>
    /// <param name="name">The PascalCase event type name to check.</param>
    /// <returns><see langword="true"/> if the last word appears to be past tense.</returns>
    static bool IsPastTense(string name)
    {
        var words = PascalCaseSplit().Split(name);
        var lastWord = words.LastOrDefault(w => w.Length > 0);
        if (lastWord is null)
        {
            return true;
        }

        return lastWord.EndsWith("ed", StringComparison.OrdinalIgnoreCase)
            || lastWord.EndsWith("ied", StringComparison.OrdinalIgnoreCase)
            || _knownIrregularPastTense.Contains(lastWord);
    }

    /// <summary>
    /// Splits a PascalCase string at each upper-case letter boundary.
    /// For example "OrderPlaced" → ["Order", "Placed"].
    /// </summary>
    [GeneratedRegex(@"(?=[A-Z])", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PascalCaseSplit();
}
