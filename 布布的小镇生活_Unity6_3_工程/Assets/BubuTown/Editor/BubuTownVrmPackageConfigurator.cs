using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BubuTown.EditorTools
{
    public static class BubuTownVrmPackageConfigurator
    {
        private static readonly string[] RequiredDefines =
        {
            "UNIGLTF_DISABLE_DEFAULT_GLB_IMPORTER",
            "UNIGLTF_DISABLE_DEFAULT_GLTF_IMPORTER"
        };

        [MenuItem("BubuTown/Configure VRM Package Import Safety")]
        public static void ConfigureVrmPackageImportSafety()
        {
            var target = NamedBuildTarget.Standalone;
            var current = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(symbol => !string.IsNullOrWhiteSpace(symbol))
                .ToList();

            var changed = false;
            foreach (var define in RequiredDefines)
            {
                if (current.Contains(define))
                {
                    continue;
                }

                current.Add(define);
                changed = true;
            }

            if (changed)
            {
                PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", current));
                AssetDatabase.SaveAssets();
            }

            Debug.Log("[BubuTown] VRM package import safety defines configured: " + string.Join(", ", RequiredDefines));
        }
    }
}
