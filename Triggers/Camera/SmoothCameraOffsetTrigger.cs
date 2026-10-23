using System.Numerics;
using Celeste.Core.Components;
using Celeste.Core.Mathematics;
using Celeste.Core.Mathematics.Extrema;
using Celeste.Core.Reflection.Runtime;
using Celeste.Core.Scheduling;
using Celeste.Everest.Modules.Entities.Metadata;
using Celeste.Formats.Map.Entities.Structure;
using Celeste.Game.Execution.Levels;
using Celeste.Simulation;
using Celeste.Simulation.Gameplay.Triggers.Foundation;

namespace Celeste.Mods.SpringCollab2020.Triggers.Camera
{
    [CustomEntity("SpringCollab2020/SmoothCameraOffsetTrigger")]
    [RuntimeTypeAlias("Celeste.Mod.SpringCollab2020.Triggers.SmoothCameraOffsetTrigger")]
    public sealed class SmoothCameraOffsetTrigger : SourceTriggerEntityState
    {
        private readonly ISourceEngineContext engine;
        private readonly Vector2 offsetFrom;
        private readonly Vector2 offsetTo;
        private readonly PositionModes positionMode;
        private readonly bool onlyOnce;

        public SmoothCameraOffsetTrigger(ISourceEngineContext engine, EntityData data, Vector2 offset)
            : base(((ISourceEntitySourceDataEnvironment)engine).EntitySourceDataRegistration,
                data.Position + offset, data.Width, data.Height,
                ((ISourceExtremaEnvironment)engine).ExtremaArithmetic)
        {
            this.engine = engine;
            offsetFrom = new Vector2(data.Float("offsetXFrom") * 48f, data.Float("offsetYFrom") * 32f);
            offsetTo = new Vector2(data.Float("offsetXTo") * 48f, data.Float("offsetYTo") * 32f);
            positionMode = data.Enum("positionMode", PositionModes.NoEffect);
            onlyOnce = data.Bool("onlyOnce");
        }

        public override void OnStay(RoomPlayerSession player)
        {
            base.OnStay(player);
            ((OriginalLevelSceneRoot)Scene).CameraOffset = Vector2Interpolation.Lerp(offsetFrom, offsetTo,
                GetPositionLerp(player, positionMode), ((ISourceScalarEnvironment)engine).ScalarArithmetic);
        }

        public override void OnLeave(RoomPlayerSession player)
        {
            base.OnLeave(player);
            if (onlyOnce) RemoveSelf();
        }
    }
}
