namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// A binding that uses a generator to create new instances.
	/// </summary>
	/// <typeparam name="T">Type of the instance that is returned by the generator.</typeparam>
	public class GeneratorBinding<T> : IDependencyBinding
	{
		private Func<T> generator;

		/// <summary>
		/// The generator that creates instances for injecting objects.
		/// </summary>
		public Func<T> Generator
		{
			get => generator;
			set
			{
				value.ThrowIfNull(nameof(value));
				generator = value;
			}
		}

		public GeneratorBinding(Func<T> generator)
		{
			generator.ThrowIfNull(nameof(generator));
			this.generator = generator;
		}

		/// <summary>
		/// Retrieves an instance of the requested type.
		/// </summary>
		public T GetInstance()
		{
			return generator();
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
