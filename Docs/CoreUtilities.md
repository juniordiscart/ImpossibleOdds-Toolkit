# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Core Utilities

## Delegates & Events

When subscribing to events in C#, a best practice is to also unsubscribe when you're done or no longer need them, e.g. when your GameObject gets destroyed. This often results in listing every method you had subscribed.

A quick way to clear the object you're done with from all events on a particular object is:

```cs
// Clear all delegates found in myEventInvokingObject from
// subscribed methods originating from myObjectToBeDoneWith.
myEventInvokingObject.PurgeDelegatesOf(myObjectToBeDoneWith);
```

In some cases, you may also want to do this for events that are defined as static, or, remove all static methods of a type registered:

```cs
// Clear all static defined delegates of type MyEventInvokingClass
// from subscribed methods linked to myObjectToBeDoneWith.
Type myEventInvokingType = typeof(MyEventInvokingClass);
myEventInvokingType.PurgeDelegatesOf(myObjectToBeDoneWith);

// Similarly, clear all static defined methods linked to MyClassToBeDoneWith
// from the delegates defined on myEventInvokingObject
myEventInvokingObject.PurgeDelegatesOf(typeof(MyTypeToBeDoneWith));
```

**Important note**: anonymous delegates subscribed to events can **not** be cleared here, as their origin cannot be tied to a specific object.

**Another note**: using `PurgeDelegatesOf()` uses reflection internally to look for any fields that inherit from the `System.Delegate` type. If you define your events with explicit `add` and `remove` implementations, then this behaviour will not work directly for that event unless it's backed by a delegate in that same class.

Some other useful methods include the `InvokeIfNotNull()` extensions, saving you of writing the `null`-check every time. These are defined for both the `System.Action`, `System.Func` and `UnityEngine.UnityAction` types, and support up to four parameters:

```cs
myEvent.InvokeIfNotNull();
```

There's also a variant that supports more than four parameters (using `params object[]`) but will perform type conversions and boxing of values, allocating memory and possibly trigger garbage collection. Use this one sparingly or in non-critical situations only!

## Enums

There are situations where you might want to display an enum value properly instead of its internal/code name. The usual way to go about it is to write a `switch`-statement and list all possibilities, but that becomes tedious the more values there are, as well as in how many places you want to display these values.

Introducing enum display names and translation keys! Using the `DisplayNameAttribute`, you can decorate your enum values with a direct display name and/or a translation key you can use to retrieve its proper translation from your localization system.

```cs
public enum MyEnum
{
	// Not interested in NONE...
	NONE,
	[DisplayName(Name="First", LocalizationKey="myEnum/first")]
	FIRST,
	[DisplayName(LocalizationKey="myEnum/second")]
	SECOND,
	[DisplayName(Name="Last")]
	LAST
}
```

Finally, these values can be retrieved using:

```cs
MyEnum value = MyEnum.First;
string name = value.DisplayName();
string locaKey = value.LocalizationKey();
```

When calling the `DisplayName()` extension method, if no name is set, it will return the result of `ToString()`. For the `LocalizationKey()` extension method, if no key is set, it will just return `string.Empty`.

## Logging

A custom logging utility to replace Unity's default way of logging. Internally, it still uses Unity's `Debug` class, but allows you to outright disable certain logging levels. This not only clears up your log file, but can also save memory allocations (and in the end, rounds of garbage collection).

```cs
Log.Info();
Log.Warning();
Log.Error();
Log.Exception();
```

These log methods allow you to format the provided string directly like with the `Debug.LogFormat()` methods.

You can set different logging levels for the player and the editor, allowing you to keep valuable info messages while testing in editor, and removing them from the player build. See the [Editor Settings](#editor-settings) section for more details on how to enable/disable these.

## Value Checking

All to frequently, there are situations where you need to check if certain values are `null` or not, or a string contains not just whitespace characters. Then these can save you some lines and time:

```cs
// Throws and ArgumentNullException when myObject is null.
myObject.ThrowIfNull();

// Variants for string.
myString.ThrowIfNullOrEmpty();
myString.ThrowIfNullOrWhitespace();

// Logs an error when myObject is null and returns true. False when not null.
myObject.LogErrorIfNull();
```

Similarly, at the end of using an object implementing the `IDisposable` interface:

```cs
myDisposable.DisposeIfNotNull();
```

[Logo]: ./Images/ImpossibleOddsLogo.png
