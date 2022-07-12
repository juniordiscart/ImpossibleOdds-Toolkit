# ![Impossible Odds Logo][Logo] C# Toolkit - HTTP

When you want to communicate with a web server you can use Unity's built-in `UnityWebRequest` class. This can be pretty convenient to retrieve leaderboards, record player progression, download content or other resources, or communicate with external web APIs. The `UnityWebRequest` class allows you to setup custom HTTP calls of any kind. One drawback is their setup, which can be cumbersome depending on the amount and complexity of parameters involved.

To relieve you from the manual labour of setting up these requests, the tools in this framework can do much of the heavy lifting. Next to that, there's a messenger system that takes care of sending your requests, processing incoming responses and notifying you when something was received.

Here's a quick overview of what you can expect of this HTTP framework:

* Generate `UnityWebRequest`-compatible data structures out of your objects.
* Process incoming responses automatically based on the requests that were sent out.
* A variety of ways on getting notified when a response is received.

The HTTP tools described below are accessed by including the `ImpossibleOdds.Http` namespace in your scripts. For each topic, you'll find small code examples. All example tidbits can be examined in full at the [bottom of this page](#example).

## Prerequisites

The obvious requirement for using this tool is, of course, that you have an actual server running that you can send requests to, and that will return you some valid data. However, you can also use this tool to setup objects for communicating with external web APIs.

## Requests

A request, in this context, is the message you send to the web server. It contains the data or parameters about what you want the server to do, be it updating a leaderboard, downloading an image, or something else.

There are several types of HTTP requests that you can create. Each type has a different meaning to the server and how you define the parameters. The following types of HTTP requests are supported in helping you pack your objects into custom requests:

* `GET`: a request type to retrieve information or download larger pieces of data.
* `POST`: a request type to send structured/complex data such as a filled in form by the user.
* `PUT`: a request type to upload larger packs of data such as images or other files.

**Note**: there are more request-types supported by the `UnityWebRequest` class, but these are currently unsupported by this tool.

Before moving on what makes each of these requests unique, let's discuss what they all have in common.

### URL & Headers

All types of requests share some common properties: they require a URL (the address to send the request to) with optionally some URL parameters, and headers. Each HTTP request object should at least implement the `IHttpRequest` interface.

The URL of your request is a required property that needs to be implemented through the `URL` property:

```cs
public class GetLeaderboardRequest : IHttpRequest
{
	public string URL
	{
		get => "https://my.domain.com/getleaderboard.php";
	}

	// Other details omitted...
}
```

Typically, a URL can also contain parameters. To add a parameter to the URL, use the `HttpURLField` attribute on the members of your request class. You can provide an optional name to be used as the parameter name. When left empty, the name of the member is used instead.

In the `URL` property it's expected to get the base address for the request, which will get appended with the generated parameters. However, you can safely define parameters already in this URL address string if you like. The framework will take care of properly appending them, if any.

```cs
public class GetLeaderboardRequest : IHttpPostRequest
{
	[HttpURLField("debug")]
	private bool includeDebugInfo = false;
	[HttpURLField]
	private string platform = "steam";

	public string URL
	{
		get => "https://my.domain.com/getleaderboard.php";
	}

	// Other details omitted...
}
```

Less common is to define additional headers, but is sometimes necessary nonetheless. This can be done by using the `HttpHeaderField` attribute in much the same way as you would do for URL parameters. They are limited to primitive or basic types, however. Just like a URL parameter, you can define an alias for the header by providing it with a string value. For more information about HTTP headers, please read up at [this documentations page](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers).

```cs
public class GetLeaderboardRequest : IHttpPostRequest
{
	[HttpURLField("debug")]
	private bool includeDebugInfo = false;
	[HttpURLField]
	private string platform = "steam";

	[HttpHeaderField]
	private string gameVersion = "v1.0";

	public string URL
	{
		get => "https://my.domain.com/getleaderboard.php";
	}

	// Other details omitted...
}
```

