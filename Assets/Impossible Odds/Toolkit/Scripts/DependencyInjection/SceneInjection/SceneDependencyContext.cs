namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Injects the current scene's GameObjects with a DependencyContainer
	/// populated by context installers on this GameObject and its children.
	/// </summary>
	[ScriptExecutionOrder(ExecutionOrderValue)]
	public class SceneDependencyContext : AbstractDependencyContextBehaviour
	{
		public const int ExecutionOrderValue = -9999;

		public override void Inject()
		{
			Log.Info(this.gameObject, "Injecting scene '{0}' with the scene dependency context.", gameObject.scene.name);
			gameObject.scene.GetRootGameObjects().Inject(DependencyContainer, true);
		}
	}
}
