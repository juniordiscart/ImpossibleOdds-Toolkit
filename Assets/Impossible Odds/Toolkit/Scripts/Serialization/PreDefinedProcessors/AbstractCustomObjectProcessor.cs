namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public abstract class AbstractCustomObjectProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private static Dictionary<Type, CustomObjectTypeCache> typeInfoCache = new Dictionary<Type, CustomObjectTypeCache>();

		private ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		public bool SupportsSerializationCallbacks
		{
			get { return definition is ICallbacksSupport; }
		}

		public AbstractCustomObjectProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		public abstract bool Serialize(object objectToSerialize, out object serializedResult);
		public abstract bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult);

		/// <summary>
		/// Let the target object know that serialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializationCallback(object target)
		{
			if (definition is ICallbacksSupport callbacks)
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
			if (definition is ICallbacksSupport callbacks)
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
			if (definition is ICallbacksSupport callbacks)
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
			if (definition is ICallbacksSupport callbacks)
			{
				InvokeCallback(target, callbacks.OnDeserializedCallbackType);
			}
		}

		/// <summary>
		/// Retrieve the cached information about a type.
		/// </summary>
		/// <param name="type">The type for which to retrieve the cached information.</param>
		/// <returns>The type cache associated with the given type.</returns>
		protected CustomObjectTypeCache GetTypeCache(Type type)
		{
			type.ThrowIfNull(nameof(type));

			if (!typeInfoCache.ContainsKey(type))
			{
				typeInfoCache[type] = new CustomObjectTypeCache(type);
			}

			return typeInfoCache[type];
		}

		private void InvokeCallback(object target, Type callbackAttributeType)
		{
			target.ThrowIfNull(nameof(target));

			if (callbackAttributeType == null)
			{
				return;
			}

			IEnumerable<MethodInfo> callbacks = GetTypeCache(target.GetType()).GetSerializationCallbacks(callbackAttributeType);
			foreach (MethodInfo callback in callbacks)
			{
				if (callback != null)
				{
					callback.Invoke(target, null);
				}
			}
		}

		/// <summary>
		/// Binds an attribute to a field for quick access.
		/// </summary>
		protected struct FieldAtrributeTuple
		{
			public FieldInfo Field
			{
				get; private set;
			}

			public Attribute Attribute
			{
				get; private set;
			}

			public FieldAtrributeTuple(FieldInfo field, Attribute attribute)
			{
				Field = field;
				Attribute = attribute;
			}
		}

		/// <summary>
		/// Lazy type cache of attributes for commonly used data in processing custom objects.
		/// </summary>
		protected class CustomObjectTypeCache
		{
			private readonly Type type = null;
			private Dictionary<Type, List<FieldAtrributeTuple>> fieldsWithAttributes = new Dictionary<Type, List<FieldAtrributeTuple>>();
			private Dictionary<Type, List<ITypeResolveParameter>> typeResolveParameters = new Dictionary<Type, List<ITypeResolveParameter>>();
			private Dictionary<Type, List<MethodInfo>> serializationCallbacks = new Dictionary<Type, List<MethodInfo>>();

			public Type Type
			{
				get { return type; }
			}

			public CustomObjectTypeCache(Type type)
			{
				type.ThrowIfNull(nameof(type));
				this.type = type;
			}

			/// <summary>
			/// Retrieve all fields with a specific attribute defined.
			/// </summary>
			/// <param name="typeOfAttribute">Type of the attribute that will be looked for on fields.</param>
			/// <returns>A collection of fields that have the attribute defined on them.</returns>
			public IReadOnlyList<FieldAtrributeTuple> GetFieldsWithAttribute(Type typeOfAttribute)
			{
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (fieldsWithAttributes.ContainsKey(typeOfAttribute))
				{
					return fieldsWithAttributes[typeOfAttribute];
				}

				// Collection in which we will store the cached fields.
				List<FieldAtrributeTuple> targetFields = new List<FieldAtrributeTuple>();

				// Fetch all fields across the type hierarchy.
				Type targetType = Type;
				while ((targetType != null) && (targetType != typeof(object)))
				{
					FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (FieldInfo field in fields)
					{
						if (field.DeclaringType != targetType)
						{
							continue;
						}

						Attribute attr = field.GetCustomAttribute(typeOfAttribute, false);
						if (attr != null)
						{
							targetFields.Add(new FieldAtrributeTuple(field, attr));
						}
					}

					targetType = targetType.BaseType;
				}

				fieldsWithAttributes[typeOfAttribute] = targetFields;
				return targetFields;
			}

			/// <summary>
			/// Retrieve all type resolve parameters defined on a type of a specific type resolve attribute type.
			/// </summary>
			/// <param name="typeOfAttribute">The type resolve parameter attribute type.</param>
			/// <returns>A collection of all type resolve parameters defined on the type and it's inheritance tree.</returns>
			public IReadOnlyList<ITypeResolveParameter> GetTypeResolveParameters(Type typeOfAttribute)
			{
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!typeof(ITypeResolveParameter).IsAssignableFrom(typeOfAttribute))
				{
					throw new Serialization.SerializationException(string.Format("The requested attribute type to look for does not implement the {0} interface.", typeof(ITypeResolveParameter).Name));
				}

				if (typeResolveParameters.ContainsKey(typeOfAttribute))
				{
					return typeResolveParameters[typeOfAttribute];
				}

				// Fetch all attributes defined in the inheritance chain.
				List<ITypeResolveParameter> typeResolveAttributes = new List<ITypeResolveParameter>();

				// Attributes defined on the class itself.
				// Filter out those that aren't assignable to the current type.
				typeResolveAttributes.AddRange(Type.GetCustomAttributes(typeOfAttribute, true).Cast<ITypeResolveParameter>().Where(t => Type.IsAssignableFrom(t.Target)));

				// Attributes defined in interfaces implemented by the class or one of it's base classes.
				// Filter out those that aren't assignable to the current type.
				foreach (Type interfaceType in Type.GetInterfaces())
				{
					typeResolveAttributes.AddRange(interfaceType.GetCustomAttributes(typeOfAttribute, true).Cast<ITypeResolveParameter>().Where(t => Type.IsAssignableFrom(t.Target)));
				}

				typeResolveParameters[typeOfAttribute] = typeResolveAttributes;
				return typeResolveAttributes;
			}

			/// <summary>
			/// Retrieve all callback methods defined for a specific callback attribute type.
			/// </summary>
			/// <param name="typeOfAttribute">Type of the callback attribute.</param>
			/// <returns>A collection of all methods that are defined as a callback.</returns>
			public IReadOnlyList<MethodInfo> GetSerializationCallbacks(Type typeOfAttribute)
			{
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (serializationCallbacks.ContainsKey(typeOfAttribute))
				{
					return serializationCallbacks[typeOfAttribute];
				}

				List<MethodInfo> callbackMethods = new List<MethodInfo>();

				Type currentType = Type;
				BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
				while ((currentType != null) && (currentType != typeof(object)))
				{
					callbackMethods.AddRange(currentType.GetMethods(methodFlags).Where(m => m.IsDefined(typeOfAttribute)));
					currentType = currentType.BaseType;
				}

				serializationCallbacks[typeOfAttribute] = callbackMethods;
				return callbackMethods;
			}
		}
	}
}
