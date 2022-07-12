using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Json;
using ImpossibleOdds.Serialization.Caching;

public class TestSerializationCallbacks : MonoBehaviour
{
	private void Start()
	{
		// while (true)
		// {
		// 	yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
		// 	JsonProcessor.Serialize(new JsonClassD());
		// }

		Log.Info(JsonProcessor.Serialize(new JsonClassC()));
		Log.Info("------");
		Log.Info(JsonProcessor.Serialize(new JsonClassD()));

		// MethodInfo baseMethod = typeof(JsonClassA).GetMethod(nameof(JsonClassA.OnSerializationCallback));
		// MethodInfo overrideMethod = typeof(JsonClassB).GetMethod(nameof(JsonClassB.OnSerializationCallback));
		// MethodInfo secondDegreeOverrideMethod = typeof(JsonClassD).GetMethod(nameof(JsonClassD.OnSerializationCallback));
		// MethodInfo newMethod = typeof(JsonClassC).GetMethod(nameof(JsonClassC.OnSerializationCallback));
		// MethodInfo interfaceMethod = typeof(IJsonClass<int>).GetMethod(nameof(IJsonClass<int>.OnSerializationCallback));
		// InterfaceMapping iMappingA = typeof(JsonClassA).GetInterfaceMap(typeof(IJsonClass<int>));
		// InterfaceMapping iMappingB = typeof(JsonClassB).GetInterfaceMap(typeof(IJsonClass<int>));

		// Log.Info("Can the override method's base definition be matched? {0}", baseMethod == overrideMethod.GetBaseDefinition());
		// Log.Info("Can the second degree override method's base definition be matched directly to the base? {0}", baseMethod == secondDegreeOverrideMethod.GetBaseDefinition());
		// Log.Info("Can the second degree override method's base definition be matched to its first-order base? {0}", overrideMethod == secondDegreeOverrideMethod.GetBaseDefinition());
		// Log.Info("Can the base method be found in the interface mapping? {0}", iMappingA.TargetMethods.Contains(baseMethod));
		// Log.Info("Can the override method be found in the interface mapping? {0}", iMappingB.TargetMethods.Contains(overrideMethod));
		// Log.Info("Is the base method virtual? {0}", baseMethod.IsVirtual);
		// Log.Info("Is the override method virtual? {0}", overrideMethod.IsVirtual);
		// Log.Info("Is the new method virtual? {0}", newMethod.IsVirtual);

		// PropertyInfo interfacePropertyInfo = typeof(IJsonClass).GetProperty(nameof(IJsonClass.SomeValue));
		// PropertyInfo implementedPropertyInfo = typeof(JsonClassA).GetProperty(nameof(JsonClassA.SomeValue));
	}

	private interface IJsonClass<T>
	{
		[JsonField("SomeValue")]
		T SomeValue
		{
			get; set;
		}

		[OnJsonSerializing]
		void OnSerializationCallback();
	}

	[JsonObject]
	private class JsonClassA : IJsonClass<int>
	{
		[JsonField("SomeValue")]
		private int someValue = 0;

		public virtual int SomeValue
		{
			get
			{
				Log.Info("Getting some value from {0}", typeof(JsonClassA).Name);
				return someValue;
			}
			set
			{
				Log.Info("Setting some value on {0}", typeof(JsonClassA).Name);
				someValue = value;
			}
		}

		[OnJsonSerializing]
		public virtual void OnSerializationCallback()
		{
			Log.Info("Serializing object of type {0}.", typeof(JsonClassA).Name);
		}
	}

	private class JsonClassB : JsonClassA
	{
		[JsonField("SomeValue")]
		public override int SomeValue
		{
			get;
			set;
		} = 10;

		[OnJsonSerializing]
		public override void OnSerializationCallback()
		{
			// base.OnSerializationCallback();
			Log.Info("Serializing object of type {0}.", typeof(JsonClassB).Name);
		}
	}

	private class JsonClassC : JsonClassB
	{
		public override int SomeValue
		{
			get;
			set;
		} = 201;

		[OnJsonSerializing]
		public new void OnSerializationCallback()
		{
			// base.OnSerializationCallback();
			Log.Info("Serializing object of type {0}.", typeof(JsonClassC).Name);
		}
	}

	private class JsonClassD : JsonClassB
	{
		[JsonField("SomeValue")]
		public new int SomeValue
		{
			get;
			set;
		} = 101;

		[OnJsonSerializing]
		public override void OnSerializationCallback()
		{
			// base.OnSerializationCallback();
			Log.Info("Serializing object of type {0}.", typeof(JsonClassD).Name);
		}
	}
}
