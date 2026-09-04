using UnityEngine;
using UnityEngine.UI;

namespace BubuTown
{
    public sealed class BubuTownRuntimeHud : MonoBehaviour
    {
        public BubuTownGameState State;
        public BubuTownInteractionSystem Interaction;
        public Text CoinText;
        public Text MessageText;
        public Text MenuText;
        public Text MinimapText;
        public Text QuestGuideText;
        public GameObject InteractionPromptPanel;
        public Text InteractionPromptText;
        public GameObject MenuPanel;
        public GameObject MinimalHudRoot;
        public GameObject PhotoModePanel;
        public Text PhotoModeText;
        public BubuTownQuestGuidanceSystem QuestGuidance;
        public BubuTownPhotoMode PhotoMode;
        public BubuTownSettingsSystem SettingsSystem;
        public BubuTownFriendVisitSystem FriendVisitSystem;
        public Text TaskPanelText;
        public Text BagPanelText;
        public Text FurniturePanelText;
        public Text DecorPanelText;
        public Text RelationshipPanelText;
        public Text HomeProgressText;
        public Text MapPanelText;
        public Text SettingsPanelText;
        public Text PhotoPanelText;
        public Text SavePanelText;
        public GameObject DialoguePanel;
        public Text DialogueSpeakerText;
        public Text DialogueBodyText;
        public Text DialogueFriendshipText;
        public Text DialogueQuestHintText;

        private void Update()
        {
            if (State == null)
            {
                return;
            }

            if (CoinText != null)
            {
                CoinText.text = "小镇币: " + State.TownCoins;
            }
            if (MessageText != null)
            {
                MessageText.text = State.LastMessage;
            }
            if (MenuText != null)
            {
                MenuText.text = State.DecorationModeEnabled ? "装修中\nTab 退出" : State.MenuOpen ? "菜单已展开\nTab 关闭" : "菜单\n任务 背包 装修 地图";
            }
            if (MinimapText != null)
            {
                MinimapText.text = "小地图\n广场 蛋糕 学校\n家具 小屋 公园";
            }
            var shouldShowMinimalHud = !State.PhotoModeEnabled && !State.HideMinimalHud;
            if (MinimalHudRoot != null && MinimalHudRoot.activeSelf != shouldShowMinimalHud)
            {
                MinimalHudRoot.SetActive(shouldShowMinimalHud);
            }
            if (PhotoModePanel != null && PhotoModePanel.activeSelf != State.PhotoModeEnabled)
            {
                PhotoModePanel.SetActive(State.PhotoModeEnabled);
            }
            if (PhotoModeText != null)
            {
                PhotoModeText.text = PhotoMode != null ? PhotoMode.PhotoModeSummary() : "拍照模式\n等待拍照系统。";
            }
            if (QuestGuideText != null)
            {
                QuestGuideText.text = QuestGuidance != null ? QuestGuidance.CurrentGuideSummary() : "任务引导\n等待引导系统。";
            }
            var shouldShowInteractionPrompt = shouldShowMinimalHud && !string.IsNullOrEmpty(State.InteractionPrompt);
            if (InteractionPromptPanel != null && InteractionPromptPanel.activeSelf != shouldShowInteractionPrompt)
            {
                InteractionPromptPanel.SetActive(shouldShowInteractionPrompt);
            }
            if (InteractionPromptText != null)
            {
                InteractionPromptText.text = State.InteractionPrompt;
            }
            if (MenuPanel != null && MenuPanel.activeSelf != State.MenuOpen)
            {
                MenuPanel.SetActive(State.MenuOpen);
            }
            if (DialoguePanel != null && DialoguePanel.activeSelf != State.DialogueOpen)
            {
                DialoguePanel.SetActive(State.DialogueOpen);
            }
            if (DialogueSpeakerText != null)
            {
                DialogueSpeakerText.text = State.DialogueSpeaker;
            }
            if (DialogueBodyText != null)
            {
                DialogueBodyText.text = State.DialogueBody;
            }
            if (DialogueFriendshipText != null)
            {
                DialogueFriendshipText.text = State.DialogueFriendship;
            }
            if (DialogueQuestHintText != null)
            {
                DialogueQuestHintText.text = State.DialogueQuestHint;
            }
            if (TaskPanelText != null)
            {
                TaskPanelText.text = State.ActiveQuestSummary() + "\n\n" + State.CompletedQuestSummary() + State.BoardUnlockSummary();
            }
            if (BagPanelText != null)
            {
                BagPanelText.text = State.BagSummary();
            }
            if (FurniturePanelText != null)
            {
                FurniturePanelText.text = State.FurnitureCatalogSummary();
            }
            if (DecorPanelText != null)
            {
                DecorPanelText.text = (State.DecorationModeEnabled ? "装修模式: 已打开\n方向键选格 / Z/X 换家具\nE 确认 / R 旋转\nTab/Esc 退出" : "装修模式: 未打开\n进入小屋后靠近网格按 E") + "\n" + State.DecorationSelectionSummary() + "\n\n" + State.HomeProgressSummary();
            }
            if (RelationshipPanelText != null)
            {
                RelationshipPanelText.text = State.FriendshipSummary();
            }
            if (HomeProgressText != null)
            {
                HomeProgressText.text = State.HomeProgressSummary() + "\n\n" + (FriendVisitSystem != null ? FriendVisitSystem.VisitSummary() : "朋友来访\n等待来访系统。");
            }
            if (MapPanelText != null)
            {
                MapPanelText.text = "地图\n中央广场 / 蛋糕店 / 学校\n家具店 / 玩家小屋 / 居民区 / 公园";
            }
            if (SettingsPanelText != null)
            {
                SettingsPanelText.text = SettingsSystem != null ? SettingsSystem.SettingsSummary() : "设置\n等待设置系统。";
            }
            if (PhotoPanelText != null)
            {
                PhotoPanelText.text = "拍照\nP 开关拍照模式\n[/] 切换推荐机位\n拍照时隐藏常驻 HUD";
            }
            if (SavePanelText != null)
            {
                SavePanelText.text = "保存\n靠近小屋保存点按 E\n当前存档包含金币、任务、家具、好感和温馨度";
            }
        }
    }
}
