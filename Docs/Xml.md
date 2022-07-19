# ![Impossible Odds Logo][Logo] C# Toolkit - XML

XML is a structured data format that is commonly used in a wide area of applications and is able to describe complex objects in a human readable way. The C# language already provides support for XML out of the box by just including the `System.Xml` namespace in your script. One big downside of this built-in tool is that only public fields and properties can be processed, or you need to start fiddling with the `DataContractSerializer` which applies namespace values overzealously and is not very intuitive to use.

This toolkit's aim on XML is to provide an easy to use tool with a great degree of control without having to write custom functions per class get the desired behaviour:

* Define whether members are serialized as child elements or attributes.
* Support for CDATA sections.
* Save type information and keep the inheritance chain intact.
* Serialization callbacks when your objects are about to be processed.

The XML functionality described here is all available in the `ImpossibleOdds.Xml` namespace. Include this in your script to get started!

## Setup

To have your custom objects be recognized by the XML processor, they must be decorated with the `XmlObject` attribute. This notifies the XML framework that your object should be taken into consideration. To create child nodes in the XML element, you can use two different kinds of attributes on the members:

* The `XmlElement` attribute creates a child element of the element, or
* The `XmlAttribute` attribute creates an attribute on the element.

Both of these can also take an optional key, which will override the name of the attribute or element of the value. When left empty, it will use the member's name as defined in the object.

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name")]
	private string name = string.Empty;
	[XmlElement("Biography")]
	private string bio = string.Empty;
	[XmlElement("DateOfBirth")]
	private DateTime dateOfBirth = DateTime.MinValue;
}
```

A serialized result of an `Actor` object could look something like this:

```xml
<Actor Name="Bob Odenkirk">
  <Biography>Robert John Odenkirk was born in Berwyn...</Biography>
  <DateOfBirth>1962-10-22</DateOfBirth>
</Actor>
```

**Note**: XML attributes can only represent a singular, non-complex value. Keep this in mind when placing it above a member!

When your object would function as the root of the XML document, you can provide the `XmlObject` attribute with an optional `RootName` value. If no root name is specified, then the name of the type is used.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	// Implementation details omitted...
}
```

The XML document's root would look like this:

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<IMDB>
  Inside elements omitted...
</IMDB>
```

### Arrays and lists

When your object contains a list or array with data that needs to be included in the XML document, you can use the `XmlListElement` attribute on that member. It allows you to define a custom name for the elements in the list using the optional `EntryName` property. When left open, the default name `Entry` will be used.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	[XmlListElement("Actors", EntryName = "Actor")]
	private List<Actor> actors = new List<Actor>();
```

An example output of such an object could be:

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<IMDB>
  <Actors>
    <Actor Name="Bob Odenkirk">
      <Biography>Robert John Odenkirk was born in Berwyn...</Biography>
      <DateOfBirth>1962-10-22</DateOfBirth>
    </Actor>
    <Actor Name="Christian Bale">
      <Biography>Christian Charles Philip Bale was born in Pembrokeshire...</Biography>
      <DateOfBirth>1974-01-30</DateOfBirth>
    </Actor>
  </Actors>
</IMDB>
```

### CDATA

One of the more unique features of XML is the ability to embed pieces of arbitrary data, that is to say, it's contents are not altered/processed by the internal XML parser. This allows, for example, adding the binary data of an image to be inserted right in between the other, structured data. In the XML document, this is stored in a CDATA section, and it is also supported by this XML tool! Simply place the `XmlCData` attribute above a member of your object, and its binary representation will be included!

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name"), XmlRequired(NullCheck = true)]
	private string name = string.Empty;
	[XmlElement("Biography")]
	private string bio = string.Empty;
	[XmlElement("DateOfBirth")]
	private DateTime dateOfBirth = DateTime.MinValue;
	[XmlCData("ProfileCapture")]
	private byte[] profileCapture = null;
}
```

```xml
<Actor Name="Bob Odenkirk">
  <Biography>Robert John Odenkirk was born in Berwyn...</Biography>
  <DateOfBirth>1962-10-22</DateOfBirth>
  <ProfileCapture><![CDATA[AABJkiRJkiRlKUUp/////wAASZIkSZIkZSlFKf////]]></ProfileCapture>
</Actor>
```

However, it is important to know what the gotcha's are for using this:

