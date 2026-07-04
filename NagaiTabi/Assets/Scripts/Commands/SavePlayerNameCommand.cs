using System;
using Naninovel;
using UnityEngine;

namespace NagaiTabi.Commands
{
	/// <summary>
	/// Comando @savePlayerName : persiste el nombre capturado con @input en el JSON propio.
	///
	/// Uso en Entry.nani, justo después de capturar el nombre:
	///   @input PlayerName summary:"Escribe tu nombre." type:string
	///   @savePlayerName
	///
	/// Así el nombre queda guardado en TrackerData (JSON) en el momento de escribirlo,
	/// sin depender de abrir el panel Profile. El botón Continue lo recuperará de ahí.
	/// </summary>
	[Serializable, Alias("savePlayerName")]
	public class SavePlayerName : Command
	{
		public override Awaitable Execute(ExecutionContext ctx)
		{
			// Lee la variable PlayerName del sistema de variables de Naninovel.
			var vars = Engine.GetService<ICustomVariableManager>();
			string playerName = vars.GetVariableValue("PlayerName").String;

			if (string.IsNullOrWhiteSpace(playerName))
			{
				Debug.LogWarning("[SavePlayerName] PlayerName está vacío; no se guarda nada.");
				return Async.Completed;
			}

			var trackerManager = UnityEngine.Object.FindFirstObjectByType<TrackerManager>();
			if (trackerManager != null)
			{
				trackerManager.SetPlayerName(playerName);
				Debug.Log($"[SavePlayerName] Nombre guardado en JSON: '{playerName}'");
			}
			else
			{
				Debug.LogWarning("[SavePlayerName] No se encontró TrackerManager en la escena.");
			}

			return Async.Completed;
		}
	}
}
