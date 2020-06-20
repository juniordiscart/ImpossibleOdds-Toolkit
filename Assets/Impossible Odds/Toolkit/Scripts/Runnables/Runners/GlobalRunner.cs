namespace ImpossibleOdds.Runnables
{
	using UnityEngine;

	public class GlobalRunner : Runner
	{
		private static GlobalRunner globalRunner = null;

		public static GlobalRunner GetRunner
		{
			get
			{
				if (globalRunner == null)
				{
					CreateGlobalRunner();
				}

				return globalRunner;
			}
		}

		private static void CreateGlobalRunner()
		{
			GameObject globalRunnerObj = new GameObject("ImpossibleOdds_GlobalRunner");
			globalRunner = globalRunnerObj.AddComponent<GlobalRunner>();
			GameObject.DontDestroyOnLoad(globalRunner);
		}
	}
}
