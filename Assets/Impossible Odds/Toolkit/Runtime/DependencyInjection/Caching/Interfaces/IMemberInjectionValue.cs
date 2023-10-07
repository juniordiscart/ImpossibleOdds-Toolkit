using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.DependencyInjection
{

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