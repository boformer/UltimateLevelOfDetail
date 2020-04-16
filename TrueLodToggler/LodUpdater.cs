using UnityEngine;

namespace TrueLodToggler
{
    public static class LodUpdater
    {
        public static void UpdateTrees()
        {
            RefreshLevelOfDetail<TreeInfo>();
            UpdateRenderGroups(TreeManager.instance.m_treeLayer);
        }

        public static void UpdateProps()
        {
            RefreshLevelOfDetail<PropInfo>();
            UpdateRenderGroups(LayerMask.NameToLayer("Props"));
        }

        public static void UpdateBuildings()
        {
            RefreshLevelOfDetail<BuildingInfo>();
            UpdateRenderGroups(LayerMask.NameToLayer("Buildings"));
        }

        public static void UpdateNetworks()
        {
            RefreshLevelOfDetail<NetInfo>();
            UpdateRenderGroups(LayerMask.NameToLayer("Road"));
        }

        public static void UpdateVehicles() {
            RefreshLevelOfDetail<VehicleInfo>();
        }

        private static void RefreshLevelOfDetail<T>() where T : PrefabInfo
        {
            var prefabCount = (uint)PrefabCollection<T>.LoadedCount();
            for (var i = 0u; i < prefabCount; i++)
            {
                var prefab = PrefabCollection<T>.GetLoaded(i);
                prefab?.RefreshLevelOfDetail();
            }

            if (ToolsModifierControl.toolController.m_editPrefabInfo is T)
            {
                ToolsModifierControl.toolController.m_editPrefabInfo.RefreshLevelOfDetail();
            }
        }

        private static void UpdateRenderGroups(int layerIndex)
        {
            foreach (var renderGroup in RenderManager.instance.m_groups)
            {
                if (renderGroup == null)
                {
                    continue;
                }

                renderGroup.SetLayerDataDirty(layerIndex);
                renderGroup.UpdateMeshData();
            }
        }
    }
}
