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
			Debug.Log($"[TrackerManager] Datos cargados. Entradas: {Data.entries.Count}");
		}
		else
		{
			Debug.LogWarning("[TrackerManager] SaveService no está asignado.");
		}
	}
	public void AddEntry(int minutes, string mode = "Listening", string mediaType = "", string title = "")
	{
		var newEntry = new ImmersionEntry
		{
			entryId = Guid.NewGuid().ToString(),
			dateIso = DateTime.Now.ToString("o"),
			minutes = minutes,
			mode = mode,
			mediaType = mediaType,
			title = title
		};

		Data.entries.Add(newEntry);
		Debug.Log($"[TrackerManager] Añadida entrada de {minutes} min.");

		if (saveService != null)
			saveService.Save(Data);

		Debug.Log("[TrackerManager] Lanzando evento OnEntryLogged...");
		OnEntryLogged?.Invoke(newEntry);
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

		if (saveService != null)
			saveService.Save(Data);

		Debug.Log("[TrackerManager] Datos reseteados.");
	}
}