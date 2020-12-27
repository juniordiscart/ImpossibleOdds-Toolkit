namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Injects only the current hierarchy of GameObjects found below this GameObject with
	/// a DependencyContainer populated by context installers on this GameObject and its children.
	/// </summary>
	public class HierarchyDependencyContext : AbstractDependencyContextBehaviour
	{
		public override void Inject()
		{
			gameObject.Inject(DependencyContainer, true);
		}
	}
}
