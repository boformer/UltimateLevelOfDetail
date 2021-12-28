using System;
using System.Reflection;
using ColossalFramework.UI;
using CitiesHarmony.API;
using ICities;
using UnityEngine;

namespace TrueLodToggler
{
    public enum LodTogglerMode
    {
        Normal,
        Max,
        Lod,
        Vanilla
    }

    public class TrueLodTogglerMod : IUserMod
    {
        public string Name => "Ultimate Level Of Detail";
        public string Description => "Increase the LOD distance of trees, buildings and networks";

        public static TrueLodTogglerConfiguration ActiveConfig
        {
            get
            {
                if (Mode == LodTogglerMode.Max)
                {
                    return TrueLodTogglerConfiguration.MaxConfig;
                }
                else if (Mode == LodTogglerMode.Lod)
                {
                    return TrueLodTogglerConfiguration.LodConfig;
                }
                else if (Mode == LodTogglerMode.Vanilla)
                {
                    return TrueLodTogglerConfiguration.VanillaConfig;
                }
                else
                {
                    return Config;
                }
            }
        }

        public static TrueLodTogglerConfiguration Config;

        public static LodTogglerMode Mode = LodTogglerMode.Normal;

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(TrueLodPatcher.PatchAll);
            Config = Configuration<TrueLodTogglerConfiguration>.Load();
        }

        public void OnDisabled()
        {
            Config = null;
            if(HarmonyHelper.IsHarmonyInstalled) {
                TrueLodPatcher.UnpatchAll();
            }
        }

        public void OnSettingsUI(UIHelperBase helper) => TrueLodTogglerSettings.OnSettingsUI(helper);

        public static void UpdateFreeCameraButton()
        {
            var freeCameraButton = GameObject.Find("Freecamera").GetComponent<UIButton>();
            if (freeCameraButton != null)
            {
                if (!Config.FreeCameraButtonDisplay)
                {
                    freeCameraButton.color = new Color32(255, 255, 255, 255);
                    if (freeCameraButton.tooltip.Contains("ULOD"))
                    {
                        freeCameraButton.tooltip = null;
                    }
                }
                else if (Mode == LodTogglerMode.Max)
                {
                    freeCameraButton.color = new Color32(255, 128, 128, 255);
                    freeCameraButton.tooltip = "ULOD Screenshot \nCTRL + [.] to leave";
                }
                else if (Mode == LodTogglerMode.Lod)
                {
                    freeCameraButton.color = new Color32(128, 128, 255, 255);
                    freeCameraButton.tooltip = "ULOD LOD\nCTRL + [.] to leave";
                }
                else if (Mode == LodTogglerMode.Vanilla)
                {
                    freeCameraButton.color = new Color32(255, 255, 128, 255);
                    freeCameraButton.tooltip = "ULOD Vanilla\nCTRL + [.] to leave";
                }
                else
                {
                    freeCameraButton.color = new Color32(255, 255, 255, 255);
                    freeCameraButton.tooltip = "Default ULOD\n" +
                                               "CTRL + [.] for Screenshot Mode\n" +
                                               "CTRL + SHIFT + [.] for Vanilla Mode\n" +
                                               "CTRL + ALT + [.] for LOD Mode";
                }
            }
        }
    }

    public class Loading : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            if (TrueLodTogglerMod.Config.VanillaModeOnStartup)
            {
                TrueLodTogglerMod.Mode = LodTogglerMode.Vanilla;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            TrueLodTogglerMod.UpdateFreeCameraButton();
            LodUpdater.UpdateShadowDistance();
        }
    }

    public class KeyInputThreading : ThreadingExtensionBase
    {
        private bool _processed = false;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            var control = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
            if (control && Input.GetKey(KeyCode.Period))
            {
                if (_processed) return;

                if (TrueLodTogglerMod.Mode != LodTogglerMode.Normal)
                {
                    TrueLodTogglerMod.Mode = LodTogglerMode.Normal;
                }
                else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    TrueLodTogglerMod.Mode = LodTogglerMode.Lod;
                }
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    TrueLodTogglerMod.Mode = LodTogglerMode.Vanilla;
                }
                else
                {
                    TrueLodTogglerMod.Mode = LodTogglerMode.Max;
                }

                LodUpdater.UpdateTrees();
                LodUpdater.UpdateProps();
                LodUpdater.UpdateBuildings();
                LodUpdater.UpdateNetworks();
                LodUpdater.UpdateVehicles();
                LodUpdater.UpdateShadowDistance();

                TrueLodTogglerMod.UpdateFreeCameraButton();

                _processed = true;
            }
            else
            {
                // not both keys pressed: Reset processed state
                _processed = false;
            }
        }
    }
}
