# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Core Utilities

These general tools can all be found by including the `ImpossibleOdds` namespace in your scripts.

In here you'll find smaller but useful utilities to speed up your programming. A quick summary of what you can expect to find in here:

* Extenions for delegates and events to immediately remove an object from all delegates found on another object.
* Extensions to quickly invoke events and delegates.
* Extensions to quickly check if the value is null, or a string is empty.
* Extensions for enum values, to provide them with a suitable name for display to your users.
* Custom logging tool to enable/disable certain levels of logging.
* Define script execution order dependencies of your scripts using attributes.

## Delegates & Events

When subscribing to events in C#, a best practice is to also unsubscribe when you're done or no longer need them, e.g. when your GameObject gets destroyed. This often results in listing every method you had subscribed to it. Manually listing these functions is tedious and can be error prone.

A quick way to clear the object you're done with from all events on a particular object is to use the `PurgeDelegatesOf` extension method defined on all objects. It searches the object for all delegate fields and removes the target object from their invokation lists.

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

Finally, these values can be retrieved using their extension `DisplayName` and `LocalizationKey` methods:

```cs
// Gets the display name and localization key of the enum value.
GameModes value = GameModes.RACE;
string name = value.DisplayName();
string locaKey = value.LocalizationKey();
```

**Note**: when your enum value has no display name defined, calling the `DisplayName` extension menthod will perform the `ToString` method on the value and return that result instead. The `LocalizationKey` extension method will return a `string.Empty` result when no translation key is set.

**Another note**: don't confuse the Impossible Odds `DisplayName` attribute with the C# `DisplayName` attribute found in the `System.ComponentModel` namespace.

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

Controlling which levels of logging are enabled/disabled can be done using the Impossible Odds preferences panel, which you can find in your project's preferences. It'll tell you which levels of logging messages are still coming through as well as which ones are supressed.

![Editor Settings][EditorSettingsImg]

**Important note**: disabling a logging level will instruct the compiler to disable/remove these logging statements from the constructed assembly. This also means that, if by any chance, you do some critical calculation inside the log statement, it will not be called anymore when that logging level is disabled.

## Script Execution Order

In an ideal Unity project environment, your scripts can work idependently from each other, meaning that, no matter in which order they get initialized and updated, the result stays the same. This is however not always the case, and you may want to define a certain order for some scripts to state that one should execute before or after another script, because its result depends on their result.

Unity already provides this through its _script execution order_ feature found in your project settings panel. However, this view does not tell you _why_ a certain script is assigned a certain execution order value, or relative to which script that value is important. Furthermore, as a project grows larger and more and more scripts are added which may have to interact with these specific scripts for which the order is important, it becomes increasingly difficult to maintain the order of execution dependencies.

To help in this matter, this toolkit provides several attributes which you can place on your scripts and state when or relative to they should get updated:

* The `ExecuteAt` attribute explicitly defines an execution order value, like you'd do in the script execution order panel.
* The `ExecuteBefore` attribute defines that a script needs to execute _before_ specific other scripts. Multiple scripts can be set.
* The `ExecuteAfter` attribute defines that a script needs to execute _after_ specific other scripts. Multiple scripts can be set.

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
