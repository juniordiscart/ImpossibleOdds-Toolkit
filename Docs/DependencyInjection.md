# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Dependency Injection

The dependency injection tools are accessed by including the `ImpossibleOdds.DependencyInjection` namespace to your scripts.

Depenendency injection is not an easy topic as it has a profound impact on how you structure your code. Its intention is to facilitate separation of concerns or responsibilities, which helps in code reusability and readability.

The basic idea is that, if an object relies on another object to which it isn't naturally related, then that object is given (injected) from the outside for it to use. For example, lets say that in order for a character to move through a world, it needs an input manager to determine where it's going next. One way is to create an input manager class and assign it through Unity's inspector view, or have it be a singleton and access it whenever needed. However, what if a different kind of input is required, such as touch controls, or simulated input for an A.I. agent to be trained to interact with the world? Dependency injection can help in this regard by providing the input manager to the character, depending on the context it is operating in:

```cs
[Injectable]
public class Character : MonoBehaviour
{
	[Inject]
	private IInputManager inputManager = null;
}
```

The above illustrates that the character relies on _a_ input manager, but shouldn't care what kind of input manager, or how it gets it.

## Injection

To have your classes be injectable, they should be decorated with the `Injectable` attribute. Next, define which members to have injected by adding the `Inject` attribute. The following members of your classes can be injected:

* Fields: these are injected first.
* Properties: requires `set` to be defined. Injected after the class' fields.
* Methods: injected after the class' properties. If a parameter cannot be resolved, then the default value for that type is passed along.

```cs
[Injectable]
public class MyInjectableClass
{
	[Inject]
	private MyClassA dependencyA = null;

	[Inject]
	public MyClassB DependencyB
	{
		get;
		set;
	}

	[Inject]
	private void Initialize(MyClassC dependencyC, MyClassD dependencyD)
	{
		// Initialize with the given dependencies.
	}
}
```

## Bindings

A binding in the context of dependency injection is associating a type with a way on how to get an instance of that type. When a dependency of an object is about to be injected, a binding defines how that instance is delivered. For example, an asset loaded from Resources, or a text file loaded from the file system, or a new instance every time it is injected, etc.

A few pre-defined bindings should cover most situations:

* `InstanceBinding`: This binding takes an existing instance upon creation and serves it with each injection.
* `NewInstanceBinding`: Creates a new instance of an object each time it is being injected. **Note**: this is not meant for instances of `MonoBehaviour`, as they cannot be created through a constructor.
* `FunctorBinding`: This binding invokes a function each injection of which the result is injected into the target.

To create other bindings or more complex bindings, simply implement the `IDependencyBinding` or `IDependencyBinding<T>` interface. Its `GetInstance()` method is called each time an instance is requested for injection.

## Containers

A dependency injection container, simply put, is a collection of bindings. It allows to store, retrieve and check if a binding for a type exists. It acts very much like a dictionary, where the types that have been bound are the keys, and the method of delivery (a binding) are the values.

In case the type you want to bind

A pre-defined container can be used to store bindings and can be found under the name of `DependencyContainer`. A custom container type can be created using the `IDependencyContainer` interface.

## Contexts & Installers

A dependency injection context is a setting in which a container is valid. For example, this toolkit has three pre-defined contexts to start out with:

* A global context that exists during the game's lifetime.
* A scene context that exists on a per-scene basis.
* A hierarchy context that operates on a specific GameObject and its children.

A context alone merely defines _when_ bindings are injected, and not _what_ is injected. That's where a context installer comes into play. A context installer gathers the resources or ways to get that resource (bindings) and adds them to the context's container. Then, when appropriate, the context will use its container and inject any objects that depend on its resources.

### Global Context

The `GlobalDependencyContext` is a context that is valid throughout the entire game or program's lifetime. In Unity, before the very first scene is loaded, it will try to bind the resources that should be available at all times, e.g. an input manager, a scene loading manager, a content manager, etc.

