namespace TrueLodToggler
{
    [ConfigurationPath("UltimateLod.xml")]
    public class TrueLodTogglerConfiguration
    {
        public float TreeLodDistance { get; set; } = 750f;
        public float PropLodDistance { get; set; } = 1000f;
        public float DecalPropFadeDistance { get; set; } = 1000f;
        public float BuildingLodDistance { get; set; } = 1000f;
        public float NetworkLodDistance { get; set; } = 1000f;
        public float VehicleLodDistance { get; set; } = 400f;
        public float VehicleRenderDistance { get; set; } = 2000f;
        public float ShadowDistance { get; set; } = 10000f;

        public bool VanillaModeOnStartup { get; set; } = false;
        public bool FreeCameraButtonDisplay { get; set; } = true;

        public static TrueLodTogglerConfiguration MaxConfig = new TrueLodTogglerConfiguration
        {
            TreeLodDistance = 100000f,
            PropLodDistance = 100000f,
            DecalPropFadeDistance = 100000f,
            BuildingLodDistance = 100000f,
            NetworkLodDistance = 100000f,
            VehicleLodDistance = 100000f,
            VehicleRenderDistance = 100000f,
            ShadowDistance = 100000f
        };

        public static TrueLodTogglerConfiguration LodConfig = new TrueLodTogglerConfiguration
        {
            TreeLodDistance = 0f,
            PropLodDistance = 0f,
            DecalPropFadeDistance = 0f,
            BuildingLodDistance = 0f,
            NetworkLodDistance = 0f,
            VehicleLodDistance = 0f,
            VehicleRenderDistance = 2000f,
            ShadowDistance = 0f
        };

        public static TrueLodTogglerConfiguration VanillaConfig = new TrueLodTogglerConfiguration
        {
            TreeLodDistance = 425f,
            PropLodDistance = 1000f,
            DecalPropFadeDistance = 1000f,
            BuildingLodDistance = 1000f,
            NetworkLodDistance = 1000f,
            VehicleLodDistance = 400f,
            VehicleRenderDistance = 2000f,
            ShadowDistance = 4000f
        };
    }
}