using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DestinyRanger.EditorTools
{
    public static class FateWeaverAnimationClipBuilder
    {
        private const string FullRoot = "Assets/DestinyRanger/Art/Generated/FateWeaverFull";
        private const string ClipRoot = "Assets/DestinyRanger/Animations/FateWeaver";

        [MenuItem("Destiny Ranger/Art Fusion/Create Fate Weaver Animation Clips")]
        public static void CreateAnimationClips()
        {
            Directory.CreateDirectory(ClipRoot);
            BuildCharacterClips();
            BuildMonsterClips();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Fate Weaver animation clips generated under " + ClipRoot);
        }

        private static void BuildCharacterClips()
        {
            foreach (var character in new[] { "aileen", "grick", "luna" })
            foreach (var action in new[] { "idle", "attack", "hit", "skill", "death" })
                CreateClip(
                    FullRoot + "/Characters/" + character,
                    character + "_" + action + "_*.png",
                    ClipRoot + "/Characters/" + character + "/" + character + "_" + action + ".anim",
                    action == "idle",
                    action == "idle" ? 6f : 10f);
        }

        private static void BuildMonsterClips()
        {
            foreach (var folder in AssetDatabase.GetSubFolders(FullRoot + "/Monsters/Forest"))
            {
                var monster = Path.GetFileName(folder);
                foreach (var action in new[] { "idle", "attack", "hit", "death" })
                    CreateClip(folder, monster + "_" + action + "_*.png", ClipRoot + "/Monsters/Forest/" + monster + "/" + monster + "_" + action + ".anim", action == "idle", action == "idle" ? 5f : 9f);
            }
        }

        private static void CreateClip(string folder, string pattern, string outputPath, bool loop, float frameRate)
        {
            var sprites = Directory.GetFiles(folder, pattern)
                .OrderBy(path => path)
                .Select(path => AssetDatabase.LoadAssetAtPath<Sprite>(path.Replace("\\", "/")))
                .Where(sprite => sprite)
                .ToList();
            if (sprites.Count == 0)
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            var clip = new AnimationClip { frameRate = frameRate };
            var binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = string.Empty,
                propertyName = "m_Sprite"
            };

            var keys = new List<ObjectReferenceKeyframe>();
            for (var i = 0; i < sprites.Count; i++)
                keys.Add(new ObjectReferenceKeyframe { time = i / frameRate, value = sprites[i] });
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys.ToArray());

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, outputPath);
        }
    }
}
