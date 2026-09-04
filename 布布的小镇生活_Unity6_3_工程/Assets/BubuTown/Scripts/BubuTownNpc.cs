using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownNpc : MonoBehaviour
    {
        public string NpcId;
        public string NpcName;
        public string Role;
        [Range(0, 3)] public int FriendshipLevel;
        public string FriendshipLabel;
        public string[] QuestIds;
        [TextArea(2, 4)] public string DefaultDialogue;
        [TextArea(2, 4)] public string[] FriendshipDialogues;

        public string DialogueForFriendshipLevel(int level)
        {
            var index = Mathf.Clamp(level, 0, 3);
            if (FriendshipDialogues != null && index < FriendshipDialogues.Length && !string.IsNullOrEmpty(FriendshipDialogues[index]))
            {
                return FriendshipDialogues[index];
            }

            return DefaultDialogue;
        }
    }
}
