# ![Impossible Odds Logo][Logo] C# Toolkit - Changelog

You'll find the full update history of this toolkit below. This might be useful for checking breaking changes.

## v1.1 - XML Support & Serialization Performance

**Note: this release contains breaking changes! Check the 'Removed' and 'Renamed' entries in the changelist below.**

Changelist:

* Added: XML serialization support. Check the XML section in the documentation to read all the details.
* Added: Support for (de)serializing properties of objects instead of only fields.
* Added: Enum alias caching for faster retrieval during (de)serialization.
* Added: Support for aliases on enums decorated with the `System.Flags` attribute.
* Added: A generic `Serialize` method on the `Serializer` class, for immediately receiving the typed result.
* Added: Required fields in JSON or XML can now enable the `NullCheck` property to perform a value check as well. When the value in the source data is null, it will stop the deserialization process.
* Improved: Serialization of Unity primitive values (`Vector3`, `Color`, etc.) can be modified with a preferred processing method, e.g. as a JSON object versus JSON array, or using XML elements versus XML attributes. This can be configured globally in the Impossible Odds project settings window, and per serialization definition.
* Improved: Serialization and deserialization of custom objects have improved memory usage by skipping several intermediate steps.
* Improved: HTTP and JSON serialization definitions now use instances of `ArrayList` instead of `List<object>` to skip the generic type checking steps.
* Improved: Objects requesting a serialization callback can now have an optional parameter of type `IProcessor` to get some context of what is processing the it.
* Improved: The `SerializationUtilities` class now contains clearly defined functions and descriptions.
* Improved: Exceptions thrown during serialization have a more clear description of what might have gone wrong.
* Improved: JSON example to include more logging and made the JSON output interactable to experiment with the contents.
* Fixed: Enum alias deserialization issue.
* Removed: Several unused interfaces in the `ImpossibleOdds.Serialization` namespace.
* Removed: Several abstract classes in the `ImpossibleOdds.Serialization` namespace that would 'glue' several unrelated classes together.
* Renamed: Several dependency injection related classes and interfaces:
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

* Renamed: FunctorBinding → GeneratorBinding. The file and documentation already used the right name, but the actual class did not reflect this yet. Note: this is a breaking change!
* Added: DependencyContainerExtensions collapsing frequently chained methods into a single call.
* Fixed: The global dependency context was only injected in the scene's root game objects. Now their children are processed as well.

## v1.0.1 - Improved Documentation & Combined Package

Small update that unifies the packages, making it the same setup and layout as you would download it from the Unity Asset Store.

Changelist:

* Added a combined documentation file in the package.
* Added the examples to the package.
* Improved documentation and added some more flavor to the Core Utilities description.
* Updated the string extension functions to throw an ArgumentException instead of an ArgumentNullException if the string argument is empty or just white space characters.

## v1.0 - Initial Release

The first release of the Impossible Odds - C# Unity Toolkit!

The following features are included in this first release:

* Core utilities
* Dependency Injection
* Runnables
* JSON serialization
* HTTP communication

[Logo]: ./Docs/Images/ImpossibleOddsLogo.png
