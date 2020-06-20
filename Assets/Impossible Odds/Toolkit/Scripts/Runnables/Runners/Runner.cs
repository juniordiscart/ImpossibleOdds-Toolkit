namespace ImpossibleOdds.Runnables
{
	using System.Collections.Generic;
	using System.Collections;

	using UnityEngine;

	using Debug = ImpossibleOdds.Debug;

	public class Runner : MonoBehaviour, IRunner, IFixedRunner, ILateRunner, IRoutineRunner
	{
		private List<IRunnable> runnables = new List<IRunnable>();
		private List<IFixedRunnable> fixedRunnables = new List<IFixedRunnable>();
		private List<ILateRunnable> lateRunnables = new List<ILateRunnable>();

		public void Add(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!runnables.Contains(runnable))
			{
				runnables.Add(runnable);
				Debug.Info("Added runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public void Add(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!fixedRunnables.Contains(runnable))
			{
				fixedRunnables.Add(runnable);
				Debug.Info("Added fixed runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public void Add(ILateRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!lateRunnables.Contains(runnable))
			{
				lateRunnables.Add(runnable);
				Debug.Info("Added late runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public void Remove(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnables.Remove(runnable))
			{
				Debug.Info("Removed runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public void Remove(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (fixedRunnables.Remove(runnable))
			{
				Debug.Info("Removed fixed runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public void Remove(ILateRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (lateRunnables.Remove(runnable))
			{
				Debug.Info("Removed late runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		public new Coroutine StartCoroutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			return base.StartCoroutine(routineHandle);
		}

		public new void StopCoroutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			base.StopCoroutine(routineHandle);
		}

		public new void StopCoroutine(Coroutine routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			base.StopCoroutine(routineHandle);
		}

		protected virtual void Update()
		{
			runnables.ForEach(r => r.Update());
		}

		protected virtual void FixedUpdate()
		{
			fixedRunnables.ForEach(r => r.FixedUpdate());
		}

		protected virtual void LateUpdate()
		{
			lateRunnables.ForEach(r => r.LateUpdate());
		}
	}
}
