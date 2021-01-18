# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Dependency Injection

The dependency injection tools are accessed by including the `ImpossibleOdds.DependencyInjection` namespace in your scripts.

Depenendency injection's intention is to facilitate separation of concerns and responsibilities, which helps in code reusability and readability. The essence is that it removes the need for your objects to go and fetch their resources they need, but rather they state that they depend on them and expect them to be delivered/injected. Sounds too abstract? Let's illustrate with an example. Let's say that, in order for a character to move through a world, it needs an input manager to determine where it's going next. One way is to create an input manager class and assign it through Unity's inspector view, or have it be a singleton and access it whenever needed. However, what if a different kind of input method is required, such as touch controls, or simulated input for an A.I. agent to be trained to interact with the world? The dependency injection framework allows the character to state that it needs _a_ input manager, and it will try its best to deliver it.

```cs
[Injectable]
public class Character : MonoBehaviour
{
	[Inject]
	private IInputManager inputManager = null;
}
```

The advantage of such an approach is that you can easily replace resources or implementations on a much higher level, adapt the project or game for other means. However, nothing is magical, and having your resources ready for injection requires some setup work, and most likely, a change in project and code structure as well. Further down you'll find the details about the dependency injection process, but here's a first glance of what to expect:

* Define which resources your classes need using the `Injectable` and `Inject` attributes.
* Create binding objects that connect a type to a way on how to get an instance of that resource.
* Register the bindings in a container object that is able to map a type to a binding.
* Define the context that determines which objects should get injected.

## Injection

To have your objects be injectable, they should be decorated with the `Injectable` attribute. This is done as a preprocessing optimisation. Rather than going through every type of object and see if they need something, defining this attribute on an object lets the framework know that this object requires something. This eliminates all classes and structs found in namespaces that don't need to be looked at in the first place, like the `System` or `UnityEngine` namespaces.

Next, define which members of your object should be injected by adding the `Inject` attribute. The following members of your objects can be injected:

* Fields. These are injected first.
* Properties. They require `set` to be defined. They are injected after the object's fields.
* Methods are injected last. If a parameter cannot be resolved, then the default value for that type is passed along.

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
	private void MyInjectableMethod(MyClassC dependencyC)
	{
		// 0, 1 or more dependency parameters can be defined
	}
}
```

## Bindings

A binding is an association of a type with a way on how to get an instance of that type. When one of your objects is about to be injected with a resource, it should know how to actually get that resource in the first place. For example, loading from Resources or from the file system, or maybe a new instance each time injection takes place, etc. In other words, a binding states _how_ it will deliver the resource to your object.

The following predefined bindings are available to use already:

* The `InstanceBinding` takes an instance upon creation and serves it with each injection.
* The `GeneratorBinding` invokes a function each injection of which the result is injected into the target.

```cs
InputManager input;
new InstanceBinding(input);	// Returns the same instance of the input manager with every injection.

Bullet bulletPrefab;
new GeneratorBinding(delegate()
{
	return GameObject.Instantiate(bulletPrefab);
});	// Returns a new instance of the bullet with every injection.
```

### Advanced

These predefined bindings are very generic and broadly usable, though might not be adequate enough for all situations. To create new kinds of bindings, simply implement the `IDependencyBinding` or `IDependencyBinding<T>` interface.

## Containers

A dependency injection container, simply put, is a collection of bindings. It allows to store, retrieve and check if a binding exists for a registered type. Such a container is used as the main source to inject your objects.

To have your binding be registered to a type in a container, simply call its `Register` method.

```cs
IDependencyBinding<MyType> binding;	// Binding to get a resource of MyType.
IDependencyContainer container;
container.Register<MyType>(myBinding);	// Register the binding with the container to MyType.
```

**Important note**: types must be explicitly registered with the container for it to be able to be found. Whenever a member of your object is about to be injected with a resource, the member's type is checked to see if a binding exists for that type explicitly in the container.

Consider the example from the introduction, where we bind an instance of `InputManager` and register it with the container to this same type. However, if the character expects an instance of the implemented interface `IInputManager`, it won't get detected.

```cs
// Input manager implementation.
public class InputManager : MonoBehaviour, IInputManager
{ }

