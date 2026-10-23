using System.Collections.Generic;
using System.Numerics;
using Celeste.Core.Maps;
using Celeste.Core.Reflection.Runtime;
using Celeste.Core.Scheduling;
using Celeste.Everest.Entities.Birds.Tutorials;
using Celeste.Everest.Modules.Entities.Metadata;
using Celeste.Game.Content.Dialogs;
using Celeste.Game.Content.Maps.Entities;
using Celeste.Game.Content.Maps.Levels;
using Celeste.Game.Entities.Birds;
using Celeste.Game.Execution.Levels;
using Celeste.Game.Execution.Startup;
using Celeste.Scene.Entities;
using Celeste.Scene.Tracking.Attributes;
using Celeste.Simulation.Presentation.Hud.Tutorials.Birds;

namespace Celeste.Mods.SpringCollab2020.Entities.Birds
{
    [CustomEntity("SpringCollab2020/CustomBirdTutorial")]
    [SceneTracked(false)]
    [RuntimeTypeAlias("Celeste.Mod.SpringCollab2020.Entities.CustomBirdTutorial")]
    public sealed class CustomBirdTutorial : BirdNpcState
    {
        public string BirdId;
        private readonly bool onlyOnce;
        private readonly bool caw;
        private bool triggered;
        private bool flewAway;
        private readonly BirdTutorialGuiState gui;

        private static readonly Dictionary<string, Vector2> directions = new()
        {
            ["Left"] = new(-1, 0), ["Right"] = new(1, 0), ["Up"] = new(0, -1), ["Down"] = new(0, 1),
            ["UpLeft"] = new(-1, -1), ["UpRight"] = new(1, -1), ["DownLeft"] = new(-1, 1), ["DownRight"] = new(1, 1)
        };

        private CustomBirdTutorial(OriginalLevelSceneRoot level, OriginalLevelEntityData data, Vector2 offset)
            : base(level, data, offset, new MapEntityId(data.Level.Name, data.ID))
        {
            BirdId = data.Attr("birdId", "");
            onlyOnce = data.Bool("onlyOnce", false);
            caw = data.Bool("caw", false);
            Facing = data.Bool("faceLeft", false) ? -1 : 1;
            var process = (IOriginalBirdProcess)EngineContext;
            var text = ((IOriginalGameTextEnvironment)EngineContext).Text;
            string infoKey = data.Attr("info", "");
            object info = process.GuiAtlas.Has(infoKey) ? process.GuiAtlas[infoKey] : (object)text.CleanDialog(infoKey);
            int spacing = 0;
            string[] commands = data.Attr("controls", "").Split(',');
            object[] controls = new object[commands.Length];
            for (int i = 0; i < controls.Length; i++)
            {
                string command = commands[i];
                if (process.GuiAtlas.Has(command))
                {
                    controls[i] = process.GuiAtlas[command];
                    continue;
                }

                if (directions.ContainsKey(command))
                {
                    controls[i] = directions[command];
                    continue;
                }

                var button = ((IEverestCustomBirdTutorialEnvironment)EngineContext).ReadCustomBirdInputField(command);
                if (button != null)
                {
                    controls[i] = button;
                    continue;
                }

                spacing++;
                if (i == 0) spacing++;
                controls[i] = command.StartsWith("dialog:") ? text.CleanDialog(command.Substring(7)) : command;
            }

            gui = new BirdTutorialGuiState(EngineContext, ((IOriginalGameTagsEnvironment)EngineContext).Tags.HUD.Value,
                process.BirdTutorialResources, this, new Vector2(0, -16), info, controls);
            gui.ApplyCustomTutorialSpacing(string.IsNullOrEmpty(infoKey), spacing);
        }

        public static SceneEntityState Load(OriginalLevelSceneRoot level, OriginalLevelRoomData room, Vector2 offset,
            OriginalLevelEntityData data) => new CustomBirdTutorial(level, data, offset);

        public void TriggerShowTutorial()
        {
            if (!triggered)
            {
                triggered = true;
                OwnedComponents.Add(new SimulationCoroutine(EngineContext, ShowTutorial(gui, caw)));
            }
        }

        public void TriggerHideTutorial()
        {
            if (triggered && !flewAway)
            {
                flewAway = true;
                OwnedComponents.Add(new SimulationCoroutine(EngineContext, HideTutorial()));
                OwnedComponents.Add(new SimulationCoroutine(EngineContext, StartleAndFlyAway()));
                if (onlyOnce) ((OriginalLevelSceneRoot)Scene).Session.DoNotLoad.Add(EntityID);
            }
        }
    }
}
