using TMPro;
using UnityEngine;
using NagaiTabi.Journey;

/// <summary>
/// Rellena el cartel de estación tipo tren japonés (debajo del mapa):
///   - NS: número de línea (01..15) de la estación actual.
///   - Estación actual: nombre japonés + romaji.
///   - つぎは: siguiente estación, nombre japonés + romaji.
///   - Horas restantes para la siguiente estación.
///
/// Mapeo de tu jerarquía -> campos:
///   StationNumber      -> stationNumberText
///   StationName        -> currentStationJpText   (japonés actual, p. ej. 沖縄)
///   StationRomaji      -> currentStationRomajiText (Okinawa)
///   NextStationName    -> nextStationJpText       (japonés siguiente, p. ej. 熊本)
///   NextStationRomaji  -> nextStationRomajiText   (Kumamoto)
///   HoursLeftText      -> hoursLeftText
/// </summary>
public class StationSignView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;

	[Header("Número de línea (NS)")]
	[SerializeField] private TMP_Text stationNumberText;

	[Header("Estación actual")]
	[SerializeField] private TMP_Text currentStationJpText;
	[SerializeField] private TMP_Text currentStationRomajiText;

	[Header("Siguiente estación (つぎは)")]
	[SerializeField] private TMP_Text nextStationJpText;
	[SerializeField] private TMP_Text nextStationRomajiText;

	[Header("Horas restantes")]
	[SerializeField] private TMP_Text hoursLeftText;

	public void Refresh()
	{
		if (trackerManager == null)
		{
			Debug.LogWarning("[StationSignView] Falta trackerManager.");
			return;
		}

		float totalHours = trackerManager.GetTotalMinutes() / 60f;

		int currentIndex = JourneyMap.GetCurrentStationIndex(totalHours);
		var current = JourneyMap.Stations[currentIndex];
		var next = JourneyMap.GetNextStation(totalHours);
		float hoursLeft = JourneyMap.GetHoursToNextStation(totalHours);

		// NS: 01..15
		if (stationNumberText != null)
			stationNumberText.text = (currentIndex + 1).ToString("00");

		// Estación actual (japonés + romaji)
		if (currentStationJpText != null) currentStationJpText.text = current.nameJp;
		if (currentStationRomajiText != null) currentStationRomajiText.text = current.name;

		if (next == null)
		{
			// Final del viaje: Wakkanai.
			if (nextStationJpText != null) nextStationJpText.text = "終点";
			if (nextStationRomajiText != null) nextStationRomajiText.text = "Shūten";
			if (hoursLeftText != null) hoursLeftText.text = "—";
		}
		else
		{
			if (nextStationJpText != null) nextStationJpText.text = next.nameJp;
			if (nextStationRomajiText != null) nextStationRomajiText.text = next.name;

			if (hoursLeftText != null)
			{
				if (hoursLeft >= 1f)
					hoursLeftText.text = $"{hoursLeft:0.#} hrs.";
				else
					hoursLeftText.text = $"{Mathf.CeilToInt(hoursLeft * 60f)} min";
			}
		}
	}

	private void Start()
	{
		Refresh();
	}
}