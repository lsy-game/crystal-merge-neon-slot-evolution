using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownPhotoMode : MonoBehaviour
    {
        public BubuTownGameState State;
        public Transform[] PhotoSpots;
        public int CurrentSpotIndex;

        public void TogglePhotoMode()
        {
            if (State == null)
            {
                return;
            }

            State.PhotoModeEnabled = !State.PhotoModeEnabled;
            State.LastMessage = State.PhotoModeEnabled ? "拍照模式已打开：P 退出，[/] 切换推荐机位。" : "拍照模式已关闭。";
        }

        public void NextSpot()
        {
            if (PhotoSpots == null || PhotoSpots.Length == 0 || State == null)
            {
                return;
            }

            CurrentSpotIndex = (CurrentSpotIndex + 1) % PhotoSpots.Length;
            State.LastMessage = "拍照机位：" + PhotoSpots[CurrentSpotIndex].name;
        }

        public string PhotoModeSummary()
        {
            var spotName = PhotoSpots != null && PhotoSpots.Length > 0 ? PhotoSpots[Mathf.Clamp(CurrentSpotIndex, 0, PhotoSpots.Length - 1)].name : "未配置";
            return "拍照模式\nP 开关\n[/] 切换机位\n当前机位: " + spotName;
        }
    }
}