* When not already a set of bytes, it will use a `BinaryFormatter` to process the data to `byte[]`. This implies that the data being sent through here adheres to the requirements of the `BinaryFormatter` class. Check out [this link][BinaryFormatterInfo] for more information.
* As a final step, the XML processor will always try to convert the result to a Base64 encoded string value, since the CDATA section in the XML document expects a string value.

### Type Information

When working with complex structured data, it's important to keep track of which types you're dealing with. Including type information in the document is vital, and also easy. This ensures that, when transforming the XMl document back to data to work with in your project, you'll know you have the properly constructed instance.

On your base class or interface, apply one or multiple `XmlType` attributes, and state which types can inherit from it, or implement it.

```cs
[XmlType(typeof(Movie)),
XmlType(typeof(Series))]
public abstract class Production
{
	// Details omitted...
}
```

```cs
[XmlObject]
public class Movie : Production
{
	// Details omitted...
}
```

```cs
[XmlObject]
public class Series : Production
{
	// Details omitted...
}
```

When adding these to the movie database example from earlier, the output looks something like this:

```xml
<?xml version="1.0" encoding="utf-16"?>
<IMDB xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Productions>
    <Production Name="Better Call Saul" xsi:type="Series" />
    <Production Name="The Dark Knight" xsi:type="Movie" />
  </Productions>
</IMDB>
```

Adding type information to the XML document can also be fully customized. The following properties are available to override on the `XmlType` attribute:

* The `Value` property defines a custom value that associates a value with a type. For example, you can fill in a string value to provide an alias for the type, or provide an enum value from which you cna infer the type.
* By default, the type information is set as an attribute of the XML element. However, by setting the `SetAsElement` property to `true`, you can have it be set as a child element instead.
* The `KeyOverride` property allows you to override the key under which the type data is saved. By default this is put under the default XML `xsi:type` name.

With the previous example adapted, it could look something like this:

```cs
[XmlEnumString]
public enum ProductionType
{
	MOVIE,
	SERIES
}
```

```cs
[XmlType(typeof(Movie), KeyOverride = "ProductionType", Value = ProductionType.MOVIE, SetAsElement = true),
XmlType(typeof(Series))]
public abstract class Production
{
	// Details omitted...
}
```

```xml
<?xml version="1.0" encoding="utf-16"?>
<IMDB xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Productions>
    <Production Name="Better Call Saul" xsi:type="Series" />
    <Production Name="The Dark Knight">
        <ProductionType>MOVIE</ProductionType>
    </Production>
  </Productions>
</IMDB>
```

### Enum String Values & Aliases

Enum value names in code often have a more readable meaning than just an integer value to distinguish between enabled features or modes. However, depending on which coding conventions you follow, enum names might still look harsh when put in a readable document such as XML. Enter enum aliases!

By denoting that your enum prefers to be serialized as a string (rather than its underlying value) using the `XmlEnumString` attribute, along with a `XmlEnumAlias` on each enum field that requires an alias, you have great control over how your data is displayed in the document. Even when your enum represents a combination of flagged values, they will get displayed properly.

```cs
[Flags, XmlEnumString]
public enum Genre
{
	[XmlEnumAlias("Undefined")]
	UNKNOWN = 0,
	[XmlEnumAlias("Action")]
	ACTION = 1 << 0,
	[XmlEnumAlias("Adventure")]
	ADVENTURE = 1 << 1,
	[XmlEnumAlias("Horror")]
	HORROR = 1 << 2,
	[XmlEnumAlias("Comedy")]
	COMEDY = 1 << 3,
	[XmlEnumAlias("Science Fiction")]
	SCI_FI = 1 << 4,
	[XmlEnumAlias("Drama")]
	DRAMA = 1 << 5,
	[XmlEnumAlias("Thriller")]
	THRILLER = 1 << 6,
	[XmlEnumAlias("Animation")]
	ANIMATION = 1 << 7,
}
```

When a movie production gets labeled with a mixture of genres, it will get displayed as follows:

```xml
<Production Name="Green Book" xsi:type="Movie">
  <Genre>Comedy, Drama</Genre>
</Production>
```

**Note**: the `,` character is used to separate flag values. Do not include this character in your custom value names, which may throw off the internal enum parser.

### Required Values

In some cases, you might want to enforce that certain values are present in the XML document. This is nothing compared to full blown XML Schema processing and verification, but it's a small step towards guaranteeing certain data is present in the XML document.

