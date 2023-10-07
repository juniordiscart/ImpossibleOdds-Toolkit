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
			if (objectToSerialize == null)
			{
				return null;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			object serializedResult = Serialize(objectToSerialize.GetType(), objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return serializedResult;
		}

		/// <inheritdoc />
		public override object Deserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

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
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, (IDictionary)dataToDeserialize);
			InvokeOnDeserializedCallback(deserializationTarget);
		}

		/// <inheritdoc />
		public override bool CanSerialize(object objectToSerialize)
		{
			// Either the object is null - which is accepted,
			// or the object does not require class marking,
			// or it requires class marking and it is class marked.
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

		private IDictionary Serialize(Type sourceType, object source)
		{
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(sourceType).GetUniqueSerializableMembers(Configuration.MemberAttribute);
			IDictionary processedValues = Configuration.CreateLookupInstance(sourceMembers.Length + 1); // Include capacity for type information.
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			// Process the source key and value pairs.
			if (ParallelProcessingEnabled && (sourceMembers.Length > 1))
			{
				object parallelLock = new object();
				Parallel.ForEach(sourceMembers, sourceMember =>
				{
					object processedKey = Serializer.Serialize(Configuration.GetLookupKey(sourceMember), Definition);
					object processedValue = Serializer.Serialize(sourceMember.GetValue(source), Definition);
					lock (parallelLock)
					{
						if (!processedValues.Contains(processedKey))
						{
							SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
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
					if (!processedValues.Contains(processedKey))
					{
						SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
					}
				});
			}

			// Include type information, if available.
			if (SupportsTypeResolution)
			{
				TypeResolutionFeature.InsertTypeInData(sourceType, processedValues, Definition);
			}

			return processedValues;
		}

		private void Deserialize(object target, IDictionary source)
		{
			// Get all of the fields that would like to get their value filled in.
			Type targetType = target.GetType();
			ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(targetType);
			ISerializableMember[] targetMembers = typeMap.GetSerializableMembers(Configuration.MemberAttribute);

			if (ParallelProcessingEnabled)
			{
				object parallelLock = new object();
				Parallel.ForEach(targetMembers, targetMember =>
				{
					if (!ContainsKey(targetMember))
					{
						return;
					}

					lock (parallelLock) targetMember.SetValue(target, DeserializeMember(targetMember));
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

				Log.Warning("The source does not contain a value associated with key '{0}' for a target of type {1}.", key, target.GetType().Name);
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
				if (SupportsRequiredValues && !RequiredValueFeature.IsValueValid(targetType, targetMember, result))
				{
					throw new SerializationException($"The member '{targetMember.Member.Name}' is marked as required on type {targetMember.Member.DeclaringType.Name} but the value is null in the source.");
				}

				Type memberType = targetMember.MemberType;
				return memberType.IsValueType ? Activator.CreateInstance(memberType, true) : null;
			}
		}
	}
}