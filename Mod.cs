using PulsarModLoader;

namespace ComponentToScrap
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "18107";

        public override string ShortDescription => "Turn components into scrap using the discard button";

        public override string Name => "Component to Scrap";

        public override string ModID => "componenttoscrap";

        public override string HarmonyIdentifier()
        {
            return "id107.componenttoscrap";
        }
    }
}
