# ![Impossible Odds Logo][Logo] C# Toolkit - Photon WebRPC Extensions

[Photon][PhotonWebsite] is a popular multiplayer framework, developed by Exit Games GmbH. It offers an easy to use solution for adding multiplayer to your projects as well as several other network-related features. Apart from offering a multiplayer solution and service, it also allows you to communicate with your web server through a feature called [_WebRPC_][PhotonWebRPC]. These are very similar to HTTP requests with the added benefit that you can verify the request originates from an authenticated and valid client, as Photon can tag along some additional information about the client making the request. This makes it very well suited to securely communicate update information about a player or its achievements.

The framework described in this document builds upon this WebRPC feature of Photon, by leveraging several tools to automate the processing of requests and incoming responses, as well as giving you some additional methods to get notified when a message completes.

If you wish to get started with this extension tool in your code, you can do so by including the `ImpossibleOdds.Photon.WebRpc` namespace in your scripts. Throughout this tool's documentation, you'll find small code excerpts to illustrate the points being discussed. The full code example can be found at the [bottom of this page](#example).

## Prerequisites

The Photon WebRPC module described further below depends on the _Photon Realtime_ library being present in your project. You can find it in each of the [Photon][PhotonWebsite] packages. Exit Games provides several flavors of Photon, depending on which featureset you need. You can find them on the [Unity Asset Store][PhotonUnityAssetStore] as well.

While not strictly necessary, it might also be best that you familiarize yourself with Photon first before jumping onto this feature.

**Note**: the Impossible Odds development team is in no way affiliated with Exit Games or the Photon development team. This tool is merely an extension to their services. For more information about using Photon and how it works, please refer to their documentation and support channels.

## WebRPC Requests

When a connection with the Photon network has been established, you can start sending WebRPC requests to your (web) server. Requests, in the context of this tool, are objects that contain the data you wish to send. Unlike the [HTTP tool][HttpTool] in this very same toolkit, a Photon request does not allow you to specify the type of request you want to send out, e.g. `GET`, `POST`, `PUT`, etc. Rather, it will always be a `POST`-type request, and the data you send over will be delivered in a JSON-format.

To start defining your custom request objects, start by having a look at the `IWebRpcRequest` interface. Your custom request objects are required to implement this one as it has a few properties that are important to Photon:

* `UriPath`: the inner routing-path of the request on your web server. Note that this is not the full URL of your request, but rather the sub-path, as the base of the URL should already be defined in your Photon application dashboard.
* `UseAuthCookie`: defines whether Photon should forward the [AuthCookie parameter][PhotonAuthCookie] along with the request. This is useful if the request requires some validation about the player making the request.
* `UseEncryption`: defines whether Photon should encrypt the request when putting it on the wire.

### URI Path

The first essential part of your WebRPC request is the URL and its potential parameters. The value of the `UriPath` property should return the inner routing-path on the server as the base URL should already be configured in your Photon application dashboard.

This tool will extract and append URL parameters when you define members on your request object with the `WebRpcUrlField` attribute:

```cs
public class UpdateLeaderboardRequest : IWebRpcRequest
{
	[WebRpcUrlField("admin")]
	private bool isAdmin = false;
	[WebRpcUrlField("v")]
	private string apiVersion = "1.0";

	// Inner-path of the request on the server. This is appended
	// to the URL value set in the Photon application dashboard.
	public string UriPath
	{
		get { return "webrpc/updateleaderboard.php";}
	}

	// Other details omitted...
}
```

The composed URL received at your server could look something like `https://my.custom.domain.com/webrpc/updateleaderboard.php?v=1.0&admin=false`. These values will also be properly escaped so that the result will be compliant within the specifications.

### Request Body

The meat of your request's data will likely be found inside its body. This data should be of a structured nature, as it will be transformed to the JSON data format when sending it over to your server. In that respect, it very much resembles the `POST`-type request found in the [HTTP framework in this toolkit][HttpToolPostRequests].

To start off, objects that must be included in the request result should be marked with either the `WebRpcObject` or `WebRpcArray` attribute. Since your objects will get transformed to JSON in the end, these two attributes represent whether your object prefers to be processed to a JSON object or a JSON array:

* The `WebRpcObject` attribute on your object will process your object to a JSON object and will use the `WebRpcField` attributes placed over the members to serialize them under their name or an alias.
* The `WebRpcArray` attribute on your object will process the data to a JSON array and will use the `WebRpcIndex` attributes over your object's members to insert their value at their desired index of the result.

**Note**: your request object itself is required to be decorated with the `WebRpcObject` attribute as well because, as you'll come to know [later](#webrpc-messenger), additional fields need to be appended to the request's body for it to be able to match incoming responses.

```cs
[WebRpcArray]
public class LeaderboardUpdateEntry
{
	[WebRpcIndex(0)]
	private int score;
	[WebRpcIndex(1)]
	private float[] checkpoints;
}
```

```cs
[WebRpcObject]
public class UpdateLeaderboardRequest : IWebRpcRequest
{
	[WebRpcUrlField("admin")]
	private bool isAdmin = false;
	[WebRpcUrlField("v")]
	private string apiVersion = "1.0";

	[WebRpcField("lbId")]
	private string leaderboardID = string.Empty;
	[WebRpcField("update")]
	private LeaderboardUpdateEntry updateEntry = null;
	[WebRpcField("force")]
	private bool forceUpdate = false;

	// Inner-path of the request on the server. This is appended
	// to the URL value set in the Photon application dashboard.
	public string UriPath
	{
		get { return "webrpc/updateleaderboard.php";}
	}

	// Should additional user information be forwarded to the server?
	public bool UseAuthCookie
	{
		get { return isAdmin; }
	}

	// Should the request be encrypted when being put on the wire?
	public bool Encrypt
	{
		get { return true; }
	}
}
```

A serialized result of an instance of such a request could look something like this:

```json
{
	"lbId": "f6675658-6a60-4fc2-9b49-412a6fd88165",
	"update": [
		1023,
		[
			13.641,
			31.879,
			59.002
		]
	],
	"force": false
}
```

and will be sent to `https://my.custom.domain.com/webrpc/updateleaderboard.php?v=1.0&admin=false`.

## WebRPC Responses

When the Photon client receives a reply back from your server, it will notify you it has received something and dump it in your lap in a pretty raw format. This framework also picks up on this and tries to preprocess them for you so that you may directly interact with the result in a meaningful way. However, just like with requests, you'll have to put some guidance markers on your objects for this framework to process them correctly.

First off, you must associate your request object with its corresponding response object. This is done by decorating the **request** class with a `WebRpcResponseType` attribute. It takes as only parameter the type of the **response** it is associated with.

```cs
[WebRpcObject, WebRpcResponseType(typeof(UpdateLeaderboardResponse))]
public class UpdateLeaderboardRequest : IWebRpcRequest
{
	// Other details omitted...
}
```

Your response objects themselves are required to implement the `IWebRpcResponse` interface. It contains a single property to implement:

* `IsSuccess` defines whether the request was processed successfully on the server and the response contains valid data.

```cs
[WebRpcObject]
public class UpdateLeaderboardResponse : IWebRpcResponse
{
	[WebRpcField]
	private UpdateLeaderboardResultCode resultCode = UpdateLeaderboardResultCode.None;

	public bool IsSuccess
	{
		get { return resultCode != UpdateLeaderboardResultCode.None; }
	}

	public UpdateLeaderboardResultCode ResultCode
	{
		get { return resultCode; }
	}
}
```

As you notice in the example above, the `WebRpcObject` attribute is used as well on the response, just like on the request. This is required as you'll come to know [below](#webrpc-messenger).

When your response contains more complex structured data, the same attributes apply that you've come to know in the [requests section](#webrpc-requests):

* The `WebRpcObject` attribute will treat the data as a JSON object and will use the `WebRpcField` attributes placed over the members to deserialize them under their matched names.
* The `WebRpcArray` attribute will treat the data as a JSON array and will use the `WebRpcIndex` attributes over your object's members to extract them from the array and insert it in your object.

## Type Information & Inheritance

You might encounter situations in which you have complex structured data for which type information is important. By default, this tool will not include any type information about your object if you don't supply it with information about its inheritance tree. This also means that data being received in responses can't be fully reconstructed if the type of the member is a base class of what the data actually represents.

This can be resolved by using the `WebRpcType` attribute. This attribute must placed at a base class or an interface, and defines which child classes can exist in the context of this WebRPC tool. Optionally, you can define a value that represents the child class with an alias value. When no alias value is supplied, the name of the type is used instead.

```cs
[WebRpcType(typeof(RaceLeaderboard), Value="Race"),
WebRpcType(typeof(FreestyleLeaderboard), Value="Freestyle")]
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

## WebRPC Messenger

To send a WebRPC request over the Photon network, you don't address it directly anymore. Instead, your request is prepared by a `WebRpcMessenger`. The messenger takes care of transforming your object into a Photon-compatible datastructure, tracking its progress and processing the response.

Any instance of the `WebRpcMessenger` requires the use of a Photon `LoadBalancingClient`. It is the component that provides the gateway for sending out requests. By default, you can provide the `PhotonNetwork.NetworkingClient` as a parameter. When you work with a more advanced Photon network setup, you might need to provide a different one. The messenger will hook into the load balancing client's event callback system to get notified about received responses.

When you're done, you can call `Dispose` on the messenger, which will clear itself from the load balancing client.

```cs
public class WebRpcTest : MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
		messenger.onMessageCompleted += OnMessageCompleted;
		messenger.onMessageFailed += OnMessageFailed;
	}

	private void OnDestroy()
	{
		messenger.Dispose();
	}

	private void OnMessageCompleted(WebRpcMessageHandle message)
	{
		Log.Info("Message completed.");
	}

	private void OnMessageFailed(WebRpcMessageHandle message)
	{
		Log.Error("Message failed.");
	}
}
```

### Message Handles

For each request sent through the `WebRpcMessenger`, it will return you a messange handle. This handle allows you to check its status and whether it's completed or not. When the response is received, it will also be immediately available through its `Response` property.

Additionally, the handle is yieldable. So you can use it in a coroutine to wait for it to complete and continue.

```cs
public class WebRpcTest: MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
	}

	private void OnDestroy()
	{
		messenger.Dispose();
	}

	public void SendRequest(IWebRpcRequest request)
	{
		request.ThrowIfNull(nameof(request));
		StartCoroutine(RoutineHandleMessage(request));
	}

	private IEnumerator RoutineHandleMessage(IWebRpcRequest request)
	{
		WebRpcMessageHandle messageHandle = messenger.SendRequest(request);

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
			IWebRpcResponse response = messageHandle.Response;
		}
	}
}
```

### Targeted Callbacks

Another way to get notified is by registering your object to the messenger's _targeted callback_ mechanism. The advantage of using one of these callbacks over subscribing to events or using the message handle is that you can define methods that are interested only when a particular type of response is received, as well as getting direct access to the full type-casted request and received response. This saves you time trying to figure out what exactly came in.

A method is marked as a targeted callback by placing the `WebRpcResponseCallback` attribute over the method. It takes the type of a _response_, which defines for which responses it will be called.

Finally, use the `RegisterCallback` method on the messenger to register your object for being interested in targeted callbacks.

```cs
public class WebRpcTest : MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
		messenger.RegisterCallback(this);
	}

	private void OnDestroy()
	{
		messenger.Dispose();
	}

	public void SendRequest(IWebRpcRequest request)
	{
		request.ThrowIfNull(nameof(request));
		messenger.SendRequest(request);
	}

	[WebRpcResponseCallback(typeof(UpdateLeaderboardResponse))]
	private void OnUpdateLeaderboardResponse(WebRpcMessageHandle handle, UpdateLeaderboardRequest request, UpdateLeaderboardResponse response)
	{
		if (response.IsSuccess)
		{
			Log.Info("Result status: {0}.", response.ResultCode.DisplayName());
		}
		else
		{
			Log.Error("Something went wrong while updating the leaderboard.");
		}
	}
}
```

**Note**: the callback methods with the `WebRpcResponseCallback` attribute can take their parameters in any order, and each of them is optional. So feel free to leave out any that you don't need.

### Request & Response Identifiers

Perhaps you've already noticed, but the way this WebRPC tool and the [HTTP tool][HttpTool] work is earily similar. However, there's one important difference: Photon does not allow you to track a request to a response out of the box. When a request is sent out, it merely lets you know that it was successful or not, but doesn't provide any additional information about the request, like `UnityWebRequest` allows us to do with HTTP requests.

That's why this tool inserts some additional unique data into the request object and expects the server to send it back in its response so that the messenger can match it. So make sure you keep this in mind when configuring your server!

When a request is about to be sent out, the messenger will insert an identifier value in the request data under the default name of `RequestId`. The server is expected to send that value back under the `ResponseId` name. This also implies that the root object of the request data is configured to be structured like a JSON object, rather than a JSON array.

For example, in the request data example from earlier, the final result may look something like this:

```json
{
	"lbId": "f6675658-6a60-4fc2-9b49-412a6fd88165",
	"update": [
		1023,
		[
			13.641,
			31.879,
			59.002
		]
	],
	"force": false,
	"RequestId": "55aeA019"
}
```

By default, this generated identifier is 8 characters long but can be changed by setting the `GeneratedIdLength` property on the messenger's message configurator, which you'll read about below.

### Advanced - Customized Serialization

If you've examined the other tools in this toolkit, like the [JSON][JsonTool] and [HTTP][HttpTool] tools, you might have noticed a great similarity between them and this WebRPC tool in terms of structure, features and how they deal with objects. This is because they all use the same [_Serialization_][SerializationTool] framework for transforming data, but each with different sets of attributes that are specific to the tool.

However, it would be a waste of time and resources to define different kinds of attributes on your objects when they serve the same purpose, just for a different tool. That's why the `WebRpcMessenger` allows you to switch out its serialization behaviour for a different one that better fits the purpose.

This serialization behaviour of the messenger is defined by a _message configurator_ object, which you can provide during construction, or change later through the `MessageConfigurator` property. It defines how messages are processed and which serialization definitions are used. The message configurator of the `WebRpcMessenger` class allows you to customize the following aspects:

* The serialization behaviour, by setting which serialization definitions should be used for the URL and body of the requests.
* The request and response key identifiers for matching requests to responses, as well as the length of the generated identifiers.

```cs
public class WebRpcTest: MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);

		WebRpcMessenger.DefaultMessageConfigurator customMessageConfigurator = new WebRpcMessenger.DefaultMessageConfigurator(
			new JsonSerializationDefinition(),	// Use a JSON definition.
			new HttpURLSerializationDefinition(),	// HTTP URL definition.
			"MyRequestKey",		// Apply a custom request key for matching responses.
			"MyResponseKey");	// Set a custom response key to be expected in the responses.

		customMessageConfigurator.GeneratedIdLength = 6;	// Set custom identifier length.
		messenger.MessageConfigurator = customMessageConfigurator;
	}
}
```

The `WebRpcMessenger`'s default implementation can also be switched out for a completely custom message configurator, by implementing the `IWebRpcMessageConfigurator` interface, which requires the implementation of setting up the serialization of your request and responses as well as generating adequate identifiers. For inspiration, you can always check out the `DefaultMessageConfigurator` implementation in the messenger.

**Note**: tread carefully when assigning custom serialization definitions as not every serialization definition's output is compatible in terms of supported datatypes, even when they share the same interfaces. For example, the `XmlSerializationDefinition` from the [XML][XmlTool] tool cannot be used as it does not output supported data structures for Photon to deal with.

**Another note**: make sure you also assign a type of serialization definition only once to each of the data streams (URL versus body), as using the same serialization definition will pick up the exact same data from the request each time, resulting in duplicating data in different streams.

## Example

Below you'll find the complete code example to illustrate the use of the Photon WebRPC feature in combination with the WebRPC extension tools discussed above. It shows a simple request-response setup for updating a leaderboard on a server. It is assumed that the web server accepts and responds with compatible data structures.

```cs
[WebRpcArray]
public class LeaderboardUpdateEntry
{
	[WebRpcIndex(0)]
	private int score;
	[WebRpcIndex(1)]
	private float[] checkpoints;
}
```

```cs
public enum UpdateLeaderboardResultCode
{
	None = 0,
	[DisplayName(Name="Rejected")]
	ResultRejected = 1,
	[DispayName(Name="Updated")]
	ResultUpdated = 2,
}
```

```cs
[WebRpcObject, WebRpcResponseType(typeof(UpdateLeaderboardResponse))]
public class UpdateLeaderboardRequest : IWebRpcRequest
{
	[WebRpcUrlField("admin")]
	private bool isAdmin = false;
	[WebRpcUrlField("v")]
	private string apiVersion = "1.0";

