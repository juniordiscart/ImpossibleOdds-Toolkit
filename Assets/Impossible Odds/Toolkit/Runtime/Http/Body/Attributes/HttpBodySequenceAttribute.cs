using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class HttpBodySequenceAttribute : Attribute, ISequenceParameter
	{
		public int Index { get; }

		public HttpBodySequenceAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException($"Index should be greater than 0. {index} given.");
			}

			Index = index;
		}
	}
}