This global context starts out by default with an instance of a `DependencyContainer` object in which global bindings can be installed. In case you have a custom container implementation, it will search the codebase for any static methods marked with the `[GlobalContainerProvider]` attribute:

```cs
private static class CustomGlobalContextProvider
{
	[GlobalContainerProvider]
	private static IDependencyContainer CreateGlobalContainer()
	{
		// Create the custom container.
		return new MyCustomContainer();
	}
}
```

Next, to install any global resources, another codebase search is performed for static methods marked with the `[GlobalContextInstaller]` attribute:

```cs
private static class MyGlobalContextInstaller
{
	[GlobalContextInstaller]
	private static void InstallContext(IDependencyContainer globalContainer)
	{
		// Install any resources that should be global.
	}
}
```

Now that the global resources have been installed, each time a scene is loaded, it will scan that scene and inject any component that requires its resources.

**Note:** injection happens _after_ `Awake` but _before_ `Start`! When a scene is loaded, the scene manager's event is only fired when a scene is activated, which contains the `Awake` phase of all active GameObjects in that scene.

### Scene Context

The `SceneDependencyContext` is, as the name implies, a context bound to the scene it is operating in. Contrary to the global context, this one is a `Component` that can be put on a GameObject in the scene. It can inject its contents at `Start`, but the `Inject()` method can also be called manually if another moment is more appropriate.

Just like in the global context, a custom container implementation can be provided by one of its (child) components that implement the `IDependencyContainerProvider` interface.

```cs
public class CustomSceneContextProvider : MonoBehaviour, IDependencyContainerProvider
{
	IDependencyContainer IDependencyContainerProvider.GetContainer()
	{
		// Create the custom container.
		return new MyCustomContainer();
	}
}
```

Here too, context installers are necessary. It will search for any (child) components implementing the `IDependencyContextInstaller` interface.

```cs
public class MySceneInstaller : MonoBheaviour, IDependencyContextInstaller
{
	void IDependencyContextInstaller.Install(IDependencyContainer container)
	{
		// Install bindings of specific importance to the scene.
	}
}
```

When the `Inject()` method is called on the context, either in `Start` or manually, it will scan the scene for any injectable components and inject them with the resources bound to its container.

### Hierarchy Context

The `HierarchyDependencyContext` is exactly the same as the [scene context](#scene-context) except it only operates on itself and any child GameObjects instead of the whole scene.

## Custom Dependency Injection



### Named Injections

```cs
// Class that is injectable, but requires certain
// fields to only be injected by named contexts.
[Injectable]
public class MyInjectableObject
{
	[Inject("security")]
	private string encryptionKey = string.Empty;

	[Inject]
	private Socket networkAccess = null;
}
```

### Extension Methods

Several extension methods are defined for the `GameObject` and `Component` classes, to speed up injecting in case you have a custom context.

## Example

Check out the dependency injection sample scene for a hands-on example!

In the example, a context installer is set up which binds an input manager and a character settings data object which are injected through a scene context.

```cs
public class ExampleDependencyInstaller : MonoBehaviour, IDependencyContextInstaller
{
	[SerializeField]
	private CharacterSettings settings = null;

	// Called by the scene context set up in the example scene.
	// Binds the resources of importance to this scene to the container.
	public void Install(IDependencyContainer container)
	{
		// Create an input manager and bind its implemented interfaces.
		KeyboardInputManager keyboard = new KeyboardInputManager();
		container.BindWithInterfaces<KeyboardInputManager>(new InstanceBinding<KeyboardInputManager>(keyboard));

		// When settings are provided, bind them as well.
		if (settings != null)
		{
			container.Bind(new InstanceBinding<CharacterSettings>(settings));
		}
	}
}

[Injectable]
public class Character : MonoBehaviour
{
	[Inject]
	private IInputManager inputManager = null;
	[Inject]
	private CharacterSettings settings = null;

	...
}
```

[Logo]: ./Images/ImpossibleOddsLogo.png
