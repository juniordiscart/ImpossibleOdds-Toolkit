# ![Impossible Odds Logo][Logo] C# Toolkit - Core Utilities

These general tools can all be found by including the `ImpossibleOdds` namespace in your scripts.

In here you'll find smaller but useful utilities to speed up your programming. A quick summary of what you can expect to find in here:

* Extensions for delegates and events to immediately remove an object from all delegates found on another object.
* Extensions to quickly invoke events and delegates.
* Extensions to quickly check if the value is null, or a string is empty.
* Extensions for enum values, to provide them with a suitable name for display to your users.
* Extensions for list and other collections to check whether they're null or empty, shuffle or insert values.
* Custom logging tool to enable/disable certain levels of logging.
* Define script execution order dependencies of your scripts using attributes.

## Delegates & Events

When subscribing to events in C#, a best practice is to also unsubscribe when you're done or no longer need them, e.g. when your GameObject gets destroyed. This often results in listing every method you had subscribed to it. Manually listing these functions is tedious and can be error prone.

A quick way to clear the object you're done with from all events on a particular object is to use the `PurgeDelegatesOf` extension method defined on all objects. It searches the object for all delegate fields and removes the target object from their invocation lists.

```cs
// Clears the delegates of myObject from methods belonging to myDisposedObject.
myObject.PurgeDelegatesOf(myDisposedObject);
```

The above works for instanced objects. In case there's the need to remove an object from all static events and delegates of a type, simply invoke the `PurgeDelegatesOf` on the type object.

```cs
// Clears the static delegates defined on MyType from methods belonging to myDisposedObject.
Type myType = typeof(MyType);
myType.PurgeDelegatesOf(myDisposedObject);
```

Similarly, in case you want to remove all static methods of a type from a certain object's events and delegates:

```cs
// Clears the delegates of myObject from static methods defined on MyType.
myObject.PurgeDelegatesOf(typeof(MyType));
```

**Important note**: anonymous delegates and lambdas subscribed to events can **not** be cleared using this extension method, as their origin cannot be traced to a specific object.

**Another note**: using `PurgeDelegatesOf` uses reflection internally to scan your object for any fields that inherit from the `System.Delegate` type. When you define events with explicit `add` and `remove` implementations, this method will only work if the values for these implementations are saved in a delegate in that object.

## Value Checking

All to frequently, there are situations where you need to check if certain values are `null` or not, or a string contains not just whitespace characters. For example checking the incoming parameters in a method. This often results in a sequence of `if`-statements that show a logging message, return the function already, or throw an exception.

To speed up writing these value-checks, which are often all very similar, some handy extension shorthands are defined:

* `ThrowIfNull` checks whether the object is null and throws an `ArgumentNullException` when it is.
* `ThrowIfNullOrEmpty` and `ThrowIfNullOrWhitespace` are string-specific variants to check if the string isn't null, empty or just whitespace characters. In case the string is null, an `ArgumentNullException` is thrown. When the string is empty or just whitespace characters, an `ArgumentException` is thrown instead.

```cs
// Throws an exception when null, empty or just whitespace characters.
myObject.ThrowIfNull(nameof(myObject));
myString.ThrowIfNullOrEmpty(nameof(myString));
myString.ThrowIfNullOrWhitespace(nameof(myString));
```

In case throwing exceptions is not the preferred way of working, similar extension functions are available to log an error instead:

* `LogErrorIfNull` will log an error to the console when the value is null.
* `LogErrorIfNullOrEmpty` and `LogErrorIfNullOrWhitespace` are string-specific variants that do the same, but will also check if the string isn't empty or just whitespace characters.

These extension methods have a predefined error message which expects the name of the argument to be given.

```cs
// Logs an error to the console when null, empty or just whitespace characters.
myObject.LogErrorIfNull(nameof(myObject));
myString.LogErrorIfNullOrEmpty(nameof(myString));
myString.LogErrorIfNullOrWhitespace(nameof(myString));
```

These last ones also return a boolean result, denoting whether they printed an error message. This can be useful to pack your checks into a single `if`-statement and, for example, have your function quit early:

```cs
public void MyMethod(MyClassA param1, string param2)
{
	// Check if all parameters are valid.
	if (param1.LogErrorIfNull(nameof(param1)) ||
		param2.LogErrorIfNullOrEmpty(nameof(param2)))
	{
		return;
	}

	// Arguments OK! Continue processing...
}
```

### Delegates

Similarly like checking incoming values, it's a good practice to check whether a delegate or event that you're about to invoke is not null. The `InvokeIfNotNull` extension method for delegates reduces this check to a simple single line:

