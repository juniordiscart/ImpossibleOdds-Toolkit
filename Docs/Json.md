# ![Impossible Odds Logo][Logo] C# Toolkit - JSON


The JSON data format is a commonly used format to represent and save data, as well as to exchange data between client and server systems. As such, there exist a lot of tools already that allow you to process your data to and from JSON, including a built-in Unity one. However, this last one processes your data in the same way as it processes your scripts for using with the inspector. This, of course, implies it operates with the same limitations.

This tool aims to provide simplicity along with extra control when necessary. Some of the things you can expect to do with this take on a JSON serialization tool:

* Define your data easily as a JSON object for great readability, or array for compact representation.
* Additional control over which members are saved and under what name.
* Save type information and keep the inheritance chain intact.
* Convenient callbacks when an object is being processed.

The JSON tool described here is all available in the `ImpossibleOdds.Json` namespace for you to include in your scripts.

## Setup

For your objects to be recognized and processed by the JSON tools here, you must mark them as such. There are two possible options for your objects to be picked up by the JSON processor:

* The `JsonObject` attribute will mark your object as a JSON object. Members of this object will be serialized under a name. The members you want to have serialized can be marked with the `JsonField` attribute. By default, the member's name is used, but a custom one can be provided.

* The `JsonArray` attribute will mark your object as a JSON array. Members of this object will be serialized using an index value. They can be marked with the `JsonIndex` attribute along with their index location in the resulting data.

```cs
[JsonObject]
public class Dog : Animal
{
	[JsonField]
	private Color furColor;
	[JsonField("desexed")]
	private bool neutered;
}
```

```cs
[JsonArray]
public class VeterinaryAppointment
{
	[JsonIndex(0)]
	private DateTime date;
	[JsonIndex(1)]
	private string reason;
}
```

**Note**: when serializing your object as a JSON array, keep in mind that the index is shared across the inheritance chain. If your base class defines indices 0, 1, and 2 to be used, your child classes should start counting from 3, unless you whish to override their values.

### Type Information

One of the more unique features of this JSON tool is the ability to save type information. This allows it to reconstruct your data more accurately when deserializing the JSON data. Saving this type information doesn't happen auto-magically though. You'll have to guide the JSON processor a little for it to build a known set of types it can use. This is done in similar fashion as it is done for C#'s XML processing library: adding attributes to the base class, defining which sub-classes exist, and under what name they can be saved.

By adding the `JsonType` attribute to your base classes, you can define what child classes exist. The attribute takes in a single type, assuming that it is a child class of the type it is defined on. When an instance of that type is saved, its typename will be used as a default value. However, you can define a custom value for the type yourself using its `Value` property.

```cs
[JsonObject,
JsonType(typeof(Cat)),
JsonType(typeof(Dog)),
JsonType(typeof(Crocodile), Value = "Kroko"),
JsonType(typeof(Pidgeon), Value = "Dove")]
public abstract class Animal
{
	[JsonField, JsonRequired]
	private string name;

	// Other details omitted...
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
			"jsi:type": "Kroko",
			"name": "Dundee"
		}
	]
}
```

**Note**: specifying a custom value for a type requires it to be unique in the context of this inheritance chain (this includes interfaces on which this attribute is defined).

**Another note**: serializing type information is only supported for objects that are serialized as JSON objects. It's currently not supported in this tool to save type information in JSON array objects.

As shown in the example above, the key for the type information is set to be `jsi:type`. This default key is defined by the _serialization definition_ which you will come to know more about below. However, sometimes, you may want to alter this key, perhaps because the type information can be infered from a value already present in the data itself! You can provide a `KeyOverride` and set a custom key value, optionally paired with a custom value.

```cs
[JsonEnumString]
public enum AnimalType
{
	CAT,
	DOG,
	CROCODILE,
	PIDGEON
}
```

