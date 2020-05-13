# `ImpossibleOdds.DependencyInjection`
NEEDS REWORK!


To keep your codebase clean and loosely coupled, a 'dependency injection' framework can help by storing references to objects that are widely used across the lifetime of your game and to inject them in objects that require them. This allows for a versatile codebase that is easily maintainable and can be easily reused for testing scenarios.

The easiest case for using dependency injection is to minimise the use of singletons. These can severly limit the flexibility of your code: needing one singleton that depends on others quickly generates a sequence of dependencies that you need to include in your scene to test a specific scenario. By using interfaces for these classes and injecting them in objects that require them, you can easily create alternate versions that can be used as dummies for testing, or create variants of game modes.

Getting started with the `ImpossibleOdds.DependencyInjection` module, it is best to have a look at creating a 'dependency installer module' first. A dependency installer module is a component that implements the `IDependencyInstallerModule` interface and receives a `IDependencyContext` to which it can start to bind instances or factories to the dependency context.

```csharp
// A class that you need across your project.
public class MyImportantClass
{
	public int MessageID
	{
		get;
		set;
	}
}

// Installer module component that will bind types/interfaces
// to the container of this context. Add this one to an installer.
public class MyInstaller : MonoBehaviour, IDependencyInstallerModule
{
	public void Install(IDependencyContext context)
	{
		MyImportantData importantData = new MyImportantData();
		importantData.MessageID = 100;

		// Bind instance of MyImportantData.
		IDependencyContainer container = context.Container;
		container.Bind<MyImportantData>(new BindingFromInstance<MyImportantData>(importantData));
	}
}
```

Add your installer module to an actual 'dependency installer'. An installer is a Component that, when its `Awake()` function gets called, searches for any installer modules on its own and in its children and request the found modules to install themselves onto the dependency context. The following types of installers are defined:

* `GlobalDependencyInstaller`: this is an installer that becomes active before the first level of your game is loaded and remains in memory until it stops. It's a GameObject and is loaded from the 'Resources` directory upon initialisation. So to add any of your installer modules, attach them directly to the prefab. This installer won't perform any dependency injection to any GameObjects, since no GameObjects have been loaded yet at the time of creation of this installer.

* `SceneDependencyInstaller`: add this kind of installer component in your scene, if you wish to have scene-specific dependencies. When this GameObject awakes, it will install any module it finds, as well as inject all objects found in the scene.

Using the `SceneDependencyInstaller` component in your scene, your GameObjects and all of their children will get their fields injected once it awakes.

```csharp
public class MyNeedyClass : MonoBevaviour
{
	[Inject]
	private MyImportantClass importantData;

	private void Awake()
	{
		Debug.Log("Message ID: " + importantData.MessageID);
	}
}
```

However, GameObjects created using the `GameObject.Instantiate()` method and non-Unity objects will not get injected automatically. To inject their dependencies at run-time, you can call the static `DependencyInjector.Inject()` method, or one of the `Inject()` extension methods of GameObject and Component. These injection methods require a dependency context, and you can get the game-wide context from the `GlobalDependencyInstaller`, or request the context specific to the current scene from the `SceneDependencyInstaller`.
