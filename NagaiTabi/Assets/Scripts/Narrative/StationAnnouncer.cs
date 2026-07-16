using UnityEngine;
using Naninovel;
using NagaiTabi.Journey;

public class StationAnnouncer : MonoBehaviour
{
	[Header("Referencias")]
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private YuinaDirector yuinaDirector;

	[Header("Scripts .nani")]
	[SerializeField] private string arrivalsScript = "Arrivals";
	[SerializeField] private string approachingScript = "Approaching";

	[Range(0.01f, 0.5f)]
	[SerializeField] private float approachRatio = 0.10f;

	private const string KEY_LAST_ARRIVED =
		"NagaiTabi_LastArrivedStation";

	private const string KEY_LAST_APPROACH =
		"NagaiTabi_LastApproachStation";

	private float lastKnownTotalHours;

	private void OnEnable()
	{
		if (trackerManager == null)
			return;

		trackerManager.OnEntryLogged += HandleEntryLogged;
		trackerManager.OnDataChanged += HandleDataChanged;

		lastKnownTotalHours =
			trackerManager.GetTotalMinutes() / 60f;
	}

	private void OnDisable()
	{
		if (trackerManager == null)
			return;

		trackerManager.OnEntryLogged -= HandleEntryLogged;
		trackerManager.OnDataChanged -= HandleDataChanged;
	}

	private void HandleEntryLogged(ImmersionEntry entry)
	{
		if (trackerManager == null)
			return;

		float totalHours =
			trackerManager.GetTotalMinutes() / 60f;

		bool announced = TryAnnounce(totalHours);

		if (!announced && yuinaDirector != null)
			yuinaDirector.PlayReactionFor(entry);
	}

	private void HandleDataChanged()
	{
		if (trackerManager == null)
			return;

		float totalHours =
			trackerManager.GetTotalMinutes() / 60f;

		bool hoursDecreased =
			totalHours < lastKnownTotalHours - 0.0001f;

		if (hoursDecreased)
			RewindAnnouncementState(totalHours);

		lastKnownTotalHours = totalHours;
	}

	private void RewindAnnouncementState(float totalHours)
	{
		int currentIndex = Mathf.Max(
			0,
			JourneyMap.GetCurrentStationIndex(totalHours)
		);

		PlayerPrefs.SetInt(
			KEY_LAST_ARRIVED,
			currentIndex
		);

		PlayerPrefs.DeleteKey(KEY_LAST_APPROACH);

		PlayerPrefs.Save();

		Debug.Log(
			$"[StationAnnouncer] Progreso reducido a " +
			$"{totalHours:0.0} h. " +
			$"Marcadores rebobinados hasta estación {currentIndex}."
		);
	}

	public bool TryAnnounce(float totalHours)
	{
		int currentIndex =
			JourneyMap.GetCurrentStationIndex(totalHours);

		int lastArrived = PlayerPrefs.GetInt(
			KEY_LAST_ARRIVED,
			0
		);

		// Llegada a una estación nueva.
		if (currentIndex > lastArrived && currentIndex >= 1)
		{
			PlayAnnouncement(
				arrivalsScript,
				currentIndex
			);

			PlayerPrefs.SetInt(
				KEY_LAST_ARRIVED,
				currentIndex
			);

			PlayerPrefs.Save();
			return true;
		}

		var next = JourneyMap.GetNextStation(totalHours);

		if (next == null)
			return false;

		int nextIndex = currentIndex + 1;

		float hoursToNext =
			JourneyMap.GetHoursToNextStation(totalHours);

		float from =
			JourneyMap.Stations[currentIndex].hoursToReach;

		float segment = Mathf.Max(
			0.0001f,
			next.hoursToReach - from
		);

		bool withinApproach =
			hoursToNext / segment <= approachRatio;

		int lastApproach = PlayerPrefs.GetInt(
			KEY_LAST_APPROACH,
			-1
		);

		// Próxima estación: aviso de aproximación.
		if (withinApproach && lastApproach != nextIndex)
		{
			PlayAnnouncement(
				approachingScript,
				nextIndex
			);

			PlayerPrefs.SetInt(
				KEY_LAST_APPROACH,
				nextIndex
			);

			PlayerPrefs.Save();
			return true;
		}

		return false;
	}

	private void PlayAnnouncement(
		string scriptName,
		int stationIndex
	)
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning(
				"[StationAnnouncer] Naninovel no inicializado."
			);

			return;
		}

		var variables =
			Engine.GetService<ICustomVariableManager>();

		variables.SetVariableValue(
			"G_Station",
			new CustomVariableValue(
				stationIndex.ToString()
			)
		);

		var audio = Engine.GetService<IAudioManager>();
		audio.StopVoice();

		var player = Engine.GetService<IScriptPlayer>();

		Debug.Log(
			$"[StationAnnouncer] {scriptName} " +
			$"-> estación {stationIndex}"
		);

		player.MainTrack
			.LoadAndPlay(scriptName)
			.Forget();
	}

	public void ResetAnnouncementState()
	{
		PlayerPrefs.DeleteKey(KEY_LAST_ARRIVED);
		PlayerPrefs.DeleteKey(KEY_LAST_APPROACH);
		PlayerPrefs.Save();

		if (trackerManager != null)
		{
			lastKnownTotalHours =
				trackerManager.GetTotalMinutes() / 60f;
		}

		Debug.Log(
			"[StationAnnouncer] Estado de anuncios reseteado."
		);
	}
}