namespace ImpossibleOdds
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;

	[InitializeOnLoad]
	internal class ScriptOrderManager
	{
		static ScriptOrderManager()
		{
			Dictionary<Type, ScriptExecutionInfo> scripts = GenerateScriptMapping();

			// Do a preliminary check to see if anything needs to be changed.
			// If not all execution orders are valid/respected, then try to resolve the execution order.
			if ((scripts.Count > 0) && !scripts.All(script => script.Value.IsValidExecutionOrder))
			{
				try
				{
					ResolveExecutionOrder(scripts.Values);
				}
				catch (Exception e)
				{
					Log.Exception(e);
					Log.Error("Failed to resolve the script execution order dependencies. New script execution order values will not be applied.");
					return;
				}
			}

			// Apply new values.
			foreach (ScriptExecutionInfo script in scripts.Values)
			{
				if (script.Order != MonoImporter.GetExecutionOrder(script.MonoScript))
				{
					MonoImporter.SetExecutionOrder(script.MonoScript, script.Order);
				}
			}
		}

		private static Dictionary<Type, ScriptExecutionInfo> GenerateScriptMapping()
		{
			Dictionary<Type, ScriptExecutionInfo> scripts = new Dictionary<Type, ScriptExecutionInfo>();

			// Find all monoscripts, and catalog them
			foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
			{
				Type monoType = monoScript.GetClass();
				if ((monoType != null) && typeof(MonoBehaviour).IsAssignableFrom(monoType))
				{
					scripts[monoType] =
						Attribute.IsDefined(monoType, typeof(ExecuteAtAttribute)) ?
						new ScriptExecutionInfo(monoScript, monoType, monoType.GetCustomAttribute<ExecuteAtAttribute>(false).Order, true) :
						new ScriptExecutionInfo(monoScript, monoType, MonoImporter.GetExecutionOrder(monoScript));
				}
			}

			// Find scripts that have execution dependencies on others
			foreach (ScriptExecutionInfo script in scripts.Values)
			{
				// Scripts that depend on executing before others
				if (Attribute.IsDefined(script.Type, typeof(ExecuteBeforeAttribute)))
				{
					ExecuteBeforeAttribute attr = script.Type.GetCustomAttribute<ExecuteBeforeAttribute>(false);
					foreach (Type executeBeforeType in attr.ExecuteBefore)
					{
						if (scripts.ContainsKey(executeBeforeType))
						{
							script.AddDependency(scripts[executeBeforeType]);
						}
					}
				}

				// Scripts that depend on executing after others
				if (Attribute.IsDefined(script.Type, typeof(ExecuteAfterAttribute)))
				{
					ExecuteAfterAttribute attr = script.Type.GetCustomAttribute<ExecuteAfterAttribute>(false);
					foreach (Type executeAfterType in attr.ExecuteAfter)
					{
						if (scripts.ContainsKey(executeAfterType))
						{
							scripts[executeAfterType].AddDependency(script);
						}
					}
				}
			}

			return scripts;
		}

		private static void ResolveExecutionOrder(IEnumerable<ScriptExecutionInfo> scripts)
		{
			// Sort the scripts per dependency 'island'.
			List<List<ScriptExecutionInfo>> sortedScriptIslands = SortScriptsTopologically(scripts);

			// Assign an order value in each island.
			foreach (List<ScriptExecutionInfo> island in sortedScriptIslands)
			{
				// Determine the highest order value in the island first, keeping in mind fixed order scripts.
				int orderValue = 0;
				if (island.Any(script => script.IsFixedOrder))
				{
					ScriptExecutionInfo highestOrderScript = island.Where(s => s.IsFixedOrder).Aggregate((i, j) => i.Order > j.Order ? i : j);
					int indexOfHighestOrder = island.IndexOf(highestOrderScript);
					orderValue = highestOrderScript.Order + (island.Count - indexOfHighestOrder - 1);
				}
				else
				{
					orderValue = island.Max(script => script.Order);
				}

				// Assign an order value, working from back to front.
				for (int i = island.Count - 1; i >= 0; --i)
				{
					ScriptExecutionInfo script = island[i];
					if (script.IsFixedOrder)
					{
						// If a fixed order script is encountered with an order value that is still higher than the expected order,
						// then an impossible dependency or timing has been discovered.
						if (script.Order > orderValue)
						{
							List<string> scriptNames = new List<string>(island.GetRange(i, island.Count - 1 - i).Select(s => s.Type.Name));
							throw new ImpossibleOddsException("Script {0} has a fixed execution order value of {1}, but other scripts following up have already been assigned an execution order lower than that.");
						}

						// Let the next order values fall through to the fixed order value.
						orderValue = script.Order;
					}
					else
					{
						script.Order = orderValue;
					}

					--orderValue;
				}
			}
		}

		/// Sorts the scripts topologically, per script island.
		private static List<List<ScriptExecutionInfo>> SortScriptsTopologically(IEnumerable<ScriptExecutionInfo> allScripts)
		{
			List<GraphNode> topoNodes = CreateTopoGraph(allScripts);
			List<GraphNode> sortedNodes = new List<GraphNode>(topoNodes.Count);
			List<HashSet<GraphNode>> islands = new List<HashSet<GraphNode>>();

			// Visit the nodes that haven't been permanently been visited yet.
			while (topoNodes.Exists(node => !node.IsPermanent))
			{
				topoNodes.First(node => !node.IsPermanent).Visit(sortedNodes, islands);
			}

			// Reverse the result for convenience
			sortedNodes.Reverse();

			// Separate the sorted nodes in islands, based on their connectivity.
			List<List<ScriptExecutionInfo>> sortedScripts = new List<List<ScriptExecutionInfo>>(islands.Count);
			foreach (HashSet<GraphNode> island in islands)
			{
				List<ScriptExecutionInfo> sortedIsland = new List<ScriptExecutionInfo>(island.Count);
				foreach (GraphNode node in sortedNodes)
				{
					if (island.Contains(node))
					{
						sortedIsland.Add(node.node);
					}
				}
				sortedScripts.Add(sortedIsland);
			}

			return sortedScripts;
		}

		/// Creates a topological graph of the scripts as defined by which set of dependencies is being requested.
		private static List<GraphNode> CreateTopoGraph(IEnumerable<ScriptExecutionInfo> allScripts)
		{
			HashSet<ScriptExecutionInfo> filteredScripts = new HashSet<ScriptExecutionInfo>();
			foreach (ScriptExecutionInfo script in allScripts)
			{
				if (script.HasDependencies)
				{
					filteredScripts.Add(script);
					foreach (ScriptExecutionInfo dependencyScript in script.Dependencies)
					{
						filteredScripts.Add(dependencyScript);
					}
				}
			}

			// Create a list of all the nodes in the graph based on the filtered scripts
			List<GraphNode> topoNodes = new List<GraphNode>();
			foreach (ScriptExecutionInfo script in filteredScripts)
			{
				topoNodes.Add(new GraphNode(script));
			}

			// Create the edges in the graph
			foreach (GraphNode topoNode in topoNodes)
			{
				if (topoNode.node.HasDependencies)
				{
					topoNode.edges.AddRange(topoNodes.Where(t => topoNode.node.Dependencies.Contains(t.node)));
				}
			}

			return topoNodes;
		}

		private class ScriptExecutionInfo : IComparable<ScriptExecutionInfo>
		{
			private readonly Type type = null;
			private readonly MonoScript monoScript = null;
			private readonly bool isFixedOrder = false;
			private int order = 0;
			private HashSet<ScriptExecutionInfo> dependsOn = new HashSet<ScriptExecutionInfo>();

			public MonoScript MonoScript
			{
				get => monoScript;
			}

			public Type Type
			{
				get => type;
			}

			public bool IsFixedOrder
			{
				get => isFixedOrder;
			}

			public int Order
			{
				get => order;
				set
				{
					if (IsFixedOrder)
					{
						throw new ImpossibleOddsException("Cannot change the execution order of a script that is fixed.");
					}

					order = value;
				}
			}

			public bool IsValidExecutionOrder
			{
				get => !HasDependencies || (order < dependsOn.Min(d => d.order));
			}

			public bool HasDependencies
			{
				get => dependsOn.Count > 0;
			}

			public HashSet<ScriptExecutionInfo> Dependencies
			{
				get => dependsOn;
			}

			public ScriptExecutionInfo(MonoScript monoScript, Type type, int order)
			: this(monoScript, type, order, false)
			{ }

			public ScriptExecutionInfo(MonoScript monoScript, Type type, int order, bool isFixedOrder)
			{
				this.monoScript = monoScript;
				this.type = type;
				this.order = order;
				this.isFixedOrder = isFixedOrder;
			}

			public int CompareTo(ScriptExecutionInfo other)
			{
				return order.CompareTo(other.order);
			}

			public void AddDependency(ScriptExecutionInfo script)
			{
				if (script == this)
				{
					throw new ImpossibleOddsException("Cannot let a script depend on itself.");
				}

				dependsOn.Add(script);
			}
		}

		/// <summary>
		/// Topological sorting node.
		/// </summary>
		private class GraphNode : IComparable<GraphNode>
		{
			public readonly ScriptExecutionInfo node;
			public readonly List<GraphNode> edges;
			private int mark = 0;

			public bool IsUnvisited
			{
				get => mark == 0;
				set => mark = (value ? 0 : mark);
			}

			public bool IsTemporary
			{
				get => mark == 1;
				set => mark = (value ? 1 : mark);
			}

			public bool IsPermanent
			{
				get => mark == 2;
				set => mark = (value ? 2 : mark);
			}

			public GraphNode(ScriptExecutionInfo script)
			{
				node = script;
				edges = new List<GraphNode>();
			}

			public void Visit(List<GraphNode> nodes, List<HashSet<GraphNode>> islands)
			{
				if (IsPermanent)
				{
					return;
				}
				else if (IsTemporary)
				{
					List<string> cyclicPath = new List<string>();
					GraphNode errorNode = this;
					do
					{
						cyclicPath.Add(errorNode.node.Type.Name);
						errorNode = errorNode.edges.First(edge => edge.IsTemporary);
					} while (errorNode != this);
					cyclicPath.Add(errorNode.node.Type.Name);
					throw new ImpossibleOddsException("Cyclic script execution order dependencies detected: {0}.", string.Join(" -> ", cyclicPath));
				}

				IsTemporary = true;

				for (int i = 0; i < edges.Count; ++i)
				{
					edges[i].Visit(nodes, islands);
				}

				IsPermanent = true;
				nodes.Add(this);

				if ((islands.Count == 0) || (edges.Count == 0))
				{
					islands.Add(new HashSet<GraphNode>() { this });
				}
				else if (edges.Count > 0)
				{
					List<HashSet<GraphNode>> connectedIslands = islands.FindAll(island => edges.Any(edge => island.Contains(edge)));

					if (connectedIslands.Count == 1)
					{
						connectedIslands[0].Add(this);
					}
					else
					{
						// This node connects multiple islands now, and should get merged into one
						HashSet<GraphNode> mainIsland = connectedIslands[0];
						for (int i = 1; i < connectedIslands.Count; ++i)
						{
							HashSet<GraphNode> secondaryIsland = connectedIslands[i];
							foreach (GraphNode connectedNode in secondaryIsland)
							{
								mainIsland.Add(connectedNode);
							}

							islands.Remove(secondaryIsland);
						}
					}
				}
			}

			public int CompareTo(GraphNode other)
			{
				if (this.node.IsFixedOrder && other.node.IsFixedOrder)
				{
					return node.Order.CompareTo(other.node.Order);
				}
				else if (this.node.IsFixedOrder)
				{
					return -1;
				}
				else if (other.node.IsFixedOrder)
				{
					return 1;
				}

				return 0;
			}
		}
	}
}