```cs
// Invokes the delegate when not null.
myDelegate.InvokeIfNotNull();
```

This extension method is also defined for instances of `System.Action`, `System.Func` and `UnityEngine.UnityAction`.

Explicit variants with parameters for each of the supported types are also defined for up to four parameters.

```cs
public class MyLeaderboard
{
	public event Action<string, name> onNewScore;

	public void UpdateScore(string name, int score)
	{
		// Invoke the event with parameters.
		onNewScore.InvokeIfNotNull(name, score);
	}
}
```

**Note**: there's also a variant that supports more than four parameters (using `params object[]`) but will perform type conversions and boxing of values when necessary. Use this one sparingly or in non-critical situations only!

### Disposables

A similar extension method is available to dispose of objects that implement the `IDisposable` interface.

```cs
// Disposes of the object when not null.
myDisposable.DisposeIfNotNull();
```

### Strings

Common value checking of objects of type `string` is to see whether they're null, empty or only contain whitespace characters. This is usually done through the static `IsNullOrEmpty` or `IsNullOrWhitespace` methods. This toolkit makes these two functions also available as extensions functions to any string object, writing the code just that little bit shorter:

```cs
string myString;
if (myString.IsNullOrEmpty())
{
	Log.Error("The string is null or empty.");
}
else if (myString.IsNullOrWhitespace())
{
	Log.Error("The string is null, empty, or a set of whitespace characters.");
}
```

### Lists & Collections

Just like with strings, you sometimes are simply interested in whether the list or collection is null or empty before doing anything with it.

```cs
List<string> myValues;
if (myValues.IsNullOrEmpty())
{
	Log.Error("The list is null or empty.");
}
```

## Enums

There are situations where you might want to display an enum value properly instead of its internal/code name. The usual way to go about it is to write a `switch`-statement and list all possibilities, but that becomes tedious the more values there are, as well as very error prone depending on the amount of locations in your code this is used.

Introducing enum display names and translation keys! Using the `DisplayName` attribute, you can decorate your enum values with a direct display name and/or a translation key. The latter one you can use to retrieve its proper translation from your localization system, if any.

```cs
public enum GameModes
{
	[DisplayName(Name="Race", LocalizationKey="gamemodes/race")]
	RACE,
	[DisplayName(Name="Time Trial", LocalizationKey="gamemodes/time_trial")]
	TIME_TRIAL
}
```

Finally, these values can be retrieved using their `DisplayName` and `LocalizationKey` extension methods:

```cs
// Gets the display name and localization key of the enum value.
GameModes value = GameModes.RACE;
string name = value.DisplayName();
string locaKey = value.LocalizationKey();
```

**Note**: when your enum value has no display name defined, calling the `DisplayName` extension method will perform the `ToString` method on the value and return that result instead. The `LocalizationKey` extension method will return a `string.Empty` result when no translation key is set.

**Another note**: don't confuse the Impossible Odds `DisplayName` attribute with the C# `DisplayName` attribute found in the `System.ComponentModel` namespace.

A caveat of enums and their underlying values is that it's impossible to distinguish them from each other if they share the same underlying value, and may return the name of another one. Take, for example, the following enum definition:

```cs
public enum Options
{
	[DisplayName(Name="None")]
	NONE = 0,
	[DisplayName(Name="Enabled")]
	ENABLED = 1,
	[DisplayName(Name="Default")]
	DEFAULT = NONE
}
```

The `Options.NONE` and `Options.DEFAULT` enum values are defined to share the same underlying value, and no meaningful distinction can be made between them. Consequently, calling `Options.NONE.DisplayName()` could result in `Default` being returned instead of `None`.

## List & Collection Extensions

Lists and other `IEnumerable` collections are commonly used data structures throughout a codebase. `System.Linq` already provides a wide set of extra tools to help write more concise code.