```cs
[JsonObject,
JsonType(typeof(Cat), KeyOverride = "animalType", Value = AnimalType.CAT),
JsonType(typeof(Dog), KeyOverride = "animalType", Value = AnimalType.DOG),
JsonType(typeof(Crocodile), Value = "Kroko"),
JsonType(typeof(Pidgeon), Value = "Dove")]
public abstract class Animal
{
	[JsonField, JsonRequired]
	private string name;
	[JsonField, JsonRequired]
	private AnimalType animalType;

	public Animal(AnimalType animalType)
	{
		this.animalType = animalType;
	}

	// Other details omitted...
}
```

A potential serialized result could be:

```json
{
	"animals": [
		{
			"name": "Bobby",
			"animalType": "DOG"
		},
		{
			"name": "Salem",
			"animalType": "CAT"
		},
		{
			"name": "Dundee",
			"animalType": "CROCODILE",
			"jsi:type": "Kroko"
		}
	]
}
```

The `Cat` and `Dog` types have a key override defined that refers to a field that contains an enum value which uniquely defines their type. During deserialization, it will use this enum value to determine the actual type of the object.

### Enum String Values & Aliases

In many cases, when serializing an enum value, their string representation is much more readable as well as more maintainable when processing them. When you insert a new value in the enum and need to reshuffle them (and possibly changing their internal value), your previously serialized data isn't valid anymore. This is less so with their string representation.

Support is provided to state that an enum should be serialized under its string form rather than its internal value representation. This can be done by marking it with the `JsonEnumString` attribute.

```cs
[JsonEnumString]
public enum TaxonomyClass
{
	None,
	Mammal,
	Reptile,
	Bird
}
```

Additionally, you can define an alias for a specific enum value using the `JsonEnumAlias` attribute. This, of course, assumes you have marked the enum with the `JsonEnumString` attribute as otherwise its internal value is still used:

```cs
[JsonEnumString]
public enum TaxonomyClass
{
	None,
	Mammal,
	[JsonEnumAlias("Scaly-boy")]
	Reptile,
	[JsonEnumAlias("Birb")]
	Bird
}
```

**Note**: only a single alias can be defined per enum value.

### Required Values

At times, certain values are required to be present in order for data to be considered valid, and when absent, doesn't need further processing.

This kind of (limited) control can be exerted by placing the `JsonRequired` attribute above a member in your object that should be present at all times when processing its data.

```cs
[JsonObject]
public abstract class Animal
{
	[JsonField, JsonRequired]
	private string name;
	[JsonField]
	private float weight;

	// Other details omitted...
}
```

When this data is not present, the serialization system will halt and throw an exception upwards to let you know the data is faulty. By default, this makes the processor check whether a field is present in the JSON data, not whether this value is `null` or not. If the data is also required not to be `null`, then the `NullCheck` property can be enabled.

```cs
[JsonObject]
public abstract class Animal
{
	[JsonField, JsonRequired(NullCheck = true)]
	private string name;
	[JsonField]
	private float weight;

	// Other details omitted...
}
```

**Note**: only members on a JSON object can be marked as required. JSON arrays are not supported by this requirement feature.

## Serialization

When your objects are decorated with the proper attributes, they are ready to be serialized to the JSON data format. Simply use the `Serialize` method (or one of its many overloads) on the static `JsonProcessor` class:

```cs
public class Veterinary
{
	private AnimalRegister animals = null;

	public void SaveAnimalRegister(string path)
	{
		string jsonResult = JsonProcessor.Serialize(animals);
		File.WriteAllText(path, jsonResult);
	}
}
```

### Formatting

The above JSON processor methods both have another variant where you can provide them with a `JsonOptions` object that allows to exert a small bit control over the way the JSON output is formatted.

