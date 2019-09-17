namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A binding that uses a function to generate new instances with each request.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FunctorBinding<T> : IDependencyBinding<T>
	{
		private Func<T> generator;

		public FunctorBinding(Func<T> generator)
		{
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}

			this.generator = generator;
		}

		public T GetInstance()
		{
			return generator();
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
