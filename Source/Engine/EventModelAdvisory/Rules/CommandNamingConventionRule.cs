// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects command names that do not follow imperative (verb-first) naming conventions.
/// Commands express user intent and should be named in the imperative mood
/// (e.g. <c>PlaceOrder</c>, <c>CancelSubscription</c>) rather than past tense
/// (e.g. <c>OrderPlaced</c>) or progressive tense (e.g. <c>PlacingOrder</c>).
/// <para>
/// The check is a static heuristic: the command name is split on PascalCase word
/// boundaries and tested for past-tense endings on the last word and progressive
/// (<c>-ing</c>) endings on the first word.
/// </para>
/// </summary>
public partial class CommandNamingConventionRule : IEventModelRule
{
    /// <summary>
    /// Common irregular past-tense verbs whose base form does not carry a <c>-ed/-ied</c> suffix.
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
                foreach (var command in slice.Commands)
                {
                    if (!IsImperative(command.Name))
                    {
                        yield return new EventModelRecommendation(
                            EventModelRecommendationSeverity.Suggestion,
                            EventModelRecommendationCategory.Naming,
                            moduleName,
                            featurePath,
                            slice.Name,
                            command.Name,
                            $"Command '{command.Name}' does not appear to use imperative naming.",
                            "Commands should express intent using an imperative verb, e.g. 'PlaceOrder' or 'CancelSubscription' rather than 'OrderPlaced' or 'PlacingOrder'.");
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
    /// Returns <see langword="true"/> when the PascalCase command name appears to be in
    /// imperative form. Returns <see langword="false"/> when the first word ends in
    /// <c>-ing</c> (progressive) or the last word is past tense (<c>-ed/-ied</c> or a
    /// known irregular).
    /// </summary>
    /// <param name="name">The PascalCase command name to check.</param>
    static bool IsImperative(string name)
    {
        var words = PascalCaseSplit().Split(name)
            .Where(w => w.Length > 0)
            .ToArray();

        if (words.Length == 0)
        {
            return true;
        }

        var firstWord = words[0];
        var lastWord = words[^1];

        if (firstWord.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !lastWord.EndsWith("ed", StringComparison.OrdinalIgnoreCase)
            && !lastWord.EndsWith("ied", StringComparison.OrdinalIgnoreCase)
            && !_knownIrregularPastTense.Contains(lastWord);
    }

    /// <summary>
    /// Splits a PascalCase string at each upper-case letter boundary.
    /// For example "PlaceOrder" → ["Place", "Order"].
    /// </summary>
    [GeneratedRegex(@"(?=[A-Z])", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PascalCaseSplit();
}
