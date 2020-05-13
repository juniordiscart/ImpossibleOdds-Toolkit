﻿namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	/// <summary>
	/// A (de)serialization processor to process custom object to dictionary-like data structures.
	/// </summary>
	public class CustomObjectLookupProcessor : AbstractCustomObjectProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private new ILookupSerializationDefinition definition;

		private bool SupportsTypeResolvement
		{
			get { return definition is ILookupBasedTypeResolve; }
		}

		public CustomObjectLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the object to a dictionary-like data structure.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if (objectToSerialize == null)
			{
				serializedResult = objectToSerialize;
				return true;
			}

			Type sourceType = objectToSerialize.GetType();
			if (!sourceType.IsDefined(definition.LookupBasedClassMarkingAttribute, true))
			{
				serializedResult = null;
				return false;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			serializedResult = Serialize(sourceType, objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the dictionary-like data to a new instance of the target type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			if (targetType == null)
			{
				deserializedResult = null;
				return false;
			}
			else if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return SerializationUtilities.IsNullableType(targetType);
			}
			else if (!(dataToDeserialize is IDictionary))
			{
				deserializedResult = null;
				return false;
			}

			object targetInstance = null;
			Type instanceType = ResolveTypeFromLookup(targetType, dataToDeserialize as IDictionary);

			try
			{
				targetInstance = CreateInstance(instanceType);
			}
			catch (System.Exception)
			{
				throw new SerializationException(string.Format("Failed to create an instance of target type {0}.", targetType.Name));
			}

			if (Deserialize(targetInstance, dataToDeserialize))
			{
				deserializedResult = targetInstance;
				return true;
			}
			else
			{
				deserializedResult = null;
				return false;
			}
		}

		/// <summary>
		/// Attempts to deserialize the list-like data to the given target.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return true;
			}
			else if (!deserializationTarget.GetType().IsDefined(definition.LookupBasedClassMarkingAttribute, true))
			{
				return false;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IDictionary);
			InvokeOnDeserializedCallback(deserializationTarget);
			return true;
		}

		private IDictionary Serialize(Type sourceType, object source)
		{
			IDictionary properties = (IDictionary)Activator.CreateInstance(definition.LookupBasedDataType, false);
			GenericTypeInfo lookupGenericInfo = new GenericTypeInfo(definition);

			ReadOnlyCollection<FieldAtrributeTuple> sourceFields = GetAttributeFields(sourceType, definition.LookupBasedFieldAttribute);
			foreach (FieldAtrributeTuple sourceField in sourceFields)
			{
				object key = GetKey(sourceField);
				object value = sourceField.field.GetValue(source);
				InsertKeyValuePairInLookup(sourceType, properties, lookupGenericInfo, key, value);
			}

			// Include type information, if available.
			ILookupTypeResolveParameter typeResolveAttr = ResolveTypeToLookup(sourceType);
			if (typeResolveAttr != null)
			{
				InsertKeyValuePairInLookup(sourceType, properties, lookupGenericInfo, typeResolveAttr.Key, typeResolveAttr.Value);
			}

			return properties;
		}

		private void InsertKeyValuePairInLookup(Type sourceType, IDictionary properties, GenericTypeInfo lookupGenericInfo, object key, object value)
		{
			// Process the key if type constraints are set.
			if (lookupGenericInfo.isKeyTypeConstrained)
			{
				key = SerializationUtilities.PostProcessRequestValue(key, lookupGenericInfo.genericParams[0]);
			}

			// Process the value if type constraints are set.
			if ((value != null) && lookupGenericInfo.isValueTypeConstrained)
			{
				value = SerializationUtilities.PostProcessRequestValue(value, lookupGenericInfo.genericParams[1]);
			}

			if (properties.Contains(key))
			{
				properties[key] = value;
				Debug.Warning("A key with value '{0}' has been defined more than once for source object of type {1}.", key.ToString(), sourceType.Name);
			}
			else
			{
				properties.Add(key, value);
			}
		}

		private void Deserialize(object target, IDictionary source)
		{
			// Get all of the fields that would like to get their value filled in
			ReadOnlyCollection<FieldAtrributeTuple> targetFields = GetAttributeFields(target.GetType(), definition.LookupBasedFieldAttribute);

			foreach (FieldAtrributeTuple targetField in targetFields)
			{
				object key = GetKey(targetField);

				// See whether the source contains a value for this field
				if (!source.Contains(key))
				{
					// Check whether this field is marked as required
					if (targetField.field.IsDefined(typeof(RequiredAttribute), false))
					{
						throw new SerializationException(string.Format("The field {0} is marked as required but is not present in the source.", targetField.field.Name));
					}
					else
					{
						Debug.Warning("The source does not contain a value associated with key '{0}' for a target of type {1}.", key, target.GetType().Name);
						continue;
					}
				}

				object result = Serializer.Deserialize(targetField.field.FieldType, source[key], definition);

				if (result == null)
				{
					Type fieldType = targetField.field.FieldType;
					targetField.field.SetValue(target, fieldType.IsValueType ? Activator.CreateInstance(fieldType, true) : null);
				}
				else
				{
					targetField.field.SetValue(target, result);
				}
			}
		}

		private object GetKey(FieldAtrributeTuple field)
		{
			ILookupParameter lookupAttribute = field.attribute as ILookupParameter;
			return (lookupAttribute.Key != null) ? lookupAttribute.Key : field.field.Name;
		}

		private ILookupTypeResolveParameter ResolveTypeToLookup(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			ILookupBasedTypeResolve typeResolveImplementation = definition as ILookupBasedTypeResolve;
			IEnumerable<ISerializationTypeResolveParameter> typeResolveAttributes = GetClassTypeResolves(sourceType, typeResolveImplementation.TypeResolveAttribute);
			foreach (ISerializationTypeResolveParameter attr in typeResolveAttributes)
			{
				ILookupTypeResolveParameter typeResolveAttr = attr as ILookupTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new SerializationException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(ILookupTypeResolveParameter).Name));
				}
				else if (typeResolveAttr.Target == sourceType)
				{
					return typeResolveAttr;
				}
			}

			return null;
		}

		private Type ResolveTypeFromLookup(Type targetType, IDictionary source)
		{
			if (!SupportsTypeResolvement)
			{
				if (targetType.IsAbstract || targetType.IsInterface)
				{
					throw new SerializationException(string.Format("The target type {0} is abstract or an interface, but no type resolve ({1}) is implemented in serialization definition of type {2}.", targetType.Name, typeof(ILookupBasedTypeResolve).Name, definition.GetType().Name));
				}

				return targetType;
			}

			ILookupBasedTypeResolve typeResolveImplementation = definition as ILookupBasedTypeResolve;
			IEnumerable<ISerializationTypeResolveParameter> typeResolveAttrs = GetClassTypeResolves(targetType, typeResolveImplementation.TypeResolveAttribute);

			foreach (ISerializationTypeResolveParameter attr in typeResolveAttrs)
			{
				ILookupTypeResolveParameter typeResolveAttr = attr as ILookupTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new SerializationException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(ILookupTypeResolveParameter).Name));
				}
				else if (source.Contains(typeResolveAttr.Key) && (source[typeResolveAttr.Key].Equals(typeResolveAttr.Value)))
				{
					if (targetType.IsAssignableFrom(typeResolveAttr.Target))
					{
						return typeResolveAttr.Target;
					}
					else
					{
						throw new SerializationException(string.Format("The attribute of type {0}, defined on type {1} or its super types, is matched but cannot be assigned from instance of type {2}.", typeResolveAttr.GetType().Name, targetType.Name, typeResolveAttr.Target.Name));
					}
				}
			}

			return targetType;
		}

		private struct GenericTypeInfo
		{
			public readonly Type genericType;
			public readonly Type[] genericParams;
			public readonly bool isKeyTypeConstrained;
			public readonly bool isValueTypeConstrained;

			public GenericTypeInfo(ILookupSerializationDefinition definition)
			{
				genericType = SerializationUtilities.GetGenericType(definition.LookupBasedDataType, typeof(IDictionary<,>));
				genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
				isKeyTypeConstrained = (genericParams != null) && (genericParams[0] != typeof(object));
				isValueTypeConstrained = (genericParams != null) && (genericParams[1] != typeof(object));
			}
		}
	}
}