using TMPro;
using UnityEngine;

public class LogInputPanel : MonoBehaviour
{
	[SerializeField] private TMP_InputField hoursInput;
	[SerializeField] private TMP_InputField minutesInput;
	[SerializeField] private TMP_InputField titleInput;
	[SerializeField] private TMP_Dropdown mediaTypeDropdown;

	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TrackerHUD trackerHUD;
	[SerializeField] private LogsListView logsListView;

	public void SubmitLog()
	{
		if (trackerManager == null)
		{
			Debug.LogWarning("[LogInputPanel] trackerManager no está asignado.");
			return;
		}

		int hours = 0;
		int minutes = 0;

		if (hoursInput != null && !string.IsNullOrWhiteSpace(hoursInput.text))
		{
			if (!int.TryParse(hoursInput.text, out hours))
			{
				Debug.LogWarning("[LogInputPanel] Las horas no son válidas.");
				return;
			}
		}

		if (minutesInput != null && !string.IsNullOrWhiteSpace(minutesInput.text))
		{
			if (!int.TryParse(minutesInput.text, out minutes))
			{
				Debug.LogWarning("[LogInputPanel] Los minutos no son válidos.");
				return;
			}
		}

		int totalMinutes = (hours * 60) + minutes;

		if (totalMinutes <= 0)
		{
			Debug.LogWarning("[LogInputPanel] Debes introducir al menos 1 minuto.");
			return;
		}

		string mediaType = mediaTypeDropdown != null
			? mediaTypeDropdown.options[mediaTypeDropdown.value].text
			: "Audio";

		string mode = MediaTypeMapper.GetModeFromMediaType(mediaType);

		string title = titleInput != null
			? titleInput.text.Trim()
			: "";

		trackerManager.AddEntry(totalMinutes, mode, mediaType, title);

		if (hoursInput != null) hoursInput.text = "";
		if (minutesInput != null) minutesInput.text = "";
		if (titleInput != null) titleInput.text = "";

		if (trackerHUD != null) trackerHUD.Refresh();
		if (logsListView != null) logsListView.Refresh();
	}
}