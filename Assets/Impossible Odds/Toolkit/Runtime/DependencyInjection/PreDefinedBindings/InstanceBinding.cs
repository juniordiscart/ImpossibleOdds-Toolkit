namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A binding that provides the same instance for each request.
	/// </summary>
	/// <typeparam name="T">The type of the instance that will be provided.</typeparam>
	public class InstanceBinding<T> : IDependencyBinding
	{
		private T instance;

		/// <summary>
		/// The instance of the binding that should be returned while injecting objects.
		/// </summary>
		public T Instance
		{
			get => instance;
			set
			{
				value.ThrowIfNull(nameof(value));
				instance = value;
			}
		}

		public InstanceBinding(T instance)
		{
			instance.ThrowIfNull(nameof(instance));
			this.instance = instance;
		}

		/// <inheritdoc />
		public T GetInstance()
		{
			return instance;
		}

		/// <inheritdoc />
		object IDependencyBinding.GetInstance()
		{
			return GetInstance();
		}

		/// <inheritdoc />
		public Type GetTypeBinding()
		{
			return typeof(T);
		}
	}
}
