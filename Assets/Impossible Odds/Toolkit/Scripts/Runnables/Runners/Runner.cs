namespace ImpossibleOdds.Runnables
{
	using System.Collections.Generic;
	using System.Collections;

	using UnityEngine;

	public class Runner : MonoBehaviour, IRunner, IFixedRunner, ILateRunner, IRoutineRunner
	{
		private List<IRunnable> runnables = new List<IRunnable>();
		private List<IFixedRunnable> fixedRunnables = new List<IFixedRunnable>();
		private List<ILateRunnable> lateRunnables = new List<ILateRunnable>();

		/// <inheritdoc />
		public void AddUpdate(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!runnables.Contains(runnable))
			{
				runnables.Add(runnable);
				Log.Info("Added runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <inheritdoc />
		public void AddFixedUpdate(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!fixedRunnables.Contains(runnable))
			{
				fixedRunnables.Add(runnable);
				Log.Info("Added fixed runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <inheritdoc />
		public void AddLateUpdate(ILateRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (!lateRunnables.Contains(runnable))
			{
				lateRunnables.Add(runnable);
				Log.Info("Added late runnable of type {0} to {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <summary>
		/// Adds an object to every kind of update, depending on the object's implemented interfaces.
		/// </summary>
		/// <param name="runnable">The object to hook into every type of update.</param>
		public void AddForAllUpdates(object runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnable is IRunnable regularUpdate)
			{
				AddUpdate(regularUpdate);
			}

			if (runnable is IFixedRunnable fixedUpdate)
			{
				AddFixedUpdate(fixedUpdate);
			}

			if (runnable is ILateRunnable lateUpdate)
			{
				AddLateUpdate(lateUpdate);
			}
		}

		/// <inheritdoc />
		public void RemoveUpdate(IRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnables.Remove(runnable))
			{
				Log.Info("Removed runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <inheritdoc />
		public void RemoveFixedUpdate(IFixedRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (fixedRunnables.Remove(runnable))
			{
				Log.Info("Removed fixed runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <inheritdoc />
		public void RemoveLateUpdate(ILateRunnable runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (lateRunnables.Remove(runnable))
			{
				Log.Info("Removed late runnable of type {0} from {1}.", runnable.GetType().Name, gameObject.name);
			}
		}

		/// <summary>
		/// Removes the object from every kind of update, depending on the object's implemented interfaces.
		/// </summary>
		/// <param name="runnable">The object to unhook from every type of update.</param>
		public void RemoveFromAllUpdates(object runnable)
		{
			runnable.ThrowIfNull(nameof(runnable));

			if (runnable is IRunnable regularUpdate)
			{
				RemoveUpdate(regularUpdate);
			}

			if (runnable is IFixedRunnable fixedUpdate)
			{
				RemoveFixedUpdate(fixedUpdate);
			}

			if (runnable is ILateRunnable lateUpdate)
			{
				RemoveLateUpdate(lateUpdate);
			}
		}

		/// <inheritdoc />
		public new Coroutine StartCoroutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			return base.StartCoroutine(routineHandle);
		}

		/// <inheritdoc />
		public new void StopCoroutine(IEnumerator routineHandle)
		{
			routineHandle.ThrowIfNull(nameof(routineHandle));
			base.StopCoroutine(routineHandle);
		}

		/// <inheritdoc />
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
