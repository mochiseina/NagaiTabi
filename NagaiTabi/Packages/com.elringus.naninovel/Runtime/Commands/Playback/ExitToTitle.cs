using System;
using Naninovel.Metadata;
using UnityEngine;

namespace Naninovel.Commands
{
    [Doc(
        @"
Resets the engine state and starts playing 'Title' script (if assigned in the scripts configuration).",
        null,
        @"
; Exit to title.
@title"
    )]
    [Serializable, Alias("title"), PlaybackGroup, Icon("RightFromBracket")]
    [Branch(BranchTraits.Endpoint, endpoint: "{" + Metadata.ExpressionEvaluator.TitleScript + "}")]
    public class ExitToTitle : Command
    {
        public override async Awaitable Execute (ExecutionContext ctx)
        {
            var player = Engine.GetServiceOrErr<IScriptPlayer>();
            var scripts = Engine.GetServiceOrErr<IScriptManager>();
            using (player.Configuration.ShowLoadingUI ? await LoadingScreen.Show() : null)
                await Engine.GetServiceOrErr<IStateManager>().ResetState(async () => {
                    if (!string.IsNullOrEmpty(scripts.Configuration.TitleScript))
                        await player.MainTrack.LoadAndPlay(scripts.Configuration.TitleScript);
                });
        }
    }
}
