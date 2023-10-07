using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Runnables
{
	/// <summary>
	/// Interface for supporting starting a coroutine in a runner.
	/// </summary>
	public interface IRoutineRunner
	{
		/// <summary>
		/// Start running a coroutine.
		/// </summary>
		/// <param name="routineHandle">Handle to start the coroutine.</param>
		/// <returns></returns>
		Coroutine StartCoroutine(IEnumerator routineHandle);
		/// <summary>
		/// Stops to coroutine identified by the given handle.
		/// </summary>
		/// <param name="routineHandle">Coroutine handle of the coroutine that should be stopped.</param>
		void StopCoroutine(IEnumerator routineHandle);
		/// <summary>
		/// Stops to coroutine identified by the given handle.
		/// </summary>
		/// <param name="routineHandle">Coroutine handle of the coroutine that should be stopped.</param>
		void StopCoroutine(Coroutine routineHandle);
	}
}