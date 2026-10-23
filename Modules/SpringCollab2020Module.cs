using Celeste.Core.Reflection.Runtime;
using Celeste.Core.Scheduling;
using Celeste.Everest.Modules.Runtime;

namespace Celeste.Mods.SpringCollab2020.Modules
{
    [RuntimeTypeAlias("Celeste.Mod.SpringCollab2020.SpringCollab2020Module")]
    public sealed class SpringCollab2020Module : SourceEverestModule
    {
        public SpringCollab2020Module(ISourceEngineContext engine) : base(engine)
        {
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }
    }
}
