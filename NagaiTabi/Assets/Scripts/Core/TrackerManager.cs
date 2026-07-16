using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
	public event Action<ImmersionEntry> OnEntryLogged;

	// Evento general para añadir, borrar o resetear datos.
	public event Action OnDataChanged;

	[SerializeField] private SaveService saveService;

	public TrackerData Data { get; private set; } = new TrackerData();

	private void Awake()
	{
		if (saveService != null)
		{
			Data = saveService.Load() ?? new TrackerData();

			Debug.Log(
				$"[TrackerManager] Datos cargados. " +
				$"Entradas: {Data.entries?.Count ?? 0}, " +
				$"nombre: '{Data.playerName}'"
			);
		}
		else
		{
			Debug.LogWarning(
				"[TrackerManager] SaveService no está asignado."
			);
		}

		Data ??= new TrackerData();
		Data.entries ??= new List<ImmersionEntry>();

		// Garantiza que incluso logs antiguos tengan un identificador.
		if (EnsureEntryIds())
			Save();
	}

	private bool EnsureEntryIds()
	{
		bool changed = false;

		for (int i = 0; i < Data.entries.Count; i++)
		{
			ImmersionEntry entry = Data.entries[i];

			if (!string.IsNullOrWhiteSpace(entry.entryId))
				continue;

			entry.entryId = Guid.NewGuid().ToString();
			Data.entries[i] = entry;
			changed = true;
		}

		if (changed)
		{
			Debug.Log(
				"[TrackerManager] Se generaron identificadores " +
				"para logs antiguos."
			);
		}

		return changed;
	}

	public void AddEntry(
		int minutes,
		string mode = "Listening",
		string mediaType = "",
		string title = "",
		int chars = 0,
		string dateOverride = null
	)
	{
		Data.entries ??= new List<ImmersionEntry>();

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

		Debug.Log(
			$"[TrackerManager] Añadida entrada de " +
			$"{minutes} min, {chars} chars."
		);

		Save();

		// Solo las adiciones activan reacciones y anuncios.
		OnEntryLogged?.Invoke(newEntry);

		// Todas las interfaces se actualizan.
		OnDataChanged?.Invoke();
	}

	public bool DeleteEntry(string entryId)
	{
		if (Data?.entries == null)
			return false;

		if (string.IsNullOrWhiteSpace(entryId))
		{
			Debug.LogWarning(
				"[TrackerManager] No se puede borrar un log sin entryId."
			);
			return false;
		}

		int index = Data.entries.FindIndex(
			entry => entry.entryId == entryId
		);

		if (index < 0)
		{
			Debug.LogWarning(
				$"[TrackerManager] No se encontró el log '{entryId}'."
			);
			return false;
		}

		ImmersionEntry removedEntry = Data.entries[index];
		Data.entries.RemoveAt(index);

		Save();
		OnDataChanged?.Invoke();

		Debug.Log(
			$"[TrackerManager] Log eliminado: " +
			$"'{removedEntry.title}', {removedEntry.minutes} min."
		);

		return true;
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

		Debug.Log(
			$"[TrackerManager] Nombre guardado en JSON: '{playerName}'"
		);
	}

	public bool HasExistingProfile()
	{
		bool hasName = !string.IsNullOrWhiteSpace(Data.playerName);
		bool hasLogs = Data.entries != null && Data.entries.Count > 0;

		return hasName || hasLogs;
	}

	public int GetTotalMinutes()
	{
		if (Data?.entries == null)
			return 0;

		int total = 0;

		foreach (ImmersionEntry entry in Data.entries)
			total += entry.minutes;

		return total;
	}

	public void ResetAllData()
	{
		Data = new TrackerData();
		Data.entries ??= new List<ImmersionEntry>();

		Save();
		OnDataChanged?.Invoke();

		Debug.Log("[TrackerManager] Datos reseteados.");
	}
}