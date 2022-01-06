namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Injects the current scene's GameObjects with a DependencyContainer
	/// populated by scope installers on this GameObject and its children.
	/// </summary>
	[ExecuteAt(ExecutionOrderValue)]
	public class SceneDependencyScope : AbstractDependencyScopeBehaviour
	{
		public const int ExecutionOrderValue = -9999;

		public override void Inject()
		{
			Log.Info(this.gameObject, "Injecting scene '{0}' with the scene dependency scope.", gameObject.scene.name);
			gameObject.scene.GetRootGameObjects().Inject(DependencyContainer, true);
		}
	}
}
