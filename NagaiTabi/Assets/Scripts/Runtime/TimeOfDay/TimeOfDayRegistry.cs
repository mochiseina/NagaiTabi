using UnityEngine;

namespace NagaiTabi.Runtime.TimeOfDay
{
	public class TimeOfDayRegistry : MonoBehaviour
	{
		public static LocalTimeOfDayController Instance {get; private set; }

		private void Awake()
		{
			Instance = GetComponent<LocalTimeOfDayController>();
		}
	}
}