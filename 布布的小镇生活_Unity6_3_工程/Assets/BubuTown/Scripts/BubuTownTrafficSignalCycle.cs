using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownTrafficSignalCycle : MonoBehaviour
    {
        public Renderer RedLight;
        public Renderer YellowLight;
        public Renderer GreenLight;
        public Renderer PedestrianPanel;
        public float GreenSeconds = 5f;
        public float YellowSeconds = 1.5f;
        public float RedSeconds = 4f;
        public float PhaseOffsetSeconds;

        public string CurrentState { get; private set; }

        private void Update()
        {
            var cycle = Mathf.Max(0.1f, GreenSeconds + YellowSeconds + RedSeconds);
            var time = Mathf.Repeat(Time.time + PhaseOffsetSeconds, cycle);
            if (time < GreenSeconds)
            {
                SetState("Green", GreenLight);
            }
            else if (time < GreenSeconds + YellowSeconds)
            {
                SetState("Yellow", YellowLight);
            }
            else
            {
                SetState("Red", RedLight);
            }
        }

        private void SetState(string state, Renderer activeRenderer)
        {
            if (CurrentState == state)
            {
                return;
            }

            CurrentState = state;
            SetLight(RedLight, activeRenderer == RedLight, new Color(1f, 0.05f, 0.03f));
            SetLight(YellowLight, activeRenderer == YellowLight, new Color(1f, 0.68f, 0.08f));
            SetLight(GreenLight, activeRenderer == GreenLight, new Color(0.05f, 0.9f, 0.28f));
            SetLight(PedestrianPanel, state == "Red", new Color(0.1f, 0.75f, 1f));
        }

        private static void SetLight(Renderer renderer, bool active, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            var material = renderer.material;
            var baseColor = active ? color : color * 0.22f;
            baseColor.a = 1f;
            material.color = baseColor;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", baseColor);
            }
            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", active ? color * 1.2f : Color.black);
            }
        }
    }
}
