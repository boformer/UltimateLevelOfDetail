namespace TrueLodToggler
{
    [ConfigurationPath("UltimateLod.xml")]
    public class TrueLodTogglerConfiguration
    {
        public float TreeLodDistance { get; set; } = 1000f;
        public float PropLodDistance { get; set; } = 1500f;
        public float BuildingLodDistance { get; set; } = 1500f;
        public float NetworkLodDistance { get; set; } = 1500f;


        public static TrueLodTogglerConfiguration MaxConfig = new TrueLodTogglerConfiguration
        {
            TreeLodDistance = 2500f,
            PropLodDistance = 100000f,
            BuildingLodDistance = 100000f,
            NetworkLodDistance = 100000f,
        };

        public static TrueLodTogglerConfiguration LodConfig = new TrueLodTogglerConfiguration
        {
            TreeLodDistance = 0f,
            PropLodDistance = 0f,
            BuildingLodDistance = 0f,
            NetworkLodDistance = 0f,
        };
    }
}