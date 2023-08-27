using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImpossibleOdds;
using ImpossibleOdds.Json;

public class EnumDuplicates : MonoBehaviour
{
	private void Start()
	{
		Log.Info(TestEnum.A.DisplayName());
		Log.Info(TestEnum.B.DisplayName());
		Log.Info(TestEnum.ADuplicate.DisplayName());
	}

	[JsonEnumString]
	private enum TestEnum
	{
		[DisplayName(Name = "A")]
		A = 0,
		[DisplayName(Name = "B")]
		B = 1,
		[DisplayName(Name = "Duplicate A")]
		ADuplicate = A
	}
}
