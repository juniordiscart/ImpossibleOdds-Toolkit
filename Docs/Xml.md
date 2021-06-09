# ![Impossible Odds Logo][Logo] C# Toolkit - XML

The XML functionality described here is all available in the `ImpossibleOdds.Xml` namespace.

XML is a structured data format that is commonly used in a wide area of applications and is able to describe complex objects in a human readable way. The C# language already provides support for XML out of the box by just including the `System.Xml` namespace in your script. One big downside is that only public fields and properties can be processed, or you need to start fiddling with the `DataContractSerializer` which applies namespace values overzealously and is not very intuitive to use.

This toolkit's aim on XML is to provide an easy to use tool with a great degree of control without having to write custom functions per class get the desired behaviour:

* Define whether members are serialized as child elements or attributes.
* Support for CDATA sections.
* Save type information and keep the inheritance chain intact.
* Serialization callbacks when your objects are about to be processed.

## Setup

To have your custom objects be recognized by the XML processor, they must be decorated with the `XmlObject` attribute. This notifies the XML framework that your object should be taken into consideration.

```cs
[XmlObject]
public class Actor
{
	// Apply the name as an attribute to the Actor element.
	[XmlAttribute]
	private string name = string.Empty;

	// Apply the biography and date of birth as child elements of the Actor element.
	[XmlElement]
	private string bio = string.Empty;
	[XmlElement]
	private DateTime dateOfBirth = DateTime.MinValue;
}
```

A serialized result of an `Actor` object could look something like this:

```xml
<Actor name="Bob Odenkirk">
  <bio>Robert John Odenkirk was born in Berwyn...</bio>
  <dateOfBirth>1962-10-22</dateOfBirth>
</Actor>
```

When your object would function as the root of the XML document, you can provide the `XmlObject` attribute with an optional `RootName` value. If no root name is specified, then the name of the type is used.

```cs
// Provide a custom root object name, if the object is used a the root of the XML document
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{ }
```

Your XML document's root would look like this:

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<IMDB>
  ...
</IMDB>
```

As seen in the example above for the `Actor` class, you can define whether the values are to be processed as child elements or as attributes on the XML element. The `XmlElement` attribute will set the value as a child element, while the `XmlAttribute` attribute will add the value as an XML attribute. Both of these can also take an optional key, which will override the name of the attribute or element of the value. When left empty, it will use the member's name as defined in the object.

**Note**: XML attributes can only represent a singular, non-complex value. Keep this in mind when placing it above a member!

### Arrays and lists

When your object contains a list or array with data that needs to be included in the XML document, you can use the `XmlListElement` attribute on that member. It allows you to define a custom name for the elements in the list using the optional `EntryName` property. When left open, a default name will be used.

```cs
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	// Rename the element and list entries in the XML result.
	[XmlListElement("Performers", EntryName = "Performer")]
	public List<Actor> Actors
	{
		get; set;
	}
}
```

An example output of such an object could be:

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<IMDB>
  <Performers>
    <Performer name="Bob Odenkirk">
      <bio>Robert John Odenkirk was born in Berwyn...</bio>
      <dateOfBirth>1962-10-22</dateOfBirth>
    </Performer>
    <Performer name="Christian Bale">
      <bio>Christian Charles Philip Bale was born in Pembrokeshire...</bio>
      <dateOfBirth>1974-01-30</dateOfBirth>
    </Performer>
  </Performers>
</IMDB>
```

### CDATA

One of the more unique features of XML is the ability to embed pieces of arbitrary data, that is to say, it's contents are not altered/processed by the internal XML parser. This allows, for example, adding the binary data of an image to be inserted right in between the other, structured data. In the XML document, this is stored in a CDATA section, and it is also supported by this XML tool! Simply place the `XmlCData` attribute above a member of your object, and its binary representation will be included!

