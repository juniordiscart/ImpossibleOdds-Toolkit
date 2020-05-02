﻿namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	/// <summary>
	/// A (de)serialization processor to process custom object to list-like data structures.
	/// </summary>
	public class CustomObjectSequenceProcessor : AbstractCustomObjectProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private new IIndexSerializationDefinition definition;

		private bool SupportsTypeResolvement
		{
			get { return definition is IIndexBasedTypeResolve; }
		}

		public CustomObjectSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the object to a list-like data structure.
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
			if (!sourceType.IsDefined(definition.IndexBasedClassMarkingAttribute, true))
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
		/// Attempts to deserialize the list-like data to a new instance of the target type.
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
			else if (!(dataToDeserialize is IList))
			{
				deserializedResult = null;
				return false;
			}

			object targetInstance = null;
			Type instanceType = ResolveTypeFromSequence(targetType, dataToDeserialize as IList);

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
			else if (!deserializationTarget.GetType().IsDefined(definition.IndexBasedClassMarkingAttribute, true))
			{
				return false;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IList);
			InvokeOnDeserializedCallback(deserializationTarget);
			return true;
		}

		private IList Serialize(Type sourceType, object source)
		{
			// Create the collection in which we will store the values.
			IList values;
			int nrOfElements = GetMaxDefinedIndex(source.GetType(), definition.IndexBasedFieldAttribute);
			nrOfElements += (nrOfElements >= 0) ? 1 : 0;

			// Check whether we want to include type information
			IIndexTypeResolveParameter typeResolveAttr = ResolveTypeToSequence(sourceType);
			if (typeResolveAttr != null)
			{
				nrOfElements = Math.Max(typeResolveAttr.Index + 1, nrOfElements);
			}

			// Create the instance of the index-based type. Arrays are a special case.
			GenericTypeInfo genericInfo = new GenericTypeInfo(definition);
			if (definition.IndexBasedDataType.IsArray)
			{
				values = Array.CreateInstance(definition.IndexBasedDataType.GetElementType(), nrOfElements);
			}
			else
			{
				values = Array.CreateInstance(genericInfo.isTypeConstrained ? genericInfo.genericParam : typeof(object), nrOfElements);
				values = Activator.CreateInstance(definition.IndexBasedDataType, new object[] { values }) as IList;
			}

			// Process the marked fields.
			ReadOnlyCollection<FieldAtrributeTuple> sourceFields = GetAttributeFields(sourceType, definition.IndexBasedFieldAttribute);
			foreach (FieldAtrributeTuple sourceField in sourceFields)
			{
				IIndexParameter indexAttribute = sourceField.attribute as IIndexParameter;
				object value = Serializer.Serialize(sourceField.field.GetValue(source), definition);
				InsertValueInSequence(sourceType, values, genericInfo, indexAttribute.Index, value);
			}

			// Include the type information, if any was found.
			if (typeResolveAttr != null)
			{
				InsertValueInSequence(sourceType, values, genericInfo, typeResolveAttr.Index, typeResolveAttr.Value);
			}

			return values;
		}

		private void InsertValueInSequence(Type sourceType, IList values, GenericTypeInfo sequenceGenericInfo, int index, object value)
		{
			if ((value != null) && sequenceGenericInfo.isTypeConstrained)
			{
				value = SerializationUtilities.PostProcessRequestValue(value, sequenceGenericInfo.genericParam);
			}

			if (values[index] != null)
			{
				Debug.Warning("Index {0} for processing an instance of type {1} is used multiple times.", index, sourceType.Name);
			}

			values[index] = value;
		}

		private void Deserialize(object target, IList source)
		{
			// Get all of the fields that would like to get their value filled in
			ReadOnlyCollection<FieldAtrributeTuple> targetFields = GetAttributeFields(target.GetType(), definition.IndexBasedFieldAttribute);

			foreach (FieldAtrributeTuple targetField in targetFields)
			{
				IIndexParameter indexParam = targetField.attribute as IIndexParameter;

				// Check whether the source has such an index.
				if (source.Count <= indexParam.Index)
				{
					Debug.Warning("The source does not contain a value at index '{0}' for a target of type {1}.", indexParam.Index, target.GetType().Name);
					continue;
				}

				object result = Serializer.Deserialize(targetField.field.FieldType, source[indexParam.Index], definition);

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

		private IIndexTypeResolveParameter ResolveTypeToSequence(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			IIndexBasedTypeResolve typeResolveImplementation = definition as IIndexBasedTypeResolve;
			IEnumerable<ISerializationTypeResolveParameter> typeResolveAttributes = GetClassTypeResolves(sourceType, typeResolveImplementation.TypeResolveAttribute);
			foreach (ISerializationTypeResolveParameter attr in typeResolveAttributes)
			{
				IIndexTypeResolveParameter typeResolveAttr = attr as IIndexTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new SerializationException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(IIndexTypeResolveParameter).Name));
				}
				else if (typeResolveAttr.Target == sourceType)
				{
					return typeResolveAttr;
				}
			}

			return null;
		}

		private Type ResolveTypeFromSequence(Type targetType, IList source)
		{
			if (!SupportsTypeResolvement)
			{
				if (targetType.IsAbstract || targetType.IsInterface)
				{
					throw new SerializationException(string.Format("The target type {0} is abstract or an interface, but no type resolve ({1}) is implemented in serialization definition of type {2}.", targetType.Name, typeof(IIndexBasedTypeResolve).Name, definition.GetType().Name));
				}

				return targetType;
			}

			IIndexBasedTypeResolve typeResolveImplementation = definition as IIndexBasedTypeResolve;
			IEnumerable<ISerializationTypeResolveParameter> typeResolveAttrs = GetClassTypeResolves(targetType, typeResolveImplementation.TypeResolveAttribute);

			foreach (ISerializationTypeResolveParameter attr in typeResolveAttrs)
			{
				IIndexTypeResolveParameter typeResolveAttr = attr as IIndexTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new SerializationException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(IIndexTypeResolveParameter).Name));
				}
				else if ((source.Count > typeResolveAttr.Index) && (source[typeResolveAttr.Index].Equals(typeResolveAttr.Value)))
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
			public readonly Type genericParam;
			public readonly bool isTypeConstrained;

			public GenericTypeInfo(IIndexSerializationDefinition definition)
			{
				genericType = SerializationUtilities.GetGenericType(definition.IndexBasedDataType, typeof(IList<>));
				genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));
			}
		}
	}
}