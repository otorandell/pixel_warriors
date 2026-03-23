using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class CharacterCardUI
    {
        public RectTransform Root { get; private set; }
        public BattleCharacter Character => _character;
        public Image BackgroundImage => _bgImage;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public bool IsDead => _isDead;

        public event Action<BattleCharacter> OnCardClicked;

        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _classLevelText;
        private TextMeshProUGUI _hpText;
        private TextMeshProUGUI _energyText;
        private TextMeshProUGUI _manaText;
        private TextMeshProUGUI _aggroText;
        private BattleCharacter _character;
        private Button _button;
        private Image _bgImage;
        private Image[] _borderImages;
        private CanvasGroup _canvasGroup;
        private LongPressHandler _longPress;
        private bool _isDead;
        private bool _useSpriteIcons;

        public void Build(Transform parent, BattleCharacter character)
        {
            _character = character;

            Root = PanelBuilder.CreatePanel("Card_" + character.Data.Name, parent);

            // Cache background image (first Image on root = panel bg)
            _bgImage = Root.GetComponent<Image>();

            // Add CanvasGroup for fade effects
            _canvasGroup = Root.gameObject.AddComponent<CanvasGroup>();

            _button = Root.gameObject.AddComponent<Button>();
            _button.interactable = false;

            _longPress = Root.gameObject.AddComponent<LongPressHandler>();
            _longPress.OnLongPress += () =>
            {
                if (!_isDead) GameEvents.RaiseCharacterDetailRequested(_character);
            };

            _button.onClick.AddListener(() =>
            {
                if (_longPress.WasLongPress) return;
                OnCardClicked?.Invoke(_character);
            });

            _borderImages = Root.GetComponentsInChildren<Image>();

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            // Name (class-colored)
            _nameText = PanelBuilder.CreateText("Name", content, character.Data.Name,
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIFormatUtil.GetClassColor(character.Data.Class));
            PanelBuilder.SetAnchored(_nameText.GetComponent<RectTransform>(), 0, 0.80f, 1, 1);

            // Class + Level (dimmed)
            string classLevelStr = character.Side == TeamSide.Player
                ? $"{character.Data.Class} Lv{character.Data.Level}"
                : $"Lv{character.Data.Level}";
            _classLevelText = PanelBuilder.CreateText("ClassLevel", content, classLevelStr,
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.TextDimmed);
            PanelBuilder.SetAnchored(_classLevelText.GetComponent<RectTransform>(), 0, 0.60f, 1, 0.80f);

            // Enable sprite icons for status indicators if sprite asset is available
            TMPro.TMP_SpriteAsset spriteAsset = IconConfig.GetSpriteAsset();
            if (spriteAsset != null)
            {
                _classLevelText.richText = true;
                _classLevelText.spriteAsset = spriteAsset;
                _useSpriteIcons = true;
            }

            // HP
            _hpText = PanelBuilder.CreateText("HP", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.HPBarColor);
            PanelBuilder.SetAnchored(_hpText.GetComponent<RectTransform>(), 0, 0.40f, 1, 0.60f);

            // Energy
            _energyText = PanelBuilder.CreateText("Energy", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.EnergyBarColor);
            PanelBuilder.SetAnchored(_energyText.GetComponent<RectTransform>(), 0, 0.20f, 1, 0.40f);

            // Mana
            _manaText = PanelBuilder.CreateText("Mana", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.ManaBarColor);
            PanelBuilder.SetAnchored(_manaText.GetComponent<RectTransform>(), 0, 0.00f, 1, 0.20f);

            // Enable rich text + sprite asset on resource lines for inline icons
            if (spriteAsset != null)
            {
                _hpText.richText = true;
                _hpText.spriteAsset = spriteAsset;
                _energyText.richText = true;
                _energyText.spriteAsset = spriteAsset;
                _manaText.richText = true;
                _manaText.spriteAsset = spriteAsset;
            }

            // Hit chance % (right-aligned, top-right)
            _aggroText = PanelBuilder.CreateText("HitChance", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopRight,
                UIStyleConfig.TextDimmed);
            PanelBuilder.SetAnchored(_aggroText.GetComponent<RectTransform>(), 0, 0.80f, 1, 1);

            Refresh();
        }

        public void Refresh()
        {
            if (_character == null) return;

            if (!_character.IsAlive && !_isDead)
            {
                SetDead();
                return;
            }

            // Revived: clear death state
            if (_isDead && _character.IsAlive)
            {
                _isDead = false;
                _canvasGroup.alpha = 1f;
                Root.localScale = Vector3.one;
                _nameText.color = UIFormatUtil.GetClassColor(_character.Data.Class);
                _classLevelText.color = UIStyleConfig.TextDimmed;
                _hpText.color = UIStyleConfig.HPBarColor;
                _energyText.color = UIStyleConfig.EnergyBarColor;
                _manaText.color = UIStyleConfig.ManaBarColor;
                SetBorderColor(UIStyleConfig.PanelBorder);
            }

            if (_isDead) return;

            // HP line — append shield value if active
            StatusEffectInstance shield = _character.GetEffect(StatusEffect.Shield);
            string hpPrefix = _useSpriteIcons ? IconConfig.HPIcon : "HP";
            if (shield != null)
                _hpText.text = $"{hpPrefix} {_character.CurrentHP}/{_character.MaxHP} +{shield.Value}";
            else
                _hpText.text = $"{hpPrefix} {_character.CurrentHP}/{_character.MaxHP}";

            string enPrefix = _useSpriteIcons ? IconConfig.EnergyIcon : "EN";
            _energyText.text = $"{enPrefix} {_character.CurrentEnergy}/{_character.MaxEnergy}";

            string mpPrefix = _useSpriteIcons ? IconConfig.ManaIcon : "MP";
            _manaText.text = $"{mpPrefix} {_character.CurrentMana}/{_character.MaxMana}";

            // Status indicators on class/level line
            string baseStr = _character.Side == TeamSide.Player
                ? $"{_character.Data.Class} Lv{_character.Data.Level}"
                : $"Lv{_character.Data.Level}";

            string indicators = BuildStatusIndicators();
            _classLevelText.text = indicators.Length > 0 ? $"{baseStr} {indicators}" : baseStr;
        }

        public void SetAggroPercent(float percent)
        {
            if (_isDead)
            {
                _aggroText.text = "";
                return;
            }
            _aggroText.text = $"{Mathf.RoundToInt(percent * 100)}%";
        }

        public void SetClickable(bool clickable)
        {
            if (_isDead) return;
            _button.interactable = clickable;
        }

        public void SetTargetable(bool targetable)
        {
            if (_isDead) return;
            _button.interactable = targetable;
            SetBorderColor(targetable ? UIStyleConfig.TargetHighlight : UIStyleConfig.PanelBorder);
        }

        public void SetHighlight(bool highlighted)
        {
            if (_isDead) return;
            SetBorderColor(highlighted ? UIStyleConfig.ActiveTurnHighlight : UIStyleConfig.PanelBorder);
        }

        public void SetStagedHighlight(bool staged)
        {
            if (_isDead) return;
            SetBorderColor(staged ? UIStyleConfig.StagedHighlight : UIStyleConfig.PanelBorder);
        }

        /// <summary>
        /// Makes a dead card targetable for Resurrect-type abilities.
        /// Bypasses the _isDead check that normally blocks interaction.
        /// </summary>
        public void SetResurrectable(bool resurrectable)
        {
            if (!_isDead) return;
            _button.interactable = resurrectable;
            SetBorderColor(resurrectable ? UIStyleConfig.TargetHighlight : UIStyleConfig.DeathBorderColor);
        }

        public void SetBorderColor(Color color)
        {
            if (_borderImages == null) return;

            foreach (Image img in _borderImages)
            {
                if (img.gameObject.name.StartsWith("Border_"))
                {
                    img.color = color;
                }
            }
        }

        /// <summary>
        /// Kill all DOTween tweens targeting this card's components. Call before destroying.
        /// </summary>
        public void Cleanup()
        {
            DOTween.Kill(Root);
            if (_canvasGroup != null) DOTween.Kill(_canvasGroup);
            if (_bgImage != null) DOTween.Kill(_bgImage);
        }

        private void SetDead()
        {
            _isDead = true;
            _button.interactable = false;

            _nameText.color = UIStyleConfig.DeathTextColor;
            _classLevelText.text = "DEFEATED";
            _classLevelText.color = UIStyleConfig.DeathTextColor;
            _hpText.text = "HP 0";
            _hpText.color = UIStyleConfig.DeathTextColor;
            _energyText.text = "";
            _manaText.text = "";
            _aggroText.text = "";

            SetBorderColor(UIStyleConfig.DeathBorderColor);

            // Death animation: shrink + fade (handled by BattleAnimationController via event)
        }

        private static readonly StatusEffect[] DisplayedEffects =
        {
            StatusEffect.Shield, StatusEffect.Mark, StatusEffect.Taunt,
            StatusEffect.Hide, StatusEffect.Conceal,
            StatusEffect.Bleed, StatusEffect.Poison, StatusEffect.Burn,
            StatusEffect.Chilled, StatusEffect.Stun, StatusEffect.Silence,
            StatusEffect.Terror,
            StatusEffect.StanceDefensive, StatusEffect.StanceBrawling, StatusEffect.StanceBerserker,
            StatusEffect.Block, StatusEffect.Regeneration, StatusEffect.Blessing,
            StatusEffect.DivineIntervention, StatusEffect.Pin,
            StatusEffect.HuntersFocus, StatusEffect.IronWill,
            StatusEffect.FrozenTomb, StatusEffect.SoulLink,
            StatusEffect.DrainSoul, StatusEffect.Trap,
            StatusEffect.Envenom, StatusEffect.Imbue,
            StatusEffect.UltimateReflexes, StatusEffect.Caltrops,
            StatusEffect.Levitate, StatusEffect.Confusion,
            StatusEffect.CorpseExplosion, StatusEffect.LeechLife,
            StatusEffect.Bodyguard, StatusEffect.ElementalArmor,
        };

        private string BuildStatusIndicators()
        {
            // Use sprite icons if sprite asset is available, fall back to text codes
            bool useSprites = _useSpriteIcons;
            string s = "";

            foreach (StatusEffect effect in DisplayedEffects)
            {
                if (!_character.HasEffect(effect)) continue;

                if (useSprites)
                {
                    s += IconConfig.GetStatusSprite(effect);
                }
                else
                {
                    s += GetFallbackCode(effect);
                }
            }

            return s;
        }

        private static string GetFallbackCode(StatusEffect effect)
        {
            return effect switch
            {
                StatusEffect.Shield => "[S]",
                StatusEffect.Mark => "[M]",
                StatusEffect.Taunt => "[T]",
                StatusEffect.Hide => "[H]",
                StatusEffect.Conceal => "[C]",
                StatusEffect.Bleed => "[B]",
                StatusEffect.Poison => "[Po]",
                StatusEffect.Burn => "[Fi]",
                StatusEffect.Chilled => "[Ch]",
                StatusEffect.Stun => "[!]",
                StatusEffect.Silence => "[X]",
                StatusEffect.Terror => "[!!]",
                StatusEffect.StanceDefensive => "[Df]",
                StatusEffect.StanceBrawling => "[Br]",
                StatusEffect.StanceBerserker => "[Bk]",
                StatusEffect.Block => "[Bl]",
                StatusEffect.Regeneration => "[Re]",
                StatusEffect.Blessing => "[+]",
                StatusEffect.DivineIntervention => "[DI]",
                StatusEffect.Pin => "[Pi]",
                StatusEffect.HuntersFocus => "[HF]",
                StatusEffect.IronWill => "[IW]",
                StatusEffect.FrozenTomb => "[FT]",
                StatusEffect.SoulLink => "[SL]",
                StatusEffect.DrainSoul => "[DS]",
                StatusEffect.Trap => "[Tr]",
                StatusEffect.Envenom => "[Ev]",
                StatusEffect.Imbue => "[Im]",
                StatusEffect.UltimateReflexes => "[UR]",
                StatusEffect.Caltrops => "[Ct]",
                StatusEffect.Levitate => "[Lv]",
                StatusEffect.Confusion => "[??]",
                StatusEffect.CorpseExplosion => "[CE]",
                StatusEffect.LeechLife => "[LL]",
                StatusEffect.Bodyguard => "[BG]",
                StatusEffect.ElementalArmor => "[EA]",
                _ => ""
            };
        }
    }
}
