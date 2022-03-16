# ![Impossible Odds Logo][Logo] Impossible Odds - C# Toolkit

The Impossible Odds C# Toolkit for Unity is a set of carefully crafted tools to help you kickstart your project's codebase. It's designed with ease-of-use in mind, provide tools to keep your codebase clean and efficient. It combines several utilities and frameworks to help you think more about your game and gameplay design rather than code design.

You can expect to find the following features in this tookit:

* [Core utilities](#core-utilities): extension and utility functions to speed-up programming work and reduce boiler-plate code.
* [Dependency injection](#dependency-injection): keep your code organized and clean by removing tightly coupled design patterns.
* [Runnables](#runnables): avoid the MonoBehaviour and GameObject tax if your classes only need the `Update` functionality.
* [JSON](#json): serialize your data to and from the JSON data format, with support for saving type information.
* [XML](#xml): serialize your data to and from the XML data format using an easier alternative compared to C#'s built-in XML tools.
* [HTTP](#http): easily transform your objects for sending data to your server using Unity’s web requests.
* [Photon - WebRPC Extensions](#photon---webrpc-extensions): a convenience framework when sending requests over Photon's multiplayer network to your custom web server.

## Core Utilities

In the overarching `ImpossibleOdds` namespace, you'll find many smaller, but useful functions to speed up your development. Some highlights of what you will find in here:

* Custom logging tool for which logging levels can be enabled/disabled.
* Clear out delegates of specific objects in a single line of code.
* Invoke actions with a null-check in a single line of code.
* Decorate enum values with a display name or translation key.
* Automatic script execution ordering based on execution dependencies between scripts.
* Object and string extensions to throw or log an error when null.

All details and more about these little additions can be found [here][CoreUtilities].

## Dependency Injection

The `ImpossibleOdds.DependencyInjection` namespace contains a simple framework for adding the dependency injection methodology to your project. There are similar frameworks out there, packed with a ton more features and validation mechanisms. However, the goal of this particular implementation is to provide a more simple and streamlined setup that takes the essence of what dependency injection is all about.

Interested in integrating this in your project? Read more about how it works [here][DependencyInjection]!

## Runnables

The `ImpossibleOdds.Runnables` namespace provides the tools to ditch the GameObject and MonoBehaviour methodology for classes and data that require the `Update` functionality, but don't need anything more than that to function properly.

Read about how to get your scripts running [here][Runnables].

## JSON

The `ImpossibleOdds.Json` namespace allows you to (de)serialize your data from/to the JSON data format. Unity already has a built-in `JsonUtility` class that allows you to serialize your objects. However, it lacks control in terms of which members and under what name you want to serialize them. On the other side, there's the widely popular Json.NET library which is packed with tons of features.

This implementation aims to be somewhere in between. The main advantages over Unity's `JsonUtility` class is that you can decide which members should be serialized (from public all the way to private) and under what name they are saved. Additionally, it also provides support for serializing type information, allowing you to keep the inheritance chain intact when deserializing your data.

Curious to know how it works? Get all the details [here][Json].

For more details about the data (de)serialization process itself, check the [Serialization][Serialization] section.

## XML

The `ImpossibleOdds.Xml` namespace provides tools to process your data to/from the XML data format. The C# language has a bunch of XML tools already available for you to use, but they have a few shortcomings such as only capable of picking up public members, or force you to use tedious classes such as the `DataContractSerializer` class.

The XML implementation in this toolkit aims to give a great amount of control which members of your classes and how they should get processed.

Get to know the XML tools and what they have to offer [here][Xml].

For more details about the data (de)serialization process, check the [Serialization][Serialization] section.

## HTTP

The `ImpossibleOdds.Http` namespace contains allows you to process your objects directly to requests and responses suitable to send over to your game sever as well as a helpful messenger system that takes care of performing callback operations on interested objects when a response is received.

To start communicating with your server, check the documentations page [here][Http].

For more details about the data (de)serialization process, check the [Serialization][Serialization] section.

## Photon - WebRPC Extensions

In the `ImpossibleOdds.Photon.WebRpc` namespace, you'll find an extension framework on Photon's multiplayer package, specifically for sending requests and receiving responses from your web server over the WebRPC feature.

For more information about this tool and how to get started, read on [here][PhotonWebRPC].

For more details about the data (de)serialization process, check the [Serialization][Serialization] section.

**Note**: this tool is only available when a valid Photon package is installed in your project. It is disabled otherwise.

## State Machines

A state machine is a commonly used pattern in game development and comes in a wide variety of shapes, sizes and complexity. The `ImpossilbleOdds.StateMachines` namespace contains a generic framework to get you started on setting up custom state machines quick and easy.

To learn all about it's ins and outs, please read its documentation page [here][StateMachines].

## Addressables Extensions



## Editor Settings

This toolkit has a custom entry in your project settings panel. It currently allows you to adjust the following settings:

* Enable editor extensions: whether or not some context senstive menu's are enabled or not.
* Editor logging level: enable/disable certain logging messages from being executed while playing in editor.
* Player logging level: enable/disable certain logging messages from being executed in the player build.
* Json serialization settings: define the default behaviour of the JSON serializer for certain Unity data types such as `Color`, `Vector3`, `Quaternion`, etc.
* Xml serialization settings: define the default behavior of the XML serializer for certain Unity data types such as `Color`, `Vector3`, `Quaternion`, etc.

![Editor Settings][EditorSettings]

Most settings are saved using preprocessor directives in the ProjectSettings.asset file.

## Advanced

The sub-sections found below are most likely not sections you'll come in direct contact with unless you wish to build or expand upon the features provided in this toolkit.

### Serialization

The `ImpossibleOdds.Serialization` namespace contains a flexible data transformation framework, ideal for serialization purposes. It allows to define a blueprint for rules on how data should be processed and transformed from one format to the other. It is the backbone for the [JSON][Json] and [HTTP][Http] features found in this toolkit.

All details can be found [here][Serialization].

### Weblink

The `ImpossibleOdds.Weblink` tool provides a request-response mechanism along with automated processing of the data coming through it as well as providing a plethora of ways on getting notified when a message has completed. This removes the worry to manually keep track of request objects and waiting for their responses, as well as processing their data to the right format. The [HTTP][Http] and [Photon WebRPC][PhotonWebRPC] extension tools are practical implementations of the Weblink framework.

For more details, check its documentations page [here][Weblink].

## Contributing

Contributions are more than welcome! If you have ideas on how to improve the concepts or structure of this toolkit, have additional ideas for features to add, come across a bug, or want to let me know you're using this toolkit in your project, feel free to [get in touch][Contact]!

## Unity Version

Developed and tested on Unity 2019.4 LTS.

## License

This package is provided under the [MIT][License] license.

## Changelog

View the [update history][Changelog] of this package.

[License]: ./LICENSE.md
[Changelog]: ./CHANGELOG.md
[EditorSettings]: ./Docs/Images/EditorSettings.png
[CoreUtilities]: ./Docs/CoreUtilities.md
[DependencyInjection]: ./Docs/DependencyInjection.md
[Runnables]: ./Docs/Runnables.md
[Json]: ./Docs/Json.md
[Xml]: ./Docs/Xml.md
[Http]: ./Docs/Http.md
[PhotonWebRPC]: ./Docs/PhotonWebRPC.md
[StateMachines]: ./Docs/StateMachines.md
[Weblink]: ./Docs/Weblink.md
[Serialization]: ./Docs/Serialization.md
[Contact]: https://www.impossible-odds.net/support-request/
[Logo]: ./Docs/Images/ImpossibleOddsLogo.png
