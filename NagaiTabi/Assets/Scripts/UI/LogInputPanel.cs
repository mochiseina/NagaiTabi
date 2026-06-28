using System;
using TMPro;
using UnityEngine;

public class LogInputPanel : MonoBehaviour
{
	[Header("Inputs existentes")]
	[SerializeField] private TMP_InputField hoursInput;
	[SerializeField] private TMP_InputField minutesInput;
	[SerializeField] private TMP_InputField titleInput;
	[Tooltip("Si usas botones individuales por medio en vez de dropdown, deja esto vacío " +
			 "y usa SetMediaType(\"Anime\") desde el OnClick de cada botón.")]
	[SerializeField] private TMP_Dropdown mediaTypeDropdown;

	[Header("Inputs nuevos")]
	[Tooltip("Caracteres leídos. Se bloquea solo si el medio es de audio.")]
	[SerializeField] private TMP_InputField charsInput;
	[Tooltip("Fecha del log (p. ej. 2026-06-28). Vacío = hoy.")]
	[SerializeField] private TMP_InputField dateInput;

	[Header("Referencias")]
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TrackerHUD trackerHUD;
	[SerializeField] private LogsListView logsListView;
	[SerializeField] private StatsPanelView statsPanelView;
	[SerializeField] private StationSignView stationSignView;
	[SerializeField] private JourneyMapView journeyMapView;

	// Medio seleccionado por botones (si no usas dropdown).
	private string selectedMediaType = "";

	private void Start()
	{
		if (mediaTypeDropdown != null)
		{
			mediaTypeDropdown.onValueChanged.AddListener(_ => UpdateCharsAvailability());
			selectedMediaType = CurrentMediaType();
		}
		UpdateCharsAvailability();
	}

	/// <summary>
	/// Llama a esto desde el OnClick de cada botón de medio (Anime, Manga, Reading...).
	/// Pasa el nombre EXACTO del medio como en MediaTypeMapper.
	/// </summary>
	public void SetMediaType(string mediaType)
	{
		selectedMediaType = mediaType;
		UpdateCharsAvailability();
	}

	private string CurrentMediaType()
	{
		if (mediaTypeDropdown != null)
			return mediaTypeDropdown.options[mediaTypeDropdown.value].text;
		return selectedMediaType;
	}

	private void UpdateCharsAvailability()
	{
		if (charsInput == null) return;

		string mediaType = CurrentMediaType();
		bool isReading = !string.IsNullOrEmpty(mediaType) &&
						 MediaTypeMapper.GetModeFromMediaType(mediaType) == "Reading";

		charsInput.interactable = isReading;
		if (!isReading) charsInput.text = "";
	}

	private bool TryGetMinutes(out int totalMinutes)
	{
		totalMinutes = 0;
		int hours = 0, minutes = 0;

		string h = hoursInput != null ? hoursInput.text.Trim() : "";
		string m = minutesInput != null ? minutesInput.text.Trim() : "";

		if (!string.IsNullOrWhiteSpace(h) && !int.TryParse(h, out hours)) return false;
		if (!string.IsNullOrWhiteSpace(m) && !int.TryParse(m, out minutes)) return false;

		hours = Mathf.Clamp(hours, 0, 23);
		minutes = Mathf.Clamp(minutes, 0, 59);
		totalMinutes = hours * 60 + minutes;
		return true;
	}

	/// <summary>
	/// ¿Es válido el log? Reglas: medio seleccionado + al menos 1 minuto.
	/// (chars y fecha son opcionales.) La palanca usa esto para permitir o no el registro.
	/// </summary>
	public bool IsValid()
	{
		if (string.IsNullOrEmpty(CurrentMediaType())) return false;
		if (!TryGetMinutes(out int total)) return false;
		return total >= 1;
	}

	public void SubmitLog()
	{
		if (trackerManager == null)
		{
			Debug.LogWarning("[LogInputPanel] trackerManager no está asignado.");
			return;
		}

		if (!IsValid())
		{
			Debug.LogWarning("[LogInputPanel] Log inválido (falta medio o tiempo).");
			return;
		}

		TryGetMinutes(out int totalMinutes);

		string mediaType = CurrentMediaType();
		string mode = MediaTypeMapper.GetModeFromMediaType(mediaType);
		string title = titleInput != null ? titleInput.text.Trim() : "";

		int chars = 0;
		if (mode == "Reading" && charsInput != null && !string.IsNullOrWhiteSpace(charsInput.text))
			int.TryParse(charsInput.text.Trim(), out chars);

		string dateOverride = null;
		if (dateInput != null && !string.IsNullOrWhiteSpace(dateInput.text))
		{
			if (DateTime.TryParse(dateInput.text.Trim(), out var dt))
				dateOverride = dt.ToString("o");
			else
				Debug.LogWarning("[LogInputPanel] Fecha no reconocida; se usa la de hoy.");
		}

		trackerManager.AddEntry(totalMinutes, mode, mediaType, title, chars, dateOverride);

		if (hoursInput != null) hoursInput.text = "";
		if (minutesInput != null) minutesInput.text = "";
		if (titleInput != null) titleInput.text = "";
		if (charsInput != null) charsInput.text = "";
		if (dateInput != null) dateInput.text = "";

		if (trackerHUD != null) trackerHUD.Refresh();
		if (logsListView != null) logsListView.Refresh();
		if (statsPanelView != null) statsPanelView.Refresh();
		if (stationSignView != null) stationSignView.Refresh();
		if (journeyMapView != null) journeyMapView.Refresh();
	}
}