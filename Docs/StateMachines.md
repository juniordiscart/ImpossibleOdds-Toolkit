# ![Impossible Odds Logo][Logo] C# Toolkit - State Machines

All functionality described in this section can be found in the `ImpossibleOdds.StateMachines` namespace.

Whether it is by using simple `enum` values or a complex graph system, state machines are a construct that in one way or form, always pops up in every project. This toolkit provides the means to create both simple and complex state machines with either the complete freedom to change states at will, or have it manage it's own way of life.

Before diving deeper, let's first briefly explain what a state machine actually is. A state machine is essentially a set or collection of state objects, in which at most one state can be active at a time. An active state will perform its duty, while others don't do anything (at least not with regards to the state machine). From each state, a set of transitions can exist to other states in the same system. A transition can be triggered when a certain condition is met, moving on to the next state. This creates a directed graph of nodes which makes it come to life as the program or game continues to run.

It can be of use at all levels of a project, be it in the user interface with different panels, a complex puzzle with multiple stages, displaying a certain animation based on character's properties, etc.

## States

A _state_ in a state machine represents a status in the system, waiting for some trigger to move the system in a different state. For example, a character with a _health_ value. Depending on the health value, the character can be considered in different states:

* Healthy: nothing remarkable about the character. It can jump and move around at full speed.
* Injured: the character may struggle to perform certain actions and shout audio cues that it needs medical attention.
* KO: the character is considered knocked out and can't do anything anymore. This may trigger a request to restart a level.

Getting more technical, the `IState` interface allows you to define custom states that can be attached to a state machine. It requires the implementation of three simple methods:

* The `Enter` method is called when the state machine transitions into the state. This allows you to define custom logic that's necessary to set everything up. Similar to Unity's `Start` method.
* The `Update` method is called when that state is the currently active state of the state machine. Here you can define a state's logic while it is active.
* Finally, the `Exit` method is called whenever the state machine moves away from the state. This can be used to perform a cleanup, just like Unity's `OnDestroy` method.

Another simple example of a state is that of a traffic light:

```cs
public class TrafficLightState : IState
{
	[SerializeField]
	private Image light;
	[SerializeField]
	private Color activeColor;
	[SerializeField]
	private Color inactiveColor;

	private float timer = 0f;

	public float TimeActive
	{
		get => timer;
	}

	public void Enter()
	{
		timer = 0f;
		light.color = activeColor;
		enabled = true;
	}

	public void Exit()
	{
		timer = 0f;
		light.color = inactiveColor;
		enabled = false;
	}

	public void Update()
	{
		timer += Time.deltaTime;
	}
}
```

A traffic light state in this example starts counting the moment the state becomes active. When the timer reaches a certain threshold, it's considered 'complete'.

## Transitions

