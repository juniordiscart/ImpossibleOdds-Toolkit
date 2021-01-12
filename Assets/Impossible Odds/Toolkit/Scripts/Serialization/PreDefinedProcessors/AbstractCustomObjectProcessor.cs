namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;

	public abstract class AbstractCustomObjectProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		public AbstractCustomObjectProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		public abstract bool Serialize(object objectToSerialize, out object serializedResult);
		public abstract bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult);

		/// <summary>
		/// Cache of attributes, organized by attribute type.
		/// </summary>
		/// <typeparam name="Type">Type of the attribute.</typeparam>
		/// <typeparam name="AttributeFieldsCache">Cache of types with the attributes defined in them.</typeparam>
		protected static Dictionary<Type, AttributeFieldsCache> attributeFieldsCache = new Dictionary<Type, AttributeFieldsCache>();

		/// <summary>
		/// Cache of type resolve parameters defines on types.
		/// </summary>
		/// <typeparam name="Type">Attribute type that is used during type resolve.</typeparam>
		/// <typeparam name="TypeResolveCache">Cache of types that have been decorated with the type resolve attribute.</typeparam>
		protected static Dictionary<Type, TypeResolveCache> typeResolveCache = new Dictionary<Type, TypeResolveCache>();

		/// <summary>
		/// Cache of (de)serialization callbacks for a specific callback attribute type.
		/// </summary>
		/// <typeparam name="Type">Attribute type that defines a (de)serialization callback.</typeparam>
		/// <typeparam name="CallbackCache">Cache of types and methods that have the (de)serialization callback attribute defined.</typeparam>
		protected static Dictionary<Type, CallbackCache> callbackCache = new Dictionary<Type, CallbackCache>();

		/// <summary>
		/// Checks whether the serialization definition has callback attributes defined to let the object know that (de)serialization will take place.
		/// </summary>
		protected bool SupportsSerializationCallbacks
		{
			get { return definition is ISerializationCallbacks; }
		}

		/// <summary>
		/// Let the target object know that serialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializationCallback(object target)
		{
			if (definition is ISerializationCallbacks callbacks)
			{
				InvokeCallback(target, callbacks.OnSerializationCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that serialization has ended.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializedCallback(object target)
		{
			if (definition is ISerializationCallbacks callbacks)
			{
				InvokeCallback(target, callbacks.OnSerializedCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that deserialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnDeserializationCallback(object target)
		{
			if (definition is ISerializationCallbacks callbacks)
			{
				InvokeCallback(target, callbacks.OnDeserializionCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that deserialization has ended.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnDeserializedCallback(object target)
		{
			if (definition is ISerializationCallbacks callbacks)
			{
				InvokeCallback(target, callbacks.OnDeserializedCallbackType);
			}
		}

		private void InvokeCallback(object target, Type callbackAttributeType)
		{
			target.ThrowIfNull(nameof(target));

			if (callbackAttributeType == null)
			{
				return;
			}

			MethodInfo callback = GetCallback(target.GetType(), callbackAttributeType);
			if (callback == null)
			{
				return;
			}

			callback.Invoke(target, null);
		}

		/// <summary>
		/// Fetch all attributes defined on the target type.
		/// </summary>
		/// <param name="targetType">The class type of which to fetch the attributes that are defined on its fields.</param>
		/// <param name="attributeType">The attribute type to look for that is defined on the target type's fields.</param>
		/// <returns></returns>
		protected static IReadOnlyList<FieldAtrributeTuple> GetAttributeFields(Type targetType, Type attributeType)
		{
			if (!attributeFieldsCache.ContainsKey(attributeType))
			{
				attributeFieldsCache.Add(attributeType, new AttributeFieldsCache());
			}

			AttributeFieldsCache attributesCache = attributeFieldsCache[attributeType];
			if (attributesCache.ContainsKey(targetType))
			{
				return attributesCache[targetType];
			}

			// Collection in which we will store the cached fields.
			List<FieldAtrributeTuple> targetFields = new List<FieldAtrributeTuple>();
			attributesCache.Add(targetType, targetFields);

			// Fetch all fields across the type hierarchy.
			while ((targetType != null) && (targetType != typeof(object)))
			{
				FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo field in fields)
				{
					if (field.DeclaringType != targetType)
					{
						continue;
					}

					Attribute attr = field.GetCustomAttributes(attributeType, false).SingleOrDefault() as Attribute;
					if (attr != null)
					{
						targetFields.Add(new FieldAtrributeTuple(field, attr));
					}
				}

				targetType = targetType.BaseType;
			}

			return targetFields.AsReadOnly();
		}

		/// <summary>
		/// Finds the maximum defined index in the hierarchy.
		/// </summary>
		/// <param name="type">Type of object for which to find the maximum defined index.</param>
		/// <param name="attributeType">Type of the index-based parameter attribute.</param>
		/// <returns>Returns the maximum defined index found. If no index-based parameter attributes were found, -1 is returned.</returns>
		protected static int GetMaxDefinedIndex(Type type, Type attributeType)
		{
			int maxIndex = -1;
			while ((type != null) && (type != typeof(object)))
			{
				IReadOnlyList<FieldAtrributeTuple> fields = GetAttributeFields(type, attributeType);
				foreach (FieldAtrributeTuple field in fields)
				{
					maxIndex = Math.Max((field.attribute as IIndexParameter).Index, maxIndex);
				}

				type = type.BaseType;
			}

			return maxIndex;
		}

		/// <summary>
		/// Fetch all attributes defined on the target type's class and across its inheritance chain which are applicable to this type.
		/// </summary>
		/// <param name="targetType">The class type of which to fetch the attributes that are defined on it.</param>
		/// <param name="attributeType">The type of the attribute to look for.</param>
		/// <returns>Collection of type resolve attributes that are applicable to the target type.</returns>
		protected static IReadOnlyList<ISerializationTypeResolveParameter> GetClassTypeResolves(Type targetType, Type attributeType)
		{
			targetType.ThrowIfNull(nameof(targetType));
			attributeType.ThrowIfNull(nameof(attributeType));

			if (!typeof(ISerializationTypeResolveParameter).IsAssignableFrom(attributeType))
			{
				throw new Serialization.SerializationException(string.Format("The requested attribute type to look for does not implement the {0} interface.", typeof(ISerializationTypeResolveParameter).Name));
			}

			if (!typeResolveCache.ContainsKey(attributeType))
			{
				typeResolveCache.Add(attributeType, new TypeResolveCache());
			}

			TypeResolveCache cache = typeResolveCache[attributeType];
			if (cache.ContainsKey(targetType))
			{
				return cache[targetType].AsReadOnly();
			}

			// Fetch all attributes defined in the inheritance chain.
			List<ISerializationTypeResolveParameter> typeResolveAttributes = new List<ISerializationTypeResolveParameter>();

			// Attributes defined on the class itself
			typeResolveAttributes.AddRange(targetType.GetCustomAttributes(attributeType, true).Cast<ISerializationTypeResolveParameter>());

			// Attributes defined in interfaces implemented by the class or one of it's base classes.
			foreach (Type interfaceType in targetType.GetInterfaces())
			{
				typeResolveAttributes.AddRange(interfaceType.GetCustomAttributes(attributeType, true).Cast<ISerializationTypeResolveParameter>());
			}

			// Filter duplicates and types we don't care about
			typeResolveAttributes = typeResolveAttributes.Where(t => targetType.IsAssignableFrom(t.Target)).Distinct().ToList();

			cache.Add(targetType, typeResolveAttributes);
			return typeResolveAttributes.AsReadOnly();
		}

		/// <summary>
		/// Retrieve a callback for an instance of target type.
		/// </summary>
		/// <param name="targetType">The type of object on which the callback is defined.</param>
		/// <param name="callbackAttributeType">The type of the attribute that should've been defined on the callback method.</param>
		/// <returns>A callback method, if any was defined. Returns null otherwise.</returns>
		protected static MethodInfo GetCallback(Type targetType, Type callbackAttributeType)
		{
			targetType.ThrowIfNull(nameof(targetType));
			callbackAttributeType.ThrowIfNull(nameof(callbackAttributeType));

			if (!callbackCache.ContainsKey(callbackAttributeType))
			{
				callbackCache.Add(callbackAttributeType, new CallbackCache());
			}

			CallbackCache cache = callbackCache[callbackAttributeType];
			if (cache.ContainsKey(targetType))
			{
				return cache[targetType];
			}

			// Fetch all methods on the target type
			Type currentType = targetType;
			while ((currentType != null) && (currentType != typeof(object)))
			{
				IEnumerable<MethodInfo> methods = currentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				foreach (MethodInfo method in methods)
				{
					if (method.IsDefined(callbackAttributeType))
					{
						cache.Add(targetType, method);
						return method;
					}
				}

				currentType = currentType.BaseType;
			}

			return null;
		}

		/// <summary>
		/// Creates a new instance of the requested type.
		/// </summary>
		/// <returns>Instance of the requested type.</returns>
		protected static object CreateInstance(Type instanceType)
		{
			if (instanceType.IsValueType)
			{
				return Activator.CreateInstance(instanceType, true);
			}
			else
			{
				return FormatterServices.GetUninitializedObject(instanceType);
			}
		}

		/// <summary>
		/// Binds an attribute to a field for quick access.
		/// </summary>
		protected class FieldAtrributeTuple
		{
			public readonly FieldInfo field;
			public readonly Attribute attribute;

			public FieldAtrributeTuple(FieldInfo field, Attribute attribute)
			{
				this.field = field;
				this.attribute = attribute;
			}
		}

		/// <summary>
		/// Thin wrapper of a dictionary. The keys are class types. The corresponding values are
		/// a list of all fields that have a certain attribute defined, stored in a tuple containing
		/// the field and the attribute associated with it.
		/// </summary>
		protected class AttributeFieldsCache : Dictionary<Type, List<FieldAtrributeTuple>> { }

		/// <summary>
		/// Thin wrapper of a dictionary. The keys are class/interface types. The corresponding values
		/// are a list of type resolve attributes defined of a certain type.
		/// </summary>
		protected class TypeResolveCache : Dictionary<Type, List<ISerializationTypeResolveParameter>> { }

		/// <summary>
		/// Thin wrapper of a dictionary. The keys are class/interface types. The corresponding values
		/// are the methods that define the callback to be used on the object of that type.
		/// </summary>
		protected class CallbackCache : Dictionary<Type, MethodInfo> { }
	}
}
