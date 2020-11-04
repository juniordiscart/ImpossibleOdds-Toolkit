using UnityEngine;

public class TestDebugMessaging : MonoBehaviour
{
	private void Start()
	{
		Log.Info(this, "This is an info message.");
		Log.Warning(this, "This is a warning message.");
		Log.Error(this, "This is an error message.");
	}
}
