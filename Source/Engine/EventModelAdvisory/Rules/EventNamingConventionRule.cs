// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using Cratis.DependencyInjection;

namespace Cratis.VerticalSlices.EventModelAdvisory.Rules;

/// <summary>
/// Detects event type names that do not follow past-tense naming conventions.
/// Domain events represent something that has already happened and should be
/// named accordingly (e.g. "OrderPlaced" instead of "PlaceOrder").
/// </summary>
[Singleton]
public partial class EventNamingConventionRule : IEventModelRule
{
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
        foreach (var (moduleName, path, slice) in modules.FlattenSlices())
        {
            foreach (var eventType in slice.Events.Where(e => !IsPastTense(e.Name)))
            {
                yield return new EventModelRecommendation(
                    EventModelRecommendationSeverity.Suggestion,
                    EventModelRecommendationCategory.Naming,
                    moduleName,
                    path,
                    slice.Name,
                    eventType.Name,
                    $"Event type '{eventType.Name}' does not appear to use past-tense naming.",
                    "Domain events represent facts that have already occurred. Consider renaming using past tense, e.g. 'OrderPlaced' instead of 'PlaceOrder'.");
            }
        }
    }

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

    /// <summary>Splits a PascalCase string at each upper-case letter boundary.</summary>
    [GeneratedRegex(@"(?=[A-Z])", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PascalCaseSplit();
}
