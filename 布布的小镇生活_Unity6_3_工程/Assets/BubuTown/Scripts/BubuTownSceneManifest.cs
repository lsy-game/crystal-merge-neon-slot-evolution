using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSceneManifest : MonoBehaviour
    {
        [TextArea(2, 6)] public string ProjectSummary;
        [TextArea(2, 6)] public string MvpGoal;
        [TextArea(3, 10)] public string CoreLoop;
        public string UnityVersionPolicy = "Develop in Unity 2022.3.62 first. Test Unity 6.3 upgrades only on a copied project.";
        public string PublicAssetPolicy = "Public builds use original cute characters. Local private skins stay out of Git.";
        public string[] PriorityQuestIds;
    }
}
