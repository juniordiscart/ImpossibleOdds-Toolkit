using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An asset reference for GameObjects with a specific component on them.
/// </summary>
[Serializable]
public class ComponentReference<T> : AssetReferenceGameObject
where T : Component
{
	public ComponentReference(string guid)
	: base(guid)
	{ }

#if UNITY_EDITOR
	public override bool ValidateAsset(UnityEngine.Object obj)
	{
		if (obj is GameObject gObj)
		{
			return gObj.GetComponent<T>() != null;
		}
		else
		{
			return false;
		}
	}

	public override bool ValidateAsset(string path)
	{
		return ValidateAsset(AssetDatabase.LoadAssetAtPath<GameObject>(path));
	}

	public new T editorAsset
	{
		get
		{
			GameObject asset = base.editorAsset as GameObject;
			return asset != null ? asset.GetComponent<T>() : null;
		}
	}
#endif
}
