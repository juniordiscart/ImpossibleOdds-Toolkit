/// <summary>
/// Base on: https://forum.unity.com/threads/script-execution-order-manipulation.130805/#post-1876943
/// </summary>

namespace ImpossibleOdds
{
	using System;
	using UnityEditor;

	[InitializeOnLoad]
	public class ScriptOrderManager
	{
		static ScriptOrderManager()
		{
			foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
			{
				if (monoScript.GetClass() != null)
				{
					foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptExecutionOrderAttribute)))
					{
						int currentOrder = MonoImporter.GetExecutionOrder(monoScript);
						int newOrder = ((ScriptExecutionOrderAttribute)a).order;
						if (currentOrder != newOrder)
						{
							MonoImporter.SetExecutionOrder(monoScript, newOrder);
						}
					}
				}
			}
		}
	}
}