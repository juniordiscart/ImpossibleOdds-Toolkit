namespace ImpossibleOdds.Testing.Runnables
{
	using UnityEngine;
	using UnityEngine.UI;
	using ImpossibleOdds.Runnables;
	using TMPro;

	public class TestRunnables : MonoBehaviour
	{
		[SerializeField]
		private Button btnPrintFrames = null;
		[SerializeField]
		private TextMeshProUGUI txtUpdateCounter = null;

		private FrameCounter frameCounter = null;

		private void Start()
		{
			Application.targetFrameRate = 60;

			frameCounter = new FrameCounter();
			SceneRunner runner = SceneRunner.GetRunner();
			runner.Add(frameCounter as IRunnable);
			runner.Add(frameCounter as IFixedRunnable);

			OnPrintFrames();
			btnPrintFrames.onClick.AddListener(OnPrintFrames);
		}

		private void OnPrintFrames()
		{
			txtUpdateCounter.text = string.Format("Updates: {0}, Fixed updates: {1}", frameCounter.UpdateCounter, frameCounter.FixedUpdateCounter);
		}
	}
}
