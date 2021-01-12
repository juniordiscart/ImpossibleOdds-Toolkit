# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Core Utilities

These general tools can all be found by including the `ImpossibleOdds` namespace in your scripts.

In here you'll find smaller but useful utilities to speed up your programming. Quick glance of what you can expect to find here:

* Extenions for delegates and events to quickly unsubscribe all functions of an object from delegates on another object.
* Extensions for enum values, to provide them with a suitable name for display to your users.
* Extensions to check if the value is null.
* Custom logging tool to enable/disable certain levels of logging.
* Define the script execution order of your `MonoBehavior` classes using an attribute.

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

**Another note**: using `PurgeDelegatesOf` uses reflection internally to look for any fields that inherit from the `System.Delegate` type. If you define your events with explicit `add` and `remove` implementations, then this behaviour will not work directly for that event unless it's backed by a delegate in that same object.

## Enums

There are situations where you might want to display an enum value properly instead of its internal/code name. The usual way to go about it is to write a `switch`-statement and list all possibilities, but that becomes tedious the more values there are, as well as in how many places you want to display these values.

Introducing enum display names and translation keys! Using the `DisplayName` attribute, you can decorate your enum values with a direct display name and/or a translation key. The latter one you can use to retrieve its proper translation from your localization system, if any.

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

Finally, these values can be retrieved using their extension functions:

```cs
MyEnum value = MyEnum.First;
string name = value.DisplayName();
string locaKey = value.LocalizationKey();
```

When calling the `DisplayName` extension method, if no name is set, it will return the result of `ToString`. For the `LocalizationKey` extension method, if no key is set, it will just return `string.Empty`.

**Note**: don't confuse the Impossible Odds `DisplayName` attribute with the C# `DisplayName` attribute found in the `System.ComponentModel` namespace.

## Value Checking

All to frequently, there are situations where you need to check if certain values are `null` or not, or a string contains not just whitespace characters. For example checking the incoming parameters in a method.

```cs
// Throws an ArgumentNullException when myObject is null.
myObject.ThrowIfNull(nameof(myObject));

// Variants specifically for the string type.
myString.ThrowIfNullOrEmpty(nameof(myString));
myString.ThrowIfNullOrWhitespace(nameof(myString));
```

A variant, instead of throwing an exception, can also log an error in case the value is null. They also return a `bool` value in case you'd want to check if the value is valid or not.

```cs
// Logs an error when myObject is null.
myObject.LogErrorIfNull(nameof(myObject));

// Variants specifically for the string type.
myString.LogErrorIfNullOrEmpty(nameof(myString));
myString.LogErrorIfNullOrWhitespace(nameof(myString));
```

These last ones are especially useful if you want to log a unique message per value checked.

```cs
public void MyMethod(MyClassA param1, string param2)
{
	// Check if all parameters are valid.
	if (param1.LogErrorIfNull(nameof(param1)) ||
		param2.LogErrorIfNullOrEmpty(nameof(param2)))
	{
		return;
	}

	...
}
```

### Delegates

Some other useful methods include the `InvokeIfNotNull` extensions defined for instances of the `System.Action`, `System.Func` and `UnityEngine.UnityAction` types. Variants are explicitly defined for up to four parameters.

```cs
// Invokes the delegate when not null.
myDelegate.InvokeIfNotNull();
```

There's also a variant that supports more than four parameters (using `params object[]`) but will perform type conversions and boxing of values when necessary. Use this one sparingly or in non-critical situations only!

### Disposables

A similar extension function is available to dispose of an object if it implements the `IDisposable` interface.

```cs
// Disposes of the object when not null.
myDisposable.DisposeIfNotNull();
```

## Logging

A custom logging utility to replace Unity's default way of logging. Internally, it still uses Unity's `Debug` class, but allows you to outright disable certain logging levels. This not only clears up your log file, but can also save memory allocations because the debug strings aren't generated anymore.

```cs
Log.Info("My info message.");
Log.Warning("My warning message.");
Log.Error("My error message.");
Log.Exception(myCaughtException);
```

These log methods allow you to format the provided string directly like with the `Debug.LogFormat` methods.

You can set different logging levels for the player and the editor, allowing you to keep valuable info messages while testing in editor, and removing them from the player build. See the [Editor Settings][Editor Settings] section for more details on how to enable/disable these.

**Note**: disabling a logging level will instruct the compiler to disable/remove these logging statements from the constructed assembly. This also means that, if by any chance, you do some critical calculation inside the log statement, it will not be called anymore when the log statement is disabled/removed.

## Script Execution Order

In certain circumstances you might want control over the script execution order for a set of scripts. For example, a specific script needs to do its `Update` before another because it depends on its result during that frame. You can already define the script execution order manually in Unity's project settings panel. However, as a project grows larger and changes are made, this execution dependency can easily be overlooked as it isn't defined explicitly anywhere. To remedy this, several attributes are defined that help in maintaining this execution order dependency:

* The `ExecuteAt` attribute explicitly defines an execution order value, like you'd do in the player preferences panel.
* The `ExecuteBefore` attribute defines that a script needs to execute _before_ specific others. Multiple scripts can be set.
* The `ExecuteAfter` attribute defines that a script needs to execute _after_ specific others. Multiple scripts can be set.

Placing such attributes above your scripts will automatically determine the execution order between them and assign them execution order values that respects these constraints.

```cs
[ExecuteAt(66)] // I_AM THE SENATE!
public class MyBehaviour : MonoBehaviour
{
	...
}

[ExecuteAfter(typeof(MyBehaviour))]
public class MyOtherBehaviour : MonoBehavior
{
	// This script's behaviour depends on what's being done in MyBehaviour.
	...
}
```

**Important note**: cyclic dependencies as well as impossible ordering dependencies are, of course, not allowed. In that case an exception will be displayed with additional information on where the conflict is situated.

**Another note**: these attributes are only valid for classes that derive from `MonoBehaviour` as the script execution order feature from Unity only relates to them.

**Final note**: `ExecuteAt` takes precedence over `ExecuteBefore` and `ExecuteAfter`. Meaning that, if the former is defined, the latter ones are ignored.

[Logo]: ./Images/ImpossibleOddsLogo.png
[Editor Settings]: ../README.md#editor-settings
