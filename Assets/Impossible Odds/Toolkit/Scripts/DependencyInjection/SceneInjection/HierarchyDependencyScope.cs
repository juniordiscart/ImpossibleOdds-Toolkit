namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Injects only the current hierarchy of GameObjects found below this GameObject with
	/// a DependencyContainer populated by scope installers on this GameObject and its children.
	/// </summary>
	[ExecuteAfter(typeof(SceneDependencyScope))]
	public class HierarchyDependencyScope : AbstractDependencyScopeBehaviour
	{
		public override void Inject()
		{
			gameObject.Inject(DependencyContainer, true);
		}
	}
}
