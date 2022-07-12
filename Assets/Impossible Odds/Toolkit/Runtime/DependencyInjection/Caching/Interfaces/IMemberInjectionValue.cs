namespace ImpossibleOdds.DependencyInjection
{
	using ImpossibleOdds.ReflectionCaching;

	internal interface IMemberInjectionValue : IMemberAttributePair
	{
		/// <summary>
		/// The injection attribute applied on the member.
		/// </summary>
		new InjectAttribute Attribute
		{
			get;
		}
	}
}
