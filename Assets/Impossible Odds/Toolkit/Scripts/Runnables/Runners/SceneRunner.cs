namespace ImpossibleOdds.Runnables
{
	using System;
	using System.Collections.Generic;
	using UnityEngine.SceneManagement;

	[ScriptExecutionOrder(-9998)]
	public class SceneRunner : AbstractRunnerCollection
	{
		private static Dictionary<Scene, SceneRunner> sceneRunners = new Dictionary<Scene, SceneRunner>();

		public static SceneRunner GetRunner(Scene scene)
		{
			return sceneRunners.ContainsKey(scene) ? sceneRunners[scene] : null;
		}

		private static void RegisterRunner(Scene scene, SceneRunner runner)
		{
			runner.ThrowIfNull(nameof(runner));

			if (!scene.IsValid())
			{
				throw new RunnablesException("Cannot register a " + typeof(SceneRunner).Name + " for scene " + scene.name + " because it is invalid.");
			}

			sceneRunners.Add(scene, runner);
		}

		private static void RemoveRunner(Scene scene)
		{
			sceneRunners.Remove(scene);
		}

		private void Awake()
		{
			RegisterRunner(gameObject.scene, this);
		}

		private void OnDestroy()
		{
			RemoveRunner(gameObject.scene);
		}
	}
}
