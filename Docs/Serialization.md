# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Serialization

The idea behind the tools found in the `ImpossibleOdds.Serialization` namespace is to be able to deconstruct complex objects into queryable and well-known datastructures, enabling further and easy post-processing to any other kind of data format. The other way should also hold true: given a structured set of known data types, it should be able to piece this data back to an instance of a complex object.

This is achieved using a _serialization definition_, which defines how objects should be (de)constructed and a set of data processing units that are specialised in handling specific kinds of data.

The structure of the tools described here aim to provide a flexible framework to easily define or provide support for custom data formats. This is proven by the [JSON][Json] and [HTTP][Http] tools found in this toolkit as well as the [Photon Extensions][PhotonExtensions] package to help speed up multiplayer development in the Photon networking framework.

## Serialization Definitions

A serialization definition defines which features are supported by the target data format and how it will treat the data. It does so in the following ways:

* The implemented interfaces define which kind of data constructs and features are supported,
* A set of natively supported types that don't require any further processing, and
* A set of data processors that will break down more complex data into supported structures and natively supported types.

A bare-minimum serialization definition is created using the `ISerializationDefinition` interface. By implementing more specific interfaces, more features can be enabled to enhance the data processing process.

### Natively Supported Types

A native type in the context of a serialization definition, as opposed to a C# language construct, is a type that can be used directly, without it needing further processing. In most cases those would be types such as `int`, `string` or `bool`. However, a serialization definition can define any set of types that are natively supported by the data format it is intended for.

The list of natively supported types by the serialization definition can be queried using its `SupportedTypes` property.

### Object Processing

Whenever a type isn't natively supported, it must be processed to smaller chunks of data that are supported. A serialization definition defines a list of _data processors_, which will attempt, each in turn, to process the given data into smaller pieces. You can read more details about them in the [Data Processors](#data-processors) section.

This list of data processors is run through sequentially whenever a piece of data is to be processed. A processor first decides if it can handle the given type of data. If it can't, it won't touch it and notify the caller. If it does accept the piece of data, it is expected to completely process it into chunks of usable data.

These processors may work recursively. They all have access to the serialization definition they belong to, and will call the `Serialize` or `Deserialize` method of the [`Serializer`][Serializer] to further process any piece of sub-data.

This list of processors defined by the serialization definition can be queried using its `SerializationProcessors` and `DeserializationProcessors` properties.

### Index-based Definitions

When a serialization definition also implements the `IIndexSerializationDefinition` interface, it states that it supports index-based datastructures such arrays and lists.

This interface requires two attributes to be defined to aid the framework in processing custom objects:

* The _index-based class marking_ attribute that states that an object wishes to be processed as an index-based datastructure, and
* The _index-based field marking_ attribute that is to be placed over the fields that need to be processed of that class.

For example, the serialization definition used by the [JSON][Json] framework has support for index-based datastructures and has defined the `JsonArray` and `JsonIndex` attributes to process custom objects to array-like objects:

```cs
[JsonArray]
public class MyClass
{
	[JsonIndex(0)]
	private int value1;
	[JsonIndex(1)]
	private string value2;
	[JsonIndex(2)]
	private bool value3;
}
```

The generic variant of the `IIndexSerializationDefinition` imposes restrictions on the type of these attributes. They should implement the `IIndexDataStructure` (class marking) and `IIndexParameter` (field marking) interfaces.

### Lookup-based Definitions

The `ILookupSerializationDefinition` interfaces provides a serialization definition with support for lookup-based data structures such as dictionaries.

Just like the index-based definition interface, it requires two attributes to be defined:

* The _lookup-based class marking_ attribute which states that an object wishes to be processed as a lookup-based datastructure, and
* The _lookup-based field marking_ attribute that is to be placed over the fields that need to be processed of that class.

For example, the same [JSON][Json] serialization definition supports lookup-based datastructures using the `JsonObject` and `JsonField` attributes:

```cs
[JsonObject]
public class MyClass
{
	[JsonField]
	private int value1;
	[JsonField]
	private string value2;
	[JsonField("OtherValue")]
	private bool value3;
}
```

The generic variant of the `ILookupSerializationDefinition` imposes restrictions on the type of these attributes. They should implement the `ILookupDataStructure` (class marking) and `ILookupParameter` (field marking) interfaces.

### Type Resolve Definitions

This framework also allows to add type resolve support to a serialization definition by implementing the `IIndexBasedTypeResolve` and/or `ILookupBasedTypeResolve` interfaces.

When one of these, or both, are implemented by the serialization definition, it defines that it supports saving type information to keep the inheritance structure intact. This is especially useful when re-constructing the object from a data structure.

