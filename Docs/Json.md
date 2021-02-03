# ![Impossible Odds Logo][Logo] Unity C# Toolkit - JSON

The JSON functionalty described here is all available in the `ImpossibleOdds.Json` namespace.

The JSON data format is a commonly used data format to represent and save data, as well as to exchange data between client and server systems. As such, there exist a lot of tools already that allow you to process your data to and from JSON, including a built-in Unity one. However, this last one processes your data based in the same way as it processes your scripts for using with the inspector. This, of course, implies it operates with the same limitations.

This tool aims to provide simplicity along with extra control when necessary. Some of the things you can expect to do with this take on JSON:

* Define your data easily as a JSON object or array.
* Additional control over which fields are saved and under what name.
* Save type information and keep the inheritance chain intact.
* Convenient callbacks when an object is being processed.

## Setup

For your classes to be recognized and processed by the JSON tools here, you must mark them as such. There are two possible options for your classes to be picked up by the JSON processor:

* The `JsonObject` attribute will mark your class as a JSON object. Fields of this class will be serialized under a name. The fields you want to have serialized can be marked with the `JsonField` attribute. By default, the field's name is used, but a custom one can be provided.

* The `JsonArray` attribute will mark your class as a JSON array. Fields of this class will be serialized using an index value. They can be marked with the `JsonIndex` attribute along with their index location.

```cs
// Serialize the class as a JSON object.
// Fields can be serialized under a different name.
[JsonObject]
public class MyClassAsObject
{
	[JsonField]
	private string name;
	[JsonField("Value")] // Optionally provide a different name.
	private int score;
}

// Serialize the class as a JSON array.
// Fields must be assigned an index.
[JsonArray]
public class MyClassAsArray
{
	[JsonIndex(0)]
	private string name;
	[JsonIndex(1)]
	private int score;
}
```

**Note**: when serializing your object as a JSON array, keep in mind that the index is shared across the inheritance chain. If your base class defines indices 0, 1, and 2 to be used, your child classes should start counting from 3, unless you whish to override their values.

### Type Information

One of the more unique features of this JSON toolkit is the ability to save type information. This allows it to reconstruct your data more accurately when deserializing the JSON data. Saving this type information doesn't happen auto-magically though. You'll have to guide the JSON processor a little for it to build a known set of types it can use. This is done in similar fashion as it is done for C#'s XML processing library: adding attributes to the base class, defining which sub-classes exist, and under what name they can be saved.

By adding the `JsonType` attribute to your base classes, you can define what child classes exist. The attribute takes in a single type, assuming that it is a child class of the type it is defined on. When an instance of that type is saved, its typename will be used as a default value. However, you can define a custom value for the type yourself using its `Value` property.

```cs
[JsonType(typeof(Dog)),
JsonType(typeof(Cat)),
JsonType(typeof(Rabbit), Value = "Bunny")]
public abstract class Animal
{
	[JsonField]
	private string name;
}
```

A possible output for a list of animals:

```json
{
	"animals": [
		{
			"jsi:type": "Dog",
			"name": "Bobby"
		},
		{
			"jsi:type": "Cat",
			"name": "Salem"
		},
		{
			"jsi:type": "Bunny",
			"name": "Paprika"
		}
	]
}
```

**Note**: specifying a custom value for a type requires it to be unique in the context of this inheritance chain (this includes interfaces on which this attribute is defined).

**Another note**: serializing type information is only supported for classes that are serialized as JSON objects. It's currently not supported in this tool to save type information in JSON array objects.

### Enum String Values & Aliases

In many cases, when serializing an enum value, their string representation is much more readable as well as more maintainable when processing them. When you insert a new value in the enum and need to reshuffle them (and possibly changing their internal value), your previously serialized data isn't valid anymore. This is less so with their string representation.

Support is provided to state that an enum should be serialized under its string form rather than its internal value representation. This can be done by marking it with the `JsonEnumString` attribute.

```cs
[JsonEnumString]
public enum MyEnum
{
	None,
	First,
	Second,
	Last
}
```

Additionally, you can define an alias for a specific enum value using the `JsonEnumAlias` attribute. This, of course, assumes you have marked the enum with the `JsonEnumString` attribute as otherwise its internal value is used.

```cs
[JsonEnumString]
public enum MyEnum
{
	None,
	[JsonEnumAlias("1st")]
	First,
	[JsonEnumAlias("2nd")]
	Second,
	Last
}
```

**Note**: only a single alias can be defined per enum value.

### Required Values

At times, certain values are required to be present in order for data to be considered valid, and when absent, doesn't need further processing.

This kind of (limited) control can be exerted by placing the `JsonRequired` attribute above a field in your object that should be present at all times when processing its data.

