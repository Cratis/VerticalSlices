---
applyTo: "**/*.cs"
---

# 🧪 How to use Concepts

- Don't use primitives like `string`, `int`, `Guid`, etc. directly in your domain models, commands, events, queries, etc.
- Create a concept for the primitive type that encapsulates the value and provides domain-specific behavior.
- Inherit from the appropriate base class provided by Cratis Concepts, such as `ConceptAs<T>`
- `ConceptAs<T>` has a default conversion from `T` to `ConceptAs<T>` and from `ConceptAs<T>` to `T`. Do NOT add an explicit implicit operator for converting from `ConceptAs<T>` to `T` — it is already provided by the base class.
- Instead of using `null` values, add a static readonly value representing an empty or default state for the concept with appropriate naming for the context (e.g. `Empty`, `NotSet`).