```cs
public class Veterinary
{
	private AnimalRegister animals = null;
	private JsonOption jsonOptions = null;

	public Veterinary()
	{
		jsonOptions = new JsonOptions();
		jsonOptions.CompactOutput = false;	// For pretty printing.
		jsonOptions.EscapeSlashCharacter = true;	// Escapes the '/' character in the output.
	}

	public void SaveAnimalRegister(string path)
	{
		string jsonResult = JsonProcessor.Serialize(animals, jsonOptions);
		File.WriteAllText(path, jsonResult);
	}
}
```

#### Advanced

The `JsonOptions` also has a `SerializationDefinition` property to provide a custom serialization definition object which defines which processors and data structures should be used during the data transformation process. When left `null`, the default one will be used.

```cs
public class Veterinary
{
	private AnimalRegister animals = null;
	private JsonOption jsonOptions = null;

	public Veterinary()
	{
		jsonOptions = new JsonOptions();
		jsonOptions.CompactOutput = false;	// For pretty printing.
		jsonOptions.EscapeSlashCharacter = true;	// Escapes the '/' character in the output.

		// Set a customized JSON serialization definition.
		JsonSerializationDefinition customDefinition = new JsonSerializationDefinition();
		customDefinition.UpdateUnityPrimitiveRepresentation(PrimitiveProcessingMethod.SEQUENCE;);
		jsonOptions.SerializationDefinition = customDefinition;
	}

	public void SaveAnimalRegister(string path)
	{
		string jsonResult = JsonProcessor.Serialize(animals, jsonOptions);
		File.WriteAllText(path, jsonResult);
	}
}
```

Some advantages of providing a custom serialization definition is, for example, when you wish to change the default key for saving type information, or if you would like that Unity-specific types (`Vector3`, `Color`, etc.) are serialized in a different way.

Please check out the [Serialization][AdvancedSerialization] documentation for more information about custom serialization definitions.

## Deserialization

Deserializing your data is (almost) as easy as serializing it. However, depending on whether you know beforehand what data you're about to process, the received result may differ.

If you don't know beforehand what the JSON data represents, you can deserialize it, and it will return you a generic data structure (most likely a `List` or `Dictionary`, depending on the JSON data) for you to search through, or further process in a way you see fit.

```cs
public class Veterinary
{
	public void LoadAnimalRegister(string path)
	{
		object unknownData = JsonProcessor.Deserialize(File.ReadAllText(path));
	}
}
```

If you do know, you can pass in a type for it to try and deserialize the JSON data into an instance of that type:

```cs
public class Veterinary
{
	private AnimalRegister animals = null;

	public void LoadAnimalRegister(string path)
	{
		animals = JsonProcessor.Deserialize<AnimalRegister>(File.ReadAllText(path));
	}
}
```

