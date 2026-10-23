using System.Numerics;
using Celeste.Core.Components;
using Celeste.Core.Mathematics.Extrema;
using Celeste.Core.Reflection.Runtime;
using Celeste.Everest.Modules.Entities.Metadata;
using Celeste.Formats.Map.Entities.Structure;
using Celeste.Mods.SpringCollab2020.Entities.Birds;
using Celeste.Scene.Tracking;
using Celeste.Simulation;
using Celeste.Simulation.Gameplay.Triggers.Foundation;
using Celeste.Core.Scheduling;

namespace Celeste.Mods.SpringCollab2020.Triggers.Birds
{
    [CustomEntity("SpringCollab2020/CustomBirdTutorialTrigger")]
    [RuntimeTypeAlias("Celeste.Mod.SpringCollab2020.Triggers.CustomBirdTutorialTrigger")]
    public sealed class CustomBirdTutorialTrigger : SourceTriggerEntityState
    {
        private readonly string birdId;
        private readonly bool showTutorial;

        public CustomBirdTutorialTrigger(ISourceEngineContext engine, EntityData data, Vector2 offset)
            : base(((ISourceEntitySourceDataEnvironment)engine).EntitySourceDataRegistration, data.Position + offset,
                data.Width, data.Height, ((ISourceExtremaEnvironment)engine).ExtremaArithmetic)
        {
            birdId = data.Attr("birdId", "");
            showTutorial = data.Bool("showTutorial", false);
        }

        public override void OnEnter(RoomPlayerSession player)
        {
            base.OnEnter(player);
            CustomBirdTutorial found = null;
            var birds = ((ISourceScene)Scene).SourceScene.Entities.GetTrackedEntities(typeof(CustomBirdTutorial));
            for (int i = 0; i < birds.Count; i++)
            {
                if (birds[i] is CustomBirdTutorial bird && bird.BirdId == birdId)
                {
                    found = bird;
                    break;
                }
            }

            if (found != null)
            {
                if (showTutorial) found.TriggerShowTutorial();
                else found.TriggerHideTutorial();
            }
        }
    }
}
