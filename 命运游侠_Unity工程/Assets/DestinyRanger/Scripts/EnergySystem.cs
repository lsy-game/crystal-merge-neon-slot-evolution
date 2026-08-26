using System;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class EnergySystem : MonoBehaviour
    {
        public int currentEnergy;
        public event Action OnEnergyFull;

        private const int MaxEnergy = 100;
        private Image fill;
        private Text valueText;
        private RectTransform pulseTarget;
        private bool fullNotified;

        public void Bind(Image energyFill, Text energyValue, RectTransform pulseRoot)
        {
            fill = energyFill;
            valueText = energyValue;
            pulseTarget = pulseRoot;
            UpdateUi();
        }

        public void AddEnergy(int amount)
        {
            currentEnergy = Mathf.Clamp(currentEnergy + Mathf.Max(0, amount), 0, MaxEnergy);
            UpdateUi();

            if (currentEnergy >= MaxEnergy && !fullNotified)
            {
                fullNotified = true;
                OnEnergyFull?.Invoke();
            }
        }

        public bool ConsumeEnergy(int amount)
        {
            if (currentEnergy < amount)
                return false;

            currentEnergy -= amount;
            fullNotified = currentEnergy >= MaxEnergy;
            UpdateUi();
            return true;
        }

        private void Update()
        {
            if (!pulseTarget)
                return;

            if (currentEnergy >= MaxEnergy)
            {
                var s = 1f + Mathf.Sin(Time.unscaledTime * 12f) * .035f;
                pulseTarget.localScale = new Vector3(s, s, 1f);
            }
            else
            {
                pulseTarget.localScale = Vector3.Lerp(pulseTarget.localScale, Vector3.one, Time.unscaledDeltaTime * 8f);
            }
        }

        private void UpdateUi()
        {
            if (fill)
                fill.fillAmount = currentEnergy / (float)MaxEnergy;
            if (valueText)
                valueText.text = currentEnergy + "/100";
        }
    }
}
