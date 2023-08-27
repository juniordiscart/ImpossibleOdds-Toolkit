using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImpossibleOdds.Serialization.Processors;
using ImpossibleOdds.Xml;

public class IteratorsMemoryPressure : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

		XmlSerializationDefinition xmlDef = new XmlSerializationDefinition();
		TestGenericEnumerator(xmlDef.SerializationProcessors as ISerializationProcessor[]); // This one apparently generates garbage still when retrieving the iterator.
		TestConcreteEnumerator(xmlDef.SerializationProcessors as ISerializationProcessor[]);    // This one does not generate garbage.
	}

	private void TestGenericEnumerator<TEnumerable>(TEnumerable processors)
	where TEnumerable : IEnumerable<ISerializationProcessor>
	{
		foreach (ISerializationProcessor processor in processors)
		{
		}
	}

	private void TestConcreteEnumerator(ISerializationProcessor[] processors)
	{
		foreach (ISerializationProcessor processor in processors)
		{
		}
	}
}
