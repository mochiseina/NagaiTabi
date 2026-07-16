using UnityEngine;
using Naninovel;
using NagaiTabi.Journey;

public class StationAnnouncer : MonoBehaviour
{
	[Header("Referencias")]
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private YuinaDirector yuinaDirector;

	[Header("Nombres de los scripts .nani (sin extensión, tal como se llaman)")]
	[SerializeField] private string arrivalsScript = "Arrivals";
	[SerializeField] private string approachingScript = "Approaching";

	[Range(0.01f, 0.5f)]
	[SerializeField] private float approachRatio = 0.10f;

	private const string KEY_LAST_ARRIVED = "NagaiTabi_LastArrivedStation";
	private const string KEY_LAST_APPROACH = "NagaiTabi_LastApproachStation";

	private void OnEnable()
	{
		if (trackerManager != null) trackerManager.OnEntryLogged += HandleEntryLogged;
	}

	private void OnDisable()
	{
		if (trackerManager != null) trackerManager.OnEntryLogged -= HandleEntryLogged;
	}

	private void HandleEntryLogged(ImmersionEntry entry)
	{
		if (trackerManager == null) return;
		float totalHours = trackerManager.GetTotalMinutes() / 60f;
		bool announced = TryAnnounce(totalHours);
		if (!announced && yuinaDirector != null) yuinaDirector.PlayReactionFor(entry);
	}

	public bool TryAnnounce(float totalHours)
	{
		int currentIndex = JourneyMap.GetCurrentStationIndex(totalHours);

		int lastArrived = PlayerPrefs.GetInt(KEY_LAST_ARRIVED, 0);
		if (currentIndex > lastArrived && currentIndex >= 1)
		{
			PlayAnnouncement(arrivalsScript, currentIndex);
			PlayerPrefs.SetInt(KEY_LAST_ARRIVED, currentIndex);
			PlayerPrefs.Save();
			return true;
		}

		var next = JourneyMap.GetNextStation(totalHours);
		if (next == null) return false;

		int nextIndex = currentIndex + 1;
		float hoursToNext = JourneyMap.GetHoursToNextStation(totalHours);
		float from = JourneyMap.Stations[currentIndex].hoursToReach;
		float segment = Mathf.Max(0.0001f, next.hoursToReach - from);
		bool withinApproach = (hoursToNext / segment) <= approachRatio;

		int lastApproach = PlayerPrefs.GetInt(KEY_LAST_APPROACH, -1);
		if (withinApproach && lastApproach != nextIndex)
		{
			PlayAnnouncement(approachingScript, nextIndex);
			PlayerPrefs.SetInt(KEY_LAST_APPROACH, nextIndex);
			PlayerPrefs.Save();
			return true;
		}
		return false;
	}
	private void PlayAnnouncement(string scriptName, int stationIndex)
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning("[StationAnnouncer] Naninovel no inicializado.");
			return;
		}

		var vars = Engine.GetService<ICustomVariableManager>();
		vars.SetVariableValue(
			"G_Station",
			new CustomVariableValue(stationIndex.ToString())
		);

		var audio = Engine.GetService<IAudioManager>();
		audio.StopVoice();

		var player = Engine.GetService<IScriptPlayer>();

		// El anuncio de llegada o aproximación tiene prioridad sobre cualquier comentario manual de Yuina que siga reproduciéndose
		player.MainTrack.Stop();

		Debug.Log($"[StationAnnouncer] {scriptName} -> estación {stationIndex}");
		player.MainTrack.LoadAndPlay(scriptName).Forget();
	}
	public void ResetAnnouncementState()
	{
		PlayerPrefs.DeleteKey(KEY_LAST_ARRIVED);
		PlayerPrefs.DeleteKey(KEY_LAST_APPROACH);
		PlayerPrefs.Save();
	}
}