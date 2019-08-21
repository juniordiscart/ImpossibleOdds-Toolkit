namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A generator that provides the same instance for each request.
	/// </summary>
	/// <typeparam name="T">The type of the instance that will be provided.</typeparam>
	public class InstanceGenerator<T> : IDependencyGenerator<T>
	{
		private T instance;

		public T Instance
		{
			get { return instance; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				instance = value;
			}
		}

		public InstanceGenerator(T instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			this.instance = instance;
		}

		public T GetInstance()
		{
			return instance;
		}

		object IDependencyGenerator.GetInstance()
		{
			return GetInstance();
		}

		public Type GetTypeBinding()
		{
			return typeof(T);
		}
	}
}
