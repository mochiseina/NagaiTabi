using System;
using System.Collections;
using UnityEngine;
using Naninovel;

namespace NagaiTabi.Runtime.TimeOfDay
{
    public class TimeOfDayWatcher : MonoBehaviour
    {
        private enum Period { Day, Sunset, Night }

        [SerializeField] private string dayScript = "TOD_Day";
        [SerializeField] private string sunsetScript = "TOD_Sunset";
		[SerializeField] private string nightScript = "TOD_Night";
        [SerializeField] private float checkIntervalSeconds = 30f;
        [SerializeField] private bool startActive = false;

        private Period currentPeriod;
        private bool hasPeriod;
        private bool active;

        public void Activate()
        {
            active = true;
            if (Engine.Initialized)
                ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), force: true);
        }

        public void Deactivate() => active = false;

        private IEnumerator Start()
        {
            while (!Engine.Initialized)
                yield return null;

            if (startActive)
                Activate();

            var wait = new WaitForSeconds(checkIntervalSeconds);
            while (true)
            {
                yield return wait;
                if (active)
                    ApplyPeriod(ResolvePeriod(DateTime.Now.Hour), force: false);
            }
        }

        private Period ResolvePeriod(int hour)
        {
            if (hour >= 6 && hour < 16) return Period.Day;
            if (hour >= 16 && hour < 20) return Period.Sunset;
            return Period.Night;
        }

        private string ScriptFor(Period period) => period switch
        {
            Period.Day => dayScript,
            Period.Sunset => sunsetScript,
            Period.Night => nightScript,
            _ => dayScript
        };

        private void ApplyPeriod(Period period, bool force)
        {
            if (!force && hasPeriod && period == currentPeriod)
                return;

            currentPeriod = period;
            hasPeriod = true;

            if (!Engine.Initialized) return;

            var script = ScriptFor(period);
            var player = Engine.GetService<IScriptPlayer>();
            player.MainTrack.LoadAndPlay(script).Forget();

        }
    }
}
