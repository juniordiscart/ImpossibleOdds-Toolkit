# `ImpossibleOdds.Http`

The `ImpossibleOdds.Http` module, makes it easy to communicate with your web server over the HTTP protocol. It works by defining custom data classes that represent the requests to, and responses from your web server.

To start communicating, you'll need to perform the following simple steps:

1. Create your request and response data classes. Let them inherit from `HttpAbstractRequest` and `HttpAbstractResponse` respectively and implement their required properties and methods.
2. Decide which HTTP method your request should be: GET, POST or PUT. This should be returned by the `Method` property.
3. Decorate your request class with a `HttpResponseAssociation` attribute to link your request with your response type.
4. Decorate your request class' fields with `HttpURLField` and `HttpHeaderField` attributes to set any URL and header parameters.
5. If you're performing a POST request, then a request body is needed as well. Just as with the `ImpossibleOdds.Json` module, you can choose between 'object'-like, or 'list'-like structure.
     * Place the `HttpObjectMapping` attribute above your request class, and decorate the fields that should go in the body of the request with the `HttpBodyField` attribute.
     * Place the `HttpListMapping` attribute above your request class, and decorate the fields that should go in the body of the request with the `HttpBodyIndex` attriute.
6. Lastly, provide the `HttpMessenger.SendRequest()` with an instance of your request, and wait for a response!

```csharp
// Create class that represents your request
// and associate it with the type of response
// that is to be expected.
[HttpResponseAssociation(typeof(MyResponse))]
public class MyRequest : HttpAbstractRequest
{
	[HttpURLField("MessageID")]
	public int messageID;

	public string URIPath
	{
		get { return "www.example.com/myrequest.php";}
	}

	public RequestMethod Method
	{
		get { return RequestMethod.GET;}
	}

	public MyRequest(int messageID)
	{
		this.messageID = messageID;
	}
}

// Create the associated response class that
// will hold the response data.
public class MyResponse : HttpAbstractResponse
{
	[HttpBodyField("Message")]
	public string messageData;
}

public class HttpMessengerTest
{
	private HttpMessenger messenger = new HttpMessenger();

	public HttpTest()
	{
		messenger.onResponseReceived += OnResponse;
	}

	public void GetMessage(int messageID)
	{
		MyRequest request = new MyRequest(messageID);
		messenger.SendRequest(MyRequest);
	}

	private void OnResponse(HttpAbstractRequest request, HttpAbstractResponse response)
	{
		// Process response
	}
}
```
Internally, no custom implementation of the HTTP protocol is implemented. Rather, this module serves as a layer to simplify the usage of the `UnityWebRequest` class.