Each interface that represents type resolve is supported by an attribute that can be placed multiple times above a class. This attribute works similarly to [C#'s XmlType attribute][XmlTypeAttribute]. It is placed above (one of) the base classes, and defines which sub-classes can be expected/used during the serialization process.

For example, the [JSON][Json] serialization definition implements support for the lookup-based type resolve technique by using the `JsonTypeResolve` attribute:

```cs
[JsonTypeResolve(typeof(Dog)),
JsonTypeResolve(typeof(Cat))]
public class Animal
{ }

public class Dog : Animal
{ }

public class Cat : Animal
{ }

// Not supported as it is not defined on the base class.
public class Rabbit : Animal
{ }
```

### Callback Definitions

When a serialization definitions allows the processing of complex custom objects, it might be useful for these objects to be notified when they're about to be processed. When the serialization definition implements the `ISerializationCallbacks` interface, the custom object may define methods that should be called whenever an action is about to happen or has completed. There's support for the following callbacks:

* When an object is about to be serialized. This could be useful when some data should be transformed on the object beforehand and is only valid in the context of the serialization process.
* When an object is done serializing. Might be of use to clean up any left-over data.
* When an object is about to be deserialized.
* When an object is done being deserialized. This can be of use to perform some kind of initialization already that can restore the state based of on the data applied during deserialization.

## Data Processors

A serialization definition essentially defines _what is supported_ by the data format it represents, but a data processor does the actual work. The serialization definition has access to a list of such data processors.

The order of these data processors as they are found in the list of the serialization definition is important. A data processor is asked first whether it is capable of processing the object. If it refuses, it will be passed on to the next data processor in line. When it accepts the object, it is expected to return a transformed result. If an object is not accepted by any data processor in the serialization definition's list of processors, then that means that object cannot be processed.

Every data processor should be designed to tackle a single kind of data, e.g. only transform primitive types, `DateTime` or `Vector3` structs, a complex object to a lookup-based data structure, etc.

Implementing a custom data processor can be done using the `ISerializationProcessor` and `IDeserializationProcessor` interfaces. The former is used when a processor supports serializing a certain kind of data, while the latter defines support for deserialization as well. In most cases a data processor implements both of these, but there are rare cases where this is not necessary. There's also a `IDeserializationToTargetProcessor` interface for a data processor that is capable of directly deserializing data to a target instance of an object already, whereas the `IDeserializationProcessor` interface is expected to create its own instances to which it maps the data to.

All this may sound pretty abstract, and providing a simple example for the sake of having an example is not useful. Instead it is recommended to look at the many predefined data processors present in this toolkit. Here's a summary of the data processors that are present already:

* `ExactMatchProcessor`: checks whether the given data is of a natively supported type and doesn't need any actual processing.
* `EnumProcessor`: processes enum values and their potential alias values.
* `PrimitiveTypeProcessor`: deals with all of C#'s primitive types and tries to convert them to a natively supported type.
* `StringProcessor`: deserializer-only processor to convert a string to a different type, e.g. a string value that represents a float value.
* `DecimalProcessor`: provides support for the `decimal` type.
* `DateTimeProcessor`: provides support for the `DateTime` type.
* `Vector2Processor`: provides support for the `Vector2` type.
* `Vector2IntProcessor`: provides support for the `Vector2Int` type.
* `Vector3Processor`: provides support for the `Vector3` type.
* `Vector3IntProcessor`: provides support for the `Vector3Int` type.
* `Vector4Processor`:  provides support for the `Vector4` type.
* `QuaternionProcessor`: provides support for the `Quaternion` type.
* `ColorProcessor`: provides support for the `Color` type.
* `Color32Processor`: provides support for the `Color32` type.
* `LookupProcessor`: processes actual dictionary-like data structures, e.g. objects of type `Dictionary<TKey, TValue>`.
* `SequenceProcessor`: processes actual array-like data structures, e.g. objects of type `List<T>` or `T[]`.
* `CustomObjectSequenceProcessor`: processes custom objects to/from an array-like data structure.
* `CustomObjectLookupProcessor`: processes custom objects to/from a dictionary-like data structure.

**Note**: all of these data processors have access to the serialization definition they belong to. When they deal with complex data, like the `SequenceProcessor` or `CustomObjectLookupProcessor`, they may call the (de)serialization process again on an individual element they think needs further processing. This way, complex objects are broken down into smaller and manageble pieces.

### Serialization Callbacks

The `CustomObjectSequenceProcessor` and `CustomObjectLookupProcessor` support providing a callback to the object they're about to (de)serialize when the serialization definition implements the `ISerializationCallbacks` interface.

## Serializer

When a serialization definition is armed with its set of natively supported types and list of data processors, an object can then be transform to/from a structured dataset by handing it over to the static `Serializer` class. It implements a `Serialize` and `Deserialize` method.

It operates by taking the set of processors defined by the definition and going by them one by one, to see if a data processor is willing to accept the data. If not, it continues to the next one. If no data processor was capable of handling the data, it will throw an exception to let know that it failed in doing so.

[Logo]: ./Images/ImpossibleOddsLogo.png
[Json]: ./Json.md
[Http]: ./Http.md
[Serializer]: #serializer
[XMLTypeAttribute]: https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmltypeattribute
[PhotonExtensions]: https://github.com/juniordiscart/ImpossibleOdds-PhotonExtensions