	[WebRpcField("lbId")]
	private string leaderboardID = string.Empty;
	[WebRpcField("update")]
	private LeaderboardUpdateEntry updateEntry = null;
	[WebRpcField("force")]
	private bool forceUpdate = false;

	// Inner-path of the request on the server. This is appended
	// to the URL value set in the Photon application dashboard.
	public string UriPath
	{
		get { return "webrpc/updateleaderboard.php";}
	}

	// Should additional user information be forwarded to the server?
	public bool UseAuthCookie
	{
		get { return isAdmin; }
	}

	// Should the request be encrypted when being put on the wire?
	public bool Encrypt
	{
		get { return true; }
	}
}
```

```cs
[WebRpcObject]
public class UpdateLeaderboardResponse : IWebRpcResponse
{
	[WebRpcField]
	private UpdateLeaderboardResultCode resultCode = UpdateLeaderboardResultCode.None;

	// When the result code is changed from its default value, then the request completed successfully.
	public bool IsSuccess
	{
		get { return resultCode != UpdateLeaderboardResultCode.None; }
	}

	public UpdateLeaderboardResultCode ResultCode
	{
		get { return resultCode; }
	}
}
```

```cs
public class WebRpcTest: MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		// Use Photon's default client, and register for targeted callbacks.
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
		messenger.RegisterCallback(this);
	}

	private void OnDestroy()
	{
		messenger.Dispose();
	}

	public void SendRequest(IWebRpcRequest request)
	{
		request.ThrowIfNull(nameof(request));
		StartCoroutine(RoutineHandleMessage(request));
	}

	[WebRpcResponseCallback(typeof(UpdateLeaderboardResponse))]
	private void OnUpdateLeaderboardResponse(WebRpcMessageHandle handle, UpdateLeaderboardRequest request, UpdateLeaderboardResponse response)
	{
		if (response.IsSuccess)
		{
			Log.Info("Result status: {0}.", response.ResultCode.DisplayName());
		}
		else
		{
			Log.Error("Something went wrong while updating the leaderboard.");
		}
	}

	private IEnumerator RoutineHandleMessage(IWebRpcRequest request)
	{
		WebRpcMessageHandle messageHandle = messenger.SendRequest(request);

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
			IWebRpcResponse response = messageHandle.Response;
		}
	}
}
```

Check out the WebRPC sample scene for a hands-on example! Please note that you need to have a [Photon package][PhotonUnityAssetStore] installed in your project for it to work. But, you're not required to have performed the initial setup of entering your app ID. The demo will connnect with the _Impossible Odds demo server_ to demonstrate it's functionality.

[Logo]: ./Images/ImpossibleOddsLogo.png
[PhotonWebsite]: https://www.photonengine.com/
[PhotonWebRPC]: https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webrpc
[PhotonAuthCookie]: https://doc.photonengine.com/en-us/pun/v2/gameplay/web-extensions/webrpc#request
[PhotonUnityAssetStore]: https://assetstore.unity.com/publishers/298
[HttpToolPostRequests]: ./Http.md#post-requests
[JsonTool]: ./Json.md
[HttpTool]: ./Http.md
[XmlTool]: ./Xml.md
[SerializationTool]: ./Serialization.md
