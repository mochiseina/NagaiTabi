using System;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
	public event Action<ImmersionEntry> OnEntryLogged;

	[SerializeField] private SaveService saveService;

	public TrackerData Data { get; private set; } = new TrackerData();

	private void Awake()
	{
		if (saveService != null)
		{
			Data = saveService.Load();
			Debug.Log($"[TrackerManager] Datos cargados. Entradas: {Data.entries.Count}, nombre: '{Data.playerName}'");
		}
		else
		{
			Debug.LogWarning("[TrackerManager] SaveService no está asignado.");
		}
	}

	public void AddEntry(int minutes, string mode = "Listening", string mediaType = "",
						 string title = "", int chars = 0, string dateOverride = null)
	{
		string dateIso = string.IsNullOrWhiteSpace(dateOverride)
			? DateTime.Now.ToString("o")
			: dateOverride;

		var newEntry = new ImmersionEntry
		{
			entryId = Guid.NewGuid().ToString(),
			dateIso = dateIso,
			minutes = minutes,
			mode = mode,
			mediaType = mediaType,
			title = title,
			chars = chars
		};

		Data.entries.Add(newEntry);
		Debug.Log($"[TrackerManager] Añadida entrada de {minutes} min, {chars} chars.");

		Save();

		OnEntryLogged?.Invoke(newEntry);
	}

	public void Save()
	{
		if (saveService != null)
			saveService.Save(Data);
	}

	public void SetPlayerName(string playerName)
	{
		Data.playerName = playerName;
		Save();
		Debug.Log($"[TrackerManager] Nombre guardado en JSON: '{playerName}'");
	}
	public bool HasExistingProfile()
	{
		bool hasName = !string.IsNullOrWhiteSpace(Data.playerName);
		bool hasLogs = Data.entries != null && Data.entries.Count > 0;
		return hasName || hasLogs;
	}

	public int GetTotalMinutes()
	{
		int total = 0;
		foreach (var entry in Data.entries)
			total += entry.minutes;
		return total;
	}

	public void ResetAllData()
	{
		Data = new TrackerData();
		Save();
		Debug.Log("[TrackerManager] Datos reseteados.");
	}
}