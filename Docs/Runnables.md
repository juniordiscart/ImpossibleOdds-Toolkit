# ![Impossible Odds Logo][Logo] C# Toolkit - Runnables

All functionality described in this section can be found in the `ImpossibleOdds.Runnables` namespace.

Sometimes, having a script be a `MonoBehaviour` feels like overkill for what it tries to do, or simply needs the update functionality without any of the Unity callstack behind it. This tool tries to minimize/eliminate this by

## Runnables

A runnable, as defined here, is an object that requests to be notified each update of the game or program. In Unity, this is facilitated by the calls to `Update`, `FixedUpdate` or `LateUpdate` methods. For each of these kinds of update calls, an interface is defined that your custom classes can implement to let the Runnables framework know in what kind of update it's interested:

* `IRunnable`: for when Unity performs a call to `Update`,
* `IFixedRunnable`: for when Unity performs a call to `FixedUpdate`, and
* `ILateRunnable`: for when Unity performs a call to, you guessed it, `LateUpdate`.

```cs
// Requires ever kind of update.
public class MyObject : IRunnable, IFixedRunnable, ILateRunnable
{
	public void Update()
	{
		// Do update stuff.
	}

	public void FixedUpdate()
	{
		// Do fixed update stuff.
	}

	public void LateUpdate()
	{
		// Do late update stuff.
	}
}
```

## Runners

To have you objects actually receive their calls to the kind of update they want, they should be registred to a runner. A runner collects the different objects that require update notifications and delegate the type of events they need.

The following types of runners are predefined for you to use:

* The `GlobalRunner` is available at all times and can be accessed from anywhere.
* The `SceneRunner` is a per-scene runner that is available as long as the scene remains loaded.

**Note**: since these work in the context of Unity, there's no magic trick involved to circumvent the `GameObject` and `MonoBehaviour` concepts entirely. In the end, the runners defined here are still components placed on a game object, who simply delegate each kind of update to the registered runnables.

### Global Runner

The `GlobalRunner` object is a runner that is created on demand and remains available afterwards for the duration of the game or program.

```cs
// Creates a global runner if it wasn't already available,
// and registers the runnable for the Update phase.
IRunnable myRunnable;
GlobalRunner.Get.AddUpdate(myRunnable);
```

### Scene Runner

A `SceneRunner` object is a runner that is created on demand as well and remains available for duration of the scene's lifetime.

```cs
// Creates a scene runner for the current active scene if it wasn't already available
// and registers the runnable for the Update phase.
IRunnable myRunnable;
SceneRunner.Get.AddUpdate(myRunnable);
```

Since it's possible to have multiple scenes loaded and running at the same time, you can get a runner for a loaded scene that is different from the active scene:

```cs
// Creates a scene runner for the requested scene provided that it's loaded.
Scene otherLoadedScene;
IRunnable myRunnable;
SceneRunner.GetRunnerForScene(otherLoadedScene).AddUpdate(myRunnable);
```

You can also query if a runner exists:

```cs
// Check whether a scene runner exists for the active scene.
if (SceneRunner.Exists)
{
	...
}

// Check whether a scene runner exists for the given scene.
Scene otherLoadedScene;
if (SceneRunner.ExistsForScene(otherLoadedScene))
{
	...
}
```

### Advanced

To create a custom runner object, there are several interfaces available to implement specific update functionality.

* `IRunner` allows a runnable to hook in to the runner's `Update` phase.
* `IFixedRunner` allows a runnable to hook into the runner's `FixedUpdate` phase.
* `ILateRunner` allows a runnable to hook into the runner's `LateUpdate` phase.
* `IRoutineRunner` allows to start coroutine on the runner. On the runners defined above this is already possible without this interface (since they are a `MonoBehaviour`), but defining it here makes it explicitly available to use when checking against this interface.

Apart from these interfaces, there's already a `Runner` class available that implements all of these, but lacks a proper lifetime context, so you are free to fill that in for any extra custom derived classes.

[Logo]: ./Images/ImpossibleOddsLogo.png
