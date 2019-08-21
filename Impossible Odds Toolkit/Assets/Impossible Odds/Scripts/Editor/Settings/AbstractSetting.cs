namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;

	public abstract class AbstractSetting
	{
		protected HashSet<string> loadedSymbols;
		protected bool isSet = false;

		public AbstractSetting(HashSet<string> loadedSymbols)
		{
			this.loadedSymbols = loadedSymbols;
			isSet = loadedSymbols.Contains(Symbol);
		}

		public bool IsChanged
		{
			get { return isSet != loadedSymbols.Contains(Symbol); }
		}

		public bool IsSet
		{
			get { return isSet; }
			set { isSet = value; }
		}

		public abstract string Symbol
		{
			get;
		}

		public abstract void DisplayGUI();

		public virtual void ApplySetting()
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