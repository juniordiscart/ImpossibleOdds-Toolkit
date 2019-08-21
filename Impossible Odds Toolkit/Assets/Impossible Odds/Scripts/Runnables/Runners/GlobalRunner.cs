namespace ImpossibleOdds.Runnables
{
	using UnityEngine;

	public class GlobalRunner : AbstractRunnerCollection
	{
		private const string GlobalRunnerName = "ImpossibleOdds_GlobalRunner";
		private const string GlobalRunnerPath = "ImpossibleOdds/Runnables/GlobalRunner";
		private static GlobalRunner globalRunner = null;

		public static new EarlyRunner Early
		{
			get
			{
				LoadGlobalRunner();
				return ((AbstractRunnerCollection)globalRunner).Early;
			}
		}

		public static new DefaultRunner Default
		{
			get
			{
				LoadGlobalRunner();
				return ((AbstractRunnerCollection)globalRunner).Default;
			}
		}

		public static new LateRunner Late
		{
			get
			{
				LoadGlobalRunner();
				return ((AbstractRunnerCollection)globalRunner).Late;
			}
		}

		private static void LoadGlobalRunner()
		{
			if (globalRunner != null)
			{
				return;
			}

			GlobalRunner globalContextPrefab = Resources.Load<GlobalRunner>(GlobalRunnerPath);
			if (globalContextPrefab == null)
			{
				throw new RunnablesException(string.Format("No global runnaer prefab was found in Resources at path '{0}'. Did you (re)move it?", GlobalRunnerPath));
			}

			globalRunner = GameObject.Instantiate<GlobalRunner>(globalContextPrefab);
			GameObject.DontDestroyOnLoad(globalRunner);
			globalRunner.name = GlobalRunnerName;
		}
	}
}
