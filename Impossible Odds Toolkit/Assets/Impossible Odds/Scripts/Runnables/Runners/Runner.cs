namespace ImpossibleOdds.Runnables
{
	using System.Collections.Generic;
	using System.Collections;
	using System;

	using UnityEngine;

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

				#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogFormat("Added runnable of type {0} to {1}.", runnable.GetType(), gameObject.name);
				#endif
			}
		}

		public void Add(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (fixedRunnables.Add(runnable))
			{
				runnable.CurrentRunner = this;

				#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogFormat("Added fixed runnable of type {0} to {1}.", runnable.GetType(), gameObject.name);
				#endif
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

				#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogFormat("Removed runnable of type {0} from {1}.", runnable.GetType(), gameObject.name);
				#endif
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

				#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogFormat("Removed fixed runnable of type {0} from {1}.", runnable.GetType(), gameObject.name);
				#endif
			}
		}

		public void RunRoutine(IEnumerator routineHandle)
		{
			StartCoroutine(routineHandle);
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
