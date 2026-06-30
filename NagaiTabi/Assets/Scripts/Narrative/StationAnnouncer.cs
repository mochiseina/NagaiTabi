using UnityEngine;
using Naninovel;
using NagaiTabi.Journey;

public class StationAnnouncer : MonoBehaviour
{
	[Header("Referencias")]
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private YuinaDirector yuinaDirector;

	[Header("Scripts Naninovel (arrastra los .nani aquí)")]
	[ScriptAssetRef] public string arrivalsScriptRef;
	[ScriptAssetRef] public string approachingScriptRef;

	[Header("Umbral de aviso 'mamonaku'")]
	[Tooltip("Fracción del tramo restante (0..1) bajo la cual suena el 'mamonaku'. 0.10 = 10%.")]
	[Range(0.01f, 0.5f)]
	[SerializeField] private float approachRatio = 0.10f;

	private const string KEY_LAST_ARRIVED = "NagaiTabi_LastArrivedStation";
	private const string KEY_LAST_APPROACH = "NagaiTabi_LastApproachStation";

	private void OnEnable()
	{
		if (trackerManager != null)
			trackerManager.OnEntryLogged += HandleEntryLogged;
	}

	private void OnDisable()
	{
		if (trackerManager != null)
			trackerManager.OnEntryLogged -= HandleEntryLogged;
	}

	private void HandleEntryLogged(ImmersionEntry entry)
	{
		if (trackerManager == null) return;
		float totalHours = trackerManager.GetTotalMinutes() / 60f;

		bool announced = TryAnnounce(totalHours);

		if (!announced && yuinaDirector != null)
			yuinaDirector.PlayReactionFor(entry);
	}

	/// <summary>Reproduce un anuncio si corresponde. Devuelve true si reprodujo algo.</summary>
	public bool TryAnnounce(float totalHours)
	{
		int currentIndex = JourneyMap.GetCurrentStationIndex(totalHours);

		// 1. ¿Llegada a estación nueva? (Okinawa=0 excluido)
		int lastArrived = PlayerPrefs.GetInt(KEY_LAST_ARRIVED, 0);
		if (currentIndex > lastArrived && currentIndex >= 1)
		{
			PlayAnnouncement(arrivalsScriptRef, currentIndex);
			PlayerPrefs.SetInt(KEY_LAST_ARRIVED, currentIndex);
			PlayerPrefs.Save();
			return true;
		}

		// 2. ¿A <=10% del tramo hacia la siguiente?
		var next = JourneyMap.GetNextStation(totalHours);
		if (next == null) return false;

		int nextIndex = currentIndex + 1;
		float hoursToNext = JourneyMap.GetHoursToNextStation(totalHours);

		float from = JourneyMap.Stations[currentIndex].hoursToReach;
		float to = next.hoursToReach;
		float segment = Mathf.Max(0.0001f, to - from);

		bool withinApproach = (hoursToNext / segment) <= approachRatio;

		int lastApproach = PlayerPrefs.GetInt(KEY_LAST_APPROACH, -1);
		if (withinApproach && lastApproach != nextIndex)
		{
			PlayAnnouncement(approachingScriptRef, nextIndex);
			PlayerPrefs.SetInt(KEY_LAST_APPROACH, nextIndex);
			PlayerPrefs.Save();
			return true;
		}

		return false;
	}

	/// <summary>
	/// Salta a StationXX usando el mismo patrón que YuinaTalkTrigger:
	/// ScriptAssets.GetPath sobre la referencia + LoadAndPlayAtLabel.
	/// </summary>
	private void PlayAnnouncement(string scriptRef, int stationIndex)
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning("[StationAnnouncer] Naninovel no inicializado todavía.");
			return;
		}
		if (string.IsNullOrWhiteSpace(scriptRef))
		{
			Debug.LogWarning("[StationAnnouncer] Falta asignar el script .nani en el Inspector.");
			return;
		}

		string label = $"Station{stationIndex:00}";
		var player = Engine.GetService<IScriptPlayer>();
		var path = ScriptAssets.GetPath(scriptRef);

		Debug.Log($"[StationAnnouncer] Reproduciendo {path}#{label}");
		player.MainTrack.LoadAndPlayAtLabel(path, label).Forget();
	}

	/// <summary>Resetea el estado de anuncios (engánchalo a tu botón de reset).</summary>
	public void ResetAnnouncementState()
	{
		PlayerPrefs.DeleteKey(KEY_LAST_ARRIVED);
		PlayerPrefs.DeleteKey(KEY_LAST_APPROACH);
		PlayerPrefs.Save();
	}
}