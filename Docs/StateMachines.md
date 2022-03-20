# ![Impossible Odds Logo][Logo] C# Toolkit - State Machines

All functionality described in this section can be found in the `ImpossibleOdds.StateMachines` namespace.

Whether it is by using simple `enum` values or a complex graph, state machines are a construct that in one way or form, always pops up in every project. This toolkit provides the means to create both simple and complex state machines with either the complete freedom to change states at will, or have it manage it's own way of life.

Before diving deeper, let's first briefly explain what a state machine actually is. A state machine is essentially a set or collection of state objects, of which at most one state can be active at a time. From each state, a set of transitions can exist to other states in the same state machine. A transition can be triggered when a certain condition is met, moving on to the next state. This creates a directed graph of nodes which makes it come to life as the program or game continues to run, triggering certain transitions and moving on to the next state.

It can be of use at all levels of a project, be it in the user interface with different panels, a complex puzzle with multiple stages, displaying a certain animations based on character properties, etc.

## States

A _state_ in a state machine represents a status in the system, waiting for some trigger to move to a different state. For example, a character with a _health_ value. Depending on the health value, the character can be considered in different states, e.g.

* Healthy: nothing remarkable about the character. It can move around at full speed and jump.
* Injured: the character may struggle to perform certain actions and shout audio cues it needs medical attention.
* KO: the character is considered knocked out and can't do anything anymore. This may trigger a request to restart a level.

Getting more technical, the `IState` interface allows you to define custom states that can be attached to a state machine. It requires the implementation of three simple methods:

* The `Enter` method is called when the state machine transitions into the state. This allows you to define custom logic that's necessary to set everything up.
* The `Update` method is called when that state is the currently active state of the state machine. This method is mostly considered optional, but may be needed in case a state needs things done while it's active.
* Finally, the `Exit` method is called whenever the state machine moves away from the state. This can be used to perform a cleanup.

Another simple example of a state is that of a traffic light:

```cs
public class TrafficLightState : MonoBehaviour, IState
{
	[SerializeField]
	private Image light;
	[SerializeField]
	private Color activeLightColor;
	[SerializeField]
	private float activeTime;

	private float timer;

	public bool IsComplete
	{
		get { return timer >= activeTime; }
	}

	public void Enter()
	{
		light.color = activeLightcolor;
		timer = 0f;
		enabled = true;
	}

	public void Exit()
	{
		light.color = Color.black;
		enabled = false;
	}

	public void Update()
	{
		timer += Time.deltaTime;
	}
}
```

A traffic light state in this example starts counting the moment the state becomes active. When the timer reaches a certain threshold, it's considered 'complete' and is ready to move to the next state.

## Transitions

