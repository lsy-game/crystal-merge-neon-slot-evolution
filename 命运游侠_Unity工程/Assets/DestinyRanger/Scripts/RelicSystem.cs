using System.Collections.Generic;
using UnityEngine;

namespace DestinyRanger
{
    public enum RelicEffect
    {
        ReplaceSkulls,
        EnableDiagonals,
        RerollChance
    }

    [CreateAssetMenu(menuName = "Destiny Ranger/Fate Weaver Relic")]
    public sealed class Relic : ScriptableObject
    {
        public string relicName;
        public Sprite icon;
        [TextArea] public string description;
        public RelicEffect effect;
    }

    public sealed class RelicSystem : MonoBehaviour
    {
        [SerializeField] private List<RelicEffect> activeEffects = new List<RelicEffect>();

        public bool HasEffect(RelicEffect effect)
        {
            return activeEffects.Contains(effect);
        }

        public void AddEffect(RelicEffect effect)
        {
            if (!activeEffects.Contains(effect))
                activeEffects.Add(effect);
        }

        public void ApplyPreEvaluationRules(int[,] grid, int skullIndex, int heartIndex)
        {
            if (!HasEffect(RelicEffect.ReplaceSkulls))
                return;

            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
                if (grid[x, y] == skullIndex)
                    grid[x, y] = heartIndex;
        }
    }
}
