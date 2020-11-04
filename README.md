# ![Impossible Odds Logo][Logo] Impossible Odds - Unity C# Toolkit

> This page is under construction.

The Impossible Odds C# Coding Toolkit for Unity is a set of carefully crafted tools to help you kickstart your project's codebase. It's designed with ease-of-use in mind, provide tools to keep your codebase clean and efficient. It combines several utilities and frameworks to help you think more about your game and gameplay design rather than code design.

A summary of what you can expect to find in this package:

* Core utilities: extension and utility functions to speed-up and reduce boiler-plate code.
* Dependency injection: keep your code organized and clean by removing tight coupling.
* Runnables: avoid the MonoBheaviour and GameObject tax if your classes only need the `Update` functionality or want to run some coroutines.
* JSON: serialize your data to and from a JSON data format, with support for saving type information.
* HTTP: easily serialize your data for Unity's WebRequests.

## Core Utilities

In the overarching `ImpossibleOdds` namespace, you'll find many smaller, but useful functions to speed up your coding and development.

### Delegates & Events

When subscribing to events in C#, a best practice is to also unsubscribe when you're done or no longer need them, e.g. when your GameObject gets destroyed. In some cases, it's not always clear which functions to unsubscribe, or you've subscribed to quite a lot of them and you end up with a whole list of `-=`-statements. A quick way for you to clear the object you're done with from all events on a particular object is:

```cs
// Clear all delegates found in myEventInvokingObject from
// subscribed methods linked to myObjectToBeDoneWith.
myEventInvokingObject.PurgeDelegatesOf(myObjectToBeDoneWith);
```

In some cases, you may also want to do this for events that are defined as static, or, remove all static functions of a type registered:

```cs
// Clear all static defined delegates of type MyEventInvokingClass
// from subscribed methods linked to myObjectToBeDoneWith.
Type myEventInvokingType = typeof(MyEventInvokingClass);
myEventInvokingType.PurgeDelegatesOf(myObjectToBeDoneWith);

// Similarly, clear all static defined methods linked to MyClassToBeDoneWith
// from the delegates defined on myEventInvokingObject
myEventInvokingObject.PurgeDelegatesOf(typeof(MyTypeToBeDoneWith));
```

**Important note**: anonymous functions subscribed to events can **not** cleared here, as their origin cannot be tied to a specific object.

**Another note**: using `PurgeDelegatesOf` uses reflection internally to look for any fields that inherit from the `System.Delegate` type. If you define your events with explicit `add` and `remove` implementations, then this behaviour will not work directly for that event unless it's backed by a delegate nonetheless.

Some other useful functions include the `InvokeIfNotNull()` extensions, saving you of writing the `null`-check every time. These are defined for both the `System.Action` and `UnityEngine.UnityAction` types, and support up to four parameters:

```cs
myEvent.InvokeIfNotNull();
```

There's also a variant that supports more than four parameters (using `params object[]`) but will perform type conversions and boxing of values, allocating memory and possibly trigger garbage collection. Use this one sparingly!

### Enums

There are situations where you might want to display an enum value properly instead of its internal value name. The usual way to go about it is to write a `switch`-statement and list all possibilities, but that becomes tedious the more values there are, as well as in how many places you want to display these values.

Introducing enum display names and translation keys! Using the `DisplayNameAttribute`, you can decorate your enum values with a direct display name and/or a translation key you can use to retrieve its proper translation from your localization system.

```cs
public enum MyEnum
{
	// Not interested in NONE...
	NONE,
	[DisplayName(Name="First", TranslationKey="myEnum/first")]
	FIRST,
	[DisplayName(TranslationKey="myEnum/second")]
	SECOND,
	[DisplayName(Name="Last")]
	LAST
}
```

Finally, these values can be retrieved using:

```cs
MyEnum value = MyEnum.First;
string name = value.DisplayName();
string loca = value.TranslationKey();
```

When calling the `DisplayName()` extension method, if no name is set, it will return the result of `ToString()`. For the `TranslationKey()` extension method, if no key is set, it will just return `string.Empty`.

### Logging

A custom logging utility to replace Unity's default way of logging. Internally, it still uses Unity's `Debug` class, but allows you to outright disable certain logging levels. This not only clears up your log file, but can also save memory allocations (and in the end, rounds of garbage collection), by disabling the lines of code calling to log a message.

```cs
Log.Info();
Log.Warning();
Log.Error();
Log.Exception();
```