The above example would generate a URL to `https://my.domain.com/getleaderboard.php?debug=false&platform=steam` with a custom header in there called `gameVersion` with value `v1.0`.

### GET Requests

`GET`-type requests mainly exist to ask the server to fetch resources, e.g. a leaderboard, an image or something else.

To have your request object be picked up by the framework as a `GET` request, simply implement the `IHttpGetRequest`. It doesn't require you to actually implement anything extra. Its parameters are defined through the URL. So, any parameters should be added using the `HttpURLField` attribute as seen in the [URL & Headers section](#url-&-headers)

```cs
public class GetLeaderboardRequest : IHttpGetRequest
{
	[HttpURLField("debug")]
	private bool includeDebugInfo = false;
	[HttpURLField]
	private string platform = "steam";

	[HttpHeaderField]
	private string gameVersion = "v1.0";

	public string URL
	{
		get => "https://my.domain.com/getleaderboard.php";
	}

	// Other details omitted...
}
```

This tool offers several other predefined and more specific kinds of `GET`-type requests already, primarily based on what Unity's framework already supports out of the box:

* Use the `IHttpGetAudioClipRequest` interface to set up a request for downloading an audio clip. Additionally, you must provide what kind of audio clip it will be by implementing the `AudioType` property.
* Use the `IHttpGetTextureRequest` interface for downloading textures and images.
* Use the `IHttpGetAssetBundleRequest` interface for downloading asset bundles.

### POST Requests

A `POST`-type request serves to send over some structured/complex data to the server and do something with it, e.g. a registration form, or updating a score on a leaderboard. It's not designed to send over big chunks of data like images, video or other forms of binary data, though. You'll read about those in an upcoming section.

To have your object be recognised as a `POST`-type request by this framework, have it implement the `IHttpPostRequest` interface. This interface on its own does not force any additional properties or methods to be implemented but does allow you to define a complex set of parameters that will be set in the request's body. The default approach of this `POST`-type request implementation is to send over your data in the JSON format. As such, it takes a very similar approach as the [JSON tool][JsonTool] in this toolkit. You have have four attributes to work with to assist the tool in how to process your objects:

* Use the `HttpBodyObject` attribute to process your object to a JSON object. It's assisted by the `HttpBodyField` attribute, which can define an optional, alternative name for the member.
* The `HttpBodyArray` attribute to process your object a JSON array. It's assisted by the `HttpBodyIndex` attribute to define the index of a member in this list.

**Note**: your `POST` request class itself should also be marked with either a `HttpBodyObject` or `HttpBodyArray` attribute as this object functions as the root for the parameters.

```cs
[HttpBodyObject]
public class GetLeaderboardRequest : IHttpPostRequest
{
	[HttpBodyField("LeaderboardId")]
	private readonly string leaderboardID;
	[HttpBodyField("NrOfEntries")]
	private readonly int nrOfEntries;
	[HttpBodyField("Offset")]
	private readonly int offset;

	// Other details omitted...
}
```

An example output of this data placed in the body of the request could be:

```json
{
	"leaderboardID": "f6675658-6a60-4fc2-9b49-412a6fd88165",
	"nrOfEntries": 3,
	"offset": 0
}
```

Just like with `GET`-type requests, you can still add any additional URL and header parameters!

### PUT Requests

To upload larger pieces of data to a server you typically use `PUT`-type requests. These can be used to send over data 'as is', without additional processing, at least, not by this framework.

To setup a `PUT`-type request, either implement the `IHttpPutStringRequest` or `IHttpPutBinaryRequest` interface. These define the two supported types of data you can transmit: string or binary data, respectively, and serve how the server should interpret the data.

Both of these interfaces require you to implement the `PutData` property. Depending on which `PUT`-type interface variant you implement, it either requires it to return a `string` value or a `byte[]` value.

```cs
public class MyStringPutRequest : IHttpPutStringRequest
{
	public string PutData
	{
		get => File.ReadAllText("configuration.xml");
	}

	// Other details omitted...
}
```