In the [Value Checking](#value-checking) section, you've already seen the `IsNullOrEmpty` extension method. In this module though, you'll find a few more that can be of use that are detailed below.

### Sorted Insertion

When a list already contains an ordered set of values, inserting a new value (or values) right in place can be done using the `SortedInsert` methods. There are a wide variety of overloads of this method to configure the way the sorting should happen. Most of them operate on the basis of that the values being inserted have the `IComparable` interface implemented, which makes comparing the values trivial for the insertion algorithm.

```cs
List<int> sortedList = new List<int>() { -1, 0, 5, 66 };
sortedList.SortedInsert(9);	// Will insert the value between values 5 and 66.
```

If no `IComparable` interface is implemented by the element type, then a custom comparison operator can be provided.

### Shuffling

In some occasions, it's desirable to have a randomly ordered set of values. The `Shuffle` extension method allows you to shuffle a set of values in it.

```cs
List<float> myValues;
myValues.Shuffle();
```

The default shuffle method will work with Unity's static `Random` class to generate the random numbers that determine the shuffle positions. Alternatively, if you want some 'predictability' to the shuffle, there's also the overload that takes a `System.Random` value as a parameter.

### Swapping Elements

When needing to swap two elements in a list or array, it takes a few lines of code to accomplish, with a tendency to make an error in where to slot the temporary value back in:

```cs
// Swap the first two elements of the list.
var tempValue = myList[0];
myList[0] = myList[1];
myList[1] = tempValue;
```

With the `Swap` extension function, you can perform this in a single line:

```cs
myList.Swap(0, 1); // Swap the first two elements of the list.
```

## Logging

Logging debug output is a necessity during development and testing to trace what's going on in your code. However, when your project edges to its release-state, you're generally only interested in the errors that may still occur, and your _info_-level messages tend to get in the way. Even worse, generating these info-level messages may allocate memory, which could trigger garbage collection. To remedy this, the custom logger in this toolkit allows you to outright disable certain levels of logging in your build, and even in the editor too, if you prefer.

The static `Log` class provides the usual logging message methods:

* `Log.Info` to write an info-level message to the console.
* `Log.Warning` to write a warning-level message to the console.
* `Log.Error` to write an error-level message to the console.
* `Log.Exception` to log an exception.

```cs
Log.Info("My info message.");
Log.Warning("My warning message.");
Log.Error("My error message.");
Log.Exception(myException);
```

The `Info`, `Warning` and `Error` methods allow you to pass string formatting parameters for quick and efficient message construction. All of the logging methods also allow to pass on a Unity context object, which can help in trace the origin of the message in your project when you click on the log message in your console.

```cs
public class MyBehaviour : MonoBehaviour
{
	public void Start()
	{
		Log.Info(this, "{0} successfully initialized!", name);
	}
}
```

Controlling which levels of logging are enabled/disabled can be done using the Impossible Odds preferences panel, which you can find in your project's preferences. It'll tell you which levels of logging messages are still coming through as well as which ones are suppressed.

![Editor Settings][EditorSettingsImg]

**Important note**: disabling a logging level will instruct the compiler to disable/remove these logging statements from the constructed assembly. This also means that, if by any chance, you do some critical calculation inside the log statement, it will not be called anymore when that logging level is disabled.

## Script Execution Order

In an ideal Unity project environment, your scripts can work independently from each other, meaning that, no matter in which order they get initialized and updated, the result stays the same. This is however not always the case, and you may want to define a certain order for some scripts to state that one should execute before or after another script, because its result depends on their result.

Unity already provides this through its _script execution order_ feature found in your project settings panel. However, this view does not tell you _why_ a certain script is assigned a certain execution order value, or relative to which script that value is important. Furthermore, as a project grows larger and more and more scripts are added which may have to interact with these specific scripts for which the order is important, it becomes increasingly difficult to maintain the order of execution dependencies.

To help in this matter, this toolkit provides several attributes which you can place on your scripts and state when or relative to what they should get updated:

* The `ExecuteAt` attribute explicitly defines an execution order value, like you'd do in the script execution order panel.
* The `ExecuteBefore` attribute defines that a script needs to execute _before_ specific other scripts.
* The `ExecuteAfter` attribute defines that a script needs to execute _after_ specific other scripts.

Placing these attributes above your scripts will automatically determine the execution order between them and assign them execution order values that attempts to respects these constraints.

```cs
[ExecuteAt(66)] // I AM THE SENATE!
public class MyBehaviour : MonoBehaviour
{ }

[ExecuteAfter(typeof(MyBehaviour))]
public class MyOtherBehaviour : MonoBehavior
{
	// This script's behaviour depends on what's being done in MyBehaviour.
}
```

**Important note**: cyclic dependencies as well as impossible ordering dependencies are, of course, not allowed. In that case an exception will be displayed with additional information on where the conflict is situated.

**Another note**: these attributes are only valid for classes that derive from `MonoBehaviour` as the script execution order feature from Unity only applies to them.

**Final note**: `ExecuteAt` takes precedence over `ExecuteBefore` and `ExecuteAfter`. Meaning that, if the former is defined, the latter ones are ignored.

[Logo]: ./Images/ImpossibleOddsLogo.png
[EditorSettingsImg]: ./Images/EditorSettings.png