The naming and parameter format deviates slightly from Unity's `Debug` class to keep them from having conflicts. All of these also have the option to directly provide any `string.Format()` parameters.

You can set different logging levels for the player and the editor, allowing you to keep valuable info messages while testing in editor, while removing them from the player build. See the [Editor Settings](#editor-settings) section for more details on how to enable/disable these.

### Value Checking

All to frequently, there are situations where you need to check if certain values are `null` or not, or a string is not empty. Then these can save you some lines and time:

```cs
value.ThrowIfNull(); // ArgumentNullException if null.
value.ThrowIfNullOrEmpty(); // ArgumentNullException if string is null or empty.
value.ThrowIfNullOrWhitespace(); // ArgumentNullException if string is null or whitespaace.
value.LogErrorIfNull(); // Logs an error if null and returns false, and true otherwise.
```

Similarly, at the end of using an object implementing the `IDisposable` interface:

```cs
value.DisposeIfNotNull();
```

## Dependency Injection

The `ImpossibleOdds.DependencyInjection` namespace contains a simple framework for adding the dependency injection methodology to your project. There are similar frameworks out there, packed with a ton more features and validation mechanisms. However, the goal of this particular implementation is to provide a more simple and streamlined setup that takes the essence of what dependency injection is all about.

> Documentation is under construction and API is not finalized!

## Runnables

The `ImpossibleOdds.Runnables` namespace provides the tools to ditch the GameObject and MonoBheaviour methodology for classes and data that require the `Update()` functionality, but don't need anything else.

> Documentation is under construction and API is not finalized!

## JSON

The `ImpossibleOdds.Json` namespace allows you to (de-)serialize your data from/to the JSON data format. Unity already has a built-in `JsonUtility` class that allows you to serialize your objects. However, this one lacks control in terms of which fields and under what name you want to serialize them. On the other side, there's the widely popular Json.NET library which is packed with tons of features.

This implementation aims to be somewhere in between. The main advantages over Unity's `JsonUtility` class, is that you can decide which fields should be serialized (from public all the way to private) and under which name they can be found. Additionally, it also provides support for serializing type information, allowing to keep the inheritance chain in tact when deserializing your data.

> Documentation is under construction and API is not finalized!

For more details about the data (de-)serialization process, check the [Serialization](#serialization) section.

## HTTP

Unity allows you to communicate with a server using web requests but requires that you provide the data you wish to send in a specific way or format. The `ImpossibleOdds.Http` namespace has tools to quickly create custom requests with automatic data processing, as well as serialize your data to a format that can be used to put in these web requests.

> Documentation is under construction and API is not finalized!

For more details about the data (de-)serialization process, check the [Serialization](#serialization) section.

## Editor Settings

This toolkit has a custom entry in your project settings panel. It currently allows you to adjust the following settings:

* Enable editor extensions: whether or not some context senstive menu's are enabled or not.
* Editor logging level: enable/disable certain logging messages from being executed while playing in editor.
* Player logging level: enable/disable certain logging messages from being executed in the player build.

![Editor Settings][EditorSettings]

Most settings are saved using preprocessor directives in the ProjectSettings.asset file.

## Advanced

The sub-sections found below are most likely not sections you'll come in direct contact with unless you wish to build and expand upon the features provided in this toolkit.

### Weblink

The `ImpossibleOdds.Weblink` namespace contains a general purpose messaging framework for sending requests and processing matching responses. It performs an automatic response class mapping in order to formulate the response data in a way which makes them directly workable for your classes.

> Documentation is under construction and API is not finalized!

It's currently used by the [`ImpossibleOdds.Http`](#http) namespace.

### Serialization

The `ImpossibleOdds.Serialization` namespace has several data processing classes to transform your data to/from intermediate data structures. This intermediate data can then be used to easily process them to other formats. What this intermediate data looks like can be defined using several flavors of 'serialization definitions'. They define what data types are natively supported by the target data format, and which data processors are used to transform the more complex data.

> Documentation is under construction and API is not finalized!

This is extensively used by the [`ImpossibleOdds.Json`](#json) and [`ImpossibleOdds.Http`](#http) namespaces.

## Unity Version

Developed and tested on Unity 2019.4 LTS.

## License

This package is provided under the [MIT][License] license.

[License]: ./LICENSE.md
[Logo]: ./ImpossibleOddsLogo.png
[EditorSettings]: ./Docs/Images/EditorSettings.png
