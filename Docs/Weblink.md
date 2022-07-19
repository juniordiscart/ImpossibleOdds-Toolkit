# ![Impossible Odds Logo][Logo] C# Toolkit - Weblink

The `ImpossibleOdds.Weblink` namespace contains a general purpose messaging framework. It packs request objects and waits for incoming responses. When a response is received, it matches it to the request it belongs to and lets you know new data is available.

**Note**: everything described below is implemented abstractly for which a custom and solid implementation should be provided based on the intended use case. For example, the tools found in [HTTP][Http] and [Photon Extensions][PhotonExtensions] extensively use this framework. If you think these tools could be useful to you, you can check them out as a guide for implementation as well.

## Messenger

The _Messenger_ concept in this Weblink framework basically keeps a register of requests that have been sent out and are awaiting a response. When it receives response data it will look up the request it belongs to and hand it back over to the caller or any other interested party.

The abstract `WeblinkMessenger` class provides a bare bones implementation already.

A complete custom messenger class can be created using the `IWeblinkMessenger` interface. There's also a generic variant of this interface that already places certain requirements on the type parameters. It requests that you implement the following functionality:

* Send out a request, depending on the intended use case of the custom implementation.
* Stop a request from processing a future incoming response.
* Check whether a request is still awaiting a response.
* Retrieve a message handle for a pending request.
* Implement a callback system.

## Message Handles

A messenger is required to return a `IWeblinkMessageHandle` when a request is sent out. This message handle is a simple data structure that provides access to the request that was sent as well as the future incoming response. A generic variant of this interface is also available that restricts the types that can be used to those that implement the `IWeblinkRequest` and `IWeblinkResponse` interfaces.

The handle also implements the `IEnumerator` interface which should make it yieldable in coroutines.

## Response Mapping

One of the tasks the messenger also takes on is to map any request it sends out to any response data that comes in, and processes it to a usable object. A request object should be marked with a singular `WeblinkResponse` attribute, which defines the type of the response it is to be associated with.

```cs
[WeblinkResponse(typeof(MyResponse))]
public class MyRequest : IWeblinkRequest
{ }

public class MyResponse : IWeblinkResponse
{ }
```

Whenever a response comes in and it is successfully mapped to a registered request, it will instantiate such a response object and attempt to unwrap the incoming data to the structure of the response object.

The `GetResponseType` method found in the static `WeblinkUtilities` class can be of help with this. Given the type of the request object, it will look for the attribute and return the type of the response that should get instantiated.

## Callbacks

The messenger class provides several ways to get notified when a response comes in. The most advanced of these notification systems is the custom callback system. Any object can register itself with the messenger. These objects can mark their methods with the `WeblinkResponseCallback` attribute, which defines that a method is interested in processing a specific kind of response.

The list of parameters of such a method can contain the fully type-qualified request and/or response as well as the message handle. Each parameter is be optional and may appear in any order.

```cs
[WeblinkResponseCallback(typeof(MyResponse))]
private void MyResponseHandle(MyRequest request, MyResponse response, IWeblinkMessageHandle messageHandle)
{
	// Handle response of type MyResponse.
}
```

This callback mechanism can easily be implemented using the `InvokeResponseCallback` method found in the the static `WeblinkUtilities` class. It takes a target object (the object interested in receiving a callback) and the completed message handle. It will scan the target object for methods marked with the callback attribute and invoke any for which the response type matches.

[Logo]: ./Images/ImpossibleOddsLogo.png
[Http]: ./Http.md
[PhotonExtensions]: https://github.com/juniordiscart/ImpossibleOdds-PhotonExtensions
