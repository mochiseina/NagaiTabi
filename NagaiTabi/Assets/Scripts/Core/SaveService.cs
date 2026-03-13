using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour
{
	private string SavePath => Path.Combine(Application.persistentDataPath, "tracker-data.json");

	public void Save(TrackerData data)
	{
		var json = JsonUtility.ToJson(data, true);
		File.WriteAllText(SavePath, json);
	}
	public TrackerData Load()
	{
		if (!File.Exists(SavePath))
			return new TrackerData();

		var json = File.ReadAllText(SavePath);
		var data = JsonUtility.FromJson<TrackerData>(json);

		return data ?? new TrackerData();
	}
}