namespace ImpossibleOdds.DependencyInjection
{
	using UnityEngine;

	[ScriptExecutionOrder(-9999)]
	public class GlobalDependencyContext : MonoBehaviour, IDependencyContext
	{
		public const string GlobalID = "ImpossibleOdds::DependencyInjection::GlobalContext";
		private const string ResourcesPath = "ImpossibleOdds/DependencyInjection/GlobalDependencyContext";

		private static GlobalDependencyContext globalContextInstance = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void LoadGlobalContext()
		{
			GlobalDependencyContext prefab = Resources.Load<GlobalDependencyContext>(ResourcesPath);
			globalContextInstance = Instantiate<GlobalDependencyContext>(prefab, Vector3.zero, Quaternion.identity);

			if (globalContextInstance == null)
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("No global dependecy context could be found in Resources at path '{0}'.", ResourcesPath);
#endif
				return;
			}

			globalContextInstance.name = GlobalID;
		}

		private DependencyContainer globalContainer = new DependencyContainer();

		public IDependencyContainer DependencyContainer
		{
			get { return globalContainer; }
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
			DependencyContextRegister.Register(GlobalID, this);
			InstallBindings();
		}

		private void InstallBindings()
		{
			IDependencyContextInstaller[] installers = GetComponentsInChildren<IDependencyContextInstaller>(false);
			foreach (IDependencyContextInstaller installer in installers)
			{
				installer.Install(this);
			}
		}
	}
}
