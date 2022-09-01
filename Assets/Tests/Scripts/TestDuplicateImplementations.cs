using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Json;
using ImpossibleOdds.ReflectionCaching;

public class TestDuplicateImplementations : MonoBehaviour
{
	private void Start()
	{
		MemberInfo[] allMembers = TypeReflectionUtilities.FindAllMembersWithAttribute(typeof(ConcreteClass), typeof(MarkerAttribute), false).ToArray();
		MemberInfo[] filteredMembers = TypeReflectionUtilities.FilterBaseMethods(allMembers).ToArray();

		JsonProcessor.Deserialize<ConcreteClass>(JsonProcessor.Serialize(new ConcreteClass()));
		int i = 0;
	}

	private abstract class AbstractClass
	{
		[Marker, JsonField]
		public abstract int IntValue
		{
			get;
		}

		[Marker, JsonField]
		public virtual float FloatValue
		{
			set { }
		}

		[Marker, JsonField]
		public abstract string StringValue
		{
			get;
			set;
		}

		[Marker, JsonField]
		public bool BoolValue
		{
			get;
			set;
		}

		[Marker]
		public void UselessFunction()
		{ }

		[Marker]
		public virtual void VirtualFunction()
		{ }

		[Marker]
		public abstract void AbstractFunction();
	}

	[JsonObject]
	private class ConcreteClass : AbstractClass
	{
		// [Marker, JsonField]
		public override int IntValue
		{
			get;
		}

		// [Marker, JsonField]
		public override float FloatValue
		{
			set { }
		}

		[Marker, JsonField]
		public override string StringValue
		{
			get;
			set;
		}

		[Marker]
		public new void UselessFunction()
		{ }

		[Marker]
		public override void VirtualFunction()
		{ }

		[Marker]
		public override void AbstractFunction()
		{ }
	}

	[AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	private sealed class MarkerAttribute : Attribute
	{ }
}
