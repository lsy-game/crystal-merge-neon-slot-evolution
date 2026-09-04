using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownGameState : MonoBehaviour
    {
        public int TownCoins;
        public List<string> ActiveQuestIds = new List<string>();
        public List<string> CompletedQuestIds = new List<string>();
        public List<string> CompletedQuestStepIds = new List<string>();
        public List<string> OwnedFurnitureIds = new List<string>();
        public List<string> PlacedFurnitureIds = new List<string>();
        public List<int> PlacedFurnitureRotations = new List<int>();
        public List<int> PlacedFurnitureCellIndices = new List<int>();
        public List<int> UnlockedWarmthMilestones = new List<int>();
        public List<BubuTownFriendshipRecord> Friendships = new List<BubuTownFriendshipRecord>();
        public int CurrentDay = 1;
        public int HomeWarmthTarget = 12;
        public int DecorationFurnitureCursor;
        public bool DecorationModeEnabled;
        public bool MenuOpen;
        public bool DialogueOpen;
        public bool PhotoModeEnabled;
        public bool HighCameraSensitivity;
        public bool FastRunEnabled;
        public bool HideMinimalHud;
        public string DialogueSpeaker;
        public string DialogueFriendship;
        [TextArea(2, 5)] public string DialogueBody;
        public string DialogueQuestHint;
        [TextArea(2, 4)] public string LastMessage = "欢迎来到布布的小镇。";
        [TextArea(2, 4)] public string InteractionPrompt;

        public bool HasQuest(string questId)
        {
            return ActiveQuestIds.Contains(questId) || CompletedQuestIds.Contains(questId);
        }

        public void AcceptQuest(string questId, string questName)
        {
            if (HasQuest(questId))
            {
                LastMessage = questName + " 已经记录在任务里。";
                return;
            }

            ActiveQuestIds.Add(questId);
            LastMessage = "接到任务：" + questName;
        }

        public void CompleteQuest(BubuTownQuestMarker quest)
        {
            if (quest == null || CompletedQuestIds.Contains(quest.QuestId))
            {
                return;
            }

            if (!ActiveQuestIds.Contains(quest.QuestId))
            {
                ActiveQuestIds.Add(quest.QuestId);
            }

            ActiveQuestIds.Remove(quest.QuestId);
            CompletedQuestIds.Add(quest.QuestId);
            TownCoins += quest.CoinReward;
            var friendshipMessage = AddFavorForQuestGiver(quest.StartsAt, quest.FavorReward);
            LastMessage = "完成任务：" + quest.QuestName + "，获得 " + quest.CoinReward + " 小镇币。" + friendshipMessage;
            SaveState();
        }

        public void AcceptPriorityQuests()
        {
            var accepted = 0;
            foreach (var quest in FindObjectsOfType<BubuTownQuestMarker>())
            {
                if (!IsQuestAvailableFromBoard(quest) || HasQuest(quest.QuestId))
                {
                    continue;
                }

                ActiveQuestIds.Add(quest.QuestId);
                accepted++;
            }

            LastMessage = accepted > 0 ? "公告牌登记了 " + accepted + " 个当前可接任务。" : "公告牌暂时没有新的可接任务。" + BoardUnlockSummary();
        }

        public string BoardUnlockSummary()
        {
            return "\n公告牌解锁: 前 5 个任务立即可接；完成 3 个前置任务开放 Q006/Q007；温馨度 4 开放 Q008；温馨度 12 开放 Q009/Q010。";
        }

        public string QuestNameForId(string questId)
        {
            foreach (var quest in FindObjectsOfType<BubuTownQuestMarker>())
            {
                if (quest.QuestId == questId)
                {
                    return quest.QuestName;
                }
            }

            return questId;
        }

        public bool IsQuestStepComplete(string stepId)
        {
            return CompletedQuestStepIds.Contains(stepId);
        }

        public string ActiveQuestSummary()
        {
            if (ActiveQuestIds.Count == 0)
            {
                return "暂无进行中任务。\n去找 NPC 或公告牌接任务。";
            }

            var lines = new List<string>();
            foreach (var questId in ActiveQuestIds)
            {
                lines.Add("进行中: " + QuestNameForId(questId) + QuestStepProgressSummary(questId));
            }

            return string.Join("\n", lines);
        }

        public string BagSummary()
        {
            if (OwnedFurnitureIds.Count == 0)
            {
                return "背包暂无家具。\n去家具店买第一件家具吧。";
            }

            var lines = new List<string>();
            foreach (var furnitureId in OwnedFurnitureIds)
            {
                var status = PlacedFurnitureIds.Contains(furnitureId) ? "已摆放" : "待摆放";
                lines.Add(FurnitureDisplayNameForId(furnitureId) + " - " + status);
            }

            return "家具背包:\n" + string.Join("\n", lines);
        }

        public string FurnitureCatalogSummary()
        {
            var lines = new List<string>();
            foreach (var furniture in FindObjectsOfType<BubuTownFurnitureMarker>())
            {
                if (!furniture.StartsInShopCatalog)
                {
                    continue;
                }

                var unlock = HomeWarmthScore() >= furniture.RequiredWarmth ? "开放" : "需温馨度 " + furniture.RequiredWarmth;
                lines.Add(furniture.FurnitureName + " " + furniture.Price + "币 +" + furniture.WarmthValue + " " + unlock);
            }

            return lines.Count > 0 ? "家具店目录\n" + string.Join("\n", lines) : "家具店目录\n暂无商品。";
        }

        public string CompletedQuestSummary()
        {
            if (CompletedQuestIds.Count == 0)
            {
                return "暂无已完成任务。";
            }

            return "已完成:\n" + string.Join("\n", CompletedQuestIds);
        }

        private bool IsQuestAvailableFromBoard(BubuTownQuestMarker quest)
        {
            if (quest == null)
            {
                return false;
            }

            if (quest.PriorityForFirstPlayableLoop)
            {
                return true;
            }

            var completedFirstFive = CompletedFirstFiveQuestCount();
            var warmth = HomeWarmthScore();
            switch (quest.QuestId)
            {
                case "Q006":
                case "Q007":
                    return completedFirstFive >= 3;
                case "Q008":
                    return completedFirstFive >= 5 && warmth >= 4;
                case "Q009":
                case "Q010":
                    return completedFirstFive >= 5 && warmth >= 12;
                default:
                    return false;
            }
        }

        private int CompletedFirstFiveQuestCount()
        {
            var count = 0;
            for (var i = 1; i <= 5; i++)
            {
                if (CompletedQuestIds.Contains("Q00" + i))
                {
                    count++;
                }
            }

            return count;
        }

        public bool CompleteQuestStep(BubuTownQuestStepMarker step, BubuTownQuestMarker quest)
        {
            if (step == null)
            {
                return false;
            }

            if (!HasQuest(step.QuestId))
            {
                AcceptQuest(step.QuestId, QuestNameForId(step.QuestId));
            }

            if (CompletedQuestStepIds.Contains(step.StepId))
            {
                LastMessage = step.StepName + " 已经完成。";
                return false;
            }

            if (!string.IsNullOrEmpty(step.RequiredCompletedStepId) && !CompletedQuestStepIds.Contains(step.RequiredCompletedStepId))
            {
                LastMessage = string.IsNullOrEmpty(step.PrerequisiteMissingMessage) ? "还需要先完成前一步：" + step.RequiredCompletedStepId + "。" : step.PrerequisiteMissingMessage;
                return false;
            }

            CompletedQuestStepIds.Add(step.StepId);
            var completed = CompletedStepCount(step.QuestId);
            var required = Mathf.Max(1, step.RequiredStepsForQuest);

            if (step.CompletesQuestOnInteract && completed >= required && quest != null)
            {
                CompleteQuest(quest);
                return true;
            }

            LastMessage = "完成步骤：" + step.StepName + "（" + completed + "/" + required + "）。";
            SaveState();
            return false;
        }

        public string FriendshipSummary()
        {
            EnsureFriendshipRecords();
            var lines = new List<string>();
            foreach (var record in Friendships)
            {
                lines.Add(record.NpcName + ": " + record.Level + " " + record.Label);
            }

            return lines.Count > 0 ? "关系\n" + string.Join("\n", lines) : "关系\n暂无 NPC 记录";
        }

        public BubuTownFriendshipRecord RecordForNpc(BubuTownNpc npc)
        {
            EnsureFriendshipRecords();
            if (npc == null)
            {
                return null;
            }

            foreach (var record in Friendships)
            {
                if (record.NpcObjectName == npc.gameObject.name)
                {
                    return record;
                }
            }

            return null;
        }

        public bool BuyFurniture(BubuTownFurnitureMarker furniture)
        {
            if (furniture == null)
            {
                return false;
            }

            if (OwnedFurnitureIds.Contains(furniture.FurnitureId))
            {
                LastMessage = "已经拥有：" + furniture.FurnitureName;
                return true;
            }

            if (HomeWarmthScore() < furniture.RequiredWarmth)
            {
                LastMessage = furniture.FurnitureName + " 需要小屋温馨度 " + furniture.RequiredWarmth + " 才会开放。";
                return false;
            }

            if (TownCoins < furniture.Price)
            {
                LastMessage = "小镇币不够，" + furniture.FurnitureName + " 需要 " + furniture.Price + "。";
                return false;
            }

            TownCoins -= furniture.Price;
            OwnedFurnitureIds.Add(furniture.FurnitureId);
            LastMessage = "买到了：" + furniture.FurnitureName + "。回小屋摆放后会提升温馨度。";
            SaveState();
            return true;
        }

        public void MarkFurniturePlaced(string furnitureId, int rotationQuarterTurns)
        {
            MarkFurniturePlaced(furnitureId, rotationQuarterTurns, PlacedFurnitureIds.Count);
        }

        public void MarkFurniturePlaced(string furnitureId, int rotationQuarterTurns, int cellIndex)
        {
            if (!PlacedFurnitureIds.Contains(furnitureId))
            {
                PlacedFurnitureIds.Add(furnitureId);
                PlacedFurnitureRotations.Add(Mathf.Clamp(rotationQuarterTurns, 0, 3));
                PlacedFurnitureCellIndices.Add(Mathf.Max(0, cellIndex));
            }

            NormalizeDecorationFurnitureCursor();
            LastMessage = "小屋温馨度提升到 " + HomeWarmthScore() + "/" + HomeWarmthTarget + "。";
            SaveState();
        }

        public int PlacedFurnitureRotationAt(int index)
        {
            return index >= 0 && index < PlacedFurnitureRotations.Count ? PlacedFurnitureRotations[index] : 0;
        }

        public int PlacedFurnitureCellAt(int index)
        {
            return index >= 0 && index < PlacedFurnitureCellIndices.Count ? PlacedFurnitureCellIndices[index] : index;
        }

        public bool IsFurnitureCellOccupied(int cellIndex)
        {
            return PlacedFurnitureCellIndices.Contains(cellIndex);
        }

        public int NextFreeFurnitureCell(int cellCount)
        {
            var max = Mathf.Max(1, cellCount);
            for (var i = 0; i < max; i++)
            {
                if (!PlacedFurnitureCellIndices.Contains(i))
                {
                    return i;
                }
            }

            return Mathf.Clamp(PlacedFurnitureIds.Count, 0, max - 1);
        }

        public int HomeWarmthScore()
        {
            var warmth = 0;
            foreach (var furnitureId in PlacedFurnitureIds)
            {
                warmth += WarmthForFurnitureId(furnitureId);
            }

            return warmth;
        }

        public string HomeProgressSummary()
        {
            var warmth = HomeWarmthScore();
            var status = warmth >= HomeWarmthTarget ? "已达成第一阶段" : "继续购买并摆放家具";
            return "第 " + CurrentDay + " 天\n小屋温馨度: " + warmth + "/" + HomeWarmthTarget + "\n" + status + "\n已摆放家具: " + PlacedFurnitureIds.Count + "\n" + WarmthUnlockSummary();
        }

        public bool IsWarmthMilestoneUnlocked(int warmth)
        {
            return UnlockedWarmthMilestones.Contains(warmth);
        }

        public void UnlockWarmthMilestone(int warmth, string unlockMessage)
        {
            if (UnlockedWarmthMilestones.Contains(warmth))
            {
                return;
            }

            UnlockedWarmthMilestones.Add(warmth);
            LastMessage = unlockMessage;
            SaveState();
        }

        public string WarmthUnlockSummary()
        {
            if (UnlockedWarmthMilestones.Count == 0)
            {
                return "已解锁: 暂无";
            }

            var labels = new List<string>();
            foreach (var milestone in UnlockedWarmthMilestones)
            {
                labels.Add(milestone + "度-" + WarmthMilestoneLabel(milestone));
            }

            return "已解锁: " + string.Join(" / ", labels);
        }

        public void AdvanceToNextDay(string message)
        {
            CurrentDay++;
            DecorationModeEnabled = false;
            MenuOpen = false;
            DialogueOpen = false;
            PhotoModeEnabled = false;
            LastMessage = message + " 现在是第 " + CurrentDay + " 天。";
            SaveState();
        }

        public string NextUnplacedFurniture()
        {
            return SelectedUnplacedFurniture();
        }

        public string SelectedUnplacedFurniture()
        {
            NormalizeDecorationFurnitureCursor();
            var current = 0;
            foreach (var furnitureId in OwnedFurnitureIds)
            {
                if (PlacedFurnitureIds.Contains(furnitureId))
                {
                    continue;
                }

                if (current == DecorationFurnitureCursor)
                {
                    return furnitureId;
                }

                current++;
            }

            return null;
        }

        public void CycleDecorationFurniture(int direction)
        {
            var count = UnplacedFurnitureCount();
            if (count == 0)
            {
                DecorationFurnitureCursor = 0;
                LastMessage = "背包里没有未摆放家具。";
                return;
            }

            DecorationFurnitureCursor = PositiveModulo(DecorationFurnitureCursor + direction, count);
            LastMessage = "选择家具：" + FurnitureDisplayNameForId(SelectedUnplacedFurniture()) + "。";
            SaveState();
        }

        public string DecorationSelectionSummary()
        {
            var selected = SelectedUnplacedFurniture();
            if (string.IsNullOrEmpty(selected))
            {
                return "当前家具: 无\n去家具店购买家具。";
            }

            return "当前家具: " + FurnitureDisplayNameForId(selected) + "\n待摆放: " + UnplacedFurnitureCount() + "\nZ/X 切换家具";
        }

        public void ToggleDecorationMode(bool enabled)
        {
            DecorationModeEnabled = enabled;
            LastMessage = enabled ? "装修模式已打开：靠近装修网格按 E 摆放家具。" : "装修模式已关闭。";
        }

        public void ToggleMenu()
        {
            MenuOpen = !MenuOpen;
            LastMessage = MenuOpen ? "菜单已打开。" : "菜单已关闭。";
        }

        public void OpenDialogue(BubuTownNpc npc, string questHint)
        {
            if (npc == null)
            {
                return;
            }

            DialogueOpen = true;
            MenuOpen = false;
            DialogueSpeaker = npc.NpcName;
            var record = RecordForNpc(npc);
            var friendshipLevel = record != null ? record.Level : npc.FriendshipLevel;
            DialogueFriendship = record != null ? "好感 " + record.Level + ": " + record.Label : "好感 " + npc.FriendshipLevel + ": " + npc.FriendshipLabel;
            DialogueBody = npc.DialogueForFriendshipLevel(friendshipLevel);
            DialogueQuestHint = questHint;
            LastMessage = "正在和 " + npc.NpcName + " 对话。";
        }

        public void CloseDialogue()
        {
            DialogueOpen = false;
            LastMessage = "对话已关闭。";
        }

        public void SaveState()
        {
            PlayerPrefs.SetInt("BubuTown.Coins", TownCoins);
            PlayerPrefs.SetString("BubuTown.ActiveQuests", string.Join("|", ActiveQuestIds));
            PlayerPrefs.SetString("BubuTown.CompletedQuests", string.Join("|", CompletedQuestIds));
            PlayerPrefs.SetString("BubuTown.CompletedQuestSteps", string.Join("|", CompletedQuestStepIds));
            PlayerPrefs.SetString("BubuTown.OwnedFurniture", string.Join("|", OwnedFurnitureIds));
            PlayerPrefs.SetString("BubuTown.PlacedFurniture", string.Join("|", PlacedFurnitureIds));
            PlayerPrefs.SetString("BubuTown.PlacedFurnitureRotations", string.Join("|", PlacedFurnitureRotations));
            PlayerPrefs.SetString("BubuTown.PlacedFurnitureCells", string.Join("|", PlacedFurnitureCellIndices));
            PlayerPrefs.SetString("BubuTown.UnlockedWarmthMilestones", string.Join("|", UnlockedWarmthMilestones));
            PlayerPrefs.SetString("BubuTown.Friendships", SerializeFriendships());
            PlayerPrefs.SetInt("BubuTown.CurrentDay", CurrentDay);
            PlayerPrefs.SetInt("BubuTown.HomeWarmthTarget", HomeWarmthTarget);
            PlayerPrefs.SetInt("BubuTown.DecorationFurnitureCursor", DecorationFurnitureCursor);
            PlayerPrefs.SetInt("BubuTown.DecorationMode", DecorationModeEnabled ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.MenuOpen", MenuOpen ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.DialogueOpen", DialogueOpen ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.PhotoMode", PhotoModeEnabled ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.HighCameraSensitivity", HighCameraSensitivity ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.FastRunEnabled", FastRunEnabled ? 1 : 0);
            PlayerPrefs.SetInt("BubuTown.HideMinimalHud", HideMinimalHud ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void LoadState()
        {
            TownCoins = PlayerPrefs.HasKey("BubuTown.Coins") ? PlayerPrefs.GetInt("BubuTown.Coins", TownCoins) : TownCoins;
            ActiveQuestIds = Split(PlayerPrefs.GetString("BubuTown.ActiveQuests", string.Empty));
            CompletedQuestIds = Split(PlayerPrefs.GetString("BubuTown.CompletedQuests", string.Empty));
            CompletedQuestStepIds = Split(PlayerPrefs.GetString("BubuTown.CompletedQuestSteps", string.Empty));
            OwnedFurnitureIds = Split(PlayerPrefs.GetString("BubuTown.OwnedFurniture", string.Empty));
            PlacedFurnitureIds = Split(PlayerPrefs.GetString("BubuTown.PlacedFurniture", string.Empty));
            PlacedFurnitureRotations = SplitInts(PlayerPrefs.GetString("BubuTown.PlacedFurnitureRotations", string.Empty));
            PlacedFurnitureCellIndices = SplitInts(PlayerPrefs.GetString("BubuTown.PlacedFurnitureCells", string.Empty));
            UnlockedWarmthMilestones = SplitInts(PlayerPrefs.GetString("BubuTown.UnlockedWarmthMilestones", string.Empty));
            if (PlacedFurnitureCellIndices.Count < PlacedFurnitureIds.Count)
            {
                for (var i = PlacedFurnitureCellIndices.Count; i < PlacedFurnitureIds.Count; i++)
                {
                    PlacedFurnitureCellIndices.Add(i);
                }
            }
            LoadFriendships(PlayerPrefs.GetString("BubuTown.Friendships", string.Empty));
            CurrentDay = PlayerPrefs.GetInt("BubuTown.CurrentDay", CurrentDay);
            HomeWarmthTarget = PlayerPrefs.GetInt("BubuTown.HomeWarmthTarget", HomeWarmthTarget);
            DecorationFurnitureCursor = PlayerPrefs.GetInt("BubuTown.DecorationFurnitureCursor", DecorationFurnitureCursor);
            DecorationModeEnabled = PlayerPrefs.GetInt("BubuTown.DecorationMode", 0) == 1;
            MenuOpen = PlayerPrefs.GetInt("BubuTown.MenuOpen", 0) == 1;
            DialogueOpen = PlayerPrefs.GetInt("BubuTown.DialogueOpen", 0) == 1;
            PhotoModeEnabled = PlayerPrefs.GetInt("BubuTown.PhotoMode", 0) == 1;
            HighCameraSensitivity = PlayerPrefs.GetInt("BubuTown.HighCameraSensitivity", 0) == 1;
            FastRunEnabled = PlayerPrefs.GetInt("BubuTown.FastRunEnabled", 0) == 1;
            HideMinimalHud = PlayerPrefs.GetInt("BubuTown.HideMinimalHud", 0) == 1;
            NormalizeDecorationFurnitureCursor();
            EnsureFriendshipRecords();
        }

        public void EnsureFriendshipRecords()
        {
            foreach (var npc in FindObjectsOfType<BubuTownNpc>())
            {
                var existing = FindFriendship(npc.gameObject.name);
                if (existing != null)
                {
                    npc.FriendshipLevel = existing.Level;
                    npc.FriendshipLabel = existing.Label;
                    continue;
                }

                Friendships.Add(new BubuTownFriendshipRecord
                {
                    NpcObjectName = npc.gameObject.name,
                    NpcName = npc.NpcName,
                    Level = npc.FriendshipLevel,
                    Label = FriendshipLabel(npc.FriendshipLevel)
                });
            }
        }

        private string AddFavorForQuestGiver(string npcObjectName, int favorReward)
        {
            if (string.IsNullOrEmpty(npcObjectName) || favorReward <= 0)
            {
                return string.Empty;
            }

            EnsureFriendshipRecords();
            var record = FindFriendship(npcObjectName);
            if (record == null)
            {
                return string.Empty;
            }

            record.Level = Mathf.Clamp(record.Level + favorReward, 0, 3);
            record.Label = FriendshipLabel(record.Level);
            foreach (var npc in FindObjectsOfType<BubuTownNpc>())
            {
                if (npc.gameObject.name == npcObjectName)
                {
                    npc.FriendshipLevel = record.Level;
                    npc.FriendshipLabel = record.Label;
                }
            }

            SaveState();
            return " " + record.NpcName + " 好感提升到 " + record.Level + "（" + record.Label + "）。";
        }

        private BubuTownFriendshipRecord FindFriendship(string npcObjectName)
        {
            foreach (var record in Friendships)
            {
                if (record.NpcObjectName == npcObjectName)
                {
                    return record;
                }
            }

            return null;
        }

        private int CompletedStepCount(string questId)
        {
            var count = 0;
            foreach (var stepId in CompletedQuestStepIds)
            {
                if (stepId.StartsWith(questId.ToLowerInvariant() + "_", StringComparison.Ordinal))
                {
                    count++;
                }
            }

            return count;
        }

        private string QuestStepProgressSummary(string questId)
        {
            var required = 0;
            foreach (var step in FindObjectsOfType<BubuTownQuestStepMarker>())
            {
                if (step.QuestId == questId)
                {
                    required = Mathf.Max(required, step.RequiredStepsForQuest);
                }
            }

            if (required <= 1)
            {
                return string.Empty;
            }

            return " [" + CompletedStepCount(questId) + "/" + required + "]";
        }

        private static string FriendshipLabel(int level)
        {
            switch (Mathf.Clamp(level, 0, 3))
            {
                case 1: return "认识";
                case 2: return "熟悉";
                case 3: return "朋友";
                default: return "陌生";
            }
        }

        private static int WarmthForFurnitureId(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_bed_poor": return 4;
                case "fur_wood_table": return 3;
                case "fur_small_chair": return 2;
                case "fur_basic_rug": return 2;
                case "fur_desk_lamp": return 2;
                case "fur_small_bookcase": return 5;
                case "fur_pink_wallpaper": return 3;
                case "fur_wood_floor": return 3;
                case "fur_cake_decor": return 3;
                case "fur_small_plant": return 3;
                default: return 1;
            }
        }

        private static string WarmthMilestoneLabel(int warmth)
        {
            switch (warmth)
            {
                case 4: return "第一张床";
                case 12: return "小屋成型";
                case 20: return "朋友来访";
                default: return "新阶段";
            }
        }

        private int UnplacedFurnitureCount()
        {
            var count = 0;
            foreach (var furnitureId in OwnedFurnitureIds)
            {
                if (!PlacedFurnitureIds.Contains(furnitureId))
                {
                    count++;
                }
            }

            return count;
        }

        private void NormalizeDecorationFurnitureCursor()
        {
            var count = UnplacedFurnitureCount();
            DecorationFurnitureCursor = count > 0 ? Mathf.Clamp(DecorationFurnitureCursor, 0, count - 1) : 0;
        }

        private static int PositiveModulo(int value, int modulo)
        {
            if (modulo <= 0)
            {
                return 0;
            }

            return (value % modulo + modulo) % modulo;
        }

        private static string FurnitureDisplayNameForId(string furnitureId)
        {
            switch (furnitureId)
            {
                case "fur_bed_poor": return "破旧小床";
                case "fur_wood_table": return "木头桌子";
                case "fur_small_chair": return "小椅子";
                case "fur_basic_rug": return "基础地毯";
                case "fur_desk_lamp": return "台灯";
                case "fur_small_bookcase": return "小书柜";
                case "fur_pink_wallpaper": return "粉色墙纸";
                case "fur_wood_floor": return "木地板";
                case "fur_cake_decor": return "蛋糕装饰摆件";
                case "fur_small_plant": return "小盆栽";
                default: return furnitureId;
            }
        }

        private string SerializeFriendships()
        {
            var parts = new List<string>();
            foreach (var record in Friendships)
            {
                parts.Add(record.NpcObjectName + "," + record.NpcName + "," + record.Level + "," + record.Label);
            }

            return string.Join("|", parts);
        }

        private void LoadFriendships(string value)
        {
            Friendships.Clear();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            foreach (var entry in value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var fields = entry.Split(',');
                if (fields.Length < 4)
                {
                    continue;
                }

                int level;
                int.TryParse(fields[2], out level);
                Friendships.Add(new BubuTownFriendshipRecord
                {
                    NpcObjectName = fields[0],
                    NpcName = fields[1],
                    Level = Mathf.Clamp(level, 0, 3),
                    Label = fields[3]
                });
            }
        }

        private static List<string> Split(string value)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(value))
            {
                return list;
            }

            list.AddRange(value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            return list;
        }

        private static List<int> SplitInts(string value)
        {
            var list = new List<int>();
            if (string.IsNullOrEmpty(value))
            {
                return list;
            }

            foreach (var part in value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int parsed;
                list.Add(int.TryParse(part, out parsed) ? Mathf.Clamp(parsed, 0, 3) : 0);
            }

            return list;
        }
    }
}
