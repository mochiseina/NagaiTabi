using System;
using Naninovel;
using UnityEngine;

namespace NagaiTabi.Commands
{
	/// <summary>
	/// Comando @quitGame : cierra el juego.
	/// En el editor detiene el Play Mode; en build cierra la aplicación.
	/// Se usa en la intro cuando el jugador rechaza el viaje 3 veces (guiño de VN).
	/// </summary>
	[Serializable, Alias("quitGame")]
	public class QuitGame : Command
	{
		public override Awaitable Execute(ExecutionContext ctx)
		{
			Debug.Log("[QuitGame] Cerrando el juego...");

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
			return Async.Completed;
		}
	}
}
