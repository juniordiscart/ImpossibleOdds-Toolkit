# ![Impossible Odds Logo][Logo] C# Toolkit - State Machines

All functionality described in this section can be found in the `ImpossibleOdds.StateMachines` namespace.

State machines are a construct that in one way or form, is always used in a project, whether it is by using simple `enum` values or a complex graph with defined conditions that can trigger the change in state. This toolkit provides the means to create both simple and complex state machines with either the complete freedom to change states at will, or have it manage it's own way of life.

## States

A _state_ in a state machine represents a status in the system, waiting for some trigger to move to a different state. For example, a character with a _health_ value. Depending on the health value, the character can be considered in different states, e.g.

* Healthy: nothing remarkable about the character. It can move around and act as normal.
* Injured: the character may struggle to perform certain actions and shout audio cues it needs medical attention.
* KO: the character is considered knocked out and can't do anything anymore.

The `IState` interface allows you to define custom states that can be attached to a state machine. It requires the implementation of three simple methods:

* The `Enter` method is called when the state machine transitions into that state. This allows you to define custom logic that's necessary to set everything up.
* The `Update` method is called when that state is the currently active state of the state machine. This method is mostly considered optional, but may be needed in case a state needs things done while it's active.
* Finally, the `Exit` method is called whenever the state machine moves away from the state. This can be used to perform a cleanup of sorts.

Another simple example of a state is that of a traffic light:

```cs
public class TrafficLightState : MonoBehaviour IState
{
	[SerializeField]
	private Image light;
	[SerializeField]
	private Color activeLightColor;

	public void Enter()
	{
		light.color = activeLightcolor;
	}

	public void Exit()
	{
		light.color = Color.black;
	}

	public void Update()
	{
		// Nothing to do here.
	}
}
```

## Transitions

A _transition_ in a state machine defines the condition and trigger for a state machine to move from one state to another. From the same example as defined in the [states](#states) section, a transition from the 'healthy' to 'injured' health state could be that the monitored value drops below 33%:

* As long as the character is above 33% health, it will remain in the 'healthy' state,
* When the health value drops to 33% or below, the transition from 'healthy' to 'injured' is trigger to notify the state machine that it should switch states.

To define your own state transitions, start by implementing the `IStateTransition<TStateKey>` interface. A transition defines an _origin_ and _destination_ key (it represents the key for those states in the state machine), as well as a check whether it should trigger or not. The type parameter `TStateKey` defines the type of these state keys.

Continuing the example of a traffic light, consider that a traffic light's transition in this case is defined by the time it has been in a state, it can be implemented as:

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

	private float timer = 0f;
	private float triggerTime = 0f;

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
		get { return timer >= triggerTime; }
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

A state machine is where both states and transitions are connected to each other and form one larger system that either lives on its own, or is managed from the outside to control into which state it should go to next.

The `StateMachine` class is a full state machine implementation where you can assign states and transitions.

### Adding States

### Manual Transitions

In simple scenarios, setting up a full state machine including transitions can be cumbersome and you might want the freedom to switch states outside the logic defined by transitions.

You can simply omit adding any transitions to the state machine, and move through the states of your own accord using the `MoveToState` method.

### Automated Transitions

[Logo]: ./Images/ImpossibleOddsLogo.png