This can be enforced by placing the `XmlRequired` attribute on the members that need this.

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name"), XmlRequired]
	private string name = string.Empty;
	[XmlElement("Biography")]
	private string bio = string.Empty;
	[XmlElement("DateOfBirth")]
	private DateTime dateOfBirth = DateTime.MinValue;
	[XmlCData("ProfileCapture")]
	private byte[] profileCapture = null;
}
```

When this data is not present, the serialization system will halt and throw an exception upwards to let you know the data is faulty. By default, this makes the processor check whether an element or attribute is present in the XML data, not whether this value is `null` or not. If the data is also required not to be `null`, then the `NullCheck` property can be enabled.

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name"), XmlRequired(NullCheck = true)]
	private string name = string.Empty;
	[XmlElement("Biography")]
	private string bio = string.Empty;
	[XmlElement("DateOfBirth")]
	private DateTime dateOfBirth = DateTime.MinValue;
	[XmlCData("ProfileCapture")]
	private byte[] profileCapture = null;
}
```

## Serialization

When all of the objects that need to be picked up for serialization have been properly decorated with the right XML attributes, it's time to actually transform the data to an XML representation. This is done using a single call to one of the many `Serialize` methods on the static `XmlProcessor` class:

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	public static void Save(MovieDatabase movieDB, string path)
	{
		using (StreamWriter writer = File.CreateText(path))
		{
			XmlProcessor.Serialize(movieDB, writer);
		}
	}
}
```

The `XmlProcessor` offers several overloads of the `Serialize` method to write the result more memory efficiently, or to append to an earlier result.

### Formatting

To steer the serializer somewhat in terms of formatting and encoding, you can use the `XmlOptions` class in each of the overloads of the `Serialize` method.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	public static void Save(MovieDatabase movieDB, string path)
	{
		XmlOptions xmlOptions = new XmlOptions();
		xmlOptions.CompactOutput = false;	// For pretty printing.
		xmlOptions.Encoding = Encoding.UTF8;	// Encoding. Might still be overridden by the writer.
		xmlOptions.HideHeader = true;	// To hide the XML header in the document.

		using (StreamWriter writer = File.CreateText(path))
		{
			XmlProcessor.Serialize(movieDB, options, writer);
		}
	}
}
```

#### Advanced

The `XmlOptions` also has a `SerializationDefinition` property to provide a custom XML serialization definition, an object that defines _how_ data should be transformed to/from the XML format. You can set and change several settings to customize the serialization behaviour, such as how several built-in Unity types should be handled (values as child elements or attributes), or update the way CDATA sections should be handled when transforming data to a binary format, etc.

Please check out the [Serialization][AdvancedSerialization] documentation for more information about custom serialization definitions.

## Deserialization

Deserializing an XML document is done at the same place where serialization takes places: the `XmlProcessor`. It has several different overloads for its `Deserialize` method, depending on how much you know beforehand the XML data will contain.

When you don't know what's in the XML document, you can simply provide the raw string or a text reader to the `Deserialize` method and it will produce an `XDocument` instance. This allows you to inspect, query and search through the data of the document.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	public static XDocument Load(string path)
	{
		return XmlProcessor.Deserialize(File.OpenText(path));
	}

	// Other details omitted...
}
```

If you do know what you're deserializing, or figured it out by querying the XML document, you can provide the expected type to the `XmlProcessor` and it will attempt unwrap the XML data to a new instance of that type.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	public static MovieDatabase Load(string path)
	{
		return XmlProcessor.Deserialize<MovieDatabase>(File.OpenText(path));
	}

	// Other details omitted...
}
```

## Callbacks

When your data is being processed by the `XmlProcessor` during (de)serialization, objects can request to be notified before/after when they're being handled. This might be helpful when you have some specific data that needs to processed prior ending up in the result. There are four types of callbacks you can request using several attributes:

