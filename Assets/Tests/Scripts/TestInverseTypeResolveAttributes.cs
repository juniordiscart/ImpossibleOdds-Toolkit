using System.Collections;
using System.Collections.Generic;
using ImpossibleOdds;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;
using UnityEngine;

public class TestInverseTypeResolveAttributes : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Process(new A());
		}
		else if (Input.GetKeyDown(KeyCode.B))
		{
			Process(new B());
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			Process(new C());
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			Process(new X());
		}
		else if (Input.GetKeyDown(KeyCode.Y))
		{
			Process(new Y());
		}
	}

	private void Process<T>(T obj)
	{
		string serializedJson = JsonProcessor.Serialize(obj);
		Log.Info(serializedJson);
		IA deserializedJsonIA = JsonProcessor.Deserialize<IA>(serializedJson);
		Log.Info($"Deserialized JSON type from {nameof(IA)}: {deserializedJsonIA.GetType().Name}");
		A deserializedJsonA = JsonProcessor.Deserialize<A>(serializedJson);
		Log.Info($"Deserialized JSON type from {nameof(A)}: {deserializedJsonA.GetType().Name}");

		string serializedXml = XmlProcessor.Serialize(obj);
		Log.Info(serializedXml);
		IA deserializedXmlIA = XmlProcessor.Deserialize<IA>(serializedXml);
		Log.Info($"Deserialized XML type from {nameof(IA)}: {deserializedXmlIA.GetType().Name}");
		A deserializedXmlA = XmlProcessor.Deserialize<A>(serializedXml);
		Log.Info($"Deserialized XML type from {nameof(A)}: {deserializedXmlA.GetType().Name}");
	}
	
	[JsonType(typeof(A)),
	 JsonType(typeof(B)),
	 JsonType(typeof(C), Value = "Citrus", KeyOverride = "Weuk"),
	 XmlType(typeof(C), Value = "Citrus", KeyOverride = "Weuk"),
	 XmlType(typeof(A)),
	 XmlType(typeof(B))]
	public interface IA
	{
	}

	[JsonObject, XmlObject]
	public class A : IA
	{
	}

	[XmlObject]
	public class B : A
	{
	}

	[XmlObject]
	public class C : B
	{
	}

	[XmlObject,
	 JsonType(typeof(IA)),
	 XmlType(typeof(IA))]
	public class X : A
	{
	}

	[XmlObject,
	 JsonType(typeof(IA), KeyOverride = "Weuk", Value = "y tho"),
	 XmlType(typeof(IA), KeyOverride = "Weuk", Value = "y tho")]
	public class Y : B
	{
	}
}