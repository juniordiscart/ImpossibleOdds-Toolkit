namespace ImpossibleOdds.Runnables
{
	using System.Collections;
	using UnityEngine;

	public interface IRoutineRunner
	{
		Coroutine StartCoroutine(IEnumerator routineHandle);
		void StopCoroutine(IEnumerator routineHandle);
		void StopCoroutine(Coroutine routineHandle);
	}
}
