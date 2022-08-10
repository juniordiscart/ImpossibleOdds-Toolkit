# ![Impossible Odds Logo][Logo] C# Toolkit - Changelog

You'll find the full update history of this toolkit below. This might be useful for checking breaking changes.

**General note about installing updates**: it is recommended that you remove the current installed package first before updating to a newer one. Some files may have been deleted or moved, which may cause conflicts or compilation errors. The default location for this package in your Unity project is at `Assets/Impossible Odds/Toolkit`.

## v1.5.2 - Named Injection Fix

Changelog:

### Fixed

* Fixed issue where injecting resources using an injection identifier would throw a `NullReferenceException`.

## v1.5.1 - Purge Delegates Fix

Changelog:

### Fixed

* Fixed issue where purging the delegates of an object would try to purge non-existing delegates on the object, and throw an exception.

## v1.5 - State Machines, Dependency Injection Improvements & Optimizations

Changelog:

### Added

* Added the State Machines functionality to set up and manage state machines.
* Added Unity package data so that this toolkit can now be included as an embedded project package.
* Added `IsNullOrEmpty` and `IsNullOrWhitespace` string extension functions in analogy of their static counterparts.
* Added `IsNullOrEmpty` collection extension functions to quickly check if a list, array or other type of collection is null or empty.
* Added `TryFindIndex` list and array extensions to find an index of an element.
* Added additional sorted insertion extension functions for list-structures.
* Added additional logging functions to provide an object as parameter, rather than a string to quickly log the value of an object without having to convert it to string manually.
* Added a serialization processor for the `System.Version` and `System.Guid` types so these can be included in serialized data. This processor has been added to all available serialization definitions.
* Added Dependency Injection extension functions to add a component to a game object and inject it immediately.
* Added Dependency Injection extension functions to inject a scene, which will loop over all game objects in the scene.
* Added an optional injection identifier for the `Scene` and `Hierarchy` scopes, to inject their scopes to objects requiring their resources to come from specific sources only.
* Added the `CompositeDependencyContainer` type which is a resource container that is built by combining multiple resource containers. This allows to inject a large set objects in one go instead of going over them per resource container.
* Added an `IReadOnlyDependencyContainer` interface which allows to only get resources from the container. The `IDependencyContainer` now inherits from this new interface as well.
* Added a reflection caching framework to optimize and unify the way meta-data is cached and stored. The enum display names, dependency injection, Weblink and serialization frameworks now use this caching instead of their internal solutions.

### Updated

* Updated the Dependency Injection framework to allow constructor-based injection.
* Updated the Dependency Injection framework to allow static injection. If the object to inject is of type `Type`, then its static fields and properties will be injected.
* Updated most dependency injection methods to receive a read-only container now if they only require resources to be read from the container.
* Updated the `GlobalDependencyScope` to have its `GlobalScope` property be `public` instead of `internal`.
* Updated the `GlobalDependencyScope` to be able to adjust the `AutoInjectLoadedScenes` at runtime.
* Updated most frameworks to work with concurrency-safe structures. This should make (most of) this toolkit safe to use and run in multi-threaded environments.
* Updated most attribute-related functions to work with the static `Attribute` functions rather than the member functions defined on the `MemberInfo` classes to ensure the frameworks work with attribute-related data.
* Updated most internal workings to use arrays instead of the `IEnumerable<T>` interface to optimize iterating the collections.
* Updated reflection-based method invocation to use a cached parameter list to reduce garbage generation.

### Fixed

* Fixed an issue where serialization callbacks would get invoked multiple times if overrides of the same method had the callback attribute defined as well.
* Fixed an issue with the `Shuffle` extension function for lists, where certain placement patterns weren't possible.

### Breaking Changes

* The `IDependencyScope` requires the implementation of an `Inject` method now. Calling this method on your scope object should inject the objects under its scope with the resources found in its resources container. If you have a custom dependency scope type, you'll have to provide an implementation for this.

