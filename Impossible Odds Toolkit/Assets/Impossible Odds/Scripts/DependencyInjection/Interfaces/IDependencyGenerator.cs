namespace ImpossibleOdds.DependencyInjection
{
	using System;

	public interface IDependencyGenerator
	{
		Type GetTypeBinding();

		object GetInstance();
	}

	public interface IDependencyGenerator<T> : IDependencyGenerator
	{
		new T GetInstance();
	}
}
