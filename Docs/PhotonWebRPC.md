# ![Impossible Odds Logo][Logo] C# Toolkit - Photon WebRPC

If you wish to get started with this extension tool in your code, you can do so by including the `ImpossibleOdds.Photon.WebRpc` namespace in your scripts. Throughout this tool's documentation, you'll find small code excerpts to illustrate the points being discussed. The full code example can be found at the [bottom of this page](#example).

[Photon][PhotonWebsite] is a popular multiplayer framework, developed by Exit Games GmbH. It offers an easy to use solution for adding multiplayer to your game, as well as a (voice) chat feature. Apart from offering a multiplayer solution and service, it also allows you to communicate with a custom web server through a feature called [_WebRPC_][PhotonWebRPC]. These are basically HTTP requests with the added benefit that you can verify the request originates from an authenticated and valid client, as Photon can tag along some additional information about the client making the request. This makes it very well suited to securely communicate update information about a player or its achievements.

The framework described in this document builds upon this WebRPC feature of Photon, by leveraging several tools to automate the processing of requests and incoming responses, as well as giving you some additional methods to get notified when a message is complete.

## Prerequisites

The Photon WebRPC module described further below depends on the _Photon Realtime_ library being present in your project. You can find it in each of the [Photon][PhotonWebsite] packages. Exit Games provides several flavors of Photon, depending on which featureset you need. You can find them on the [Unity Asset Store][PhotonUnityAssetStore] as well.

On top of that, WebRPC is a fairly advanced feature and you should probably get familiar with the Photon package first before diving in here further. Unless, of course, you're using it specifically for the WebRPC feature...

## WebRPC Requests

When the connection with the Photon network has been established, you can start sending WebRPC requests to your (web) server. Requests, in the context of this tool, are objects that contain the data you wish to send over to your server. Unlike the [HTTP tool][HttpTool] in this very same toolkit, a Photon request does not allow you to specify the type of request you may want to send out (`GET`, `POST`, `PUT`, etc.). Rather, it will always be a `POST`-type request, and the data you send over will be delivered in a JSON-format.

To start defining your custom request objects, start by having a look at the `IWebRpcRequest` interface. Your custom requests objects should implement this one as it has a few properties that are important to Photon:

* `UriPath`: the inner routing-path of the request on your web server. Note that this value gets appended to the URL that you define in your Photon application dashboard.
* `UseAuthCookie`: defines whether Photon should forward the [AuthCookie parameter][PhotonAuthCookie] along with the request. This is usefull if the request requires some validation of the player or client sending out the request.
* `UseEncryption`: defines whether Photon should encrypt the request when putting it on the wire.

### URI Path

The first essential part of your WebRPC request is the URL and its potential parameters. The value of the `UriPath` property should return the inner routing-path on the server as the base URL should already be configured in your Photon application dashboard.

This tool will extract and append URL parameters when you define members on your request object with the `WebRpcUrlField` attribute:

```cs
// A simple request that attempts to update a score on the leaderboard.
[WebRpcObject, WebRpcResponseType(typeof(UpdateLeaderboardResponse))]
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

	// Further request implementation follows...
}
```

The composed URL received at your server could look something like `https://my.custom.domain.com/webrpc/updateleaderboard.php?v=1.0&admin=false`. These values will also be properly escaped so that the result will be compliant within the specifications.

### Request Body

The meat of your request's data will likely be found inside its body. This data should be of a structured nature, as it will be transformed to the JSON data format when sending it over to your server. In that respect, it very much resembles the POST request-type found in the [HTTP framework in this toolkit][HttpToolPostRequests].

To start off, objects that must be included in the request result should be marked with either the `WebRpcObject` or `WebRpcArray` attribute. Since your objects will get transformed to JSON in the end, these two attributes represent whether your object prefers to be processed to a JSON object or a JSON array.

* The `WebRpcObject` attribute on your object will process your object to a JSON object and will use the `WebRpcField` attributes placed over the members to serialize them under their name or an alias.
* The `WebRpcArray` attribute on your object will process the data to a JSON array and will use the `WebRpcIndex` attributes over your object's members to insert their value at their desired index of the result.

**Note**: your request object itself is required to be decorated with the `WebRpcObject` attribute as well because, as you'll come to know [later](#webrpc-messenger), additional fields need to be appended to the request's body for it to be able to match incoming responses.

```cs
// To keep the data compact, treat this as an array as it's function is well understood in the context of the program.
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
// A simple request that attempts to update a score on the leaderboard.
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

A serialized result of an instance of such a request could look something like this:

```json
{
	"lbId": "Track01 - Reversed - Best Race Times",
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

When the Photon client receives a reply back from your server, it will notify you it has received something and dump it in your lap in a pretty raw form. This framework also picks up on this and tries to preprocess them for you so that you may directly interact with the result in a meaningful way. However, just like with requests, you'll have to put some guidance markers on your object for this framework to process them correctly.

First off, You must associate your request object with its corresponding response object. This is done by decorating the **request** class with a `WebRpcResponseType` attribute. It takes as only parameter the type of the **response** it is associated with. Please note that this type parameter should implement the `IWebRpcResponse` interface. This interface also requires you to implement a single and simple property on your response object:

* `IsSuccess` defines whether the request was processed successfully on the server and the response contains valid data.

```cs
[WebRpcObject, WebRpcResponseType(typeof(UpdateLeaderboardResponse))]
public class UpdateLeaderboardRequest : IWebRpcRequest
{
	// See the request section for implementation details...
}

// A response object that will contain the status code of the update operation.
[WebRpcObject]
public class UpdateLeaderboardResponse : IWebRpcResponse
{
	private UpdateLeaderboardResultCode resultCode = UpdateLeaderboardResultCode.None;

	// When the result code is changed from its default value,
	// then the request completed successfully.
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

## WebRPC Messenger

To send a WebRPC request over the Photon network, you don't address it directly anymore. Instead, your request is prepared by a `WebRpcMessenger` object. The messenger takes care of transforming your object into a Photon-compatible datastructure, tracking its progress and processing the responses.

Any instance of the `WebRpcMessenger` requires the use of a Photon `LoadBalancingClient`. It is the component that provides the gateway for sending out requests. By default, you can provide the `PhotonNetwork.NetworkingClient` as the parameter. The messenger will offer itself to this load balancing client as a callback object for when responses come in.

When you're done, you can call `Dispose` on the messenger, which will remove itself from that load balancing client.

```cs
// A test class that has a messenger for sending out requests.
public class WebRpcTest : MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		// Provide Photon's default networking client as a gateway for the messenger to send its requests through.
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
	}

	private void OnDestroy()
	{
		messenger.Dispose();
	}
}
```

### Request & Response Identifiers

When a WebRPC request is sent out, the messenger will append a custom field to the POST-data. The name of this field is set to `RequestId`. It is expected though, that the server sends back a response with the same value back under the `ResponseId` name. These are default names and you can customise them by changing the `RequestIDKey` and `ResponseIDKey` on the messenger. These values are needed for the messenger to match requests with incoming responses, as Photon itself does not provide a handle to track or check on pending calls.

For example, in the request data example from earlier, the final result may look something like this:

```json
{
	"lbId": "Track01 - Reversed - Best Race Times",
	"update": [
		1023,
		[
			13.641,
			31.879,
			59.002
		]
	],
	"force": false,
	"RequestId": "55aea019"
}
```

You can also define the length of the identifier by changing the value of the `GeneratedIDLength` property. Note though, to prevent collision of generated identifiers requests pending a response still, this should at least be 4 characters or more. The default length of the generated values is 8 characters.

### Message Handles

For each request sent through a `WebRpcMessenger`, it will return you a messange handle. This handle allows you to check its status and whether it's done or not. When the response is received, it will also be immediately available through its `Response` property.

Additionally, the handle is yieldable. So you can use it in a coroutine to wait for it to complete and continue.

```cs
// A test class that has a messenger for sending out requests.
public class WebRpcTest: MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		// Provide Photon's default networking client as a gateway for the messenger to send its requests through.
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

