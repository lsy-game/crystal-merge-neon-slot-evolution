using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DestinyRanger
{
    public sealed class SkillManager : MonoBehaviour
    {
        private RectTransform fxRoot;
        private Text combatLog;
        private Image hpFill;
        private Image shieldFill;
        private Image redFlash;
        private float hp = 1f;

        public void Bind(RectTransform effectRoot, Text logText, Image playerHpFill, Image playerShieldFill, Image warningFlash)
        {
            fxRoot = effectRoot;
            combatLog = logText;
            hpFill = playerHpFill;
            shieldFill = playerShieldFill;
            redFlash = warningFlash;
            SetLog("能量收集中");
        }

        public void TriggerPerfect(int symbol, RectTransform origin)
        {
            switch (symbol)
            {
                case 0:
                    SetLog("三剑：全屏剑气 100% 攻击");
                    StartCoroutine(ProjectileBurst(origin, new Color32(212, 175, 55, 230), "剑气"));
                    break;
                case 1:
                    SetLog("三杖：8 枚追踪魔法弹");
                    StartCoroutine(ProjectileBurst(origin, new Color32(200, 80, 180, 230), "魔弹"));
                    break;
                case 2:
                    hp = Mathf.Clamp01(hp + .4f);
                    if (hpFill) hpFill.fillAmount = hp;
                    SetLog("三心：回复 40% 最大生命");
                    StartCoroutine(GlowColumn(origin, new Color32(80, 200, 100, 210)));
                    break;
                case 3:
                    if (shieldFill) shieldFill.fillAmount = .3f;
                    SetLog("三盾：护盾 15 秒");
                    StartCoroutine(ShieldDecay());
                    break;
                case 4:
                    hp = Mathf.Clamp01(hp * .8f);
                    if (hpFill) hpFill.fillAmount = hp;
                    SetLog("三骷髅：真实伤害 20%，眩晕 2 秒");
                    StartCoroutine(FlashRed());
                    break;
                default:
                    SetLog("三星：命运暴击");
                    StartCoroutine(ProjectileBurst(origin, new Color32(240, 235, 220, 230), "星落"));
                    break;
            }

            Handheld.Vibrate();
        }

        public void TriggerPartial(int symbol)
        {
            string name;
            switch (symbol)
            {
                case 0:
                    name = "剑势 +30%";
                    break;
                case 1:
                    name = "魔力增幅";
                    break;
                case 2:
                    name = "小治疗 10%";
                    break;
                case 3:
                    name = "护甲上升";
                    break;
                case 4:
                    name = "诅咒抵抗";
                    break;
                default:
                    name = "幸运提升";
                    break;
            }
            SetLog("半完美：" + name);
        }

        private IEnumerator ProjectileBurst(RectTransform origin, Color color, string label)
        {
            if (!fxRoot)
                yield break;

            for (var i = 0; i < 8; i++)
            {
                var go = new GameObject(label + "_" + i, typeof(Image));
                go.transform.SetParent(fxRoot, false);
                var img = go.GetComponent<Image>();
                img.color = color;
                img.sprite = FateWeaverGame.WhiteSprite;
                var rect = img.rectTransform;
                rect.sizeDelta = new Vector2(34, 120);
                rect.position = origin ? origin.position : fxRoot.position;
                rect.rotation = Quaternion.Euler(0, 0, -45 + i * 12);
                StartCoroutine(FlyAndFade(rect, img, new Vector2(-360 + i * 100, 900 + (i % 3) * 80)));
            }
            yield return null;
        }

        private IEnumerator FlyAndFade(RectTransform rect, Graphic graphic, Vector2 target)
        {
            var start = rect.anchoredPosition;
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / .55f)
            {
                rect.anchoredPosition = Vector2.Lerp(start, target, Mathf.SmoothStep(0, 1, t));
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1f - t);
                yield return null;
            }
            Destroy(rect.gameObject);
        }

        private IEnumerator GlowColumn(RectTransform origin, Color color)
        {
            if (!fxRoot)
                yield break;

            var go = new GameObject("HealingColumn", typeof(Image));
            go.transform.SetParent(fxRoot, false);
            var img = go.GetComponent<Image>();
            img.sprite = FateWeaverGame.WhiteSprite;
            img.color = color;
            var rect = img.rectTransform;
            rect.sizeDelta = new Vector2(160, 700);
            rect.position = origin ? origin.position : fxRoot.position;
            yield return FlyAndFade(rect, img, rect.anchoredPosition + Vector2.up * 220);
        }

        private IEnumerator ShieldDecay()
        {
            for (var t = 15f; t > 0f; t -= Time.unscaledDeltaTime)
            {
                if (shieldFill) shieldFill.fillAmount = t / 15f * .3f;
                yield return null;
            }
            if (shieldFill) shieldFill.fillAmount = 0f;
        }

        private IEnumerator FlashRed()
        {
            if (!redFlash)
                yield break;
            redFlash.gameObject.SetActive(true);
            for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime / .3f)
            {
                redFlash.color = new Color(0.72f, 0.2f, 0.2f, Mathf.Sin(t * Mathf.PI) * .5f);
                yield return null;
            }
            redFlash.gameObject.SetActive(false);
        }

        private void SetLog(string text)
        {
            if (combatLog)
                combatLog.text = text;
        }
    }
}
