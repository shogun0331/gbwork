#if !GB_CORE

using UnityEditor;

namespace GB
{
    public class GBPackagePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            EditorWindow.GetWindow(typeof(Setup));
            
        }
    }

}

#endif