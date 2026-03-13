using TMPro;
using UnityEngine;

public class LogInputPanel : MonoBehaviour
{
	[SerializeField] private TMP_InputField minutesInput;
	[SerializeField] private TrackerManager trackerManager;
	[SerializeField] private TrackerHUD trackerHUD;

	public void SubmitLog()
	{
		if (minutesInput == null || trackerManager == null)
		{
			Debug.LogWarning("[LogInputPanel] Falta minutesInput o trackerManager.");
			return;
		}

		Debug.Log($"[LogInputPanel] Texto recibido: '{minutesInput.text}'");

		if (!int.TryParse(minutesInput.text, out int minutes))
		{
			Debug.LogWarning("[LogInputPanel] No se pudo convertir el input a número.");
			return;
		}

		if (minutes <= 0)
		{
			Debug.LogWarning("[LogInputPanel] Los minutos deben ser mayores que 0.");
			return;
		}

		trackerManager.AddEntry(minutes);
		minutesInput.text = "";

		if (trackerHUD != null)
			trackerHUD.Refresh();
	}
}