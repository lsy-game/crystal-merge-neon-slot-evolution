using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownInteractionSystem : MonoBehaviour
    {
        public BubuTownPlayerController Player;
        public BubuTownGameState State;
        public BubuTownDecorationGrid HomeGrid;
        public BubuTownPhotoMode PhotoMode;
        public BubuTownSettingsSystem SettingsSystem;
        public float InteractionRadius = 3f;
        public BubuTownInteractable CurrentTarget;
        public BubuTownQuizStation ActiveQuiz;

        private void Update()
        {
            if (Player == null || State == null)
            {
                return;
            }

            CurrentTarget = FindNearestInteractable();
            if (CurrentTarget != null)
            {
                State.InteractionPrompt = CurrentTarget.DisplayName + " - " + CurrentTarget.InteractionPrompt;
            }
            else
            {
                State.InteractionPrompt = string.Empty;
            }

            if (Input.GetKeyDown(KeyCode.E) && CurrentTarget != null)
            {
                Interact(CurrentTarget);
            }
            if (ActiveQuiz != null)
            {
                AnswerActiveQuizFromInput();
            }
            if (Input.GetKeyDown(KeyCode.R) && State.DecorationModeEnabled && HomeGrid != null)
            {
                HomeGrid.RotatePreviewClockwise();
                State.LastMessage = "家具预览旋转 90 度。";
            }
            if (State.DecorationModeEnabled && HomeGrid != null)
            {
                CycleDecorationFurnitureFromInput();
                MoveDecorationPreviewFromInput();
            }
            if (Input.GetKeyDown(KeyCode.P) && PhotoMode != null)
            {
                PhotoMode.TogglePhotoMode();
            }
            if (Input.GetKeyDown(KeyCode.F1) && SettingsSystem != null)
            {
                SettingsSystem.ToggleCameraSensitivity();
            }
            if (Input.GetKeyDown(KeyCode.F2) && SettingsSystem != null)
            {
                SettingsSystem.ToggleFastRun();
            }
            if (Input.GetKeyDown(KeyCode.F3) && SettingsSystem != null)
            {
                SettingsSystem.ToggleMinimalHud();
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket) && State.PhotoModeEnabled && PhotoMode != null)
            {
                PhotoMode.NextSpot();
            }
            if (Input.GetKeyDown(KeyCode.RightBracket) && State.PhotoModeEnabled && PhotoMode != null)
            {
                PhotoMode.NextSpot();
            }
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (ActiveQuiz != null)
                {
                    ActiveQuiz = null;
                    State.LastMessage = "已退出课后小测验。";
                }
                else if (State.PhotoModeEnabled && PhotoMode != null)
                {
                    PhotoMode.TogglePhotoMode();
                }
                else if (State.DialogueOpen)
                {
                    State.CloseDialogue();
                }
                else if (State.DecorationModeEnabled)
                {
                    if (HomeGrid != null)
                    {
                        HomeGrid.ClearPreview();
                    }
                    State.ToggleDecorationMode(false);
                }
                else
                {
                    State.ToggleMenu();
                }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                State.LastMessage = "地图：中央广场、蛋糕店、学校、家具店、玩家小屋、居民区、公园。";
            }
        }

        private BubuTownInteractable FindNearestInteractable()
        {
            BubuTownInteractable best = null;
            var bestDistance = InteractionRadius;
            foreach (var interactable in FindObjectsOfType<BubuTownInteractable>())
            {
                var distance = Vector3.Distance(Player.transform.position, interactable.transform.position);
                if (distance < bestDistance)
                {
                    best = interactable;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private void Interact(BubuTownInteractable interactable)
        {
            switch (interactable.Type)
            {
                case BubuTownInteractableType.Npc:
                    TalkToNpc(interactable.GetComponent<BubuTownNpc>());
                    break;
                case BubuTownInteractableType.QuestItem:
                    CompleteQuestItem(interactable);
                    break;
                case BubuTownInteractableType.Furniture:
                    BuyFurniture(interactable.GetComponent<BubuTownFurnitureMarker>());
                    break;
                case BubuTownInteractableType.QuestBoard:
                    State.AcceptPriorityQuests();
                    break;
                case BubuTownInteractableType.Shop:
                    State.LastMessage = interactable.DisplayName + "营业中：靠近家具样品按 E 可购买。";
                    break;
                case BubuTownInteractableType.Door:
                    UseDoor(interactable.GetComponent<BubuTownDoorLink>(), interactable);
                    break;
                case BubuTownInteractableType.DecorationGrid:
                    PlaceNextFurniture();
                    break;
                case BubuTownInteractableType.SavePoint:
                    State.SaveState();
                    State.LastMessage = "已保存小镇进度。";
                    break;
                case BubuTownInteractableType.QuizStation:
                    StartQuiz(interactable.GetComponent<BubuTownQuizStation>());
                    break;
                case BubuTownInteractableType.DayEndPoint:
                    CompleteDayEnd(interactable.GetComponent<BubuTownDayEndPoint>());
                    break;
                case BubuTownInteractableType.HomeVisitorSpot:
                    VisitHomeFriend(interactable.GetComponent<BubuTownHomeVisitorSpot>());
                    break;
                default:
                    State.LastMessage = interactable.DisplayName + " 还在施工中。";
                    break;
            }
        }

        private void VisitHomeFriend(BubuTownHomeVisitorSpot spot)
        {
            if (spot == null)
            {
                State.LastMessage = "朋友来访站位还没有配置。";
                return;
            }

            State.LastMessage = spot.Interact(State);
        }

        private void CompleteDayEnd(BubuTownDayEndPoint dayEnd)
        {
            if (dayEnd == null)
            {
                State.LastMessage = "日终点还没有配置。";
                return;
            }

            if (!State.HasQuest(dayEnd.QuestId))
            {
                State.AcceptQuest(dayEnd.QuestId, State.QuestNameForId(dayEnd.QuestId));
            }

            var questMarker = FindQuestMarker(dayEnd.QuestId);
            if (questMarker != null)
            {
                State.CompleteQuest(questMarker);
            }

            if (HomeGrid != null)
            {
                HomeGrid.ClearPreview();
            }

            State.AdvanceToNextDay(dayEnd.EndDayMessage);
        }

        private void StartQuiz(BubuTownQuizStation quiz)
        {
            if (quiz == null)
            {
                State.LastMessage = "测验台还没有配置题目。";
                return;
            }

            if (!State.HasQuest(quiz.QuestId))
            {
                State.AcceptQuest(quiz.QuestId, State.QuestNameForId(quiz.QuestId));
            }

            ActiveQuiz = quiz;
            State.LastMessage = quiz.PromptText();
        }

        private void AnswerActiveQuizFromInput()
        {
            var choice = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                choice = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                choice = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                choice = 2;
            }

            if (choice < 0)
            {
                return;
            }

            CompleteQuiz(ActiveQuiz, choice);
        }

        private void CompleteQuiz(BubuTownQuizStation quiz, int choiceIndex)
        {
            if (quiz == null)
            {
                ActiveQuiz = null;
                State.LastMessage = "测验台还没有配置题目。";
                return;
            }

            if (!quiz.IsCorrectChoice(choiceIndex))
            {
                State.LastMessage = quiz.FailMessage;
                return;
            }

            var questMarker = FindQuestMarker(quiz.QuestId);
            if (questMarker != null)
            {
                State.CompleteQuest(questMarker);
                State.LastMessage = quiz.PassMessage + " " + State.LastMessage;
            }
            else
            {
                State.LastMessage = quiz.PassMessage;
            }

            ActiveQuiz = null;
        }

        private void TalkToNpc(BubuTownNpc npc)
        {
            if (npc == null)
            {
                return;
            }

            if (npc.QuestIds != null)
            {
                foreach (var questId in npc.QuestIds)
                {
                    if (!State.HasQuest(questId))
                    {
                        var questName = State.QuestNameForId(questId);
                        State.AcceptQuest(questId, questName);
                        State.OpenDialogue(npc, "新任务：《" + questName + "》已记录");
                        return;
                    }
                }
            }

            State.OpenDialogue(npc, "暂无新任务。");
        }

        private void CompleteQuestItem(BubuTownInteractable interactable)
        {
            var quest = interactable.GetComponent<BubuTownQuestMarker>();
            if (quest != null)
            {
                State.CompleteQuest(quest);
                return;
            }

            var step = interactable.GetComponent<BubuTownQuestStepMarker>();
            if (step == null)
            {
                State.LastMessage = interactable.DisplayName + "：这个任务物品还没有绑定任务数据。";
                return;
            }

            State.CompleteQuestStep(step, FindQuestMarker(step.QuestId));
        }

        private static BubuTownQuestMarker FindQuestMarker(string questId)
        {
            foreach (var quest in FindObjectsOfType<BubuTownQuestMarker>())
            {
                if (quest.QuestId == questId)
                {
                    return quest;
                }
            }

            return null;
        }

        private void BuyFurniture(BubuTownFurnitureMarker furniture)
        {
            if (State.BuyFurniture(furniture) && furniture != null && furniture.FurnitureId == "fur_small_chair")
            {
                EnsureQuestAccepted("Q007");
            }
        }

        private void PlaceNextFurniture()
        {
            if (HomeGrid == null)
            {
                State.LastMessage = "没有找到装修网格。";
                return;
            }

            if (!State.DecorationModeEnabled)
            {
                State.ToggleDecorationMode(true);
                HomeGrid.ShowPreview(State.SelectedUnplacedFurniture(), State.NextFreeFurnitureCell(HomeGrid.CellCount));
                return;
            }

            var furnitureId = State.SelectedUnplacedFurniture();
            if (string.IsNullOrEmpty(furnitureId))
            {
                State.LastMessage = "背包里没有未摆放家具。先去家具店买一件吧。";
                return;
            }

            var index = HomeGrid.PreviewPlacementIndex;
            if (!HomeGrid.IsPreviewPlacementValid())
            {
                State.LastMessage = "当前位置不能摆放这件家具。大件要留足格子，靠墙家具要贴近墙边。";
                return;
            }

            if (State.IsFurnitureCellOccupied(index))
            {
                State.LastMessage = "这个格子已经摆了家具。用方向键移动预览到空格子。";
                return;
            }

            BubuTownSaveBootstrap.CreatePlacedFurnitureVisual(HomeGrid.transform, furnitureId, HomeGrid.LocalPositionForIndex(index), HomeGrid.PreviewRotationQuarterTurns);
            State.MarkFurniturePlaced(furnitureId, HomeGrid.PreviewRotationQuarterTurns, index);
            if (furnitureId == "fur_small_chair")
            {
                CompleteQuestById("Q007");
            }
            HomeGrid.ShowPreview(State.SelectedUnplacedFurniture(), State.NextFreeFurnitureCell(HomeGrid.CellCount));
            State.LastMessage = "摆好了：" + furnitureId + "。小屋温馨度 " + State.HomeWarmthScore() + "/" + State.HomeWarmthTarget + "。";
        }

        private void CycleDecorationFurnitureFromInput()
        {
            var direction = 0;
            if (Input.GetKeyDown(KeyCode.Z))
            {
                direction = -1;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                direction = 1;
            }

            if (direction == 0)
            {
                return;
            }

            State.CycleDecorationFurniture(direction);
            HomeGrid.ShowPreview(State.SelectedUnplacedFurniture(), HomeGrid.PreviewPlacementIndex);
        }

        private void MoveDecorationPreviewFromInput()
        {
            var columnDelta = 0;
            var rowDelta = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                columnDelta = -1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                columnDelta = 1;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                rowDelta = 1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                rowDelta = -1;
            }

            if (columnDelta == 0 && rowDelta == 0)
            {
                return;
            }

            HomeGrid.MovePreview(columnDelta, rowDelta);
            State.LastMessage = "预览移动到网格 " + HomeGrid.PreviewPlacementIndex + "。E 确认，R 旋转。";
        }

        private void UseDoor(BubuTownDoorLink link, BubuTownInteractable interactable)
        {
            if (link == null || link.TargetPoint == null)
            {
                State.LastMessage = interactable.DisplayName + "：门链接还没有配置目标点。";
                return;
            }

            var controller = Player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            Player.transform.position = link.TargetPoint.position + link.TargetOffset;
            Player.transform.rotation = link.TargetPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }

            State.LastMessage = link.MessageAfterUse;
            if (interactable.Id == "PLAYER_HOME" || interactable.Id == "PLAYER_HOME_DOOR")
            {
                CompleteQuestById("Q006");
            }
        }

        private void EnsureQuestAccepted(string questId)
        {
            if (!State.HasQuest(questId))
            {
                State.AcceptQuest(questId, State.QuestNameForId(questId));
            }
        }

        private void CompleteQuestById(string questId)
        {
            EnsureQuestAccepted(questId);
            var questMarker = FindQuestMarker(questId);
            if (questMarker != null)
            {
                State.CompleteQuest(questMarker);
            }
        }
    }
}
