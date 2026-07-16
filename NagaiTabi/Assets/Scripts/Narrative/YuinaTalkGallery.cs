using System.Collections;
using System.Globalization;
using UnityEngine;
using Naninovel;
using NagaiTabi.Journey;

public class YuinaTalkGallery : MonoBehaviour
{
	[Header("Referencias")]
	[SerializeField] private TrackerManager trackerManager;

	[Header("Scripts")]
	[Tooltip("Escenario con los comentarios generales de Yuina.")]
	[SerializeField] private string generalTalkScript = "YuinaTalk";

	[Tooltip("Escenario con información sobre la estación actual.")]
	[SerializeField] private string stationTalkScript = "YuinaStationTalk";

	[Tooltip("Escenario que oculta y limpia el printer.")]
	[SerializeField] private string dismissScript = "YuinaDismiss";

	[Header("Rotación")]
	[Tooltip("Comentarios generales mostrados antes de insertar información de estación.")]
	[Min(0)]
	[SerializeField] private int generalTalksBeforeStationInfo = 2;

	[Header("Cierre automático")]
	[Tooltip("Segundos sin pulsar a Yuina antes de ocultar el diálogo.")]
	[Range(3f, 15f)]
	[SerializeField] private float autoDismissSeconds = 7f;

	private int clickCounter;
	private Coroutine autoDismissRoutine;

	private static readonly string[] StationNames =
	{
		"Okinawa",
		"Kumamoto",
		"Fukuoka",
		"Yamaguchi",
		"Hiroshima",
		"Takamatsu",
		"Osaka",
		"Tokyo",
		"Oarai",
		"Fukushima",
		"Sendai",
		"Tsugaru",
		"Hakodate",
		"Sapporo",
		"Wakkanai"
	};

	private static readonly string[] StationJapanese =
	{
		"沖縄",
		"熊本",
		"福岡",
		"山口",
		"広島",
		"高松",
		"大阪",
		"東京",
		"大洗",
		"福島",
		"仙台",
		"津軽",
		"函館",
		"札幌",
		"稚内"
	};

	public void TriggerTalk()
	{
		if (!Engine.Initialized)
		{
			Debug.LogWarning(
				"[YuinaTalkGallery] Naninovel todavía no está inicializado."
			);
			return;
		}

		if (trackerManager == null)
		{
			Debug.LogError(
				"[YuinaTalkGallery] Falta asignar TrackerManager en el Inspector."
			);
			return;
		}

		var player = Engine.GetService<IScriptPlayer>();

		if (player == null)
		{
			Debug.LogError(
				"[YuinaTalkGallery] No se pudo obtener IScriptPlayer."
			);
			return;
		}
		if (player.Playing)
		{
			Debug.Log(
				"[YuinaTalkGallery] Naninovel está ejecutando otro escenario. Clic ignorado."
			);
			return;
		}

		int cycleLength = Mathf.Max(
			1,
			generalTalksBeforeStationInfo + 1
		);

		bool stationTurn =
			clickCounter % cycleLength == generalTalksBeforeStationInfo;

		clickCounter++;

		bool talkStarted = stationTurn
			? PlayStationTalk(player)
			: PlayGeneralTalk(player);

		if (talkStarted)
			RestartAutoDismissTimer();
	}

	private bool PlayGeneralTalk(IScriptPlayer player)
	{
		if (string.IsNullOrWhiteSpace(generalTalkScript))
		{
			Debug.LogWarning(
				"[YuinaTalkGallery] General Talk Script está vacío."
			);
			return false;
		}

		Debug.Log(
			$"[YuinaTalkGallery] Diálogo general: {generalTalkScript}"
		);

		player.MainTrack
			.LoadAndPlay(generalTalkScript)
			.Forget();

		return true;
	}