A _transition_ in a state machine defines the condition and trigger for a state machine to move from one state to another. From the same example as defined in the [states](#states) section, a transition from the 'healthy' to 'injured' health state could be that the monitored value drops below 33%:

* As long as the character is above 33% health, it will remain in the 'healthy' state,
* When the health value drops to 33% or below, the transition from 'healthy' to 'injured' is trigger to notify the state machine that it should switch states.
* In the same vein, a transition back, from 'injured' to 'healthy' could be added when the player's character finds a healing pack and gets it back up again above 33%.

To define your own state transitions, start by implementing the `IStateTransition<TStateKey>` interface. A transition defines a `From` and `To` key (it represents the key for those states in the state machine), as well as a check whether it should trigger or not. The type parameter `TStateKey` defines the type of these state keys.

Continuing the example of a traffic light, consider that the traffic light's transitions will be triggered when the state has been active for a specified amount of time, it can be implemented as:

```cs
public class TrafficLightTransition : IStateTransition<TrafficLightStateKey>
{
	private TrafficLightStateKey from;
	private TrafficLightStateKey to;
	private Func<bool> triggerCheck;

	public TrafficLightStateKey From
	{
		get => from;
	}

	public TrafficLightStateKey To
	{
		get => to;
	}

	public bool CanTrigger
	{
		get => triggerCheck();
	}

	public event Action<IStateTransition> onTriggered;

	public TrafficLightTransition(TrafficLightStateKey from, TrafficLightStateKey to, Func<bool> triggerCheck)
	{
		triggerCheck.ThrowIfNull(nameof(triggerCheck));
		this.from = from;
		this.to = to;
		this.triggerCheck = triggerCheck;
	}

	public void Trigger()
	{
		onTriggered.InvokeIfNotNull(this);
	}
}
```

The transition here will receive a trigger check `Func<bool>` object, as the timer value is managed outside of the transition object itself.

## State Machines

A state machine is where both state and transition objects are connected to each other and form one larger system.

The `StateMachine` class is a full state machine implementation where you can assign states and transitions. It keeps a record of all states and transitions that exist between them. Each state is identified using a descriptive key for the state, e.g. a string or enum value, denoted by the generic parameter of the state machine class. A transition is stored likewise, identified by the to- and from-keys it creates a bridge for.

### Adding States

Populating the state machine with states is done using `AddState`. It takes the descriptive key of the state and the state object itself, to create a key-value-pair like in a `Dictionary<TKey, TValue>`.

```cs
public class TrafficLight : MonoBehaviour
{
	[SerializeField]
	private TrafficLightState green;
	[SerializeField]
	private TrafficLightState yellow;
	[SerializeField]
	private TrafficLightState red;

	private StateMachine<TrafficLightStateKey> stateMachine = null;

	private void Start()
	{
		// Build the state machine.
		stateMachine = new StateMachine<TrafficLightStateKey>();
		stateMachine.AddState(TrafficLightStateKey.Green, green);
		stateMachine.AddState(TrafficLightStateKey.Yellow, yellow);
		stateMachine.AddState(TrafficLightStateKey.Red, red);

		// Move the state machine to its initial state.
		stateMachine.MoveToState(TrafficLightStateKey.Green);
	}
}
```

After all states have been added, the state machine itself does not reside in a valid state yet. You can start it by moving it to what you consider it its first or default state using the `MoveToState` method.

### Automated Transitions

In the above example, the traffic light state machine has a bunch of states assigned, but not any transitions yet. With the `AddTransition` method, the state machine is able to monitor the different conditions of the current state to move to a new one.

Completing to the `Start` method of the above example:

```cs
private void Start()
{
	// Setup state machine and states.
	stateMachine = new StateMachine<TrafficLightStateKey>();
	stateMachine.AddState(TrafficLightStateKey.Red, redState);
	stateMachine.AddState(TrafficLightStateKey.Yellow, yellowState);
	stateMachine.AddState(TrafficLightStateKey.Green, greenState);

	// Add transitions.
	stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Red, TrafficLightStateKey.Green, () => redState.TimeActive > 20f));
	stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Green, TrafficLightStateKey.Yellow), () => yellowState.TimeActive > 5f);
	stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Yellow, TrafficLightStateKey.Red), () => greenState.TimeActive > 30f);

	// Initialize the state machine to its start state.
	stateMachine.MoveToState(startState);
}
```

**Note**: multiple transitions can be created between the same two states in the same direction. In that case, whichever transition's condition is triggered first, will move the state machine to that state.

To make use of these automated transitions, the state machine should regularly poke the outgoing transitions of the current state. Since the `StateMachine` class does not inherit from `MonoBehaviour` (so that it may be used outside of the Unity-specific context), it must be taken care of externally using its `Update` method. Two immediate solutions are available though:

* Either, manually call the `Update` method from your `MonoBehaviour` script, or
* Hook it in the [Runnables system][Runnables], as the state machine implements the `IRunnable` interface.

In the traffic light example above, we'll just update the state machine through our own `Update` method:

```cs
private void Update()
{
	stateMachine.Update();
}
```

### Manual Transitions

In simple state machines though, setting up a full state machine including transitions can be cumbersome and you might want the freedom to switch states outside the logic defined by transitions.

You can simply omit adding any transitions to the state machine, and move through the states of your own accord using the `MoveToState` method.

```cs
// Manually move to the desired state, regardless of the state machine's current condition,
// or whether a transition exists from the current state to the new state.
trafficLight.MoveToState(TrafficLightStateKey.Green);
```

## Example

Below, you'll find the traffic light example in full.

```cs
public enum TrafficLightStateKey
{
	Green,
	Yellow,
	Red
}
```

``` cs
public class TrafficLightState : IState
{
	[SerializeField]
	private Image light;
	[SerializeField]
	private Color activeColor;
	[SerializeField]
	private Color inactiveColor;

	private float timer = 0f;

	public float TimeActive
	{
		get => timer;
	}

	public void Enter()
	{
		timer = 0f;
		light.color = activeColor;
		enabled = true;
	}

	public void Exit()
	{
		timer = 0f;
		light.color = inactiveColor;
		enabled = false;
	}

	public void Update()
	{
		timer += Time.deltaTime;
	}
}
```

```cs
public class TrafficLightTransition : IStateTransition<TrafficLightStateKey>
{
	private TrafficLightStateKey from;
	private TrafficLightStateKey to;
	private Func<bool> triggerCheck;

	public TrafficLightStateKey From
	{
		get => from;
	}

	public TrafficLightStateKey To
	{
		get => to;
	}

	public bool CanTrigger
	{
		get => triggerCheck();
	}

	public event Action<IStateTransition> onTriggered;

	public TrafficLightTransition(TrafficLightStateKey from, TrafficLightStateKey to, Func<bool> triggerCheck)
	{
		triggerCheck.ThrowIfNull(nameof(triggerCheck));
		this.from = from;
		this.to = to;
		this.triggerCheck = triggerCheck;
	}

	public void Trigger()
	{
		onTriggered.InvokeIfNotNull(this);
	}
}
```

```cs
public class TrafficLight : MonoBehaviour
{
	[SerializeField]
	private TrafficLightState green;
	[SerializeField]
	private TrafficLightState yellow;
	[SerializeField]
	private TrafficLightState red;

	private StateMachine<TrafficLightStateKey> stateMachine = null;

	private void Start()
	{
		// Setup state machine and states.
		stateMachine = new StateMachine<TrafficLightStateKey>();
		stateMachine.AddState(TrafficLightStateKey.Red, redState);
		stateMachine.AddState(TrafficLightStateKey.Yellow, yellowState);
		stateMachine.AddState(TrafficLightStateKey.Green, greenState);

		// Add transitions.
		stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Red, TrafficLightStateKey.Green, () => redState.TimeActive > 20f));
		stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Green, TrafficLightStateKey.Yellow), () => yellowState.TimeActive > 5f);
		stateMachine.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Yellow, TrafficLightStateKey.Red), () => greemState.TimeActive > 30f);

		// Initialize the state machine to its start state.
		stateMachine.MoveToState(startState);
	}

	private void Update()
	{
		stateMachine.Update();
	}
}
```

[Logo]: ./Images/ImpossibleOddsLogo.png
[Runnables]: ./Runnables.md