[Injectable]
public class Character : MonoBehaviour
{
	[Inject]
	private IInputManager input;	// Expects instance implementing the interface.
}

// Register the input manager under the InputManager type.
InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.Register<InputManager>(binding);
```

The binding is registered under its fully qualified implementation type (which is perfectly valid), but won't be detected as a suitable value to inject in the character. Instead, or additionally, register that same binding again under its implemented interfaces as well.

```cs
InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.Register<InputManager>(binding);	// Register the type to the binding.
container.Register<IInputManager>(binding);	// Register the implemented interface to the same binding.
```

That way, a single binding can be registered for multiple types. A shorthand notation is also available:

```cs
InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.RegisterWithInterfaces<InputManager>(binding);	// Registers the type and all implemented interfaces to the same binding.
```

**Note**: registering a binding to a container of course requires that the types match in terms of being assignable to each other. The container performs a type check upon registration to make sure it won't run into trouble later on during the injection process itself. If it detects such a type mismatch, it will throw an exception.

### Advanced

The predefined `DependencyContainer` type can be used to store bindings suitable for injection. A custom container type can be created however by implementing the `IDependencyContainer` interface.

## Contexts & Installers

Now that your objects can be prepared for injection and containers have been filled with bindings, it's time to define _who_ will be injected, and _when_. This is done using the concept of a dependency injection context.

A context defines who will be injected based on the scope of objects it has access to. For example, if the context we're talking about would be the currently active scene, then it should search through the scene to find components that have the `Injectable` attribute.

The following contexts are predefined by this framework:

* The `GlobalDependencyContext` exists during the game's lifetime. Ideal for resources that should always be available.
* The `SceneDependencyContext` exists on a per-scene basis. It's constrained to objects found in the scene it operates in.
* The `HierarchyDependencyContext` operates on a specific GameObject and its children.

A context defines a single container, which is to be filled up with bindings. It does so by looking for installers, which should install the bindings appropriate for the current context into its container. After the installation of the bindings is complete, the context is ready to inject the objects it has access to with its registered resources.

### Global Context

The `GlobalDependencyContext` is a context that is valid throughout the entire game or program's lifetime. In Unity, before the very first scene is loaded, it will bind the resources that should be available at all times, e.g. an input manager, a scene loading manager, a content manager, etc. This context is always created upon start of the game or program when including this framework in your project. No need to do anything yourself in terms of setting it up.

To install any resources in this global container, a codebase search is performed for static methods marked with the `GlobalContextInstaller` attribute. This allows to already load in data, scriptable objects, etc. and assign them to the global container.

```cs
private static class MyGlobalContextInstaller
{
	[GlobalContextInstaller]
	private static void InstallContext(IDependencyContainer globalContainer)
	{
		// Bind any resources that should be global.
	}
}
```

Now that global resources have been installed, each time a scene is loaded, it will scan that scene and inject any component that requires its resources.

While this omits the point of the dependency injection methodology entirely, you can always get access to the global container by using `GlobalDependencyContext.DependencyContainer`. This allows you to alter it at other points in time rather than just initialization. A better way for that is seen in the [Custom Dependency Injection](#custom-dependency-injection) section, where you bind a container to itself.

#### Advanced

If you have a custom container implementation, you can provide an instance of this container to the global context by decorating a static method with the `GlobalContainerProvider` attribute.

```cs
private static class CustomGlobalContextProvider
{
	[GlobalContainerProvider]
	private static IDependencyContainer CreateGlobalContainer()
	{
		// Create and return the custom container.
		return new MyCustomContainer();
	}
}
```

### Scene Context

The `SceneDependencyContext` is, as the name implies, a context bound to a scene. Contrary to the global context, this one is a `Component` that can be put on a GameObject in the scene. It has the option to inject its resources at `Start`, but the `Inject` method can also be called manually if another moment is more appropriate.

To install your bindings to the scene context's container, create an installer script that implements the `IDependencyContextInstaller` interface and add it to (a child of) the scene context's game object.

```cs
public class MySceneInstaller : MonoBheaviour, IDependencyContextInstaller
{
	void IDependencyContextInstaller.Install(IDependencyContainer container)
	{
		// Install bindings of specific importance to the scene.
	}
}
```

When the `Inject` method is called on the scene context, either in `Start` or manually, it will scan the scene for any injectable components and inject them with the resources bound to its container.

#### Advanced

Just like in the global context, a custom container implementation can be provided. Here, this is done by one of its (child) components that implements the `IDependencyContainerProvider` interface.

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

### Hierarchy Context

The `HierarchyDependencyContext` is exactly the same as the [scene context](#scene-context) except it only operates on itself and any child GameObjects rather than the whole scene.

### Order of Events

Both the `SceneDependencyContext` and `HierarchyDependencyContext` are components to be placed on GameObjects and have a very low script execution order value set, where the `SceneDependencyContext` is set to work before `HierarchyDependencyContext`. This works in tandem with the `GlobalDependencyContext` and is also the reason why the former two perform their work in `Start` rather than `Awake`.

Whenever a scene is loaded, the very first thing that happens is the `Awake` phase of all active objects in the scene. Next, Unity's scene manager notifies interested parties (among which is the `GlobalDependencyContext`), that the scene is ready, for which it will inject its resources to the objects found in this scene. Next, the `Start` phase is initiated, where the `SceneDependencyContext` and `HierarchyDependencyContext` can inject their resources when enabled to do so, before any other object performs their `Start`.

This has the benefit that objects will have their resources injected in a top-down way, where more globally defined resources are injected first, and if a duplicate registration exists in a context that is more local to the object, it gets overridden.

## Custom Dependency Injection

When a situation arises in which above outlined contexts do not fit the circumstances, it's possible to start the injection process yourself. All you need is a container with bindings and a target (or targets) to be injected. This is done using the static `DependencyInjector` class.

```cs
// Inject the bindings found in the container into the target.
DependencyInjector.Inject(myContainer, myTarget);
```

This might be helpfull in cases that objects are spawned further down the line and still need an injection with the resources found in a container. You can bind the container to itself and have it injected into objects that need the container later on.

```cs
// Placed on a SceneDependencyContext.
public class CustomContextInstaller : MonoBehaviour, IDependencyContextInstaller
{
	void IDependencyContextInstaller.Install(IDependencyContainer container)
	{
		// Bind the container to itself.
		container.Bind<IDependencyContainer>(new InstanceBinding(container));
	}
}

