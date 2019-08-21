namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	// [CreateAssetMenu(fileName = "RejectionRegister", menuName = "Impossible Odds/Dependency Injection/Rejection Register")]
	public class DependencyInjectionRejectionRegister : ScriptableObject
	{
		private const string RejectionRegisterPath = "ImpossibleOdds/DependencyInjection/RejectionRegister";

		private static DependencyInjectionRejectionRegister register = null;

		[SerializeField, Tooltip("Types found in these namespaces are rejected during the injection process.")]
		private List<string> rejectedNamespaces = new List<string>();
		private Dictionary<Type, bool> rejectedTypes = new Dictionary<Type, bool>();

		public static DependencyInjectionRejectionRegister Register
		{
			get
			{
				if (register == null)
				{
					register = LoadRegister();
					if (register == null)
					{
						throw new DependencyInjectionException(string.Format("Could not load instance of {0} from resources at path '{1}'.", typeof(DependencyInjectionRejectionRegister).Name, RejectionRegisterPath));
					}
				}

				return register;
			}
		}

		public List<string> RejectedNamespaces
		{
			get { return Register?.rejectedNamespaces; }
		}

		/// <summary>
		/// Loads the rejection register from Resources at a specific path.
		/// </summary>
		public static DependencyInjectionRejectionRegister LoadRegister()
		{
			return Resources.Load<DependencyInjectionRejectionRegister>(RejectionRegisterPath);
		}

		public bool IsRejected(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (rejectedTypes.ContainsKey(type))
			{
				return rejectedTypes[type];
			}
			else
			{
				bool rejected = rejectedNamespaces.Contains(type.Namespace);
				rejectedTypes.Add(type, rejected);
				return rejected;
			}
		}
	}
}