Besides checking and yielding the message handle to see if a response came in, you can also register any object to receive targeted callbacks when a (specific type of) response has been received. It's unique in the sense that you can register any kind of object and the messenger will search on the registered objects to methods marked with the `WebRpcResponseCallback` attribute.

A targeted callback method allows you to directly receive the fully type qualified request and response, so you don't have to try and guess what came in. Additionally, you can also set the message hande as a parameter if that's still of use to you at that point.

When placing the `WebRpcResponseCallback` attribute above a method, it expects the type of the expected response to be set as its sole parameter. When a response of that type is received, it will use this type as a guide for invoking the callback method.

```cs
// A test class that has a messenger for sending out requests,
// and is interested in received targeted callbacks for specific types of responses.
public class WebRpcTest : MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		// Provide Photon's default networking client as a gateway for the messenger to send its requests through.
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);

		// Register this object with the messenger so it will received targeted callbacks.
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
	private void OnMyResponseReceived(WebRpcMessageHandle handle, UpdateLeaderboardRequest request, UpdateLeaderboardResponse response)
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

## Example

Below you'll find the complete code example to illustrate the use of the Photon WebRPC feature in combination with the WebRPC extension tools discussed above.

```cs
// To keep the data compact, treat this as an array as it's function is well understood in the context of the program.
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
// Status indication of the update operation.
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
// A simple request that attempts to update a score on the leaderboard.
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
// A response object that will contain the status code of the update operation.
[WebRpcObject]
public class UpdateLeaderboardResponse : IWebRpcResponse
{
	private UpdateLeaderboardResultCode resultCode = UpdateLeaderboardResultCode.None;

	// When the result code is changed from its default value,
	// then the request completed successfully.
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
// A test class that has a messenger for sending out requests,
// and is interested in received targeted callbacks for specific types of responses.
public class WebRpcTest: MonoBehaviour
{
	private WebRpcMessenger messenger = null;

	private void Awake()
	{
		// Provide Photon's default networking client as a gateway for the messenger to send its requests through.
		messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);

		// Register this object with the messenger so it will received targeted callbacks.
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
	private void OnMyResponseReceived(WebRpcMessageHandle handle, UpdateLeaderboardRequest request, UpdateLeaderboardResponse response)
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
[HttpTool]: ./Http.md
[HttpToolPostRequests]: ./Http.md#post-requests
