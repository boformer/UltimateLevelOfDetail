using System;
using System.Reflection;
using Harmony;
using ICities;
using UnityEngine;

namespace TrueLodToggler
{
    public enum LodTogglerMode
    {
        Normal,
        Max,
        Lod,
    }

    public class TrueLodTogglerMod : IUserMod
    {
        public string Name => "Ultimate Level Of Detail";
        public string Description => "Increase the LOD distance of trees, buildings and networks";

        private const string HarmonyId = "boformer.TrueLodToggler";
        private HarmonyInstance _harmony;

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
                else
                {
                    return _config;
                }
            }
        }

        private static TrueLodTogglerConfiguration _config;

        public static LodTogglerMode Mode = LodTogglerMode.Normal;

        public void OnEnabled()
        {
            if (_harmony != null) return;

            _harmony = HarmonyInstance.Create(HarmonyId);
            _harmony.PatchAll(GetType().Assembly);

            _config = Configuration<TrueLodTogglerConfiguration>.Load();
        }

        public void OnDisabled()
        {
            _config = null;

            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;
        }

        public void OnSettingsUI(UIHelperBase helper) => TrueLodTogglerSettings.OnSettingsUI(helper);
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
                else
                {
                    TrueLodTogglerMod.Mode = LodTogglerMode.Max;
                }

                LodUpdater.UpdateTrees();
                LodUpdater.UpdateProps();
                LodUpdater.UpdateBuildings();
                LodUpdater.UpdateNetworks();
                _processed = true;
            }
            else
            {
                // not both keys pressed: Reset processed state
                _processed = false;
            }
        }
    }


    #region Trees
    [HarmonyPatch(typeof(TreeManager), "PopulateGroupData")]
    public static class TreeManagerPopulateGroupDataPatch
    {
        public static void Postfix(int layer, ref float maxInstanceDistance)
        {
            if (layer == TreeManager.instance.m_treeLayer)
            {
                maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.TreeLodDistance);
            }
        }
    }


    [HarmonyPatch(typeof(TreeInfo), "RefreshLevelOfDetail")]
    public static class TreeInfoRefreshLevelOfDetailPatch
    {
        public static void Postfix(TreeInfo __instance)
        {
            __instance.m_lodRenderDistance = TrueLodTogglerMod.ActiveConfig.TreeLodDistance;
        }
    }

    [HarmonyPatch]
    public static class ApvTreeInfoDetourPatch
    {
        public static bool Prepare()
        {
            try
            {
                return TargetMethod() != null;
            }
            catch
            {
                return false;
            }
        }

        public static MethodBase TargetMethod() => Type.GetType("AdaptivePropVisibilityDistance.Detours.TreeInfoDetour, AdaptivePropVisibilityDistance")
            ?.GetMethod("RefreshLevelOfDetail");

        public static void Postfix(TreeInfo __instance)
        {
            var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.TreeLodDistance / 425f;
            __instance.m_lodRenderDistance *= lodTogglerFactor;
        }
    }
    #endregion

    #region Props
    [HarmonyPatch(typeof(PropInfo), "RefreshLevelOfDetail")]
    public static class PropInfoRefreshLevelOfDetailPatch
    {
        public static void Postfix(PropInfo __instance)
        {
            var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.PropLodDistance / 1000f;

            __instance.m_lodRenderDistance *= lodTogglerFactor;
            __instance.m_maxRenderDistance *= lodTogglerFactor;
        }
    }

    [HarmonyPatch]
    public static class ApvPropInfoDetourPatch
    {
        public static bool Prepare()
        {
            try
            {
                return TargetMethod() != null;
            }
            catch
            {
                return false;
            }
        }

        public static MethodBase TargetMethod() => Type.GetType("AdaptivePropVisibilityDistance.Detours.PropInfoDetour, AdaptivePropVisibilityDistance")
            ?.GetMethod("RefreshLevelOfDetail");

        public static void Postfix(PropInfo __instance)
        {
            var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.PropLodDistance / 1000f;
            __instance.m_lodRenderDistance *= lodTogglerFactor;
            __instance.m_maxRenderDistance *= lodTogglerFactor;
        }
    }
    #endregion

    #region Buildings
    [HarmonyPatch(typeof(BuildingAI), "PopulateGroupData")]
    public static class BuildingAiPopulateGroupDataPatch
    {
        public static void Postfix(BuildingAI __instance, int layer, ref float maxInstanceDistance)
        {
            if (__instance.m_info.m_prefabDataLayer == layer)
            {
                maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.BuildingLodDistance);
            }
        }
    }

    [HarmonyPatch(typeof(BuildingInfo), "RefreshLevelOfDetail", new Type[] {})]
    public static class BuildingInfoRefreshLevelOfDetailPatch
    {
        public static void Postfix(BuildingInfo __instance)
        {
            if (__instance.m_lodMesh != null)
            {
                var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.BuildingLodDistance / 1000f;
                __instance.m_minLodDistance *= lodTogglerFactor;
                __instance.m_maxLodDistance *= lodTogglerFactor;
            }
        }
    }

    [HarmonyPatch(typeof(BuildingInfoSub), "RefreshLevelOfDetail", new Type[] { })]
    public static class BuildingInfoSubRefreshLevelOfDetailPatch
    {
        public static void Postfix(BuildingInfoSub __instance)
        {
            if (__instance.m_lodMesh != null)
            {
                var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.BuildingLodDistance / 1000f;
                __instance.m_minLodDistance *= lodTogglerFactor;
                __instance.m_maxLodDistance *= lodTogglerFactor;
            }
        }
    }
    #endregion

    #region Networks
    [HarmonyPatch(typeof(NetManager), "PopulateGroupData")]
    public static class NetManagerPopulateGroupDataPatch
    {
        public static void Postfix(int layer, ref float maxInstanceDistance)
        {
            if (layer == LayerMask.NameToLayer("Road"))
            {
                maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.NetworkLodDistance);
            }
        }
    }

    [HarmonyPatch(typeof(NetInfo), "RefreshLevelOfDetail")]
    public static class NetInfoRefreshLevelOfDetailPatch
    {
        public static void Postfix(NetInfo __instance)
        {
            var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.NetworkLodDistance / 1000f;
            if (__instance.m_segments != null)
            {
                for (int i = 0; i < __instance.m_segments.Length; i++)
                {
                    if (__instance.m_segments[i].m_lodMesh != null)
                    {
                        __instance.m_segments[i].m_lodRenderDistance *= lodTogglerFactor;
                    }
                }
            }
            if (__instance.m_nodes != null)
            {
                for (int j = 0; j < __instance.m_nodes.Length; j++)
                {
                    if (__instance.m_nodes[j].m_lodMesh != null)
                    {
                        __instance.m_nodes[j].m_lodRenderDistance *= lodTogglerFactor;
                    }
                }
            }
            __instance.m_maxPropDistance *= lodTogglerFactor;
        }
    }
    #endregion
}
