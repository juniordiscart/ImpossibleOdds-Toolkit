namespace ImpossibleOdds.DependencyInjection
{
	using UnityEngine;

	/// <summary>
	/// Injects the current scene's GameObjects with a DependencyContainer
	/// populated by scope installers on this GameObject and its children.
	/// </summary>
	[ExecuteAt(ExecutionOrderValue)]
	public class SceneDependencyScope : AbstractDependencyScopeBehaviour
	{
		public const int ExecutionOrderValue = -9999;

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
				Log.Info(this.gameObject, "Injecting scene '{0}' with the scene dependency scope.", gameObject.scene.name);
				gameObject.scene.GetRootGameObjects().Inject(DependencyContainer, true);
			}
			else
			{
				Log.Info(this.gameObject, "Injecting scene '{0}' with the scene dependency scope using '{1}' as injection identifier.", gameObject.scene.name, injectionId);
				gameObject.scene.GetRootGameObjects().Inject(DependencyContainer, injectionId, true);
			}
		}
	}
}
