using System;
using System.Collections;
using System.Threading.Tasks;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor to process custom object to dictionary-like data structures.
	/// </summary>
	public class CustomObjectLookupProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
	{
		public bool SupportsTypeResolution => TypeResolutionFeature != null;

		public bool SupportsRequiredValues => RequiredValueFeature != null;

		public bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true };

		public IRequiredValueFeature RequiredValueFeature { get; set; }

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		public ILookupTypeResolutionFeature TypeResolutionFeature { get; set; }

		public ILookupSerializationConfiguration Configuration { get; }


		/// <summary>
		/// Are objects being processed required to be marked with a processing attribute?
		/// </summary>
		public bool RequiresMarking { get; }

		public CustomObjectLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration, bool requiresObjectMarking = true)
		: base(definition)
		{
			definition.ThrowIfNull(nameof(definition));
			configuration.ThrowIfNull(nameof(configuration));

			RequiresMarking = requiresObjectMarking;
			Configuration = configuration;
		}

		/// <inheritdoc />
		public override object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			
			if (objectToSerialize == null)
			{
				return null;
			}
			
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(objectToSerialize.GetType()).GetUniqueSerializableMembers(Configuration.MemberAttribute);
			IDictionary targetLookup = Configuration.CreateLookupInstance(sourceMembers.Length + 1); // Include capacity for type information.

			InvokeOnSerializationCallback(objectToSerialize, targetLookup);
			Serialize(objectToSerialize.GetType(), targetLookup);
			InvokeOnSerializedCallback(objectToSerialize, targetLookup);
			return targetLookup;
		}

		/// <inheritdoc />
		public override object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

			Type instanceType =
				SupportsTypeResolution ?
					TypeResolutionFeature.FindTypeInSourceData(targetType, (IDictionary)dataToDeserialize, Definition) :
					targetType;
			object targetInstance = SerializationUtilities.CreateInstance(instanceType);
			Deserialize(targetInstance, dataToDeserialize);
			return targetInstance;
		}

		/// <inheritdoc />
		public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			this.ThrowIfCantDeserializeToTarget(deserializationTarget, dataToDeserialize);

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return;
			}

			InvokeOnDeserializationCallback(deserializationTarget, dataToDeserialize);
			Deserialize(deserializationTarget, (IDictionary)dataToDeserialize);
			InvokeOnDeserializedCallback(deserializationTarget, dataToDeserialize);
		}

		/// <inheritdoc />
		public override bool CanSerialize(object objectToSerialize)
		{
			// Either the object is null - which is accepted,
			// or the object does not require class marking,
			// or it requires class marking, and it is class marked.
			return
				(objectToSerialize == null) ||
				!RequiresMarking ||
				Attribute.IsDefined(objectToSerialize.GetType(), Configuration.TypeMarkingAttribute);
		}

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			switch (dataToDeserialize)
			{
				case null:
					return SerializationUtilities.IsNullableType(targetType);
				case IDictionary dictionary:
				{
					Type instanceType =
						SupportsTypeResolution ?
							TypeResolutionFeature.FindTypeInSourceData(targetType, dictionary, Definition) :
							targetType;

					return !RequiresMarking || Attribute.IsDefined(instanceType, Configuration.TypeMarkingAttribute);
				}
				default:
					return false;
			}
		}
		
		/// <inheritdoc />
		public bool CanDeserialize(object deserializationTarget, object dataToDeserialize)
		{
			return deserializationTarget != null && CanDeserialize(deserializationTarget.GetType(), dataToDeserialize);
		}

		private void Serialize(object source, IDictionary targetLookup)
		{
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(source.GetType()).GetUniqueSerializableMembers(Configuration.MemberAttribute);
			DictionaryCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetLookup);

			// Process the source key and value pairs.
			if (ParallelProcessingEnabled && (sourceMembers.Length > 1))
			{
				Parallel.ForEach(sourceMembers, sourceMember =>
				{
					object processedKey = Serializer.Serialize(Configuration.GetLookupKey(sourceMember), Definition);
					object processedValue = Serializer.Serialize(sourceMember.GetValue(source), Definition);
					lock (targetLookup)
					{
						if (!targetLookup.Contains(processedKey))
						{
							SerializationUtilities.InsertInLookup(targetLookup, collectionInfo, processedKey, processedValue);
						}
					}
				});
			}
			else
			{
				Array.ForEach(sourceMembers, sourceMember =>
				{
					object processedKey = Serializer.Serialize(Configuration.GetLookupKey(sourceMember), Definition);
					object processedValue = Serializer.Serialize(sourceMember.GetValue(source), Definition);
					if (!targetLookup.Contains(processedKey))
					{
						SerializationUtilities.InsertInLookup(targetLookup, collectionInfo, processedKey, processedValue);
					}
				});
			}

			// Include type information, if available.
			if (SupportsTypeResolution)
			{
				TypeResolutionFeature.InsertTypeInData(source.GetType(), targetLookup, Definition);
			}
		}

		private void Deserialize(object target, IDictionary source)
		{
			// Get all the fields that would like to get their value filled in.
			Type targetType = target.GetType();
			ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(targetType);
			ISerializableMember[] targetMembers = typeMap.GetSerializableMembers(Configuration.MemberAttribute);

			if (ParallelProcessingEnabled)
			{
				Parallel.ForEach(targetMembers, targetMember =>
				{
					if (!ContainsKey(targetMember))
					{
						return;
					}

					object deserializedMember = DeserializeMember(targetMember);
					lock (target) targetMember.SetValue(target, deserializedMember);
				});
			}
			else
			{
				Array.ForEach(targetMembers, targetMember =>
				{
					if (!ContainsKey(targetMember))
					{
						return;
					}

					targetMember.SetValue(target, DeserializeMember(targetMember));
				});
			}

			return;

			bool ContainsKey(ISerializableMember targetMember)
			{
				object key = Configuration.GetLookupKey(targetMember);

				// See whether the source contains a value for this field.
				if (source.Contains(key))
				{
					return true;
				}

				// Check whether this field is marked as required.
				if (SupportsRequiredValues && RequiredValueFeature.IsMemberRequired(targetType, targetMember))
				{
					throw new SerializationException($"The member '{targetMember.Member.Name}' is marked as required on type {targetMember.Member.DeclaringType.Name} but is not present in the source.");
				}

				Log.Warning($"The source does not contain a value associated with key '{key}' for a target of type {target.GetType().Name}.");
				return false;
			}

			object DeserializeMember(ISerializableMember targetMember)
			{
				object result = Serializer.Deserialize(targetMember.MemberType, source[Configuration.GetLookupKey(targetMember)], Definition);

				if (result != null)
				{
					return result;
				}

				// If the value is not allowed to be null, then quit.
				if (SupportsRequiredValues && !RequiredValueFeature.IsValueValid(targetType, targetMember, null))
				{
					throw new SerializationException($"The member '{targetMember.Member.Name}' is marked as required on type {targetMember.Member.DeclaringType.Name} but the value is null in the source.");
				}

				Type memberType = targetMember.MemberType;
				return memberType.IsValueType ? Activator.CreateInstance(memberType, true) : null;
			}
		}
	}
}