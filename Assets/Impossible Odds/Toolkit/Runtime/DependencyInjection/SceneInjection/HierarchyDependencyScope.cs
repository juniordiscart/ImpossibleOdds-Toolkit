namespace ImpossibleOdds.DependencyInjection
{
	using UnityEngine;

	/// <summary>
	/// Injects only the current hierarchy of GameObjects found below this GameObject with
	/// a DependencyContainer populated by scope installers on this GameObject and its children.
	/// </summary>
	[ExecuteAfter(typeof(SceneDependencyScope))]
	public class HierarchyDependencyScope : AbstractDependencyScopeBehaviour
	{
		[SerializeField, Tooltip("Set an injection identifier to use during injection. When left empty no identifier will be used.")]
		private string injectionId = string.Empty;

		public string InjectionId
		{
			get => injectionId;
			set => injectionId = value;
		}

		public override void Inject()
		{
			if (string.IsNullOrEmpty(injectionId))
			{
				gameObject.Inject(DependencyContainer, true);
			}
			else
			{
				gameObject.Inject(DependencyContainer, injectionId, true);
			}
		}
	}
}
