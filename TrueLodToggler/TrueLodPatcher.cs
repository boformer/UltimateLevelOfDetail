using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace TrueLodToggler {
    internal static class TrueLodPatcher {
        private const string HarmonyId = "boformer.TrueLodToggler";

        internal static void PatchAll() {
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void UnpatchAll() {
            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);
        }


        #region Trees
        [HarmonyPatch(typeof(TreeManager), "PopulateGroupData")]
        public static class TreeManagerPopulateGroupDataPatch {
            public static void Postfix(int layer, ref float maxInstanceDistance) {
                if (layer == TreeManager.instance.m_treeLayer) {
                    maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.TreeLodDistance);
                }
            }
        }

        // Unlimited Trees
        [HarmonyPatch]
        public static class LimitTreeManagerPopulateGroupDataPatch {
            public static bool Prepare() {
                try {
                    return TargetMethod() != null;
                } catch {
                    return false;
                }
            }

            public static MethodBase TargetMethod() => Type.GetType("TreeUnlimiter.Detours.LimitTreeManager, TreeUnlimiter")
                ?.GetMethod("PopulateGroupData", BindingFlags.NonPublic | BindingFlags.Static);

            public static void Postfix(int layer, ref float maxInstanceDistance) {
                if (layer == TreeManager.instance.m_treeLayer) {
                    maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.TreeLodDistance);
                }
            }
        }


        [HarmonyPatch(typeof(TreeInfo), "RefreshLevelOfDetail")]
        public static class TreeInfoRefreshLevelOfDetailPatch {
            public static void Postfix(TreeInfo __instance) {
                __instance.m_lodRenderDistance = TrueLodTogglerMod.ActiveConfig.TreeLodDistance;
            }
        }

        [HarmonyPatch]
        public static class ApvTreeInfoDetourPatch {
            public static bool Prepare() {
                try {
                    return TargetMethod() != null;
                } catch {
                    return false;
                }
            }

            public static MethodBase TargetMethod() => Type.GetType("AdaptivePropVisibilityDistance.Detours.TreeInfoDetour, AdaptivePropVisibilityDistance")
                ?.GetMethod("RefreshLevelOfDetail");

            public static void Postfix(TreeInfo __instance) {
                var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.TreeLodDistance / 425f;
                __instance.m_lodRenderDistance *= lodTogglerFactor;
            }
        }
        #endregion

        #region Props
        [HarmonyPatch(typeof(PropInfo), "RefreshLevelOfDetail")]
        public static class PropInfoRefreshLevelOfDetailPatch {
            public static void Postfix(PropInfo __instance) {
                if (__instance.m_material != null && __instance.m_material.shader.name == "Custom/Props/Decal/Blend") {
                    // Decals
                    var renderDistance = TrueLodTogglerMod.ActiveConfig.DecalPropFadeDistance;

                    __instance.m_lodRenderDistance = renderDistance;
                    __instance.m_maxRenderDistance = renderDistance;

                    var fadeDistanceFactor = 1f / (renderDistance * renderDistance) * 2.6f;
                    __instance.m_material.SetFloat("_FadeDistanceFactor", fadeDistanceFactor);
                } else {
                    // Normal props
                    var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.PropLodDistance / 1000f;
                    __instance.m_lodRenderDistance *= lodTogglerFactor;
                    __instance.m_maxRenderDistance *= lodTogglerFactor;
                }
            }
        }

        [HarmonyPatch]
        public static class ApvPropInfoDetourPatch {
            public static bool Prepare() {
                try {
                    return TargetMethod() != null;
                } catch {
                    return false;
                }
            }

            public static MethodBase TargetMethod() => Type.GetType("AdaptivePropVisibilityDistance.Detours.PropInfoDetour, AdaptivePropVisibilityDistance")
                ?.GetMethod("RefreshLevelOfDetail");

            public static void Postfix(PropInfo __instance) {

                if (__instance.m_material != null && __instance.m_material.shader.name == "Custom/Props/Decal/Blend") {
                    // Decals
                    var renderDistance = TrueLodTogglerMod.ActiveConfig.DecalPropFadeDistance;

                    __instance.m_lodRenderDistance = renderDistance;
                    __instance.m_maxRenderDistance = renderDistance;

                    var fadeDistanceFactor = 1f / (renderDistance * renderDistance) * 2.6f;
                    __instance.m_material.SetFloat("_FadeDistanceFactor", fadeDistanceFactor);
                } else {
                    // Normal props
                    var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.PropLodDistance / 1000f;
                    __instance.m_lodRenderDistance *= lodTogglerFactor;
                    __instance.m_maxRenderDistance *= lodTogglerFactor;
                }

            }
        }
        #endregion

        #region Buildings
        [HarmonyPatch(typeof(BuildingAI), "PopulateGroupData")]
        public static class BuildingAiPopulateGroupDataPatch {
            public static void Postfix(BuildingAI __instance, int layer, ref float maxInstanceDistance) {
                if (__instance.m_info.m_prefabDataLayer == layer) {
                    maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.BuildingLodDistance);
                }
            }
        }

        [HarmonyPatch(typeof(BuildingInfo), "RefreshLevelOfDetail", new Type[] { })]
        public static class BuildingInfoRefreshLevelOfDetailPatch {
            public static void Postfix(BuildingInfo __instance) {
                if (__instance.m_lodMesh != null) {
                    var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.BuildingLodDistance / 1000f;
                    __instance.m_minLodDistance *= lodTogglerFactor;
                    __instance.m_maxLodDistance *= lodTogglerFactor;
                }
            }
        }

        [HarmonyPatch(typeof(BuildingInfoSub), "RefreshLevelOfDetail", new Type[] { })]
        public static class BuildingInfoSubRefreshLevelOfDetailPatch {
            public static void Postfix(BuildingInfoSub __instance) {
                if (__instance.m_lodMesh != null) {
                    var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.BuildingLodDistance / 1000f;
                    __instance.m_minLodDistance *= lodTogglerFactor;
                    __instance.m_maxLodDistance *= lodTogglerFactor;
                }
            }
        }
        #endregion

        #region Networks
        [HarmonyPatch(typeof(NetManager), "PopulateGroupData")]
        public static class NetManagerPopulateGroupDataPatch {
            public static void Postfix(int layer, ref float maxInstanceDistance) {
                if (layer == LayerMask.NameToLayer("Road")) {
                    maxInstanceDistance = Mathf.Max(maxInstanceDistance, TrueLodTogglerMod.ActiveConfig.NetworkLodDistance);
                }
            }
        }

        [HarmonyPatch(typeof(NetInfo), "RefreshLevelOfDetail")]
        public static class NetInfoRefreshLevelOfDetailPatch {
            public static void Postfix(NetInfo __instance) {
                var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.NetworkLodDistance / 1000f;
                if (__instance.m_segments != null) {
                    for (int i = 0; i < __instance.m_segments.Length; i++) {
                        if (__instance.m_segments[i].m_lodMesh != null) {
                            __instance.m_segments[i].m_lodRenderDistance *= lodTogglerFactor;
                        }
                    }
                }
                if (__instance.m_nodes != null) {
                    for (int j = 0; j < __instance.m_nodes.Length; j++) {
                        if (__instance.m_nodes[j].m_lodMesh != null) {
                            __instance.m_nodes[j].m_lodRenderDistance *= lodTogglerFactor;
                        }
                    }
                }
                __instance.m_maxPropDistance *= lodTogglerFactor;
            }
        }
        #endregion

        #region Vehicles
        [HarmonyPatch(typeof(VehicleInfo), "RefreshLevelOfDetail", new Type[0])]
        public static class VehicleInfoRefreshLevelOfDetailPatch {
            public static void Postfix(VehicleInfo __instance) {
                UnityEngine.Debug.Log($"{__instance.name} {TrueLodTogglerMod.ActiveConfig.VehicleLodDistance} {TrueLodTogglerMod.ActiveConfig.VehicleRenderDistance}");
                __instance.m_lodRenderDistance *= (TrueLodTogglerMod.ActiveConfig.VehicleLodDistance / 400f);
                __instance.m_maxRenderDistance *= (TrueLodTogglerMod.ActiveConfig.VehicleRenderDistance / 2000f);
            }
        }

        [HarmonyPatch(typeof(VehicleInfoSub), "RefreshLevelOfDetail", new Type[0])]
        public static class VehicleInfoSubRefreshLevelOfDetailPatch {
            public static void Postfix(VehicleInfoSub __instance) {
                __instance.m_lodRenderDistance *= (TrueLodTogglerMod.ActiveConfig.VehicleLodDistance / 400f);
                __instance.m_maxRenderDistance *= (TrueLodTogglerMod.ActiveConfig.VehicleRenderDistance / 2000f);
            }
        }
        #endregion
    }
}
