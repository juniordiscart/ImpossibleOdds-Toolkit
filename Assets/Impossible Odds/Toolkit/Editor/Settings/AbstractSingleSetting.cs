namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;

	public abstract class AbstractSingleSetting : IProjectSetting
	{
		protected HashSet<string> loadedSymbols;
		protected bool isSet = false;

		public AbstractSingleSetting(HashSet<string> loadedSymbols)
		{
			this.loadedSymbols = loadedSymbols;
			isSet = loadedSymbols.Contains(Symbol);
		}

		public bool IsChanged
		{
			get => isSet != loadedSymbols.Contains(Symbol);
		}

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

			if (isSet && !loadedSymbols.Contains(Symbol))
			{
				loadedSymbols.Add(Symbol);
			}
			else if (!isSet && loadedSymbols.Contains(Symbol))
			{
				loadedSymbols.Remove(Symbol);
			}
		}
	}
}
