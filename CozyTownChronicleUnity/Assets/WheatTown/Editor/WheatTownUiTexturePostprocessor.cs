using UnityEditor;
using UnityEngine;

namespace WheatTown.EditorTools
{
    /// <summary>
    /// Keeps generated UI frames crisp and preserves the alpha edge produced by
    /// Tools/chroma_key_ui.py. These are text-free UI sprites; every label is
    /// rendered independently with TextMeshPro at runtime.
    /// </summary>
    public sealed class WheatTownUiTexturePostprocessor : AssetPostprocessor
    {
        private const string GeneratedUiFolder = "Assets/Resources/WheatTown/generated-ui-v13/";
        private const string RuntimeFieldFolder = "Assets/Resources/WheatTown/field-baked-v14/";

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(GeneratedUiFolder, System.StringComparison.Ordinal) &&
                !assetPath.StartsWith(RuntimeFieldFolder, System.StringComparison.Ordinal))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.maxTextureSize = 4096;

            // Unity 2022 exposes spriteMeshType through TextureImporterSettings,
            // not as a direct TextureImporter property.
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
        }
    }
}
