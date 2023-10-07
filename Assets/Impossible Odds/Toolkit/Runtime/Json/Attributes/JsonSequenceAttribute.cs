using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class JsonSequenceAttribute : Attribute, ISequenceParameter
	{
		/// <inheritdoc />
		public int Index { get; }

		public JsonSequenceAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException($"Index should be greater than 0. {index} given.");
			}

			Index = index;
		}
	}
}