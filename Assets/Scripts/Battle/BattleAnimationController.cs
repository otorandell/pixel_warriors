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

        // Track persistent effects so we can kill them when removed
        private readonly Dictionary<BattleCharacter, Tween> _stunWobbleTweens = new();
        private readonly Dictionary<BattleCharacter, Tween> _lowHPFlickerTweens = new();

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
            GameEvents.OnCharacterDefeated += HandleCharacterDefeated;
            GameEvents.OnStatusEffectApplied += HandleStatusEffectApplied;
            GameEvents.OnStatusEffectRemoved += HandleStatusEffectRemoved;
            GameEvents.OnDamageDealt += HandleDamageDealt;
        }

        private void OnDisable()
        {
            GameEvents.OnAttackStarted -= HandleAttackStarted;
            GameEvents.OnHitResolved -= HandleHitResolved;
            GameEvents.OnHealingReceived -= HandleHealingReceived;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
            GameEvents.OnCharacterDefeated -= HandleCharacterDefeated;
            GameEvents.OnStatusEffectApplied -= HandleStatusEffectApplied;
            GameEvents.OnStatusEffectRemoved -= HandleStatusEffectRemoved;
            GameEvents.OnDamageDealt -= HandleDamageDealt;
        }

        private void OnDestroy()
        {
            foreach (Tween t in _stunWobbleTweens.Values) t?.Kill();
            foreach (Tween t in _lowHPFlickerTweens.Values) t?.Kill();
            _stunWobbleTweens.Clear();
            _lowHPFlickerTweens.Clear();
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

            DOTween.Kill(tmp.rectTransform);
            DOTween.Kill(cg);

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
                PlaySqueezeStretch(card, delay);
                PlayColorFlash(card, result.IsCrit ? UIStyleConfig.AccentYellow : UIStyleConfig.AccentRed, delay);

                if (result.IsCrit)
                    PlayScreenShake(delay);

                // Check low HP flicker after damage
                UpdateLowHPFlicker(target);
            }
        }

        private void HandleHealingReceived(BattleCharacter target, int amount)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
            if (card == null) return;

            PlayFloatingText(card, $"+{amount}", UIStyleConfig.AccentGreen,
                AnimationConfig.FloatingTextFontSize, 0f);
            PlayHealPulse(card);

            // Healing may bring character above low HP threshold
            UpdateLowHPFlicker(target);
        }

        private void HandleTurnStarted(BattleCharacter character)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(character);
            if (card == null) return;

            PlayTurnPulse(card);
        }

        private void HandleCharacterDefeated(BattleCharacter character)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(character);
            if (card == null) return;

            // Stop any persistent effects
            StopPersistentEffects(character);

            PlayDeathAnimation(card);
        }

        private void HandleStatusEffectApplied(BattleCharacter target, StatusEffect effect, int value)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
            if (card == null) return;

            switch (effect)
            {
                case StatusEffect.Shield:
                    PlayShieldShimmer(card);
                    break;

                case StatusEffect.Poison:
                    PlayColorFlash(card, UIStyleConfig.AccentGreen, 0f);
                    break;

                case StatusEffect.Burn:
                    PlayColorFlash(card, UIStyleConfig.AccentRed, 0f);
                    break;

                case StatusEffect.Stun:
                    StartStunWobble(target, card);
                    break;

                case StatusEffect.Blessing:
                case StatusEffect.Regeneration:
                case StatusEffect.IronWill:
                case StatusEffect.HuntersFocus:
                    PlayBuffGlow(card);
                    break;
            }
        }

        private void HandleStatusEffectRemoved(BattleCharacter target, StatusEffect effect)
        {
            switch (effect)
            {
                case StatusEffect.Stun:
                    StopStunWobble(target);
                    break;
            }
        }

        private void HandleDamageDealt(BattleCharacter target, int damage, DamageType type)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(target);
            if (card == null) return;

            // Flash on DoT tick: check if character has poison/burn active
            if (target.HasEffect(StatusEffect.Poison))
                PlayColorFlash(card, UIStyleConfig.AccentGreen, 0f);
            if (target.HasEffect(StatusEffect.Burn))
                PlayColorFlash(card, UIStyleConfig.AccentRed, 0f);
        }

        // --- Core Animation Methods ---

        private void PlayLunge(CharacterCardUI card, CharacterCardUI targetCard)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "lunge");

            Vector2 originalPos = rect.anchoredPosition;

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

        // --- New Effects ---

        private void PlaySqueezeStretch(CharacterCardUI card, float delay)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "squeeze");

            Sequence seq = DOTween.Sequence();
            seq.SetDelay(delay);
            seq.Append(rect.DOScale(
                new Vector3(AnimationConfig.SqueezeX, AnimationConfig.SqueezeY, 1f),
                AnimationConfig.SqueezeDuration).SetEase(AnimationConfig.SqueezeEase));
            seq.Append(rect.DOScale(Vector3.one, AnimationConfig.SqueezeReturnDuration)
                .SetEase(AnimationConfig.SqueezeReturnEase));
            seq.SetTarget(rect);
            seq.SetId("squeeze");
        }

        private void PlayColorFlash(CharacterCardUI card, Color flashColor, float delay)
        {
            Image bg = card.BackgroundImage;
            if (bg == null) return;

            DOTween.Kill(bg, "colorFlash");

            Sequence seq = DOTween.Sequence();
            seq.SetDelay(delay);
            seq.Append(bg.DOColor(flashColor, AnimationConfig.ColorFlashDuration * 0.3f));
            seq.Append(bg.DOColor(UIStyleConfig.PanelBackground, AnimationConfig.ColorFlashDuration * 0.7f));
            seq.SetTarget(bg);
            seq.SetId("colorFlash");
        }

        private void PlayDeathAnimation(CharacterCardUI card)
        {
            RectTransform rect = card.Root;
            CanvasGroup cg = card.CanvasGroup;
            if (cg == null) return;

            DOTween.Kill(rect, "death");

            Sequence seq = DOTween.Sequence();
            seq.Append(rect.DOScale(AnimationConfig.DeathShrinkScale, AnimationConfig.DeathShrinkDuration)
                .SetEase(AnimationConfig.DeathShrinkEase));
            seq.Insert(AnimationConfig.DeathShrinkDuration - AnimationConfig.DeathFadeDuration,
                cg.DOFade(0f, AnimationConfig.DeathFadeDuration));
            seq.SetTarget(rect);
            seq.SetId("death");
        }

        private void PlayHealPulse(CharacterCardUI card)
        {
            RectTransform rect = card.Root;
            DOTween.Kill(rect, "healPulse");

            // Brief scale pulse
            Sequence seq = DOTween.Sequence();
            seq.Append(rect.DOScale(AnimationConfig.HealPulseScale,
                AnimationConfig.HealPulseDuration * 0.4f).SetEase(Ease.OutQuad));
            seq.Append(rect.DOScale(1f,
                AnimationConfig.HealPulseDuration * 0.6f).SetEase(Ease.InQuad));
            seq.SetTarget(rect);
            seq.SetId("healPulse");

            // Green border flash
            PlayBorderFlash(card, UIStyleConfig.AccentGreen, AnimationConfig.HealBorderFlashDuration);
        }

        private void PlayShieldShimmer(CharacterCardUI card)
        {
            PlayBorderFlash(card, UIStyleConfig.TextPrimary, AnimationConfig.ShieldShimmerDuration,
                AnimationConfig.ShieldShimmerLoops);
        }

        private void PlayBuffGlow(CharacterCardUI card)
        {
            PlayBorderFlash(card, UIStyleConfig.AccentYellow, AnimationConfig.BuffGlowDuration);
        }

        private void PlayBorderFlash(CharacterCardUI card, Color flashColor, float duration, int loops = 1)
        {
            if (card.IsDead) return;

            // Flash border to flashColor then back to current highlight state
            Color originalColor = UIStyleConfig.PanelBorder;

            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < loops; i++)
            {
                float halfDur = duration / (2f * loops);
                seq.AppendCallback(() => card.SetBorderColor(flashColor));
                seq.AppendInterval(halfDur);
                seq.AppendCallback(() => card.SetBorderColor(originalColor));
                seq.AppendInterval(halfDur);
            }
            seq.SetTarget(this);
        }

        // --- Persistent Status Effects ---

        private void StartStunWobble(BattleCharacter character, CharacterCardUI card)
        {
            StopStunWobble(character);

            RectTransform rect = card.Root;

            Tween tween = rect.DORotate(
                    new Vector3(0, 0, AnimationConfig.StunWobbleAngle),
                    AnimationConfig.StunWobbleDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .From(new Vector3(0, 0, -AnimationConfig.StunWobbleAngle))
                .SetTarget(rect)
                .SetId("stunWobble");

            _stunWobbleTweens[character] = tween;
        }

        private void StopStunWobble(BattleCharacter character)
        {
            if (!_stunWobbleTweens.TryGetValue(character, out Tween tween)) return;

            tween?.Kill();
            _stunWobbleTweens.Remove(character);

            // Reset rotation
            CharacterCardUI card = _battleScreen?.BattleGrid.FindCard(character);
            if (card != null)
                card.Root.localRotation = Quaternion.identity;
        }

        private void UpdateLowHPFlicker(BattleCharacter character)
        {
            if (_battleScreen == null) return;
            CharacterCardUI card = _battleScreen.BattleGrid.FindCard(character);
            if (card == null) return;

            float hpPercent = character.MaxHP > 0 ? (float)character.CurrentHP / character.MaxHP : 1f;

            if (hpPercent <= AnimationConfig.LowHPThreshold && character.IsAlive)
            {
                StartLowHPFlicker(character, card);
            }
            else
            {
                StopLowHPFlicker(character);
            }
        }

        private void StartLowHPFlicker(BattleCharacter character, CharacterCardUI card)
        {
            // Already flickering
            if (_lowHPFlickerTweens.ContainsKey(character)) return;

            CanvasGroup cg = card.CanvasGroup;
            if (cg == null) return;

            Tween tween = cg.DOFade(AnimationConfig.LowHPFlickerMinAlpha,
                    AnimationConfig.LowHPFlickerDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetTarget(cg)
                .SetId("lowHPFlicker");

            _lowHPFlickerTweens[character] = tween;
        }

        private void StopLowHPFlicker(BattleCharacter character)
        {
            if (!_lowHPFlickerTweens.TryGetValue(character, out Tween tween)) return;

            tween?.Kill();
            _lowHPFlickerTweens.Remove(character);

            // Restore alpha
            CharacterCardUI card = _battleScreen?.BattleGrid.FindCard(character);
            if (card?.CanvasGroup != null)
                card.CanvasGroup.alpha = 1f;
        }

        private void StopPersistentEffects(BattleCharacter character)
        {
            StopStunWobble(character);
            StopLowHPFlicker(character);
        }
    }
}
