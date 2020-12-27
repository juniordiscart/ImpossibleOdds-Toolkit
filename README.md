# ![Impossible Odds Logo][Logo] Impossible Odds - Unity C# Toolkit

> This page is still under construction and subject to change.

The Impossible Odds C# Coding Toolkit for Unity is a set of carefully crafted tools to help you kickstart your project's codebase. It's designed with ease-of-use in mind, provide tools to keep your codebase clean and efficient. It combines several utilities and frameworks to help you think more about your game and gameplay design rather than code design.

A summary of what you can expect to find in this package:

* Core utilities: extension and utility functions to speed-up and reduce boiler-plate code.
* Dependency injection: keep your code organized and clean by removing tight coupling.
* Runnables: avoid the MonoBheaviour and GameObject tax if your classes only need the `Update` functionality or want to run some coroutines.
* JSON: serialize your data to and from the JSON data format, with support for saving type information.
* HTTP: easily serialize your data for sending and receiving Unity's WebRequests.

## Core Utilities

In the overarching `ImpossibleOdds` namespace, you'll find many smaller, but useful functions to speed up your coding and development. Some highlights of what you will find in here:

* Custom logging tool for which logging levels can be enabled/disabled.
* Clear out delegates of specific objects in a single line of code.
* Invoke actions with a null-check in a single line of code.
* Decorate enum values with a suitable display name or translation key.
* Extensions functions for List.
* Object and string extensions to throw or log an error when null.

Check its documentations page [here][CoreUtilities].

## Dependency Injection

The `ImpossibleOdds.DependencyInjection` namespace contains a simple framework for adding the dependency injection methodology to your project. There are similar frameworks out there, packed with a ton more features and validation mechanisms. However, the goal of this particular implementation is to provide a more simple and streamlined setup that takes the essence of what dependency injection is all about.

Check its documentations page [here][DependencyInjection].

## Runnables

The `ImpossibleOdds.Runnables` namespace provides the tools to ditch the GameObject and MonoBheaviour methodology for classes and data that require the `Update()` functionality, but don't need anything else.

Check its documentations page [here][Runnables].

## JSON

The `ImpossibleOdds.Json` namespace allows you to (de-)serialize your data from/to the JSON data format. Unity already has a built-in `JsonUtility` class that allows you to serialize your objects. However, it lacks control in terms of which fields and under what name you want to serialize them. On the other side, there's the widely popular Json.NET library which is packed with tons of features.

This implementation aims to be somewhere in between. The main advantages over Unity's `JsonUtility` class is that you can decide which fields should be serialized (from public all the way to private) and under what name they can be found. Additionally, it also provides support for serializing type information, allowing to keep the inheritance chain in tact when deserializing your data.

Check its documentations page [here][Json].

For more details about the data (de-)serialization process itself, check the [Serialization][Advanced-Serialization] section.

## HTTP

Unity allows you to communicate with a server using web requests but requires that you provide the data you wish to send in a specific way or format. The `ImpossibleOdds.Http` namespace has tools to quickly create custom requests with automatic data processing, as well as serialize your data to a format that can be used to put in these web requests.

Check its documentations page [here][Http].

For more details about the data (de-)serialization process, check the [Serialization][Advanced-Serialization] section.

## Editor Settings

This toolkit has a custom entry in your project settings panel. It currently allows you to adjust the following settings:

* Enable editor extensions: whether or not some context senstive menu's are enabled or not.
* Editor logging level: enable/disable certain logging messages from being executed while playing in editor.
* Player logging level: enable/disable certain logging messages from being executed in the player build.

![Editor Settings][EditorSettings]

Most settings are saved using preprocessor directives in the ProjectSettings.asset file.

## Advanced

The sub-sections found below are most likely not sections you'll come in direct contact with unless you wish to build and expand upon the features provided in this toolkit.

Check their documentations page [here][Advanced].

## Unity Version

Developed and tested on Unity 2019.4 LTS.

## License

This package is provided under the [MIT][License] license.

[License]: ./LICENSE.md
[EditorSettings]: ./Docs/Images/EditorSettings.png
[CoreUtilities]: ./Docs/CoreUtilities.md
[DependencyInjection]: ./Docs/DependencyInjection.md
[Runnables]: ./Docs/Runnables.md
[Json]: ./Docs/Json.md
[Http]: ./Docs/Http.md
[Advanced]: ./Docs/Advanced.md
[Advanced-Weblink]: ./Docs/Advanced.md#weblink
[Advanced-Serialization]: ./Docs/Advanced.md#serialization
[Logo]: ./Docs/Images/ImpossibleOddsLogo.png
