﻿namespace ImpossibleOdds.Runnables
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class SceneRunner : Runner
	{
		private static Dictionary<Scene, SceneRunner> sceneRunners = new Dictionary<Scene, SceneRunner>();

		/// <summary>
		/// Get the runner that is associated with the currently active scene.
		/// If no runner is available for the current scene, one is created and registered.
		/// </summary>
		public static SceneRunner Get
		{
			get { return GetRunnerForScene(SceneManager.GetActiveScene()); }
		}

		/// <summary>
		/// Checks whether a scene runner exists for the currently active scene.
		/// </summary>
		public static bool Exists
		{
			get { return ExistsForScene(SceneManager.GetActiveScene()); }
		}

		/// <summary>
		/// Get the runner that is associated with the given scene.
		/// If no runner is available for the scene, a runner is created and registered.
		/// </summary>
		/// <param name="scene">The scene to retrieve the runner for.</param>
		/// <returns>The scene runner associated with the given scene.</returns>
		public static SceneRunner GetRunnerForScene(Scene scene)
		{
			if (!scene.IsValid())
			{
				throw new RunnablesException("Couldn't search a {0} for scene {1} because it is invalid.", typeof(SceneRunner).Name, scene.name);
			}
			else if (!scene.isLoaded)
			{
				throw new RunnablesException("Could'nt search a {0} for scene {1} because it isn't loaded.", typeof(SceneRunner).Name, scene.name);
			}

			if (sceneRunners.ContainsKey(scene))
			{
				return sceneRunners[scene];
			}

			GameObject sceneRunnerObj = new GameObject("ImpossibleOdds::SceneRunner_" + scene.name);
			SceneRunner sceneRunner = sceneRunnerObj.AddComponent<SceneRunner>();

			// Move the object to the requested scene, if necessary
			if (sceneRunnerObj.scene != scene)
			{
				SceneManager.MoveGameObjectToScene(sceneRunnerObj, scene);
			}

			return sceneRunner;
		}

		/// <summary>
		/// Checks whether a scene runner exists for the given scene.
		/// </summary>
		/// <param name="scene">The scene to check for whether a scene runner exists.</param>
		/// <returns>True if a runner is associated with the given scene. False otherwise.</returns>
		public static bool ExistsForScene(Scene scene)
		{
			return sceneRunners.ContainsKey(scene);
		}

		private static void RegisterRunner(Scene scene, SceneRunner runner)
		{
			runner.ThrowIfNull(nameof(runner));

			if (!scene.IsValid())
			{
				throw new RunnablesException("Cannot register a {0} for scene {1} because it is invalid.", typeof(SceneRunner).Name, scene.name);
			}
			else if (sceneRunners.ContainsKey(scene))
			{
				throw new RunnablesException("Only one instance of a {0} can be associated with scene {1}.", typeof(SceneRunner).Name, scene.name);
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
