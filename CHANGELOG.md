# ![Impossible Odds Logo][Logo] C# Toolkit - Changelog

You'll find the full update history of this toolkit below. This might be useful for checking breaking changes.

**General note about installing updates**: it is recommended to remove the current installed package first before updating to a newer one. Some files may have been deleted or moved, which may cause conflicts or compilation errors. The default location for this package in your project is at `Assets/Impossible Odds/Toolkit`.

## v1.2 - Photon WebRPC Support

**Note: this release contains minor breaking changes regarding the JSON tool. Please check the 'Renamed' and 'Removed' entries in the changelist below.**

Changelist:

* **Added**: Photon WebRPC support. Check the Photon - WebRPC Extensions section in the documentation to read all the details.
* **Added**: `UrlUtilities` static class for generating/appending parameters to an URL. This can be found in the `ImpossibleOdds.Http` namespace.
* **Added**: The `HttpBodySerializationDefinition` now has support for enum aliases and type information.
* **Added**: The `IIndexAndLookupSerializationDefinition` interface and its generic variant.
* **Added**: The abstract `WeblinkMessenger` (used by the `HttpMessenger`) now allows to stop or remove a pending request based on the request object too.
* **Added**: The `HttpMessenger` can now switch out it's serialization definitions for others, which may help in reusing attributes already placed using other tools in this toolkit.
* **Added**: The `HttpBodySerializationDefinition` and `JsonSerializationDefinition` can now update the type resolve key being used by setting the `TypeResolveKey` property.
* **Improved**: The `HttpMessenger` now returns the handle of a request that was already pending instead of throwing an exception.
* **Improved**: Tightened the type parameter restriction for the `HttpResponseCallback`, `HttpResponseType` attributes. An excplicit exception is now thrown when an incompatible type is provided.
* **Updated**: The `CustomObjectLookupProcessor` and `CustomObjectSequenceProcessor` now may receive an options parameter to skip checking for a class marking attribute.
* **Updated**: The `HttpURLSerializationDefinition` and `HttpHeaderSerializationDefinition` stepped down from implementing the abstract `LookupDefinition` class and instead implement the `ILookupSerializationDefinition` interface. Additionally, they skip the checking for a class marking attribute for their custom object processors.
* **Updated**: The `InvokeIfNotNull` with flexible parameter list has been marked obsolete as it functions as an non-intentional fallback when not using the intended generic overloads.
* **Updated**: The default logging level of the `Log` utility is now set to the 'information' level when the Toolkit is added to the project.
* **Updated**: The `Json` and `Xml` example scenes now have their data loaded from a text asset instead of being hardcoded in the example code.
* **Updated**: The documentation of the `Http`, `Json` and `Xml` topics have been updated to contain more concrete examples as well as a complete code example at the end.
* **Updated**: Additional documentation for several updated topics.
* **Fixed**: Deserializing a CData section in a neatly formatted XML document.
* **Renamed**: `JsonDefaultSerializationDefinition` → `JsonSerializationDefinition`.
* **Removed**: The abstract generic `JsonSerializationDefinition<TJsonObject, TJsonArray>` class.
* **Removed**: The generic `Deserialize<TJsonObject, TJsonArray>` overload method on the `JsonProcessor`.

## v1.1 - XML Support & Serialization Performance

**Note: this release contains breaking changes! Check the 'Removed' and 'Renamed' entries in the changelist below.**

Changelist:

* **Added**: XML serialization support. Check the XML section in the documentation to read all the details.
* **Added**: Support for (de)serializing properties of objects instead of only fields.
* **Added**: Enum alias caching for faster retrieval during (de)serialization.
* **Added**: Support for aliases on enums decorated with the `System.Flags` attribute.
* **Added**: A generic `Serialize` method on the `Serializer` class, for immediately receiving the typed result.
* **Added**: Required fields in JSON or XML can now enable the `NullCheck` property to perform a value check as well. When the value in the source data is null, it will stop the deserialization process.
* **Improved**: Serialization of Unity primitive values (`Vector3`, `Color`, etc.) can be modified with a preferred processing method, e.g. as a JSON object versus JSON array, or using XML elements versus XML attributes. This can be configured globally in the Impossible Odds project settings window, and per serialization definition.
* **Improved**: Serialization and deserialization of custom objects have improved memory usage by skipping several intermediate steps.
* **Improved**: HTTP and JSON serialization definitions now use instances of `ArrayList` instead of `List<object>` to skip the generic type checking steps.
* **Improved**: Objects requesting a serialization callback can now have an optional parameter of type `IProcessor` to get some context of what is processing the object.
* **Improved**: The `SerializationUtilities` class now contains clearly defined functions and descriptions.
* **Improved**: Exceptions thrown during serialization have a more clear description of what might have gone wrong.
* **Improved**: The JSON example scene includes more logging and made the JSON output interactable to experiment with the contents.
* **Fixed**: Enum alias deserialization issue.
* **Removed**: Several unused interfaces in the `ImpossibleOdds.Serialization` namespace.
* **Removed**: Several abstract classes in the `ImpossibleOdds.Serialization` namespace that would 'glue' several unrelated classes together.
* **Renamed**: Several dependency injection related classes and interfaces:
	* `IDependencyContext` → `IDependencyScope`
	* `DependencyContext` → `DependencyScope`
	* `GlobalContextInstallerAttribute` → `GlobalScopeInstallerAttribute`
	* `GlobalDependencyContext` → `GlobalDependencyScope`
	* `HierarchyDependencyContext` → `HierarchyDependencyScope`
	* `SceneDependencyContext` → `SceneDependencyScope`
	* `IDependencyContextInstaller` → `IDependencyScopeInstaller`
	* `AbstractDependencyContextBehaviour` → `AbstractDependencyScopeBehaviour`

## v1.0.2 - Extensions & Fixes

**Note: this release contains a breaking change! Check the 'Renamed' entries in the changelist below.**

Changelist:

* **Renamed**: FunctorBinding → GeneratorBinding. The file and documentation already used the right name, but the actual class did not reflect this yet. Note: this is a breaking change!
* **Added**: DependencyContainerExtensions collapsing frequently chained methods into a single call.
* **Fixed**: The global dependency context was only injected in the scene's root game objects. Now their children are processed as well.

## v1.0.1 - Improved Documentation & Combined Package

Small update that unifies the packages, making it the same setup and layout as you would download it from the Unity Asset Store.

Changelist:

* **Added**: a combined documentation file in the package.
* **Added**: the examples to the package.
* **Improved**: documentation and added some more flavor to the Core Utilities description.
* **Updated**: the string extension functions to throw an ArgumentException instead of an ArgumentNullException if the string argument is empty or just white space characters.

## v1.0 - Initial Release

The first release of the Impossible Odds - C# Unity Toolkit!

The following features are included in this first release:

* Core utilities
* Dependency Injection
* Runnables
* JSON serialization
* HTTP communication

[Logo]: ./Docs/Images/ImpossibleOddsLogo.png
