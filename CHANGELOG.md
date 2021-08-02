# ![Impossible Odds Logo][Logo] C# Toolkit - Changelog

You'll find the full update history of this toolkit below. This might be useful for checking breaking changes.

**General note about installing updates**: it is recommended that you remove the current installed package first before updating to a newer one. Some files may have been deleted or moved, which may cause conflicts or compilation errors. The default location for this package in your Unity project is at `Assets/Impossible Odds/Toolkit`.

## v1.2 - Photon WebRPC Support & Tool Interoperability

**Note: this release contains minor breaking changes regarding the JSON tool. Please check the 'Renamed' and 'Removed' entries in the changelist below.**

Changelist:

### Added

* Photon WebRPC support. Check the Photon - WebRPC Extensions section in the documentation to read all the details.
* A utility for creating and appending URL parameters. This can be found in the new static `UrlUtilities` class in the `ImpossibleOdds.Http` namespace.
* The `HttpBodySerializationDefinition` now supports enum aliases and saving type information and brings it on par with the supported features by the JSON tool.
* The `HttpMessenger` and `WebRpcMessenger` can switch their internal message processing. This allows for reusing other serialization tools found in this toolkit. More information can be found in the HTTP and WebRPC documentation pages.
* Request and response messengers can now check how many pending requests are left by checking the `Count` property.
* Several serialization definitions can set a different type resolve key by setting the `TypeResolveKey` property.
* Introduced the `IHttpStructuredResponse` interface as a replacement for the `IHttpJsonResponse` interface.

### Improved

* Request and response messengers no longer throw an exception when attempting to send an already pending request again. Now, the current handle is returned instead.
* Tightened the type parameter restriction for the `ResponseCallback`, `ResponseType` attributes. An explicit exception is now thrown when an incompatible type is provided.

### Updated

* The `InvokeIfNotNull` extension method with flexible parameter list has been marked obsolete because of its unintentional fallback for otherwise invalid delegate invokations.
* The `IHttpJsonResponse` interface is marked obsolete as the name does not fully cover the extends of where the interface should be applied. Implement the `IHttpStructuredResponse` interface instead.
* The `JsonProcessor` now accepts an optional instance of `JsonOptions` for all of its serialization and deserialization methods. If a custom serialization definition is defined, it will use those data structures instead of the predefined types by the processor itself.
* The default logging level of the `Log` utility is now set to the 'info'-level when the toolkit detects no logging level has been set before.
* The example scenes for the JSON and XML tools now have their data loaded from a text asset instead of being hardcoded in the example code.
* The documentation of the HTTP, JSON and XML topics have been updated to contain more concrete examples as well as a complete code example at the end.
* Additional documentation for updated tools and features.

### Fixed

* Deserializing a CData section in a pretty-printed XML document would fail as the preceding whitespace characters were seen as a first node in the internal XML document representation.

### Renamed

* `JsonDefaultSerializationDefinition` → `JsonSerializationDefinition`.

### Removed

* The abstract generic `JsonSerializationDefinition<TJsonObject, TJsonArray>` class.
* The generic `Deserialize<TJsonObject, TJsonArray>` overload method on the `JsonProcessor`.

## v1.1 - XML Support & Serialization Performance

**Note: this release contains breaking changes! Check the 'Removed' and 'Renamed' entries in the changelist below.**

Changelist:

### Added

* XML serialization support. Check the XML section in the documentation to read all the details.
* Support for (de)serializing properties of objects instead of only fields.
* Enum alias caching for faster retrieval during (de)serialization.
* Support for aliases on enums decorated with the `System.Flags` attribute.
* A generic `Serialize` method on the `Serializer` class, for immediately receiving the typed result.
* Required fields in JSON or XML can now enable the `NullCheck` property to perform a value check as well. When the value in the source data is null, it will stop the deserialization process.

### Improved

* Serialization of Unity primitive values (`Vector3`, `Color`, etc.) can be modified with a preferred processing method, e.g. as a JSON object versus JSON array, or using XML elements versus XML attributes. This can be configured globally in the Impossible Odds project settings window, and per serialization definition.
* Serialization and deserialization of custom objects have improved memory usage by skipping several intermediate steps.
* HTTP and JSON serialization definitions now use instances of `ArrayList` instead of `List<object>` to skip the generic type checking steps.
* Objects requesting a serialization callback can now have an optional parameter of type `IProcessor` to get some context of what is processing the object.
* The `SerializationUtilities` class now contains clearly defined functions and descriptions.
* Exceptions thrown during serialization have a more clear description of what might have gone wrong.
* The JSON example scene includes more logging and made the JSON output interactable to experiment with the contents.

### Fixed

* Enum alias deserialization issue.

### Removed

* Several unused interfaces in the `ImpossibleOdds.Serialization` namespace.
* Several abstract classes in the `ImpossibleOdds.Serialization` namespace that would 'glue' several unrelated classes together.

### Renamed

* Several dependency injection related classes and interfaces:
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

### Added

* DependencyContainerExtensions collapsing frequently chained methods into a single call.

### Fixed

* The global dependency context was only injected in the scene's root game objects. Now their children are processed as well.

### Renamed

* `FunctorBinding` → `GeneratorBinding`. The file and documentation already used the right name, but the actual class did not reflect this yet. Note: this is a breaking change!

## v1.0.1 - Improved Documentation & Combined Package

Changelist:

### Added

* A combined documentation file in the package.
* The examples to the package.

### Improved

* Documentation and added some more flavor to the Core Utilities description.

### Updated

* The string extension functions to throw an ArgumentException instead of an ArgumentNullException if the string argument is empty or just white space characters.

## v1.0 - Initial Release

The first release of the Impossible Odds - C# Unity Toolkit!

The following features are included in this first release:

* Core utilities
* Dependency Injection
* Runnables
* JSON serialization
* HTTP communication

[Logo]: ./Docs/Images/ImpossibleOddsLogo.png
