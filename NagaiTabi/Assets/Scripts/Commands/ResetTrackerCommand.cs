using System;
using Naninovel;
using UnityEngine;

namespace NagaiTabi.Commands
{
	/// <summary>
	/// Comando @resetTracker : borra todos los datos del tracker (empezar de cero).
	/// Llama a TrackerManager.ResetAllData(). Úsalo en el flujo de New Game tras
	/// confirmar que el jugador quiere empezar una partida nueva.
	///
	/// Uso en NewGameConfirm.nani:
	///   @resetTracker
	///   @goto Entry
	/// </summary>
	[Serializable, Alias("resetTracker")]
	public class ResetTracker : Command
	{
		public override Awaitable Execute(ExecutionContext ctx)
		{
			var trackerManager = UnityEngine.Object.FindFirstObjectByType<TrackerManager>();
			if (trackerManager != null)
			{
				trackerManager.ResetAllData();
				Debug.Log("[ResetTracker] Datos del tracker reseteados para nueva partida.");
			}
			else
			{
				Debug.LogWarning("[ResetTracker] No se encontró TrackerManager en la escena.");
			}

			return Async.Completed;
		}
	}
}
