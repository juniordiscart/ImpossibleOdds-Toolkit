﻿namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public struct FieldAttributeTuple : IMemberAttributeTuple<FieldInfo>
	{
		private readonly FieldInfo field;
		private readonly Attribute attribute;

		/// <summary>
		/// The registered field.
		/// </summary>
		public FieldInfo Field
		{
			get { return field; }
		}

		/// <inheritdoc />
		public Type MemberType
		{
			get { return field.FieldType; }
		}

		/// <inheritdoc />
		public FieldInfo Member
		{
			get { return field; }
		}

		/// <inheritdoc />
		public Attribute Attribute
		{
			get { return attribute; }
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributeTuple.Member
		{
			get { return Member; }
		}

		public FieldAttributeTuple(FieldInfo field, Attribute attribute)
		{
			field.ThrowIfNull(nameof(field));
			attribute.ThrowIfNull(nameof(attribute));

			this.field = field;
			this.attribute = attribute;
		}

		/// <inheritdoc />
		public object GetValue(object source)
		{
			return field.GetValue(source);
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			field.SetValue(source, value);
		}
	}
}
