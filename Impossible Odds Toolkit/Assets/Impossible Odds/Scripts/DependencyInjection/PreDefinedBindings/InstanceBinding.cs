namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A binding that provides the same instance for each request.
	/// </summary>
	/// <typeparam name="T">The type of the instance that will be provided.</typeparam>
	public class InstanceBinding<T> : IDependencyBinding<T>
	{
		private T instance;

		public T Instance
		{
			get { return instance; }
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

		public T GetInstance()
		{
			return instance;
		}

		object IDependencyBinding.GetInstance()
		{
			return GetInstance();
		}

		public Type GetTypeBinding()
		{
			return typeof(T);
		}
	}
}
