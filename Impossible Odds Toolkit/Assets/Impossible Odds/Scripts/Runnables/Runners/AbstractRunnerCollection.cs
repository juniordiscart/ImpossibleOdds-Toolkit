namespace ImpossibleOdds.Runnables
{
	using UnityEngine;

	[RequireComponent(typeof(EarlyRunner), typeof(DefaultRunner), typeof(LateRunner))]
	public abstract class AbstractRunnerCollection : MonoBehaviour
	{
		private EarlyRunner earlyRunner = null;
		private DefaultRunner defaultRunner = null;
		private LateRunner lateRunner = null;

		public EarlyRunner Early
		{
			get
			{
				if (earlyRunner == null)
				{
					earlyRunner = GetComponent<EarlyRunner>();
				}

				return earlyRunner;
			}
		}

		public DefaultRunner Default
		{
			get
			{
				if (defaultRunner == null)
				{
					defaultRunner = GetComponent<DefaultRunner>();
				}

				return defaultRunner;
			}
		}

		public LateRunner Late
		{
			get
			{
				if (lateRunner == null)
				{
					lateRunner = GetComponent<LateRunner>();
				}

				return lateRunner;
			}
		}
	}
}
