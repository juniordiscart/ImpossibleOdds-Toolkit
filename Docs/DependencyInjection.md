# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Dependency Injection

The dependency injection tools are accessed by including the `ImpossibleOdds.DependencyInjection` namespace in your scripts.

Depenendency injection's intention is to facilitate separation of concerns and responsibilities, which helps in code reusability and readability. The essence is that it removes the need for your objects to go and fetch their resources they depend on, but rather they state that they depend on them and expect them to be delivered/injected. Sounds too abstract? Let's illustrate with an example. Let's say that, in order for a character to move through a world, it needs an input manager to determine where it's going next. One way is to create an input manager class and assign it through Unity's inspector view, or have it be a singleton and access it whenever needed. However, what if a different kind of input method is required, such as touch controls, or simulated input for an A.I. agent to be trained to interact with the world? The dependency injection framework allows the character to state that it needs _a_ input manager, and it will try its best to deliver it.

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

To have your objects be injectable, their definition should be decorated with the `Injectable` attribute. Next, define which members to have injected by adding the `Inject` attribute. The following members of your objects can be injected:

* Fields are injected first.
* Properties require `set` to be defined. They are injected after the object's fields.
* Methods are injected last, after the object's properties. If a parameter cannot be resolved, then the default value for that type is passed along.

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

A binding is an association of a type with a way on how to get an instance of that type. When a resource for an object is about to be injected, the binding defines how that instance is delivered. For example, loading an asset from Resources or from the file system, a new instance every injection, etc.

These pre-defined bindings are available to use already:

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

These pre-defined bindings are very generic and broadly usable, though might not be adequate enough to resolve all situations. To create new kinds of bindings, implement the `IDependencyBinding` or `IDependencyBinding<T>` interface.

## Containers

A dependency injection container, simply put, is a collection of bindings. It allows to store, retrieve and check if a binding exists. To have your binding be registered in a container, simply call:

```cs
IDependencyContainer container;
IDependencyBinding binding;
container.Register<MyType>(myBinding);
```

**Note**: types must be explicitly registered in the container for it to be able to be found. Whenever a member is about to be injected with a resource, its type is checked to see if a binding exists for that type explicitly in the container. Consider the following example:

```cs
public class InputManager : MonoBehaviour, IInputManager
{ }

[Injectable]
public class Character : MonoBehaviour
{
	[Inject]
	private IInputManager input;	// Expects instance implementing the interface
}

InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.Register<InputManager>(binding);
```

The binding is registered under its fully qualified implementation type (which is perfectly valid), but won't be detected as a suitable value to be injected. Instead, or additionally, register that same binding under its implemented interfaces as well:

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

### Advanced

The pre-defined `DependencyContainer` type can be used to store bindings suitable for injection. A custom container type can be created however by implementing the `IDependencyContainer` interface.

## Contexts & Installers

A dependency injection context defines _when_ and _who_ will be injected with the resources bound to a container. For example, this toolkit has three pre-defined contexts to start out with:

* The `GlobalDependencyContext` exists during the game's lifetime. Ideal for resources that should always be available.
* The `SceneDependencyContext` exists on a per-scene basis. It's constrained to objects found in the scene it operates in.
* The `HierarchyDependencyContext` operates on a specific GameObject and its children only.

A context alone merely defines _when_ bindings are injected, but not _what_ is injected. That's where a context installer comes into play. A context installer gathers the resources, binds them and registers them to the context's container. Then, when appropriate, the context will use its container and inject any objects that depend on its resources.

### Global Context

The `GlobalDependencyContext` is a context that is valid throughout the entire game or program's lifetime. In Unity, before the very first scene is loaded, it will bind the resources that should be available at all times, e.g. an input manager, a scene loading manager, a content manager, etc.

To install any global resources into the global container, a codebase search is performed for static methods marked with the `[GlobalContextInstaller]` attribute.

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

**Note:** injection happens _after_ `Awake` but _before_ `Start`! When a scene is loaded, Unity's scene manager event is only fired when a scene is activated, which contains the `Awake` phase of all active GameObjects in that scene.

#### Advanced

If you have a custom container implementation, you can provide an instance of this container by decorating a static method with the `[GlobalContainerProvider]` attribute.

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

The `SceneDependencyContext` is, as the name implies, a context bound to a scene. Contrary to the global context, this one is a `Component` that can be put on a GameObject in the scene. It can inject its contents at `Start`, but the `Inject` method can also be called manually if another moment is more appropriate.

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

When the `Inject` method is called on the context, either in `Start` or manually, it will scan the scene for any injectable components and inject them with the resources bound to its container.

**Note**: the `SceneDependencyContext` dependency context has a low script execution order value, so that its injection happens before others, ensuring all resources are delivered before other objects initiate their `Start` phase.

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

The `HierarchyDependencyContext` is exactly the same as the [scene context](#scene-context) except it only operates on itself and any child GameObjects instead of the whole scene.

**Note**: the `HierarchyDependencyContext` also has a low script execution order value, but slightly higher than the one from `SceneDependencyContext`.

## Custom Dependency Injection

When a situation arises in which above outlined contexts do not fit the circumstances, it's possible to start the injection process yourself. All you need is a container with bindings and a target (or targets) to be injected.

```cs
// Inject the bindings found in the container into the target.
DependencyInjector.Inject(myContainer, myTarget);
```

This might be helpfull in cases that objects are spawned further down the line and still need an injection with the resources found in a container. You can bind the container to itself and have it injected into objects that need the container later on.

```cs
// For example, placed on a SceneDependencyContext.
public class CustomContextInstaller : MonoBehaviour, IDependencyContextInstaller
{
	void IDependencyContextInstaller.Install(IDependencyContainer container)
	{
		// Bind the container to itself.
		container.Bind<IDependencyContainer>(new InstanceBinding(container));
	}
}

// Somewhere in the scene.
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
