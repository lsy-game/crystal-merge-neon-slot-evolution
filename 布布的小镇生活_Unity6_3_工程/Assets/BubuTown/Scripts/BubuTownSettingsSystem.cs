using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownSettingsSystem : MonoBehaviour
    {
        public BubuTownGameState State;
        public BubuTownPlayerController Player;
        public BubuTownThirdPersonCamera CameraRig;
        public BubuTownRuntimeHud Hud;

        public void Apply()
        {
            if (State == null)
            {
                return;
            }

            if (Player != null)
            {
                Player.RunSpeed = State.FastRunEnabled ? 8.5f : 7f;
            }

            if (CameraRig != null)
            {
                CameraRig.MouseSensitivity = State.HighCameraSensitivity ? 3.8f : 2.5f;
            }
        }

        public void ToggleCameraSensitivity()
        {
            if (State == null)
            {
                return;
            }

            State.HighCameraSensitivity = !State.HighCameraSensitivity;
            Apply();
            State.LastMessage = State.HighCameraSensitivity ? "镜头灵敏度：高。" : "镜头灵敏度：标准。";
            State.SaveState();
        }

        public void ToggleFastRun()
        {
            if (State == null)
            {
                return;
            }

            State.FastRunEnabled = !State.FastRunEnabled;
            Apply();
            State.LastMessage = State.FastRunEnabled ? "跑步速度：轻快。" : "跑步速度：标准。";
            State.SaveState();
        }

        public void ToggleMinimalHud()
        {
            if (State == null)
            {
                return;
            }

            State.HideMinimalHud = !State.HideMinimalHud;
            State.LastMessage = State.HideMinimalHud ? "极简 HUD 已隐藏。" : "极简 HUD 已显示。";
            State.SaveState();
        }

        public string SettingsSummary()
        {
            if (State == null)
            {
                return "设置\n等待设置系统。";
            }

            return "设置\nF1 镜头: " + (State.HighCameraSensitivity ? "高" : "标准") +
                "\nF2 跑步: " + (State.FastRunEnabled ? "轻快" : "标准") +
                "\nF3 HUD: " + (State.HideMinimalHud ? "隐藏" : "显示");
        }
    }
}
