# ![Impossible Odds Logo][Logo] C# Toolkit - Dependency Injection

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

The advantage of such an approach is that you can easily replace resources or implementations on a much higher level, adapt the project or game for other means. However, nothing is magical, and having your resources ready for injection requires some setup work, and most likely, a change in project and code structure as well. Further down you'll find the details about the dependency injection process, but here's a quick glance of the setup procedure:

* Define which objects require resources that should be injected.
* Bind your injectable resources to a delivery method and bundle them in a collection of resources.
* Define the scope for this collection of resources to be used.

## Injection

To have your objects have their necessary resources be injected, place the `Injectable` attribute above them. This tells the framework that your object requires resources and allows it to efficiently skip any other objects that don't need anything.

Next, define which members of your object should be injected by adding the `Inject` attribute. The following members of your objects can be injected:

* Fields. These are injected first.
* Properties. They require `set` to be defined and are injected after the object's fields.
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
		// Zero, one or more dependency parameters can be defined.
	}
}
```

## Bindings

A binding is an association of a resource's type with a delivery method on how to get an instance of that resource. When one of your objects is about to be injected with a resource, it should know how to actually get that resource in the first place. For example, loading from Resources or from the file system, or maybe a new instance each time injection takes place, etc. In other words, a binding states _how_ it will deliver the resource to your object.

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

A dependency injection container, simply put, is a collection of bindings. It allows to store, retrieve and check if a binding exists for a type. Such a container is used as the main source to inject your objects.

To have your binding be registered to a type in a container, simply call its `Register` method.

```cs
IDependencyBinding<MyType> binding;	// Binding to get a resource of MyType.
IDependencyContainer container;
container.Register<MyType>(myBinding);	// Register the binding with the container to MyType.
```

**Important note**: types must be explicitly registered with the container for it to be able to be found. Whenever a member of your object is about to be injected with a resource, the member's type is checked to see if a binding exists for that type specifically in the container.

Consider the example from the introduction, where an instance of `InputManager` was bound and registered with the container to this same type. However, if the character expects an instance of the implemented interface `IInputManager`, it won't get detected.

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

The binding is registered under its fully qualified implementation type (which is perfectly valid), but won't be detected as a suitable value to inject into the character object. Instead, or additionally, register that same binding again under its implemented interfaces as well.

```cs
InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.Register<InputManager>(binding);	// Register the type to the binding.
container.Register<IInputManager>(binding);	// Register the interface to the same binding.
```

That way, a single binding can be registered for multiple types in the same container. A shorthand notation is also available:

```cs
InputManager input;
IDependencyBinding binding = new InstanceBinding(input);
container.RegisterWithInterfaces<InputManager>(binding);	// Registers the type and all implemented interfaces to the same binding.
```

**Note**: registering a binding to a container, of course, requires that the types match in terms of being assignable to each other. The container performs a type check upon registration to make sure it won't run into trouble later on during the injection process itself. If it detects such a type mismatch, it will throw an exception.

### Advanced

The predefined `DependencyContainer` type can be used to store bindings suitable for injection. A custom container type can be created however by implementing the `IDependencyContainer` interface.

## Scopes & Installers

Now that your objects are prepared for injection and we know how containers can be populated with resources, it's time to define _who_ will be injected, and _when_. This is done using the concept of a dependency injection scope. For example, if the scope we're talking about would be the currently active scene, then it should search through the scene to find components that have the `Injectable` attribute.

The following scopes are predefined by this framework:

* The `GlobalDependencyScope` exists during the game's lifetime. Ideal for resources that should always be available.
* The `SceneDependencyScope` exists on a per-scene basis. It's constrained to work on objects found in the scene in which it is defined.
* The `HierarchyDependencyScope` operates on a specific GameObject and its children.

A dependency injection scope defines a single resource container. To actually fill it up with bindings to resources, it will look for installers. The scope defines how it will look for these installers, and will provide them with the container to install any available resource it has access to.

After the installation of resources in the scope's container is complete, it's ready to start the injection process.

### Global Scope

The `GlobalDependencyScope` is a scope that is valid throughout the entire lifetime of the game or program. In Unity, before the very first scene is loaded, it will bind the resources that should be available at all times, e.g. an input manager, a scene loading manager, a content manager, etc. This scope is always created upon start of the game or program when including this framework in your project. No need to do anything yourself in terms of setting it up.

To install any resources in its container, a codebase search is performed for static methods marked with the `GlobalScopeInstaller` attribute. This allows you to already load in and prepare your data, scriptable objects, etc. and assign them to the global container.

```cs
private static class MyGlobalScopeInstaller
{
	[GlobalScopeInstaller]
	private static void InstallScopeResources(IDependencyContainer globalContainer)
	{
		// Bind any resources that should be global.
	}
}
```

You can create multiple such methods spread across your project if your resources need to come from different places.

Now that global resources have been installed, each time a scene is loaded, it will scan that scene and inject any component that requires its resources.

#### Advanced

If you have a custom container implementation, you can provide an instance of this container for the global scope to use by decorating a static method with the `GlobalContainerProvider` attribute.

```cs
private static class CustomGlobalScopeProvider
{
	[GlobalContainerProvider]
	private static IDependencyContainer CreateGlobalContainer()
	{
		// Create and return the custom container.
		return new MyCustomContainer();
	}
}
```

### Scene Scope

The `SceneDependencyScope` is, as the name implies, a scope bound to a scene. Contrary to the global scope, this one is a `Component` that can be put on a GameObject. It has the option to inject its resources at `Start`, but the `Inject` method can also be called manually if another moment is more appropriate.

To install your bindings to the scene scope's container, create an installer script that implements the `IDependencyScopeInstaller` interface and add it to (a child of) the scene scope's game object.

```cs
public class MySceneInstaller : MonoBheaviour, IDependencyScopeInstaller
{
	void IDependencyScopeInstaller.Install(IDependencyContainer container)
	{
		// Install bindings of importance to the scene.
	}
}
```

When the `Inject` method is called of this scope, either in `Start` or manually, it will scan the scene for any injectable components and inject them with the resources bound to its container.

#### Advanced

Just like in the global scope, a custom container implementation can be provided. Here, this is done by one of its (child) components that implement the `IDependencyContainerProvider` interface.

```cs
public class CustomSceneScopeProvider : MonoBehaviour, IDependencyContainerProvider
{
	IDependencyContainer IDependencyContainerProvider.GetContainer()
	{
		// Create the custom container.
		return new MyCustomContainer();
	}
}
```

### Hierarchy Scope

The `HierarchyDependencyScope` is exactly the same as the [scene scope](#scene-scope) except it only operates on itself and any child GameObjects rather than the whole scene.

### Order of Events

Both the `SceneDependencyScope` and `HierarchyDependencyScope` are components to be placed on GameObjects. This means they are susceptible to being unpredictable in terms of when their injection process starts relative to other objects in the scene. That's why they have a very low script execution order value set, where the `SceneDependencyScope` is set to work before `HierarchyDependencyScope`. This works in tandem with the `GlobalDependencyScope` and is also the reason why the former two perform their work in `Start` rather than `Awake`.

Whenever a scene is loaded, the very first thing that happens is the `Awake` phase of all active objects in the scene. Next, Unity's scene manager notifies interested parties (among which is the `GlobalDependencyScope`), that the scene is ready, for which it will inject its resources in the objects found in this newly loaded scene. Next, the `Start` phase is initiated, where the `SceneDependencyScope` and `HierarchyDependencyScope` can inject their resources when enabled to do so, before any other object performs their `Start`.

This has the benefit that objects will have their resources injected in a top-down way, where more globally defined resources are injected first, and if a duplicate registration exists in a scope that is more local to the object, it gets overridden.

## Custom Dependency Injection

When a situation arises in which above outlined scopes do not fit the circumstances, it's possible to start the injection process yourself. All you need is a container with bindings and a target (or targets) to be injected. This is done using the static `DependencyInjector` class.

```cs
// Inject the bindings found in the container into the target.
DependencyInjector.Inject(myContainer, myTarget);
```

This might be helpfull in cases that objects are spawned further down the line and still need an injection with the resources found in a container. Additionally, you can bind the container to itself and have it injected into objects that need the container later on.

```cs
// Placed on a SceneDependencyScope.
public class CustomScopeInstaller : MonoBehaviour, IDependencyScopeInstaller
{
	void IDependencyScopeInstaller.Install(IDependencyContainer container)
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

Along with custom injections, _named_ injections may prove to be useful under specific circumstances. A member can be labeled as injectable along with a name that restricts where the dependency comes from. When injecting a target, a name can be passed along which states that only members labeled with that name will receive these resources.

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