```cs
public class MyBinaryPutRequest : IHttpPutBinaryRequest
{
	public byte[] PutData
	{
		get => File.ReadAllBytes("banner.png");
	}

	// Other details omitted...
}
```

Here too, you can also define any additional URL parameters and custom headers if needed.

## Responses

When the server decides to send something back, this framework can immediately process the incoming data to usable objects. Your request objects can state what type of response they expect the server to return.

To assist the tool with what kind of response it should process the data to, you decorate your **request** object with a `HttpResponseType` attribute, and it takes the type of the corresponding response as its parameter.

```cs
[HttpResponseType(typeof(GetLeaderboardResponse))]
public class GetLeaderboardRequest : IHttpGetRequest
{
	// Implementation details omitted...
}
```

At the very least, your response objects should implement the `IHttpResponse` interface.

```cs
public class GetLeaderboardResponse : IHttpResponse
{
	// Implementation details omitted...
}
```

The `IHttpResponse` interface is pretty bland though, and doesn't really define what the expected data in the response is. There are some more specific flavors available, and greatly help in getting the result to you faster:

* Use the `IHttpStructuredResponse` interface to define that the expected response data is formatted in a structured way, like JSON. See the [Structured Responses section](#structured-responses) for more details.
* Use the `IHttpCustomResponse` interface for other kinds of data that might need your attention, such as compressed/zipped data that needs decompression, decoding, etc. See the [Custom Data Responses section](#custom-data-responses) for more information.
* Use the `IHttpAudioClipResponse` interface when the response is an audio clip. This works in combination with `IHttpAudioClipRequest`. See the earlier [`GET` Requests section](#get-requests).
* Use the `IHttpTextureResponse` interface when the response is a texture. This works in combination with `IHttpTextureRequest`. See the [`GET` Requests section](#get-requests).
* Use the `IHttpAssetBundleResponse` interface when the response is an asset bundle. This works in combination with `IHttpAssetBundleRequest`. See the [`GET` Requests section](#get-requests).

**Note**: whichever of the above your response object implements, the result is only processed when the request didn't encounter a network or server error.

### Structured Responses

If you expect the server to return data in the form of a structured string, like JSON, you can have your response object implement the `IHttpStructuredResponse` interface. This works in exactly the same way as described in the [POST request](#post-requests) setup:

* The `HttpBodyObject` attribute will treat the response data as JSON object and will search for the `HttpBodyField` attributes placed over the members to deserialize them under their matched names.
* The `HttpBodyArray` attribute will treat the response data as a JSON array and will search for the `HttpBodyIndex` attributes placed at its members to extract them from the array and insert them in your object.

**Note**: the response object itself should also be marked with either a `HttpBodyObject` or `HttpBodyArray` attribute as it serves as the root for unwrapping the response data.

```cs
[HttpBodyArray]
public class LeaderboardEntry
{
	[HttpBodyIndex(0)]
	private int rank = 0;
	[HttpBodyIndex(1)]
	private int playerID = 0;
	[HttpBodyIndex(2)]
	private int score = 0;
}
```

```cs
[HttpBodyObject]
public class Leaderboard
{
	[HttpBodyField("Name")]
	private string name = string.Empty;
	[HttpBodyField("Entries")]
	private List<LeaderboardEntry> entries = null;
}
```

```cs
[HttpBodyObject]
public class GetLeaderboardResponse : IHttpStructuredResponse
{
	[HttpBodyField("ErrorCode")]
	private ResponseError responseError = 0;
	[HttpBodyField("Leaderboard")]
	private Leaderboard leaderboard = null;
}
```

### Custom Data Responses

When your response implements the `IHttpCustomResponse` interface, the framework expects you to process the response further. This might be necessary in case you need to unpack or decrypt the returned data. The `ProcessResponse` method will get called on the response object along with the `UnityWebRequest` that is associated with it. This allows you to take the raw data of the response and perform the actions that are necessary.

```cs
public class MyCustomResponse : IHttpCustomResponse
{
	public void ProcessResponse(UnityWebRequest request)
	{
		// Process the data
	}
}
```

## Type Information & Inheritance

When dealing with JSON data of a request's `POST`-body or its response, it's possible it may contain data with a chain of ancestor types. By default, no type information about the data being processed is taken into account. This means data returned in a response may not be fully reconstructable when received.

This is where the `HttpType` attribute comes into play. You can place these at any of a base class in the hierarchy or interfaces. This attribute defines which child classes are eligible for including its type information in the serialized result. By default, the type's name will be used as a value for identification. However, you can provide a custom alias for the type by setting the `Value` property on the attribute. This also makes it more resiliant to refactoring or name changes of the type.

```cs
[HttpType(typeof(RaceLeaderboard), Value="Race"),
HttpType(typeof(FreestyleLeaderboard), Value="Freestyle")]
public abstract class BaseLeaderboard
{
	// Implementation details omitted...
}

public class RaceLeaderboard
{
	// Treat score as a time value, with lower values being better.
}

public class FreestyleLeaderboard
{
	// Treat score as an accumulated score of tricks, with higher values being better.
}
```

The default implementation will treat the request's `POST`-body and structured responses as JSON-formatted data. There are some additional features regarding how type information is presented and processed when dealing with such data, as described in the [JSON's tool type information description][JsonTypeInfo]. Summarized, you can set the `KeyOverride` property to override the default key being used to either save the type information under a different name, or have the key refer to an existing field from which it should infer the type. In the latter case, make sure the `Value` property is also set with the value that links type and value together.

## HTTP Messenger

Now that you know how to create and set up your request and response objects, it's time to send them over to the server. This is done by using the `HttpMessenger` class. It takes care of transforming requests and responses back & forth to instances of `UnityWebRequest` as well as offering tons of possibilities to receive callbacks when a message completes.

Whenever you want to send a request, offer your request object by using its `SendRequest` method and it will take care of (almost) everything. All that remains is to sit back and wait for a reply to come back. You can subscribe to a few events to get notified when a message completes, or fails when an error was encountered:

```cs
public class HttpTest : MonoBehaviour
{
	private HttpMessenger messenger = null;

	private void Awake()
	{
		messenger = new HttpMessenger();
		messenger.onMessageCompleted += OnMessageCompleted;
		messenger.onMessageFailed += OnMessageFailed;
	}

	public void SendRequest(IHttpRequest request)
	{
		messenger.SendRequest(requestd);
	}

	private void OnMessageCompleted(HttpMessageHandle message)
	{
		Log.Info("Message completed.");
	}

	private void OnMessageFailed(HttpMessageHandle message)
	{
		Log.Error("Message failed.");
	}
}
```

### Message Handles

Whenever you send a request through the `HttpMessenger`, it will return you a _message handle_. This handle allows you to check up on the progress of your request, including whether it encountered an error. It's also yieldable, so you can wait for it to complete in a coroutine.

```cs
public class HttpTest : MonoBehaviour
{
	private HttpMessenger messenger = null;

	private void Awake()
	{
		messenger = new HttpMessenger();
	}

	public void SendRequest(IHttpRequest request)
	{
		StartCoroutine(RoutineHandleMessage(request));
	}

	private IEnumerator RoutineHandleMessage(IHttpRequest request)
	{
		HttpMessageHandle messageHandle = messenger.SendRequest(request);

		// Wait for it to complete.
		yield return messageHandle;

		if (messageHandle.IsError)
		{
			// Handle error.
			yield break;
		}
		else if (messageHandle.IsDone)
		{
			// Use response.
			IHttpResponse response = messageHandle.Response;
		}
	}
}
```

The handle also allows you to work with the actual instance of the `UnityWebRequest` through its `WebRequest` property. This way you can check up on any download progress of resources, or see what kind of error happened exactly.

### Targeted Callbacks

Another way to get notified is by registering your object to the messenger's _targeted callback_ mechanism. The advantage of using one of these callbacks over subscribing to events or using the message handle is that you can define methods that are interested only when a particular type of response is received, as well as getting direct access to the full type-casted request and received response. This saves you time trying to figure out what exactly came in.

A method is marked as a targeted callback by placing the `HttpResponseCallback` attribute over the method. It takes the type of a _response_, which defines for which responses it will be called.

Finally, use the `RegisterCallback` method on the messenger to register your object for being interested in targeted callbacks.

```cs
public class HttpTest : MonoBehaviour
{
	private HttpMessenger messenger = null;

	private void Awake()
	{
		messenger = new HttpMessenger();
		messenger.RegisterCallback(this);
	}

	public void SendRequest(IHttpRequest request)
	{
		messenger.SendRequest(requestd);
	}

	[HttpResponseCallback(typeof(GetLeaderboardResponse))]
	private void OnGetLeaderboardResponseReceived(HttpMessageHandle handle, GetLeaderboardRequest request, GetLeaderboardResponse response)
	{
		if (handle.IsError)
		{
			Log.Error("Something went wrong while fetching the leaderboard: {0}.", handle.WebRequest.error);
		}
		else
		{
			Log.Info("Successfully got the leaderboard.");
		}
	}
}
```

For these callbacks, the parameters can be in any order and every parameter is optional. So feel free to leave out any that you don't need from the parameter list. There are also no special restrictions imposed on the callback object in terms of interfaces or derived types. It can be just any kind of object!

### Advanced - Customized Serialization

If you've examined the other tools in this toolkit, like the [JSON][JsonTool] and [Photon - WebRPC extension][WebRPCTool] tools, you might have noticed a great similarity between them and this HTTP tool in terms of structure, features and how they deal with objects. This is because they all use the same [_Serialization_][SerializationTool] framework for transforming data, but each with different sets of attributes that are specific to the tool.

However, it would be a waste of time and resources to define different kinds of attributes on your objects when they serve the same purpose, just for a different tool. That's why the `HttpMessenger` allows you to switch out its serialization behaviour for a different one that better fits the purpose.

This serialization behaviour of the messenger is defined by a _message configurator_ object, which you can provide during construction, or change later through the `MessageConfigurator` property. It defines how messages are processed and which serialization definitions are used.

```cs
public class HttpTest : MonoBehaviour
{
	private HttpMessenger messenger = null;

	private void Awake()
	{
		// Create a HTTP messenger.
		messenger = new HttpMessenger();

		// Create and set a custom message configurator that uses the JSON serialization attributes
		// for the body of the POST-type requests, while keeping everything else the same.
		HttpMessenger.DefaultMessageConfigurator customMessageConfigurator = new HttpMessenger.DefaultMessageConfigurator(
			new JsonSerializationDefinition(),	// Use a JSON definition.
			new HttpURLSerializationDefinition(),	// Standard URL definition.
			new HttpHeaderSerializationDefinition());	// Standard header definition.

		messenger.MessageConfigurator = customMessageConfigurator;
	}
}
```

The `HttpMessenger`'s default implementation can also be switched out for a completely custom message configurator, by implementing the `IHttpMessageConfigurator` interface, which requires the implementation of setting up the serialization of your request and responses. For inspiration, you can always check out the `DefaultMessageConfigurator` implementation in the messenger class.

**Note**: tread carefully when assigning custom serialization definitions to message configurators as not every serialization definition's output is compatible in terms of supported datatypes, even when they share the same interfaces. For example, the `XmlSerializationDefinition` from the [XML][XmlTool] tool cannot be used as it does not output supported data structures for the `UnityWebRequest` class to deal with, e.g. it stores data in `XElement` structures, rather than dictionaries and lists. For XML support, a fully custom message configurator should be written.

**Another note**: make sure you also assign a type of serialization definition only once to each of the data streams (URL versus headers versus body), as using the same serialization definition will pick up the exact same data from the request objects each time, resulting in duplicating data in different streams.

## Example

Most of the small tidbits of example code discussed in the topics above can be found in a more complete example here. The example shows a request-response setup for retrieving a simple leaderboard from the web server. It is assumed that the web server, of course, accepts and returns compatible data structures.

```cs
[HttpBodyArray]
public class LeaderboardEntry
{
	[HttpBodyIndex(0)]
	private int rank = 0;
	[HttpBodyIndex(1)]
	private int playerID = 0;
	[HttpBodyIndex(2)]
	private int score = 0;
}
```

```cs
[HttpBodyObject]
public class Leaderboard
{
	[HttpBodyField("Name")]
	private string name = string.Empty;
	[HttpBodyField("Entries")]
	private List<LeaderboardEntry> entries = null;
}
```

```cs
[Flags]
public enum ResponseError
{
	None = 0,
	InvalidID = 1,
	InvalidEntries = 1 << 1,
	InvalidOffset = 1 << 2
}
```

```cs
[HttpBodyObject, HttpResponseType(typeof(GetLeaderboardResponse))]
public class GetLeaderboardRequest : IHttpPostRequest
{
	[HttpURLField("debug")]
	private bool includeDebugInfo = false;
	[HttpURLField]
	private string platform = "steam";

	[HttpHeaderField]
	private string gameVersion = "v1.0";

	[HttpBodyField("LeaderboardId")]
	private readonly string leaderboardID;
	[HttpBodyField("NrOfEntries")]
	private readonly int nrOfEntries;
	[HttpBodyField("Offset")]
	private readonly int offset;

	public string URL
	{
		get => "https://my.domain.com/getleaderboard.php";
	}
}
```

```cs
[HttpBodyObject]
public class GetLeaderboardResponse : IHttpStructuredResponse
{
	[HttpBodyField("ErrorCode")]
	private ResponseError responseError = 0;
	[HttpBodyField("Leaderboard")]
	private Leaderboard leaderboard = null;
}
```

```cs
public class HttpTest : MonoBehaviour
{
	private HttpMessenger messenger = null;

	private void Awake()
	{
		// Create a HTTP messenger and register this object for regular and targeted callbacks.
		messenger = new HttpMessenger();
		messenger.RegisterCallback(this);

		messenger.onMessageCompleted += OnMessageCompleted;
		messenger.onMessageFailed += OnMessageFailed;
	}

	public void SendRequest(IHttpRequest request)
	{
		request.ThrowIfNull(nameof(request));
		StartCoroutine(RoutineHandleMessage(request));
	}

	[HttpResponseCallback(typeof(GetLeaderboardResponse))]
	private void OnGetLeaderboardResponseReceived(HttpMessageHandle handle, GetLeaderboardRequest request, GetLeaderboardResponse response)
	{
		if (handle.IsError)
		{
			Log.Error("Something went wrong while fetching the leaderboard: {0}.", handle.WebRequest.error);
		}
		else
		{
			Log.Info("Successfully got the leaderboard.");
		}
	}

	private void OnMessageCompleted(HttpMessageHandle message)
	{
		Log.Info("Message completed.");
	}

	private void OnMessageFailed(HttpMessageHandle message)
	{
		Log.Error("Message failed.");
	}

	private IEnumerator RoutineHandleMessage(IHttpRequest request)
	{
		HttpMessageHandle messageHandle = messenger.SendRequest(request);

		// Wait for it to complete.
		yield return messageHandle;

		if (messageHandle.IsError)
		{
			// Handle error.
			yield break;
		}
		else if (messageHandle.IsDone)
		{
			// Use response.
			IHttpResponse response = messageHandle.Response;
		}
	}
}
```

Check out the HTTP sample scene for a hands-on example! It will make requests to the _Impossible Odds demo server_ to demonstrate its functionality.

[Logo]: ./Images/ImpossibleOddsLogo.png
[JsonTool]: ./Json.md
[XmlTool]: ./Xml.md
[WebRPCTool]: ./PhotonWebRPC.md
[SerializationTool]: ./Serialization.md
[JsonTypeInfo]: ./Json.md#type-information
