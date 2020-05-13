namespace ImpossibleOdds.Settings
{
	public interface IProjectSetting
	{
		bool IsChanged
		{
			get;
		}

		void DisplayGUI(string searchContext);
		void ApplyChanges();
	}
}