* `OnXmlSerializing` when the object is about to get serialized.
* `OnXmlSerialized` when the object is done being serialized.
* `OnXmlDeserializing` when the object is about to be deserialized.
* `OnXmlDeserialized` when the object is done being deserialized.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	[OnXmlSerializing]
	private void OnSerializing()
	{
		SerializationLog.AppendLine("Serializing the movie database.");
	}

	[OnXmlSerialized]
	private void OnSerialized()
	{
		SerializationLog.AppendLine("Serialized the movie database.");
	}

	[OnXmlDeserializing]
	private void OnDeserializing()
	{
		SerializationLog.AppendLine("Deserializing the movie database.");
	}

	[OnXmlDeserialized]
	private void OnDeserialized()
	{
		SerializationLog.AppendLine("Deserialized the movie database.");
	}

	// Other details omitted...
}
```

Each of these callbacks may accept a single parameter that is of type `IProcessor` (see the [Advanced Serialization][AdvancedSerialization] topic for more information). It basically functions as a serialization context object that is currently processing your data. In the context of XML, this processor will be of type `XmlCustomObjectProcessor`.

## Example

```cs
[Flags, XmlEnumString]
public enum Genre
{
	[XmlEnumAlias("Undefined")]
	UNKNOWN = 0,
	[XmlEnumAlias("Action")]
	ACTION = 1 << 0,
	[XmlEnumAlias("Adventure")]
	ADVENTURE = 1 << 1,
	[XmlEnumAlias("Horror")]
	HORROR = 1 << 2,
	[XmlEnumAlias("Comedy")]
	COMEDY = 1 << 3,
	[XmlEnumAlias("Science Fiction")]
	SCI_FI = 1 << 4,
	[XmlEnumAlias("Drama")]
	DRAMA = 1 << 5,
	[XmlEnumAlias("Thriller")]
	THRILLER = 1 << 6,
	[XmlEnumAlias("Animation")]
	ANIMATION = 1 << 7,
}
```

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name"), XmlRequired(NullCheck = true)]
	private string name = string.Empty;
	[XmlElement("Biography")]
	private string bio = string.Empty;
	[XmlElement("DateOfBirth")]
	private DateTime dateOfBirth = DateTime.MinValue;
	[XmlCData("ProfileCapture")]
	private byte[] profileCapture = null;
}
```

```cs
[XmlType(typeof(Movie), Value = "Movie"),
XmlType(typeof(Series), Value = "Series")]
public abstract class Production
{
	[XmlAttribute, XmlRequired]
	private string name = string.Empty;
	[XmlElement]
	private float score = 0f;
	[XmlElement]
	private Genre genre = Genre.UNKNOWN;
	[XmlElement]
	private string director = string.Empty;
	[XmlListElement(EntryName = "Actor")]
	private string[] actors = null;
}
```

```cs
[XmlObject]
public class Movie : Production
{
	[XmlElement]
	private int releaseYear = 0;
	[XmlCData("Poster")]
	private byte[] poster = null;
}
```

```cs
[XmlObject]
public class Series : Production
{
	[XmlElement]
	private int nrOfEpisodes = 0;
	[XmlElement]
	private int nrOfSeasons = 0;
	[XmlElement]
	private DateTime runningSince = DateTime.MinValue;
	[XmlElement]
	private DateTime endedOn = DateTime.MinValue;
}
```

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	public static MovieDatabase Load(string path)
	{
		return XmlProcessor.Deserialize<MovieDatabase>(File.OpenText(path));
	}

	public static void Save(MovieDatabase movieDB, string path)
	{
		XmlOptions xmlOptions = new XmlOptions();
		xmlOptions.CompactOutput = false;

		using (StreamWriter writer = File.CreateText(path))
		{
			XmlProcessor.Serialize(movieDB, options, writer);
		}
	}

	[XmlListElement("Actors", EntryName = "Actor")]
	private List<Actor> actors = new List<Actor>();
	[XmlListElement("Productions", EntryName = "Production")]
	private List<Production> productions = new List<Production>();

	[OnXmlSerializing]
	private void OnSerializing()
	{
		SerializationLog.AppendLine("Serializing the movie database.");
	}

	[OnXmlSerialized]
	private void OnSerialized()
	{
		SerializationLog.AppendLine("Serialized the movie database.");
	}

	[OnXmlDeserializing]
	private void OnDeserializing()
	{
		SerializationLog.AppendLine("Deserializing the movie database.");
	}

	[OnXmlDeserialized]
	private void OnDeserialized()
	{
		SerializationLog.AppendLine("Deserialized the movie database.");
	}
}
```

[Logo]: ./Images/ImpossibleOddsLogo.png
[BinaryFormatterInfo]: https://docs.microsoft.com/en-us/dotnet/standard/serialization/binary-serialization
[AdvancedSerialization]: ./Serialization.md
