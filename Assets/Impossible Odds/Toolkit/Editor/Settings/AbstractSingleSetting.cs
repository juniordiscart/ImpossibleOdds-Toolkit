namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;

	public abstract class AbstractSingleSetting : IProjectSetting
	{
		protected readonly HashSet<string> loadedSymbols;
		protected bool isSet;

		public AbstractSingleSetting(HashSet<string> loadedSymbols)
		{
			this.loadedSymbols = loadedSymbols;
			isSet = loadedSymbols.Contains(Symbol);
		}

		public bool IsChanged => isSet != loadedSymbols.Contains(Symbol);

		public bool IsSet
		{
			get => isSet;
			set => isSet = value;
		}

		public abstract string SettingName
		{
			get;
		}

		public abstract string Symbol
		{
			get;
		}

		public abstract void DisplayGUI(string searchContext);

		public virtual void ApplyChanges()
		{
			if (!IsChanged)
			{
				return;
			}

			switch (isSet)
			{
				case true when !loadedSymbols.Contains(Symbol):
					loadedSymbols.Add(Symbol);
					break;
				case false when loadedSymbols.Contains(Symbol):
					loadedSymbols.Remove(Symbol);
					break;
			}
		}
	}
}