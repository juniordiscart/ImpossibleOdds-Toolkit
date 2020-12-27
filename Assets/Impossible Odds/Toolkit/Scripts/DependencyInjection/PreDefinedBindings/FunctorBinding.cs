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

		public Func<T> Generator
		{
			get { return generator; }
			set { generator = value; }
		}

		public FunctorBinding(Func<T> generator)
		{
			generator.ThrowIfNull(nameof(generator));
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
