namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Generates a new instance every time a new request is made.
	/// </summary>
	/// <typeparam name="T">The type of instance to generate each request.</typeparam>
	public class NewInstanceGenerator<T> : IDependencyGenerator<T>
	where T : new()
	{
		public T GetInstance()
		{
			return new T();
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
