namespace ImpossibleOdds.DependencyInjection.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(DependencyInjectionRejectionRegister))]
	public class DependencyInjectionRejectionRegisterEditor : Editor
	{
		private Vector2 scrollPosition = Vector2.zero;
		private bool showLoadedNamespaces = true;
		private List<string> allLoadedNamespaces = new List<string>();
		private List<string> singularNamespaces = new List<string>();
		private List<NamespaceGroup> namespaceGroups = new List<NamespaceGroup>();

		public override void OnInspectorGUI()
		{
			DependencyInjectionRejectionRegister register = target as DependencyInjectionRejectionRegister;

			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			DrawDefaultInspector();

			EditorGUILayout.Separator();

			EditorGUILayout.HelpBox("The list below shows all currently loaded namespaces in the project. Tick the box next to the namespace to exclude types within this namespace during the dependency injection process.", MessageType.None);
			EditorGUILayout.Space();

			showLoadedNamespaces = EditorGUILayout.Foldout(showLoadedNamespaces, "Loaded namespaces");
			if (showLoadedNamespaces)
			{
				int cachedIndentLevel = EditorGUI.indentLevel;
				EditorGUI.indentLevel += 1;

				// Singular namespaces
				foreach (string ns in singularNamespaces)
				{
					ShowNamespaceToggle(register, ns);
				}

				EditorGUILayout.Space();

				// Namespace groups
				foreach (NamespaceGroup nsGroup in namespaceGroups)
				{
					ShowNamespaceGroup(register, nsGroup);
				}

				EditorGUI.indentLevel = cachedIndentLevel;
			}

			EditorGUILayout.Space();
			GUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void OnEnable()
		{
			allLoadedNamespaces = GetAllLoadedNamespaces();
			allLoadedNamespaces.Sort();
			namespaceGroups = GetNamespaceGroups(allLoadedNamespaces);
			singularNamespaces = GetSingularNamespaces(allLoadedNamespaces, namespaceGroups);
		}

		private void ShowNamespaceToggle(DependencyInjectionRejectionRegister register, string ns)
		{
			string nsDisplay = !string.IsNullOrEmpty(ns) ? ns : "global:: - Types with no namespace defined.";
			bool currentValue = register.RejectedNamespaces.Contains(ns);
			bool newValue = EditorGUILayout.ToggleLeft(nsDisplay, currentValue);
			if (currentValue != newValue)
			{
				SetNamespaceStatus(register, ns, newValue);
			}
		}

		private void ShowNamespaceGroup(DependencyInjectionRejectionRegister register, NamespaceGroup nsGroup)
		{
			nsGroup.show = EditorGUILayout.Foldout(nsGroup.show, nsGroup.header);

			if (!nsGroup.show)
			{
				return;
			}

			int nrOfRejectedNamespaces = nsGroup.includedNamespaces.Count(ns => register.RejectedNamespaces.Contains(ns));
			bool allRejected = (nrOfRejectedNamespaces == nsGroup.includedNamespaces.Count);
			bool isMixedState = (nrOfRejectedNamespaces != 0) && !allRejected;

			EditorGUI.showMixedValue = isMixedState;
			bool groupValue = EditorGUILayout.ToggleLeft(nsGroup.header, allRejected);
			EditorGUI.showMixedValue = false;

			if (groupValue != allRejected)
			{
				SetNamespaceGroupStatus(register, nsGroup, groupValue);
			}

			int cachedIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 1;

			foreach (string ns in nsGroup.includedNamespaces)
			{
				ShowNamespaceToggle(register, ns);
			}

			EditorGUI.indentLevel = cachedIndentLevel;
		}

		private void SetNamespaceStatus(DependencyInjectionRejectionRegister register, string ns, bool rejected)
		{
			Undo.RecordObject(register, "RejectedNamespaces - Single");
			if (rejected)
			{
				if (!register.RejectedNamespaces.Contains(ns))
				{
					register.RejectedNamespaces.Add(ns);
					register.RejectedNamespaces.Sort();
				}
			}
			else
			{
				register.RejectedNamespaces.RemoveAll(rns => rns == ns);
			}
			EditorUtility.SetDirty(register);
		}

		private void SetNamespaceGroupStatus(DependencyInjectionRejectionRegister register, NamespaceGroup group, bool rejected)
		{
			Undo.RecordObject(register, "RejectedNamespaces - Group");

			foreach (string ns in group.includedNamespaces)
			{
				if (rejected)
				{
					if (!register.RejectedNamespaces.Contains(ns))
					{
						register.RejectedNamespaces.Add(ns);
						register.RejectedNamespaces.Sort();
					}
				}
				else
				{
					register.RejectedNamespaces.RemoveAll(rns => rns == ns);
				}
			}

			EditorUtility.SetDirty(register);
		}

		private List<string> GetAllLoadedNamespaces()
		{
			HashSet<string> namespaces = new HashSet<string>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (!namespaces.Contains(type.Namespace))
					{
						namespaces.Add(type.Namespace);
					}
				}
			}

			return namespaces.ToList();
		}

		private List<NamespaceGroup> GetNamespaceGroups(List<string> namespaces)
		{
			List<NamespaceGroup> namespaceGroups = new List<NamespaceGroup>();
			for (int i = 0; i < namespaces.Count;)
			{
				string ns = namespaces[i];
				if (string.IsNullOrEmpty(ns) || !ns.Contains('.'))
				{
					++i;
					continue;
				}

				string topLevelName = ns.Substring(0, ns.IndexOf('.'));
				NamespaceGroup namespaceGroup = new NamespaceGroup(topLevelName);
				namespaceGroups.Add(namespaceGroup);

				// Check if the top-level namespace also exists as an actual namespace
				if (namespaces.Contains(topLevelName))
				{
					namespaceGroup.includedNamespaces.Add(topLevelName);
				}

				while (ns.StartsWith(namespaceGroup.header + '.'))
				{
					namespaceGroup.includedNamespaces.Add(ns);
					++i;

					if (i < namespaces.Count)
					{
						ns = namespaces[i];
					}
					else
					{
						break;
					}
				}
			}

			return namespaceGroups;
		}

		private List<string> GetSingularNamespaces(List<string> namespaces, List<NamespaceGroup> namespaceGroups)
		{
			List<string> singularNamespaces = new List<string>();

			foreach (string ns in namespaces)
			{
				if (!namespaceGroups.Exists(nsg => nsg.ContainsNamespace(ns)))
				{
					singularNamespaces.Add(ns);
				}
			}

			return singularNamespaces;
		}

		private class NamespaceGroup
		{
			public bool show = false;
			public List<string> includedNamespaces = new List<string>();
			public readonly string header = string.Empty;

			public NamespaceGroup(string header)
			{
				this.header = header;
			}

			public bool ContainsNamespace(string ns)
			{
				return includedNamespaces.Contains(ns);
			}
		}
	}
}
