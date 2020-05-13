# `ImpossibleOdds.Runnables`
Some objects need the ability to update themselves at the rate of Unity's `Update()` or `FixedUpdate()` loop, but don't need to be associated with Unity's system of GameObjects or MonoBehaviours. The `ImpossibleOdds.Runnables` module provides a framework to hook your non-Unity objects into the update loops.

There are two types of 'update runners' pre-defined to use. A `GlobalRunner` which is a persistent GameObject and runs as long as the game is running, and a `SceneRunner`, which is a GameObject that exists while the scene remains loaded. Each update runner also defines three timeframes in which the update call can run:

* `EarlyRunner` class: has a very low script execution order value, and will get updated before everyone else.

* `DefaultRunner` class: has no script execution order value defined.

* `LateRunner` class: has a very high script execution order value, and will get updated after everyone else.

To hook your object into one of these update runners, either implement the `IRunnable` or `IFixedRunnable` to hook into the `Update()` or `FixedUpdate()` loop respectively. Next, get a hold of an update runner and hook it in one of the update timeslots:

```csharp
public class MyClass : IRunnable
{
	public void EnableUpdate(bool enable)
	{
		// Use the early runner.
		if (enable)
		{
			GlobalRunner.Early.Add(this);
		}
		else
		{
			GlobalRunner.Early.Remove(this);
		}
	}

	public void Update()
	{
		// Doing update stuff...
	}
}
```

In case you'd like to define your own context on how such an update runner should work, the `IRunner` or `IFixedRunner` interfaces as well as the `AbstractRunnerCollection` class are a good place to start.
