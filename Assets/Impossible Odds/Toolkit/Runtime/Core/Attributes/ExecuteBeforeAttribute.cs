using System;
using UnityEngine;

namespace ImpossibleOdds
{
	/// <summary>
	/// Defines that a MonoBehaviour's script execution should happen before other scripts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExecuteBeforeAttribute : Attribute
	{
		public Type[] ExecuteBefore { get; }

		public ExecuteBeforeAttribute(params Type[] executeBefore)
		{
			executeBefore.ThrowIfNull(nameof(executeBefore));
			foreach (Type type in executeBefore)
			{
				type.ThrowIfNull(nameof(type));
				if (!typeof(MonoBehaviour).IsAssignableFrom(type))
				{
					throw new ImpossibleOddsException("The type '{0}' is not assignable from a {1}.", type.Name, nameof(MonoBehaviour));
				}
			}

			ExecuteBefore = executeBefore;
		}
	}
}