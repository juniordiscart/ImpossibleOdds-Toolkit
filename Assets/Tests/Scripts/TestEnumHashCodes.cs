using System.Collections;
using System.Collections.Generic;
using ImpossibleOdds;
using UnityEngine;

public class TestEnumHashCodes : MonoBehaviour
{
	private enum EnumTypeA
	{
		[DisplayName(Name = "Enum Type A - A")]
		A,
		B
	}

	private enum EnumTypeB
	{
		[DisplayName(Name = "Enum Type B - A")]
		A,
		B
	}


	void Start()
	{
		Log.Info("{0} - {1}", EnumTypeA.A.DisplayName(), EnumTypeB.A.DisplayName());
	}
}
