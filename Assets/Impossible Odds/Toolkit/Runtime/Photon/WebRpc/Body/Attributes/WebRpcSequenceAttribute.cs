using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class WebRpcSequenceAttribute : Attribute, ISequenceParameter
	{
		public int Index { get; }

		public WebRpcSequenceAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException($"Index should be greater than 0. {index} given.");
			}

			Index = index;
		}
	}
}