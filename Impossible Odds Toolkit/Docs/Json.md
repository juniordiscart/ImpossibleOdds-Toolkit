# `ImpossibleOdds.Json`

Yet another JSON tool. This module provides a simple interface to process data to and from the JSON format.

Serializing your objects is easy. First define whether you want your class to be serialised as a `JsonObject` or as a `JsonArray`, then decorate your class' fields with the `JsonField` or `JsonIndex` attributes repsectively. Call `JsonProcessor.ToJson()` to serialise your object.

* Serialize as JSON objects
```csharp
[JsonObject]
public class MyClass
{
	[JsonField]
	public int myValue = 1;

	[JsonField("Text")]
	private string textValue = "Test";

	public string ToJson()
	{
		// Result: {"myValue":1,"Text":"Test"}
		return JsonProcessor.ToJson(this);
	}
}
```

* Serialise as JSON arrays
```csharp
[JsonArray]
public class MyClass
{
	[JsonIndex(0)]
	public int myValue = 1;

	[JsonIndex(1)]
	private string textValue = "Test";

	public string ToJson()
	{
		// Result: [1,"Test"]
		return JsonProcessor.ToJson(this);
	}
}
```

Deserialising your object can be done using the `JsonProcessor.FromJson()` method. It has some options to control over how and what is returned:

* If you know what kind of data you're going to process, you can already provide the type to which you'd like it to process to:
```csharp
string jsonString = @"{""myValue"":1,""Text"":""Test""}";
MyClass result = JsonProcessor.FromJson<MyClass>(jsonString);
```

* If you don't know beforehand, a sensible datastructure that reflects the JSON data will be returned for you to process later:
```csharp
// Result is of type Dictionary<string, object>.
string jsonString = @"{""myValue"":1,""Text"":""Test""}";
object result = JsonProcessor.FromJson(jsonString);
```
```csharp
// Result is of type List<object>.
string jsonString = @"[1,""Test""]";
object result = JsonProcessor.FromJson(jsonString);
```

The (de)serialisation process can be further influenced by providing a custom mapping definition. See the `ImpossibleOdds.DataMapping` module for more information.
