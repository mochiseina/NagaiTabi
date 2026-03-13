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
			return;

		if (!int.TryParse(minutesInput.text, out int minutes))
		{
			Debug.LogWarning("No se pudo convertir el input a número.");
			return;
		}
		if (minutes <= 0)
		{
			Debug.LogWarning("Los minutos deben ser mayores que 0.");
			return;
		}
		trackerManager.AddEntry(minutes);
		minutesInput.text = "";

		if (trackerHUD != null)
			trackerHUD.Refresh();
	}
}