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
			Data = saveService.Load();
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

		if (saveService != null)
			saveService.Save(Data);

		OnEntryLogged?.Invoke(newEntry);
	}
	public int GetTotalMinutes()
	{
		int total = 0;

		foreach (var entry in Data.entries)
			total += entry.minutes;

		return total;
	}
}