namespace ImpossibleOdds.Settings
{
	public interface IProjectSetting
	{
		bool IsChanged
		{
			get;
		}

		string SettingName
		{
			get;
		}

		void DisplayGUI(string searchContext);
		void ApplyChanges();
	}
}
