using UnityEngine;

namespace ImpossibleOdds.Runnables
{
	public class GlobalRunner : Runner
	{
		private static GlobalRunner globalRunner;

		public static GlobalRunner Get
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
			GameObject globalRunnerObj = new GameObject("ImpossibleOdds::GlobalRunner");
			globalRunner = globalRunnerObj.AddComponent<GlobalRunner>();
			GameObject.DontDestroyOnLoad(globalRunner);
		}
	}
}