using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class BattleAnimationController : MonoBehaviour
    {
        private BattleScreenUI _battleScreen;
        private TextMeshProUGUI[] _textPool;
        private CanvasGroup[] _textCanvasGroups;
        private int _poolIndex;
        private int _hitCounter;

        public void Initialize(BattleScreenUI battleScreen)
        {
            _battleScreen = battleScreen;
            BuildTextPool();
        }

        private void OnEnable()
        {
            GameEvents.OnAttackStarted += HandleAttackStarted;
            GameEvents.OnHitResolved += HandleHitResolved;
            GameEvents.OnHealingReceived += HandleHealingReceived;
            GameEvents.OnTurnStarted += HandleTurnStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnAttackStarted -= HandleAttackStarted;
            GameEvents.OnHitResolved -= HandleHitResolved;
            GameEvents.OnHealingReceived -= HandleHealingReceived;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        // --- Text Pool ---

        private void BuildTextPool()
        {
            RectTransform overlay = _battleScreen.FloatingTextOverlay;
            _textPool = new TextMeshProUGUI[AnimationConfig.FloatingTextPoolSize];
            _textCanvasGroups = new CanvasGroup[AnimationConfig.FloatingTextPoolSize];

            for (int i = 0; i < AnimationConfig.FloatingTextPoolSize; i++)
            {
                GameObject go = new GameObject($"FloatText_{i}");
                RectTransform rect = go.AddComponent<RectTransform>();
                rect.SetParent(overlay, false);
                rect.sizeDelta = new Vector2(200f, 40f);

                TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.font = FontManager.GetFont();
                tmp.fontSize = AnimationConfig.FloatingTextFontSize;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.overflowMode = TextOverflowModes.Overflow;
                tmp.textWrappingMode = TextWrappingModes.NoWrap;
                tmp.raycastTarget = false;

                CanvasGroup cg = go.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = false;
                cg.interactable = false;

                go.SetActive(false);
                _textPool[i] = tmp;
                _textCanvasGroups[i] = cg;
            }
        }

        private (TextMeshProUGUI text, CanvasGroup canvasGroup) GetPooledText()
        {
            TextMeshProUGUI tmp = _textPool[_poolIndex];
            CanvasGroup cg = _textCanvasGroups[_poolIndex];
            _poolIndex = (_poolIndex + 1) % AnimationConfig.FloatingTextPoolSize;

            // Kill any active tweens on this element
            DOTween.Kill(tmp.rectTransform);
            DOTween.Kill(cg);

            // Reset state
            cg.alpha = 1f;
            tmp.gameObject.SetActive(true);

            return (tmp, cg);
        }

        // --- Event Handlers ---

        private void HandleAttackStarted(BattleCharacter attacker, List<BattleCharacter> targets)
        {
            _hitCounter = 0;

            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(attacker);
            if (card == null) return;

            // Find first target's card for lunge direction
            CharacterCardUI targetCard = null;
            if (targets != null && targets.Count > 0)
                targetCard = _battleScreen.BattleGrid.FindCard(targets[0]);

            PlayLunge(card, targetCard);
        }

        private void HandleHitResolved(BattleCharacter target, HitResult result, DamageType type)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
            if (card == null) return;

            float delay = AnimationConfig.HitVisualDelay + _hitCounter * AnimationConfig.FloatingTextStaggerDelay;
            _hitCounter++;

            if (result.Missed)
            {
                PlayFloatingText(card, "MISS", UIStyleConfig.TextDimmed,
                    AnimationConfig.FloatingTextFontSize, delay);
            }
            else if (result.Dodged)
            {
                PlayFloatingText(card, "DODGE", UIStyleConfig.TextDimmed,
                    AnimationConfig.FloatingTextFontSize, delay);
            }
            else if (result.Blocked)
            {
                PlayFloatingText(card, "BLOCK", UIStyleConfig.TextDimmed,
                    AnimationConfig.FloatingTextFontSize, delay);
            }
            else
            {
                Color color = result.IsCrit ? UIStyleConfig.AccentYellow : UIStyleConfig.AccentRed;
                float fontSize = result.IsCrit
                    ? AnimationConfig.FloatingTextCritFontSize
                    : AnimationConfig.FloatingTextFontSize;
                string text = result.IsCrit ? $"{result.Damage}!" : result.Damage.ToString();

                PlayFloatingText(card, text, color, fontSize, delay);
                PlayReceiverShake(card, result.IsCrit, delay);

                if (result.IsCrit)
                    PlayScreenShake(delay);
            }
        }

        private void HandleHealingReceived(BattleCharacter target, int amount)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
            if (card == null) return;

            PlayFloatingText(card, $"+{amount}", UIStyleConfig.AccentGreen,
                AnimationConfig.FloatingTextFontSize, 0f);
        }

        private void HandleTurnStarted(BattleCharacter character)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(character);
            if (card == null) return;

            PlayTurnPulse(card);
        }

        // --- Animation Methods ---

        private void PlayLunge(CharacterCardUI card, CharacterCardUI targetCard)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "lunge");

            Vector2 originalPos = rect.anchoredPosition;

            // Direction toward target card's position, or fallback to up
            Vector2 direction;
            if (targetCard != null)
            {
                Vector3 delta = targetCard.Root.position - rect.position;
                direction = delta.magnitude > 0.01f ? ((Vector2)delta).normalized : Vector2.up;
            }
            else
            {
                direction = Vector2.up;
            }

            Vector2 lungeTarget = originalPos + direction * AnimationConfig.LungeDistance;

            Sequence seq = DOTween.Sequence();
            seq.Append(rect.DOAnchorPos(lungeTarget, AnimationConfig.LungePushDuration)
                .SetEase(AnimationConfig.LungePushEase));
            seq.Append(rect.DOAnchorPos(originalPos, AnimationConfig.LungeReturnDuration)
                .SetEase(AnimationConfig.LungeReturnEase));
            seq.SetTarget(rect);
            seq.SetId("lunge");
        }

        private void PlayReceiverShake(CharacterCardUI card, bool isCrit, float delay)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "shake");

            float strength = isCrit ? AnimationConfig.CritShakeStrength : AnimationConfig.ShakeStrength;

            rect.DOShakeAnchorPos(AnimationConfig.ShakeDuration, strength,
                    AnimationConfig.ShakeVibrato, AnimationConfig.ShakeRandomness)
                .SetDelay(delay)
                .SetTarget(rect)
                .SetId("shake");
        }

        private void PlayScreenShake(float delay)
        {
            RectTransform target = _battleScreen.RootRect;
            if (target == null) return;

            DOTween.Kill(target, "screenShake");

            target.DOShakeAnchorPos(AnimationConfig.ScreenShakeDuration,
                    AnimationConfig.ScreenShakeStrength, AnimationConfig.ScreenShakeVibrato)
                .SetDelay(delay)
                .SetTarget(target)
                .SetId("screenShake");
        }

        private void PlayFloatingText(CharacterCardUI card, string text, Color color, float fontSize, float delay)
        {
            var (tmp, cg) = GetPooledText();

            tmp.text = text;
            tmp.color = color;
            tmp.fontSize = fontSize;

            // Position at card's world position
            RectTransform textRect = tmp.rectTransform;
            Vector3 cardWorldPos = card.Root.position;
            textRect.position = cardWorldPos + Vector3.up * AnimationConfig.FloatingTextSpawnOffsetY;

            Vector3 endPos = textRect.position + Vector3.up * AnimationConfig.FloatingTextRiseDistance;

            Sequence seq = DOTween.Sequence();
            seq.SetDelay(delay);
            seq.Append(textRect.DOMove(endPos, AnimationConfig.FloatingTextDuration)
                .SetEase(Ease.OutQuad));
            seq.Insert(AnimationConfig.FloatingTextFadeDelay,
                cg.DOFade(0f, AnimationConfig.FloatingTextFadeDuration));
            seq.OnComplete(() => tmp.gameObject.SetActive(false));
            seq.SetTarget(textRect);
        }

        private void PlayTurnPulse(CharacterCardUI card)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "pulse");

            rect.localScale = Vector3.one;
            Sequence seq = DOTween.Sequence();
            seq.Append(rect.DOScale(AnimationConfig.TurnPulseScale,
                    AnimationConfig.TurnPulseDuration * 0.5f)
                .SetEase(AnimationConfig.TurnPulseEase));
            seq.Append(rect.DOScale(1f, AnimationConfig.TurnPulseDuration * 0.5f)
                .SetEase(Ease.InQuad));
            seq.SetTarget(rect);
            seq.SetId("pulse");
        }
    }
}