```cs
[XmlObject]
public class WebPage
{
	[XmlAttribute("Header")]
	private string name = string.Empty;
	[XmlAttribute("URL")]
	private string url = string.Empty;
	[XmlCData("FaviconData")]
	private Image favicon = null;
}
```

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<WebPage Header="Impossible Odds - Unity C# Tools">
  <URL>https://impossible-odds.net</URL>
  <FaviconData><![CDATA[AABJkiRJkiRlKUUp/////wAASZIkSZIkZSlFKf////]]></FaviconData>
</IMDB>
</WebPage>
```

**Note**: the example above uses an `Image` class that represents a serializable version of some image data. The `Image` class found in the `Unity.UI` namespace is **not** serializable! This merely serves the purpose of showing an example with an element of non-structured data.

However, it is important to know what the gotcha's are for using this:

* When not already a set of bytes, it will use a `BinaryFormatter` to process the data to `byte[]`. This implies that the data being sent through here adheres to the requirements of the `BinaryFormatter` class. Check out [this link][BinaryFormatterInfo] for more information.
* As a final step, the XML processor will always try to convert the result to a Base64 encoded string value, since the CDATA section in the XML document expects a string value.

### Type Information

When working with complex structured data, it's important to keep track of which types you're dealing with. Including type information in the document is vital, and also easy. This ensures that, when transforming the XMl document back to data to work with in your project, you'll know you have the properly constructed instance.

On your base class or interface, apply one or multiple `XmlType` attributes, and state which types can inherit from it, or implement it. Optionally, you can define a value that is used to identify this class, which might help in maintaining readability of the document. When this value is left empty, the name of the class itself is used instead.

```cs
[XmlType(typeof(Movie)),
XmlType(typeof(Series))]
public abstract class Production
{ }

[XmlObject]
public class Movie : Production
{ }

[XmlObject]
public class Series : Production
{ }
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
}
```

When this data is not present, the serialization system will halt and throw an exception upwards to let you know the data is faulty. By default, this makes the processor check whether an element or attribute is present in the XML data, not whether this value is `null` or not. If the data is also required not to be `null`, then the `NullCheck` property can be enabled.

```cs
[XmlObject]
public class Actor
{
	[XmlAttribute("Name"), XmlRequired(NullCheck = true)]
	private string name = string.Empty;
}
```

## Serialization

Finally, you've added all thinkable XML-related attributes to your object and they are ready to be serialized to an XML document. There's a single point of entry: the `XmlProcessor` and its many `Serialize` methods.

```cs
MyObject myObject;
string xmlResult = XmlProcessor.Serialize(myObject);
```

The `XmlProcessor` offers several overloads of the `Serialize` method to write the result more memory efficiently, or to append to an earlier result.

**Note**: the overloads taking in a `StringBuilder` object to store the result in, will **not** clear the builder before writing the result! The result is simply appended.

### Formatting

To steer the serializer somewhat in terms of formatting and encoding, you can use the `XmlOptions` class in each of the overloads of the `Serialize` method.

```cs
XmlOptions options = new XmlOptions();
options.CompactOutput = true;	// Writes the XML on as few lines as possible and minimizes the use of whitespace.
options.Encoding = Encoding.UTF8;	// Define the encoding used for the XMl document. Note that this might also depend on the writer being used!
options.HideHeader = true;	// Skip writing the XML header, and directly start with the root object instead.

MyObject myObject;
XmlProcessor.Serialize(myObject, options);
```

#### Advanced

The `XmlOptions` also has a `SerializationDefinition` property to provide a custom XML serialization definition object which defines which processors and data structures should be used during the data transformation process. When left `null`, the default internal one will be used.

Please check out the [Serialization][AdvancedSerialization] documentation for more information about custom serialization definitions.

## Deserialization

Deserializing an XML document is done at the same place where serialization takes places: the `XmlProcessor`. It has several different overloads to deserialize your XML data. Depending on how much you know beforehand of what the XML data will contain, you have different options.

When you don't know what's in the XML document, you can simply provide the raw string or a text reader to the `Deserialize` method and it will produce an `XDocument` instance from the `System.Xml.Linq` namespace. This allows you to query and search through the document.

```cs
// When not knowing what will be deserialized, the returned result will be a
// an XML document which can be used for further processing.
string xmlData = File.ReadAllText("imdb.xml");
XDocument xmlDocument = XmlProcessor.Deserialize(xmlData);
```

If you do know what you're deserializing, or figured it out by querying the XML document, you can provide the expected type to the deserializer and it will attempt unwrap the XML data to a new instance of the target type.

```cs
// When the type of the data is known beforehand, but no instance is available.
string xmlData = File.ReadAllText("imdb.xml");
MovieDatabase movieDB = XmlProcessor.Deserialize<movieDB>(xmlData);
```

If you already have an instance of the object that you want to apply the XML data to, you can also provide it for deserializing it directly on that object.

```cs
// When the type of the data to be deserialized is known beforehand,
// instruct the processor to process to the target instance of the object.
MovieDatabase movieDB = new MovieDatabase();
string xmlData = File.ReadAllText("imdb.xml");
XmlProcessor.Deserialize(movieDB, xmlData);
```

## Callbacks

When your data is being processed by the `XmlProcessor` during (de)serialization, objects can request to be notified before/after when they're being handled. This might be helpful when you have some specific data that needs to processed prior ending up in the result. There are four types of callbacks you can request using several attributes:

* `OnXmlSerializing` when the object is about to get serialized.
* `OnXmlSerialized` when the object is done being serialized.
* `OnXmlDeserializing` when the object is about to be deserialized.
* `OnXmlDeserialized` when the object is done being deserialized.

```cs
[XmlObject]
public class MyClass
{
	[OnXmlSerializing]
	private void Serializing()
	{
		Log.Info("Serializing instance of {0}.", this.GetType().Name);
	}

	[OnXmlSerialized]
	private void Serialized(IProcessor currentProcessor)
	{
		Log.Info("Serialized instance of {0}.", this.GetType().Name);
	}

	[OnXmlDeserializing]
	private void Deserializing(XmlCustomObjectProcessor currentProcessor)
	{
		Log.Info("Deserializing instance of {0}.", this.GetType().Name);
	}

	[OnXmlDeserialized]
	private void Deserialized()
	{
		Log.Info("Deserialized instance of {0}.", this.GetType().Name);
	}
}
```

Each of these callbacks may accept a single parameter that is of type `IProcessor` (see the [Advanced Serialization][AdvancedSerialization] topic for more information). It basically functions as a serialization context object that is currently processing your data. In the context of XML, this processor will be of type `XmlCustomObjectProcessor`.

[Logo]: ./Images/ImpossibleOddsLogo.png
[BinaryFormatterInfo]: https://docs.microsoft.com/en-us/dotnet/standard/serialization/binary-serialization
[AdvancedSerialization]: ./Serialization.md
