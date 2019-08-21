﻿namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Request that values of the enum prefer to be serialized to a string value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
	public sealed class EnumStringMappingAttribute : Attribute { }
}