	private bool PlayStationTalk(IScriptPlayer player)
	{
		if (string.IsNullOrWhiteSpace(stationTalkScript))
		{
			Debug.LogWarning(
				"[YuinaTalkGallery] Station Talk Script está vacío."
			);
			return false;
		}

		if (StationNames.Length == 0 ||
			StationJapanese.Length != StationNames.Length)
		{
			Debug.LogError(
				"[YuinaTalkGallery] Las listas de estaciones no coinciden."
			);
			return false;
		}

		float totalHours = trackerManager.GetTotalMinutes() / 60f;

		int stationIndex = Mathf.Clamp(
			JourneyMap.GetCurrentStationIndex(totalHours),
			0,
			StationNames.Length - 1
		);

		int stationNumber = stationIndex + 1;

		string variantKey =
			$"NT_StationTalkVariant_{stationIndex}";

		int variant = Mathf.Clamp(
			PlayerPrefs.GetInt(variantKey, 0),
			0,
			1
		);

		PlayerPrefs.SetInt(
			variantKey,
			(variant + 1) % 2
		);

		PlayerPrefs.Save();

		bool finalStation =
			stationIndex >= StationNames.Length - 1;

		string nextStationName = finalStation
			? StationNames[stationIndex]
			: StationNames[stationIndex + 1];

		float hoursToNext = finalStation
			? 0f
			: Mathf.Max(
				0f,
				JourneyMap.GetHoursToNextStation(totalHours)
			);

		float segmentProgress = CalculateSegmentProgress(
			stationIndex,
			totalHours
		);

		var variables =
			Engine.GetService<ICustomVariableManager>();

		if (variables == null)
		{
			Debug.LogError(
				"[YuinaTalkGallery] No se pudo obtener ICustomVariableManager."
			);
			return false;
		}

		SetVariable(
			variables,
			"G_Station",
			stationNumber.ToString(CultureInfo.InvariantCulture)
		);

		SetVariable(
			variables,
			"G_StationIndex",
			stationIndex.ToString(CultureInfo.InvariantCulture)
		);

		SetVariable(
			variables,
			"G_StationTalkVariant",
			variant.ToString(CultureInfo.InvariantCulture)
		);

		SetVariable(
			variables,
			"G_StationName",
			StationNames[stationIndex]
		);

		SetVariable(
			variables,
			"G_StationJapanese",
			StationJapanese[stationIndex]
		);

		SetVariable(
			variables,
			"G_StationNumber",
			stationNumber.ToString(
				"00",
				CultureInfo.InvariantCulture
			)
		);

		SetVariable(
			variables,
			"G_TotalStations",
			StationNames.Length.ToString(
				CultureInfo.InvariantCulture
			)
		);

		SetVariable(
			variables,
			"G_NextStationName",
			nextStationName
		);

		SetVariable(
			variables,
			"G_HoursToNext",
			hoursToNext.ToString(
				"0.0",
				CultureInfo.InvariantCulture
			)
		);

		SetVariable(
			variables,
			"G_StationProgress",
			segmentProgress.ToString(
				"0",
				CultureInfo.InvariantCulture
			)
		);

		SetVariable(
			variables,
			"G_AtFinalStation",
			finalStation ? "true" : "false"
		);

		Debug.Log(
			$"[YuinaTalkGallery] Estación {stationNumber:00}: " +
			$"{StationNames[stationIndex]} ({StationJapanese[stationIndex]}), " +
			$"variante {variant}."
		);

		player.MainTrack
			.LoadAndPlay(stationTalkScript)
			.Forget();

		return true;
	}

	private static float CalculateSegmentProgress(
		int stationIndex,
		float totalHours
	)
	{
		if (stationIndex >= StationNames.Length - 1)
			return 100f;

		float currentThreshold =
			JourneyMap.Stations[stationIndex].hoursToReach;

		float nextThreshold =
			JourneyMap.Stations[stationIndex + 1].hoursToReach;

		if (nextThreshold <= currentThreshold)
			return 100f;

		return Mathf.InverseLerp(
			currentThreshold,
			nextThreshold,
			totalHours
		) * 100f;
	}

	private void RestartAutoDismissTimer()
	{
		if (autoDismissRoutine != null)
			StopCoroutine(autoDismissRoutine);

		autoDismissRoutine = StartCoroutine(
			AutoDismissAfterDelay()
		);
	}

	private IEnumerator AutoDismissAfterDelay()
	{
		yield return new WaitForSecondsRealtime(
			autoDismissSeconds
		);

		autoDismissRoutine = null;

		if (!Engine.Initialized)
			yield break;

		var player = Engine.GetService<IScriptPlayer>();

		if (player == null)
			yield break;

		if (player.Playing)
		{
			Debug.Log(
				"[YuinaTalkGallery] Cierre automático cancelado: Naninovel está ocupado."
			);
			yield break;
		}

		if (string.IsNullOrWhiteSpace(dismissScript))
		{
			Debug.LogWarning(
				"[YuinaTalkGallery] Dismiss Script está vacío."
			);
			yield break;
		}

		player.MainTrack
			.LoadAndPlay(dismissScript)
			.Forget();
	}
	public void DismissNow()
	{
		CancelPendingDismiss();

		if (!Engine.Initialized)
			return;

		var player = Engine.GetService<IScriptPlayer>();

		if (player == null || player.Playing)
			return;

		player.MainTrack
			.LoadAndPlay(dismissScript)
			.Forget();
	}
	public void CancelPendingDismiss()
	{
		if (autoDismissRoutine == null)
			return;

		StopCoroutine(autoDismissRoutine);
		autoDismissRoutine = null;
	}

	public void ResetGalleryState()
	{
		CancelPendingDismiss();

		clickCounter = 0;

		for (int i = 0; i < StationNames.Length; i++)
		{
			PlayerPrefs.DeleteKey(
				$"NT_StationTalkVariant_{i}"
			);
		}

		PlayerPrefs.Save();

		Debug.Log(
			"[YuinaTalkGallery] Rotación y variantes reiniciadas."
		);
	}

	private static void SetVariable(
		ICustomVariableManager variables,
		string variableName,
		string value
	)
	{
		variables.SetVariableValue(
			variableName,
			new CustomVariableValue(value)
		);
	}

	private void OnDisable()
	{
		CancelPendingDismiss();
	}
}