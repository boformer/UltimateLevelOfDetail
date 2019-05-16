using ICities;

namespace TrueLodToggler
{
    public static class TrueLodTogglerSettings
    {
        public static void OnSettingsUI(UIHelperBase helper)
        {
            var config = Configuration<TrueLodTogglerConfiguration>.Load();

            var group = helper.AddGroup("Ultimate Level Of Detail");

            var treeLodDistanceOptions = new LodDropdownOption[]
            {
                new LodDropdownOption("1m :P", 1f),
                new LodDropdownOption("50m", 50f),
                new LodDropdownOption("100m", 100f),
                new LodDropdownOption("200m", 200f),
                new LodDropdownOption("300m", 300f),
                new LodDropdownOption("425m (Game Default)", 425f),
                new LodDropdownOption("500m", 500f),
                new LodDropdownOption("625m", 625f),
                new LodDropdownOption("750m", 750f),
                new LodDropdownOption("875m", 875f),
                new LodDropdownOption("1000m", 1000f),
                new LodDropdownOption("1250m", 1250f),
                new LodDropdownOption("1500m", 1500f),
                new LodDropdownOption("2000m", 2000f),
                new LodDropdownOption("3000m", 3000f),
                new LodDropdownOption("4000m", 4000f),
                new LodDropdownOption("5000m", 5000f),
                new LodDropdownOption("10000m (Good luck!)", 10000f),
                new LodDropdownOption("100000m (Goodbye!)", 10000f),
            };

            group.AddDropdown(
                "Tree LOD Distance", 
                LodDropdownOption.BuildLabels(treeLodDistanceOptions), 
                LodDropdownOption.FindIndex(treeLodDistanceOptions, config.TreeLodDistance), 
                sel =>
                {
                    // Change config value and save config
                    config.TreeLodDistance = treeLodDistanceOptions[sel].Value;
                    Configuration<TrueLodTogglerConfiguration>.Save();
                    LodUpdater.UpdateTrees();
                });

            var lodDistanceOptions = new LodDropdownOption[]
            {
                new LodDropdownOption("1m :P", 1f),
                new LodDropdownOption("50m", 50f),
                new LodDropdownOption("100m", 100f),
                new LodDropdownOption("200m", 200f),
                new LodDropdownOption("300m", 300f),
                new LodDropdownOption("400m", 400f),
                new LodDropdownOption("500m", 500f),
                new LodDropdownOption("750m", 750f),
                new LodDropdownOption("1000m (Game Default)", 1000f),
                new LodDropdownOption("1125m", 1125f),
                new LodDropdownOption("1250m", 1250f),
                new LodDropdownOption("1500m", 1500f),
                new LodDropdownOption("1750m", 1750f),
                new LodDropdownOption("2000m", 2000f),
                new LodDropdownOption("3000m", 3000f),
                new LodDropdownOption("4000m", 4000f),
                new LodDropdownOption("5000m", 5000f),
                new LodDropdownOption("10000m (Good luck!)", 10000f),
                new LodDropdownOption("100000m (Goodbye!)", 100000f),
            };

            group.AddDropdown(
                "Prop LOD Distance",
                LodDropdownOption.BuildLabels(lodDistanceOptions),
                LodDropdownOption.FindIndex(lodDistanceOptions, config.PropLodDistance),
                sel =>
                {
                    // Change config value and save config
                    config.PropLodDistance = lodDistanceOptions[sel].Value;
                    Configuration<TrueLodTogglerConfiguration>.Save();
                    LodUpdater.UpdateProps();
                });

            var decalFadeDistanceOptions = new LodDropdownOption[]
            {
                new LodDropdownOption("1m :P", 1f),
                new LodDropdownOption("50m", 50f),
                new LodDropdownOption("100m", 100f),
                new LodDropdownOption("200m", 200f),
                new LodDropdownOption("300m", 300f),
                new LodDropdownOption("400m", 400f),
                new LodDropdownOption("500m", 500f),
                new LodDropdownOption("750m", 750f),
                new LodDropdownOption("1000m (Game Default)", 1000f),
                new LodDropdownOption("1250m", 1250f),
                new LodDropdownOption("1500m", 1500f),
                new LodDropdownOption("1750m", 1750f),
                new LodDropdownOption("2000m", 2000f),
                new LodDropdownOption("3000m", 3000f),
                new LodDropdownOption("4000m", 4000f),
                new LodDropdownOption("5000m", 5000f),
                new LodDropdownOption("10000m", 10000f),
                new LodDropdownOption("15000m", 15000f),
                new LodDropdownOption("20000m", 20000f),
                new LodDropdownOption("25000m", 25000f),
                new LodDropdownOption("100000m (Goodbye!)", 100000f),
            };

            group.AddDropdown(
                "Decal Prop Fade Distance",
                LodDropdownOption.BuildLabels(decalFadeDistanceOptions),
                LodDropdownOption.FindIndex(decalFadeDistanceOptions, config.DecalPropFadeDistance),
                sel =>
                {
                    // Change config value and save config
                    config.DecalPropFadeDistance = decalFadeDistanceOptions[sel].Value;
                    Configuration<TrueLodTogglerConfiguration>.Save();
                    LodUpdater.UpdateProps();
                });

            group.AddDropdown(
                "Building LOD Distance", 
                LodDropdownOption.BuildLabels(lodDistanceOptions), 
                LodDropdownOption.FindIndex(lodDistanceOptions, config.BuildingLodDistance), 
                sel =>
                {
                    // Change config value and save config
                    config.BuildingLodDistance = lodDistanceOptions[sel].Value;
                    Configuration<TrueLodTogglerConfiguration>.Save();
                    LodUpdater.UpdateBuildings();
                });

            group.AddDropdown(
                "Network LOD Distance",
                LodDropdownOption.BuildLabels(lodDistanceOptions),
                LodDropdownOption.FindIndex(lodDistanceOptions, config.NetworkLodDistance),
                sel =>
                {
                    // Change config value and save config
                    config.NetworkLodDistance = lodDistanceOptions[sel].Value;
                    Configuration<TrueLodTogglerConfiguration>.Save();
                    LodUpdater.UpdateNetworks();
                });

            group.AddCheckbox("Disable on startup (press CTRL + SHIFT + [.] to enable)", config.VanillaModeOnStartup, sel =>
            {
                config.VanillaModeOnStartup = sel;
                Configuration<TrueLodTogglerConfiguration>.Save();
            });

            group.AddCheckbox("Display current ULOD mode in Free Camera Button", config.FreeCameraButtonDisplay, sel =>
            {
                config.FreeCameraButtonDisplay = sel;
                Configuration<TrueLodTogglerConfiguration>.Save();
                TrueLodTogglerMod.UpdateFreeCameraButton();
            });

            if (RenderManager.LevelOfDetailFactor < 1.4)
            {
                helper.AddGroup("Warning: Set your 'Level Of Detail' graphics option to 'Very High' for good results!");
            }
        }
    }

    internal class LodDropdownOption
    {
        public LodDropdownOption(string label, float value)
        {
            this.Label = label;
            this.Value = value;
        }

        public readonly string Label;
        public readonly float Value;

        public static int FindIndex(LodDropdownOption[] options, float value)
        {
            for (var i = 0; i < options.Length; i++)
            {
                if (options[i].Value == value)
                {
                    return i;
                }
            }
            return 0;
        }

        public static string[] BuildLabels(LodDropdownOption[] options)
        {
            var labels = new string[options.Length];
            for (var i = 0; i < options.Length; i++)
            {
                labels[i] = options[i].Label;
            }

            return labels;
        }
    }
}