**Note**: the given target type is allowed to be a base class, or even an abstract class or interface, provided that it has the right type information available for it to be able to create an instance of the expected result. See the [Type Information](#type-information) section for more details.

## Callbacks

During the JSON (de)serialization process, objects can request to be notified when they will be (de)serialized, or when that process is done. This can help in case an object needs something done before it is being processed, e.g. pre-process or transform some data. These callbacks can be defined on methods of the target object by using the following attributes:

* `OnJsonSerializing` when the object is about to get serialized.
* `OnJsonSerialized` when the object is done being serialized.
* `OnJsonDeserializing` when the object is about to be deserialized.
* `OnJsonDeserialized` when the object is done being deserialized.

```cs
[JsonObject]
public abstract class Animal
{
	[OnJsonSerializing]
	private void OnSerializing()
	{
		Log.Info("Serializing animal of type {0} with name {1}.", this.GetType().Name, Name);
	}

	[OnJsonSerialized]
	private void OnSerialized()
	{
		Log.Info("Serialized animal of type {0} with name {1}.", this.GetType().Name, Name);
	}

	[OnJsonDeserializing]
	private void OnDeserializing()
	{
		Log.Info("Deserializing animal of type {0}. No name is available yet.", this.GetType().Name);
	}

	[OnJsonDeserialized]
	private void OnDeserialized()
	{
		Log.Info("Deserialized animal of type {0} with name {1}.", this.GetType().Name, Name);
	}
}
```

Each of these callbacks may accept a single parameter that is of type `IProcessor` (see the [Advanced Serialization][AdvancedSerialization] topic for more information). It basically functions as a serialization context object that is currently processing your data.

## Example

In the topics discussed above, you'll have read small tidbits of example code. Most of these can be read in full below.

```cs
[JsonArray]
public class VeterinaryAppointment
{
	[JsonIndex(0)]
	private DateTime date;
	[JsonIndex(1)]
	private string reason;
}
```

```cs
[JsonEnumString]
public enum TaxonomyClass
{
	None,
	Mammal,
	[JsonEnumAlias("Scaly-boy")]
	Reptile,
	[JsonEnumAlias("Birb")]
	Bird
}
```

```cs
[JsonObject,
JsonType(typeof(Cat), KeyOverride = "animalType", Value = AnimalType.CAT),
JsonType(typeof(Dog), KeyOverride = "animalType", Value = AnimalType.DOG),
JsonType(typeof(Crocodile), Value = "Kroko"),
JsonType(typeof(Pidgeon), Value = "Dove")]
public abstract class Animal
{
	[JsonField, JsonRequired]
	private string name;
	[JsonField]
	private float weight;
	[JsonField]
	private DateTime dateOfBirth;
	[JsonField("Taxonomy")]
	private TaxonomyClass classification;
	[JsonField]
	private VeterinaryAppointment nextAppointment;
	[JsonField, JsonRequired]
	private AnimalType animalType;

	public Animal(AnimalType animalType)
	{
		this.animalType = animalType;
	}

	[OnJsonSerializing]
	private void OnSerializing()
	{
		Log.Info("Serializing animal of type {0} with name {1}.", this.GetType().Name, Name);
	}

	[OnJsonSerialized]
	private void OnSerialized()
	{
		Log.Info("Serialized animal of type {0} with name {1}.", this.GetType().Name, Name);
	}

	[OnJsonDeserializing]
	private void OnDeserializing()
	{
		Log.Info("Deserializing animal of type {0}. No name is available yet.", this.GetType().Name);
	}

	[OnJsonDeserialized]
	private void OnDeserialized()
	{
		Log.Info("Deserialized animal of type {0} with name {1}.", this.GetType().Name, Name);
	}
}
```

```cs
[JsonObject]
public class Cat : Animal
{
	[JsonField]
	private Color32 furColor;
	[JsonField]
	private bool chipped;

	public Cat()
	: base(AnimalType.CAT)
	{ }
}
```

```cs
[JsonObject]
public class Dog : Animal
{
	[JsonField]
	private Color furColor;
	[JsonField]
	private bool neutered;

	public Dog()
	: base(AnimalType.DOG)
	{ }
}
```

```cs
[JsonObject]
public class AnimalRegister
{
	[JsonField(Key = "AnimalRegister")]
	private List<Animal> registeredAnimals = new List<Animal>();
}
```

```cs
public class Veterinary
{
	private AnimalRegister animals = null;
	private JsonOption jsonOptions = null;

	public Veterinary()
	{
		jsonOptions = new JsonOptions();
		jsonOptions.CompactOutput = false;	// For pretty printing.
	}

	public void LoadAnimalRegister(string path)
	{
		animals = JsonProcessor.Deserialize<AnimalRegister>(File.ReadAllText(path));
	}

	public void SaveAnimalRegister(string path)
	{
		string jsonResult = JsonProcessor.Serialize(animals, jsonOptions);
		File.WriteAllText(path, jsonResult);
	}
}
```

Check out the JSON sample scene to see this tool in action!

[Logo]: ./Images/ImpossibleOddsLogo.png
[AdvancedSerialization]: ./Serialization.md
