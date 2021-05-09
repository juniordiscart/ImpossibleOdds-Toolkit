namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A binding that uses a generator to create new instances.
	/// </summary>
	/// <typeparam name="T">Type of the instance that is returned by the generator.</typeparam>
	public class GeneratorBinding<T> : IDependencyBinding<T>
	{
		private Func<T> generator;

		public Func<T> Generator
		{
			get { return generator; }
			set { generator = value; }
		}

		public GeneratorBinding(Func<T> generator)
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
