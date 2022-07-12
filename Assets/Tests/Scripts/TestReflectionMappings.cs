using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Caching;

public class CustomDictionary : Dictionary<int, string>
{ }

public class TestReflectionMappings : MonoBehaviour
{
	private void Start()
	{
		Type concreteListType = typeof(List<int>);
		Type genericListTypeDefintion = concreteListType.GetGenericTypeDefinition();

		Log.Info("Is {0} a sub-class of {1}: {2}.", genericListTypeDefintion.Name, concreteListType.Name, genericListTypeDefintion.IsAssignableFrom(concreteListType));

		Type concreteDictionaryType = typeof(Dictionary<int, string>);
		Type genericDictionaryTypeDefinition = concreteDictionaryType.GetGenericTypeDefinition();

		Log.Info(string.Join(", ", concreteDictionaryType.GetInterfaces().Select(t => t.Name)));
		Log.Info(string.Join(", ", genericDictionaryTypeDefinition.GetInterfaces().Select(t => t.Name)));
		Log.Info("Is still generic type? {0}", genericDictionaryTypeDefinition.IsGenericType);

		Type customGenericInheritedType = typeof(CustomDictionary);

		Log.Info("Is the custom generic inherited type still considered generic? {0}", customGenericInheritedType.IsGenericType);
		Log.Info("Is the base type of the custom generic inherited type still considered generic? {0}", customGenericInheritedType.BaseType.IsGenericType);
	}
}