// Somewhere on a GameObject in the scene.
public class MyPrefabSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab = null;
	[Inject]
	private IDependencyContainer diContainer = null;

	public void Spawn()
	{
		// Inject a new instance (and its children) with the resources in the cached container.
		GameObject newInstance = Instantiate<GameObject>(prefab);
		newInstance.Inject(container, true);
	}
}
```

### Named Injections

Along with custom injections, _named_ injections may prove to be useful under specific circumstances. A member can be labeled as injectable along with a name that restricts where the dependency comes from. When injecting a target, a name can be passed along that defines only members labeled with that name will receive these dependencies.

```cs
// Class that is injectable, but expects certain fields to be injected from named sources.
[Injectable]
public class MyInjectableObject
{
	[Inject("security")]
	private SecurityToken encryptionKey = null;

	[Inject]
	private Socket networkAccess = null;
}

// Only fields that are marked as injectable with a matching label will be injected with the resources.
MyInjectableObject myTarget = new MyInjectableObject();
DependencyInjector.Inject(myContainer, "security", myTarget);
```

### Extension Methods

Several injection extension methods are defined for the `GameObject` and `Component` classes as well as enumerables of these, so that your components can be injected quickly and hassle-free.

```cs
// Inject the GameObject's components and all of its children.
GameObject myGameObject;
myGameObject.Inject(myContainer, true);

// Inject all components on the GameObjects, excluding their children.
GameObject[] myGameObjects;
myGameObjects.Inject(myContainer, false);
```

## Example

Check out the dependency injection sample scene for a hands-on example!

![Dependency Injection Sample][SampleImg]

[Logo]: ./Images/ImpossibleOddsLogo.png
[SampleImg]: ./Images/DependencyInjectionSample.png
