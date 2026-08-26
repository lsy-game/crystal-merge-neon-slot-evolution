using UnityEditor;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public sealed class FateWeaverAssetPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/DestinyRanger/Art/") || !assetPath.EndsWith(".png"))
                return;

            var importer = (TextureImporter)assetImporter;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.maxTextureSize = 4096;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = IsStageBackground(assetPath) ? FilterMode.Bilinear : FilterMode.Point;
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            if (!assetPath.StartsWith("Assets/DestinyRanger/Art/") || !assetPath.EndsWith(".png"))
                return;

            if (HasBadOpaqueBorder(texture))
                Debug.LogError("FateWeaverAssetPostprocessor: border is over 90% pure black/white and may show a hard edge: " + assetPath);
        }

        private static bool HasBadOpaqueBorder(Texture2D texture)
        {
            var bad = 0;
            var total = 0;
            for (var x = 0; x < texture.width; x++)
            {
                CountPixel(texture.GetPixel(x, 0), ref bad, ref total);
                CountPixel(texture.GetPixel(x, texture.height - 1), ref bad, ref total);
            }

            for (var y = 1; y < texture.height - 1; y++)
            {
                CountPixel(texture.GetPixel(0, y), ref bad, ref total);
                CountPixel(texture.GetPixel(texture.width - 1, y), ref bad, ref total);
            }

            return total > 0 && bad / (float)total > .9f;
        }

        private static void CountPixel(Color color, ref int bad, ref int total)
        {
            if (color.a <= .95f)
                return;

            total++;
            var pureBlack = color.r < .03f && color.g < .03f && color.b < .03f;
            var pureWhite = color.r > .97f && color.g > .97f && color.b > .97f;
            if (pureBlack || pureWhite)
                bad++;
        }

        private static bool IsStageBackground(string path)
        {
            return path.Contains("stage") || path.Contains("background") || path.Contains("-bg");
        }
    }
}