A _transition_ in a state machine defines the condition and trigger for a state machine to move from one state to another. From the same example as defined in the [states](#states) section, a transition from the 'healthy' to 'injured' health state could be that the monitored value drops below 33%:

* As long as the character is above 33% health, it will remain in the 'healthy' state,
* When the health value drops to 33% or below, the transition from 'healthy' to 'injured' is trigger to notify the state machine that it should switch states.
* In the same vein, a transition back, from 'injured' to 'healthy' could be added when the player's character finds a healing pack and gets it back up again above 33%.

To define your own state transitions, start by implementing the `IStateTransition<TStateKey>` interface. A transition defines a `From` and `To` key (it represents the key for those states in the state machine), as well as a check whether it should trigger or not. The type parameter `TStateKey` defines the type of these state keys.

Continueing the example of a traffic light, consider that a traffic light's transition in this case is defined by the time it has been in a state, it can be implemented as:

```cs
public enum TrafficLightStateKey
{
	Green,
	Yellow,
	Red
}

public class TrafficLightTransition : IStateTransition<TrafficLightStateKey>
{
	public event Action<IStateTransition> onTriggered;

	private TrafficLightStateKey from;
	private TrafficLightStateKey to;

	public TrafficLightTransition(TrafficLightStateKey from, TrafficLightStateKey to)
	{
		this.from = from;
		this.to = to;
	}

	public TrafficLightStateKey From
	{
		get;
	}

	public TrafficLightStateKey To
	{
		get;
	}

	public bool CanTrigger
	{
		get { return from.IsComplete; }
	}

	public void Trigger()
	{
		// If the current state has reached it's time threshold, it should trigger.
		// Invoking the event will signal the state machine it should do the transition.
		if (CanTrigger)
		{
			onTriggered.InvokeIfNotNull(this);
		}
	}
}
```

Note here that, in this example, the `TrafficLightStateKey` enum is defined as the key for states in the state machine. You'll come to know of what these key values mean in the [State Machines](#state-machines) section.

## State Machines

A state machine is where both states and transitions are connected to each other and form one larger system.

The `StateMachine` class is a full state machine implementation where you can assign states and transitions. It keeps a record of all states and transitions that exist between them. Each state is identified using a descriptive key for the state, e.g. a string or enum value. A transition is stored likewise, identified using the keys for the states it creates a bridge for.

### Adding States

Populating the state machine with states is done using `AddState`. It takes the descriptive key for the state and the state object itself, of course.

```cs
public class TrafficLight : MonoBehaviour
{
	[SerializeField]
	private TrafficLightState redState;
	[SerializeField]
	private TrafficLightState yellowState;
	[SerializeField]
	private TrafficLightState greenState;

	[SerializeField]
	private TrafficLightStateKey startState;

	private StateMachine<TrafficLightStateKey> trafficLightSM;

	private void Start()
	{
		// Setup state machine and states.
		trafficLightSM = new StateMachine<TrafficLightStateKey>();
		trafficLightSM.AddState(TrafficLightStateKey.Red, redState);
		trafficLightSM.AddState(TrafficLightStateKey.Yellow, yellowState);
		trafficLightSM.AddState(TrafficLightStateKey.Green, greenState);

		// Initialize the state machine to its start state.
		trafficLightSM.MoveToState(startState);
	}
}
```

After all states have been added, the state machine itself does not reside in a valid state. You can start it by moving it to what you consider its first state using th `MoveToState` method.

### Automated Transitions

In the above example, the traffic light state machine has a bunch of states assigned, but not any transitions yet. With the `AddTransition` method, the state machine is able to monitor the different conditions of the current state to move to a new one.

Adding to the `Start` method of the above example:

```cs
private void Start()
{
	// Setup state machine and states.
	trafficLightSM = new StateMachine<TrafficLightStateKey>();
	trafficLightSM.AddState(TrafficLightStateKey.Red, redState);
	trafficLightSM.AddState(TrafficLightStateKey.Yellow, yellowState);
	trafficLightSM.AddState(TrafficLightStateKey.Green, greenState);

	// Add transitions.
	trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Red, TrafficLightStateKey.Green));
	trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Green, TrafficLightStateKey.Yellow));
	trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Yellow, TrafficLightStateKey.Red));

	// Initialize the state machine to its start state.
	trafficLightSM.MoveToState(startState);
}
```

**Note**: multiple transitions can be created between the same two states in the same direction. In that case, whichever transition's condition is triggered first, will move the state machine to that state.

To make use of these automated transitions, the state machine should regularly poke the outgoing transitions of the current state. Since the `StateMachine` class does not inherit from `MonoBehaviour` (so that it may be used outside of the Unity-specific context), it must be taken care of externally using the `MonitorCurrentState` method. Two immediate solutions are available though:

* Either, manually call the `MonitorCurrentState` method from your script's `Update` method, or
* Use the `RunnableStateMachine` class instead (it derives from `StateMachine`) and hook it to the [Runnables system][Runnables].

In the traffic light example above, we've opted to use the regular `StateMachine` class, and thus will need to take care of that ourselves through `Update`:

```cs
private void Update()
{
	trafficLightSM.MonitorCurrentState();
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
public class TrafficLightState : MonoBehaviour, IState
{
	[SerializeField]
	private Image light;
	[SerializeField]
	private Color activeLightColor;
	[SerializeField]
	private float activeTime;

	private float timer;

	public bool IsComplete
	{
		get { return timer >= activeTime; }
	}

	public void Enter()
	{
		light.color = activeLightcolor;
		timer = 0f;
		enabled = true;
	}

	public void Exit()
	{
		light.color = Color.black;
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
	public event Action<IStateTransition> onTriggered;

	private TrafficLightStateKey from;
	private TrafficLightStateKey to;

	public TrafficLightTransition(TrafficLightStateKey from, TrafficLightStateKey to)
	{
		this.from = from;
		this.to = to;
	}

	public TrafficLightStateKey From
	{
		get;
	}

	public TrafficLightStateKey To
	{
		get;
	}

	public bool CanTrigger
	{
		get { return from.IsComplete; }
	}

	public void Trigger()
	{
		// If the current state has reached it's time threshold, it should trigger.
		// Invoking the event will signal the state machine it should do the transition.
		if (CanTrigger)
		{
			onTriggered.InvokeIfNotNull(this);
		}
	}
}
```

```cs
public class TrafficLight : MonoBehaviour
{
	[SerializeField]
	private TrafficLightState redState;
	[SerializeField]
	private TrafficLightState yellowState;
	[SerializeField]
	private TrafficLightState greenState;

	[SerializeField]
	private TrafficLightStateKey startState;

	private StateMachine<TrafficLightStateKey> trafficLightSM;

	private void Start()
	{
		// Setup state machine and states.
		trafficLightSM = new StateMachine<TrafficLightStateKey>();
		trafficLightSM.AddState(TrafficLightStateKey.Red, redState);
		trafficLightSM.AddState(TrafficLightStateKey.Yellow, yellowState);
		trafficLightSM.AddState(TrafficLightStateKey.Green, greenState);

		// Add transitions.
		trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Red, TrafficLightStateKey.Green));
		trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Green, TrafficLightStateKey.Yellow));
		trafficLightSM.AddTransition(new TrafficLightTransition(TrafficLightStateKey.Yellow, TrafficLightStateKey.Red));

		// Initialize the state machine to its start state.
		trafficLightSM.MoveToState(startState);
	}

	private void Update()
	{
		trafficLightSM.MonitorCurrentState();
	}
}
```

[Logo]: ./Images/ImpossibleOddsLogo.png
[Runnables]: ./Runnables.md
