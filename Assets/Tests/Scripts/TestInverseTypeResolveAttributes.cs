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
		A deserializedJson = JsonProcessor.Deserialize<A>(serializedJson);
		Log.Info($"Deserialized JSON type: {deserializedJson.GetType().Name}");

		string serializedXml = XmlProcessor.Serialize(obj);
		Log.Info(serializedXml);
		A deserializedXml = XmlProcessor.Deserialize<A>(serializedXml);
		Log.Info($"Deserialized XML type: {deserializedXml.GetType().Name}");
	}


	[JsonType(typeof(A)),
	 JsonType(typeof(B)),
	 JsonType(typeof(C), KeyOverride = "Weuk"),
	 XmlType(typeof(A)),
	 XmlType(typeof(B)),
	 XmlType(typeof(C), Value = "Citrus", KeyOverride = "Weuk")]
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
	 JsonType(typeof(A)),
	 XmlType(typeof(A))]
	public class X : A
	{
	}

	[XmlObject,
	 JsonType(typeof(A)),
	 XmlType(typeof(A), KeyOverride = "Weuk", Value = "y tho")]
	public class Y : B
	{
	}
}