using UnityEngine;

public class TabController : MonoBehaviour
{
	[SerializeField] private GameObject profilePanel;
	[SerializeField] private GameObject statsPanel;
	[SerializeField] private GameObject guidePanel;

	[SerializeField] private PlayerProfileView playerProfileView;

	private GameObject currentOpenPanel;

	private void Start()
	{
		CloseAllPanels();
	}

	public void ToggleProfile()
	{
		TogglePanel(profilePanel);

		if (currentOpenPanel == profilePanel && playerProfileView != null)
			playerProfileView.RefreshFromNaninovel();
	}
	public void ToggleStats()
	{
		TogglePanel(statsPanel);
	}

	public void ToggleGuide()
	{
		TogglePanel(guidePanel);
	}
	private void TogglePanel(GameObject targetPanel)
	{
		if (targetPanel == null) return;

		if (currentOpenPanel == targetPanel && targetPanel.activeSelf)
		{
			CloseAllPanels();
			return;
		}

		CloseAllPanels();
		targetPanel.SetActive(true);
		currentOpenPanel = targetPanel;
	}
	private void CloseAllPanels()
	{
		if (profilePanel != null) profilePanel.SetActive(false);
		if (statsPanel != null) statsPanel.SetActive(false);
		if (guidePanel != null) guidePanel.SetActive(false);

		currentOpenPanel = null;
	}
}