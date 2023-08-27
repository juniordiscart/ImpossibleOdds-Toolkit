using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Caching;
using ImpossibleOdds.Json;
using ImpossibleOdds.Xml;

public class BindingFlags : MonoBehaviour
{
	private void Start()
	{
		// SerializationTypeMap testClassMapping = SerializationUtilities.GetTypeMap(typeof(TestClass));
		// SerializationTypeMap TestEnumMapping = SerializationUtilities.GetTypeMap(typeof(TestEnum));

		Log.Info(TestEnum.ONE.DisplayName());
		Log.Info(TestEnum.TWO.DisplayName());
		Log.Info(TestEnum.THREE.DisplayName());
	}

	private void Update()
	{

	}

	[JsonObject, XmlObject]
	public class TestClass : ITestInterface
	{
		[JsonField, JsonRequired]
		private string strValue;

		[JsonField, XmlAttribute]
		public int intValue;

		public bool Enabled
		{
			get => false;
		}
	}

	[JsonEnumString, XmlEnumString]
	public enum TestEnum
	{
		[JsonEnumAlias("1"), DisplayName(Name = "1")]
		ONE,
		[XmlEnumAlias("2")]
		TWO,
		THREE,
	}

	[JsonType(typeof(TestClass), Value = "Test")]
	public interface ITestInterface
	{
		[JsonField]
		bool Enabled
		{
			get;
		}
	}
}