```cs
[JsonObject]
public class MyObject
{
	[JsonField, JsonRequired]
	private string name;
}
```

When this data is not present, the serialization system will halt and throw an exception upwards to let you know the data is faulty.

**Note**: only fields on a JSON object can be marked as valid. JSON arrays are not supported by this requirement feature.

**Another note**: a field marked as required means that its key is expected to be present in the JSON object, not necessarily that its value can't be `null`. The reason for this is that data explicitly set to `null` can still be valid, while data not present might mean an error on the side where the data is generated.

## Serialization

When your classes are decorated with the proper attributes, they are ready to be serialized to the JSON data format. Simply call the following method:

```cs
MyClass myObject;
string jsonResult = JsonProcessor.Serialize(myObject);
```

This produces a JSON-compliant string respresentation of your data. There's also a variant available that will take a `StringBuilder`. This can be useful in case you want to append or generate the result in a more memory-efficient way.

```cs
MyClass myObject;
StringBuilder resultStore;
JsonProcessor.Serialize(myObject, resultStore);
```

**Note**: the `StringBuilder` will not be cleared by the processor before use. The result is simply appended.

### Formatting

The above JSON processor methods both have another variant where you can provide them with a `JsonOptions` object that allows to exert a small bit control over the way the JSON output is formatted.

```cs
JsonOptions options = new JsonOptions();
options.CompactOutput = true;	// Writes the whole JSON result on a single line and leaves out formatting whitespace characters.
options.EscapeSlashCharacter = true;	// Escapes the '/' character in the output.

MyClass myObject;
JsonProcessor.Serialize(myObject, options);
```

#### Advanced

The `JsonOptions` also has a `SerializationDefinition` property to provide a custom JSON serialization definition object which defines which processors and data structures should be used during the data transform process. When left `null`, the default internal one will be used.

Please check out the [Serialization][AdvancedSerialization] documentation for more information about custom serialization definitions.

## Deserialization

Deserializing your data is (almost) as easy as serializing it. However, depending on whether you know beforehand what data you're about to process, the output result may differ.

If you don't know beforehand what the JSON data represents, you can deserialize it, and it will return you a generic data structure (most likely a `List` or `Dictionary`, depending on the JSON data) for you to search through, or further process in a way you see fit.

```cs
// When not knowing what will be deserialized, the returned result will be a
// generic data structure (List, Dictionary) which can be used for further processing.
string jsonData;
object result = JsonProcessor.Deserialize(jsonData);
```

If you do know, you can pass in a type for it to try and deserialize the JSON data into an instance of that type.

```cs
// When the type of the data is known beforehand, but no instance is available.
string jsonData;
MyClass myObject = JsonProcessor.Deserialize<MyClass>(jsonData);
```

**Note**: the given target type is allowed to be a base class, or even an abstract class or interface, provided that it has the right type information available for it to be able to create an instance of the expected result. See the [Type Information](#type-information) section for more details.

Lastly, you can already pass an instance of the expected type and the processor will try to map the data onto it.

```cs
// When the type of the data to be deserialized is known beforehand,
// instruct the processor to process to the target instance of the object.
MyClass myObject;
string jsonData;
JsonProcessor.Deserialize(myObject, jsonData);
```

## Callbacks

During the JSON (de)serialization process, objects can request to be notified when they will be (de)serialized, or when that process is done. This can help in case an object needs something done before it is being processed, e.g. pre-process or transform some data. These callbacks can be defined on methods of the target object by using the following attributes:

* `OnJsonSerializing` when the object is about to get serialized.
* `OnJsonSerialized` when the object is done being serialized.
* `OnJsonDeserializing` when the object is about to be deserialized.
* `OnJsonDeserialized` when the object is done being deserialized.

```cs
[JsonObject]
public class MyClass
{
	[OnJsonSerializing]
	private void Serializing()
	{
		Log.Info("Serializing instance of {0}.", this.GetType().Name);
	}

	[OnJsonSerialized]
	private void Serialized()
	{
		Log.Info("Serialized instance of {0}.", this.GetType().Name);
	}

	[OnJsonDeserializing]
	private void Deserializing()
	{
		Log.Info("Deserializing instance of {0}.", this.GetType().Name);
	}

	[OnJsonDeserialized]
	private void Deserialized()
	{
		Log.Info("Deserialized instance of {0}.", this.GetType().Name);
	}
}
```

**Note**: methods decorated with one of these attributes shouldn't have any parameters.

## Example

Check out the JSON sample scene for a hands-on example!

[Logo]: ./Images/ImpossibleOddsLogo.png
[AdvancedSerialization]: ./Serialization.md
