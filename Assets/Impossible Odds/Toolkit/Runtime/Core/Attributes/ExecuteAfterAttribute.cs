namespace ImpossibleOdds
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Defines that a MonoBehaviour's script execution should happen after other scripts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExecuteAfterAttribute : Attribute
	{
		private readonly Type[] executeAfter = null;

		public Type[] ExecuteAfter
		{
			get => executeAfter;
		}

		public ExecuteAfterAttribute(params Type[] executeAfter)
		{
			executeAfter.ThrowIfNull(nameof(executeAfter));
			foreach (Type type in executeAfter)
			{
				type.ThrowIfNull(nameof(type));
				if (!typeof(MonoBehaviour).IsAssignableFrom(type))
				{
					throw new ImpossibleOddsException("The type '{0}' is not assignable from a {1}.", type.Name, typeof(MonoBehaviour).Name);
				}
			}

			this.executeAfter = executeAfter;
		}
	}
}
