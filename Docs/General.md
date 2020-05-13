# `ImpossibleOdds`

In the top-layer namespace, you can find some general and simple utilities that don't really require a need to contain them in a separate module. It contains some nice-to-have and elegant extensions:

* Delegate purger: quick and easy way to remove an object from another object's set of delegates/events. The `PurgeDelegatesOf` method is an extension to the `object` type.

```csharp
objectWithDelegates.PurgeDelegatesOf(this);
```

* Enum display names: instead of writing switch-statements over and over again to display an enum value to your users, the `DisplayNameAttribute` can be placed over an enum value to quickly retrieve a standard name. A translation key can be provided as well. The `DisplayName` and `TranslationKey` methods are extensions to the `Enum` type.

```csharp
public enum MyEnum
{
	[DisplayName(Name="None")]
	NONE = 0,
	[DisplayName(Name="One", TranslationKey="en.one")]
	ONE = 1,
	[DisplayName(Name="Two", TranslationKey="en.two")]
	TWO = 2,
	[DisplayName(TranslationKey="en.last")]
	LAST
}

MyEnum myValue = MyEnum.TWO;
Debug.Log(string.Format("Display name: {0}, Translation key: {1}.", myValue.DisplayName(), myValue.TranslationKey()));
// Prints: Display name: Two, Translation key: en.two.
```
