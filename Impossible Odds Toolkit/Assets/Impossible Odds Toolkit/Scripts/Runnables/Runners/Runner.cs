namespace ImpossibleOdds.Runnables
{
	using System.Collections.Generic;
	using System.Collections;
	using System;

	using UnityEngine;

	using Debug = ImpossibleOdds.Debug;

	public abstract class Runner : MonoBehaviour, IRunner, IFixedRunner
	{
		private HashSet<IRunnable> runnables = new HashSet<IRunnable>();
		private HashSet<IFixedRunnable> fixedRunnables = new HashSet<IFixedRunnable>();

		public void Add(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnables.Add(runnable))
			{
				runnable.CurrentRunner = this;
				Debug.Info("Added runnable of type {0} to {1}.", runnable.GetType(), gameObject.name);
			}
		}

		public void Add(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (fixedRunnables.Add(runnable))
			{
				runnable.CurrentRunner = this;
				Debug.Info("Added fixed runnable of type {0} to {1}.", runnable.GetType(), gameObject.name);
			}
		}

		public void Remove(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnables.Remove(runnable))
			{
				if (runnable.CurrentRunner == (IRunner)this)
				{
					runnable.CurrentRunner = null;
				}

				Debug.Info("Removed runnable of type {0} from {1}.", runnable.GetType(), gameObject.name);
			}
		}

		public void Remove(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (fixedRunnables.Remove(runnable))
			{
				if (runnable.CurrentRunner == (IFixedRunner)this)
				{
					runnable.CurrentRunner = null;
				}

				Debug.Info("Removed fixed runnable of type {0} from {1}.", runnable.GetType(), gameObject.name);
			}
		}

		public void RunRoutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			StartCoroutine(routineHandle);
		}

		public void StopRoutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			StopCoroutine(routineHandle);
		}

		protected virtual void Update()
		{
			foreach (IRunnable runnable in runnables)
			{
				if (runnable != null)
				{
					runnable.Update();
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			foreach (IFixedRunnable runnable in fixedRunnables)
			{
				if (runnable != null)
				{
					runnable.FixedUpdate();
				}
			}
		}

		protected virtual void OnDestroy()
		{
			foreach (IRunnable runnable in runnables)
			{
				if ((runnable != null) && (runnable is IDisposable))
				{
					(runnable as IDisposable).Dispose();
				}
			}
		}
	}
}
