using UnityEngine;

public class LogsListView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private Transform contentRoot;
	[SerializeField] private LogItemView logItemPrefab;
	[SerializeField] private int maxLogsToShow = 3;

	public void Refresh()
	{
		if (trackerManager == null || contentRoot == null || logItemPrefab == null)
		{
			Debug.LogWarning("[LogsListView] Faltan referencias.");
			return;
		}

		for (int i = contentRoot.childCount - 1; i >= 0; i--)
			Destroy(contentRoot.GetChild(i).gameObject);

		var entries = trackerManager.Data.entries;
		int startIndex = Mathf.Max(0, entries.Count - maxLogsToShow);

		for (int i = entries.Count - 1; i >= startIndex; i--)
		{
			var item = Instantiate(logItemPrefab, contentRoot);
			item.Setup(entries[i]);
		}

		Debug.Log($"[LogsListView] Mostrando {Mathf.Min(entries.Count, maxLogsToShow)} logs.");
	}

	private void Start()
	{
		Refresh();
	}
}