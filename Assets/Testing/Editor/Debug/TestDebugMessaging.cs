using UnityEngine;

using Debug = ImpossibleOdds.Debug;

public class TestDebugMessaging : MonoBehaviour
{
	private void Start()
	{
		Debug.Info(this, "This is an info message.");
		Debug.Warning(this, "This is a warning message.");
		Debug.Error(this, "This is an error message.");
	}
}
