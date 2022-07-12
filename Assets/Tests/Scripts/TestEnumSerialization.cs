using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Json;

public class TestEnumSerialization : MonoBehaviour
{
	private void Start()
	{
		List<TestEnum> enums = new List<TestEnum>
		{
			TestEnum.FIRST,
			TestEnum.SECOND,
			TestEnum.LAST
		};

		string jsonValue = JsonProcessor.Serialize(enums);
		Log.Info(jsonValue);

		List<TestEnum> result = JsonProcessor.Deserialize<List<TestEnum>>(jsonValue);

		Log.Info("Lists are equal: {0}", enums.SequenceEqual(result));
	}

	[JsonEnumString]
	private enum TestEnum
	{
		[JsonEnumAlias("First")]
		FIRST,
		[JsonEnumAlias("The second")]
		SECOND,
		LAST
	}
}
