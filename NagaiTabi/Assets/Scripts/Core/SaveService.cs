using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour
{
	private string SavePath => Path.Combine(Application.persistentDataPath, "tracker-data.json");

	public void Save(TrackerData data)
	{
		var json = JsonUtility.ToJson(data, true);
		File.WriteAllText(SavePath, json);
		Debug.Log($"[SaveService] Guardado en: {SavePath}");
	}
	public TrackerData Load()
	{
		if (!File.Exists(SavePath))
		{
			Debug.Log("[SaveService] No existe archivo de guardado. Se crea TrackerData nuevo.");
			return new TrackerData();
		}

		var json = File.ReadAllText(SavePath);
		var data = JsonUtility.FromJson<TrackerData>(json);

		Debug.Log($"[SaveService] Archivo cargado desde: {SavePath}");
		return data ?? new TrackerData();
	}
	public void DeleteSaveFile()
	{
		if (File.Exists(SavePath))
		{
			File.Delete(SavePath);
			Debug.Log("[SaveService] Archivo de guardado eliminado.");
		}
		else
		{
			Debug.Log("[SaveService] No había archivo para borrar.");
		}
	}
}