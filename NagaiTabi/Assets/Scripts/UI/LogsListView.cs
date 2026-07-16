using UnityEngine;

public class LogsListView : MonoBehaviour
{
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private Transform contentRoot;
	[SerializeField] private LogItemView logItemPrefab;
	[SerializeField] private int maxLogsToShow = 3;

	private void OnEnable()
	{
		if (trackerManager != null)
			trackerManager.OnDataChanged += Refresh;

		Refresh();
	}

	private void OnDisable()
	{
		if (trackerManager != null)
			trackerManager.OnDataChanged -= Refresh;
	}

	private void HandleDeleteRequested(string entryId)
	{
		if (trackerManager == null)
			return;

		trackerManager.DeleteEntry(entryId);
	}

	public void Refresh()
	{
		if (
			trackerManager == null ||
			contentRoot == null ||
			logItemPrefab == null
		)
		{
			Debug.LogWarning(
				"[LogsListView] Faltan referencias."
			);
			return;
		}

		// Se separan antes de destruir para que el Layout Group
		// no siga contando objetos pendientes de destrucción.
		for (int i = contentRoot.childCount - 1; i >= 0; i--)
		{
			Transform child = contentRoot.GetChild(i);
			child.SetParent(null);
			Destroy(child.gameObject);
		}

		var entries = trackerManager.Data.entries;

		int startIndex = Mathf.Max(
			0,
			entries.Count - maxLogsToShow
		);

		for (int i = entries.Count - 1; i >= startIndex; i--)
		{
			LogItemView item = Instantiate(
				logItemPrefab,
				contentRoot
			);

			item.Setup(
				entries[i],
				HandleDeleteRequested
			);
		}

		Debug.Log(
			$"[LogsListView] Mostrando " +
			$"{Mathf.Min(entries.Count, maxLogsToShow)} logs."
		);
	}
}