## v1.4 - Serialization Type Key Overrides & Several Fixes

Changelog:

### Added

* All serialization frameworks can now define a serialization type key override through the `KeyOverride` property on the attribute. This allows to set or infer a type based on a different or already present value in the data. The XML serialization can also define whether the type information should be saved as a child element rather than an attribute using the `SetAsElement` property on the `XmlType` attribute.
* JSON, HTTP body and Photon WebRPC serialization frameworks now also have support for (de)serializing structs.
* A `NullValueProcessor` class for dealing with `null` data during deserialization. This processor will assign a default or null when the value to deserialize is null. This processor is added to every pre-defined serialization definition in the toolkit. This also fixes issue [#6](https://github.com/juniordiscart/ImpossibleOdds-Toolkit/issues/6).

### Updated

* The `HttpMessageHandle` is updated to cope with the deprecated properties of the `UnityWebRequest` class set in Unity 2020.2. This gets rid of the warning messages in newer versions of Unity.
* The default nested message configurator classes in `HttpMessenger` and `WebRpcMessenger` have most of their methods and fields defined as virtual and/or protected, allowing for simple override behaviour without a full custom implementation.
* Loosened the type restriction for Type-defining attributes in several serialization frameworks. Instead of just instances of type `string`, instances of `object` are now accepted. This allows for other kinds of values to serve as type identifiers, e.g. enums.
* All pre-defined serialization processors now require the `targetType` parameter of their `Deserialize` method to be non-null.
* The `InvokeResponseCallback` method in the Weblink utilities framework now takes an additional generic parameter to reliably retrieve the callback method. This also fixes issue [#7](https://github.com/juniordiscart/ImpossibleOdds-Toolkit/issues/7).

### Fixed

* The type verification check of a value against generic type arguments of a list or dictionary would fail for values that were not null. See issue [#4](https://github.com/juniordiscart/ImpossibleOdds-Toolkit/issues/4).
* All registered targeted callback methods were called when a message completed instead of only the matching ones to the response. See issue [#5](https://github.com/juniordiscart/ImpossibleOdds-Toolkit/issues/5).
* The `ExactMatchProcessor` would have an unhandled scenario where it would attempt to set the wrong kind of data during deserialization.

### Removed

* The `InvokeIfNotNull` delegate extension function with dynamic set of parameters.

## v1.3 - Multi-Named Injections & Small Optimizations

Changelog:

### Added

* The global dependency injection scope can now be disabled to inject loaded scenes automatically. This prevents the scene from being scraped for injectable components when activated, and can help in reducing loading times of larger scenes. This can be toggled in the Impossible Odds section of the project's preferences panel.

### Updated

* The `Inject` attribute can now be placed multiple times on a member, allowing for different named context to inject the same member.

### Improved

* The `Log` class has now multiple overloads for each log function that can take up to three parameters, similar to the `String.Format` overloads, to minimize the amount of memory allocations that would otherwise occur from creating the list of parameters.

## v1.2 - Photon WebRPC Support & Tool Interoperability

**Note: this release contains minor breaking changes regarding the JSON tool. Please check the 'Renamed' and 'Removed' entries in the changelog below.**

Changelog:

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

* The `InvokeIfNotNull` extension method with flexible parameter list has been marked obsolete because of its unintentional fallback for otherwise invalid delegate invocations.
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

**Note: this release contains breaking changes! Check the 'Removed' and 'Renamed' entries in the changelog below.**

Changelog:

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

**Note: this release contains a breaking change! Check the 'Renamed' entries in the changelog below.**

Changelog:

### Added

* DependencyContainerExtensions collapsing frequently chained methods into a single call.

### Fixed

* The global dependency context was only injected in the scene's root game objects. Now their children are processed as well.

### Renamed

* `FunctorBinding` → `GeneratorBinding`. The file and documentation already used the right name, but the actual class did not reflect this yet. Note: this is a breaking change!

## v1.0.1 - Improved Documentation & Combined Package

Changelog:

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
