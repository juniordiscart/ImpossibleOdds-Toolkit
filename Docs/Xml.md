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
	[XmlAttribute]
	private string name = string.Empty;
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
[XmlObject(RootName = "IMDB")]
public class MovieDatabase
{
	...
}
```

```xml
<?xml version="1.0" encoding="UTF-16" standalone="yes" ?>
<IMDB>
	...
</IMDB>
```

As seen in the example above for the `Actor` class, you can define whether the values are to be processed as attributes on the XML element using the `XmlAttribute` attribute. The `XmlElement` attribute makes the member of your class be an XML _child_ element of the object.

**Note**: XML attributes can only represent a singular, non-complex value.

### Arrays and lists



[Logo]: ./Images/ImpossibleOddsLogo.png
