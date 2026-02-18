using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class CharacterCardUI
    {
        public RectTransform Root { get; private set; }
        public BattleCharacter Character => _character;

        public event Action<BattleCharacter> OnCardClicked;

        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _classLevelText;
        private TextMeshProUGUI _hpText;
        private TextMeshProUGUI _energyText;
        private TextMeshProUGUI _manaText;
        private TextMeshProUGUI _aggroText;
        private BattleCharacter _character;
        private Button _button;
        private Image[] _borderImages;
        private LongPressHandler _longPress;
        private bool _isDead;

        public void Build(Transform parent, BattleCharacter character)
        {
            _character = character;

            Root = PanelBuilder.CreatePanel("Card_" + character.Data.Name, parent);

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

            // HP (red)
            _hpText = PanelBuilder.CreateText("HP", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.HPBarColor);
            PanelBuilder.SetAnchored(_hpText.GetComponent<RectTransform>(), 0, 0.40f, 1, 0.60f);

            // Energy (yellow)
            _energyText = PanelBuilder.CreateText("Energy", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.EnergyBarColor);
            PanelBuilder.SetAnchored(_energyText.GetComponent<RectTransform>(), 0, 0.20f, 1, 0.40f);

            // Mana (cyan)
            _manaText = PanelBuilder.CreateText("Mana", content, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft,
                UIStyleConfig.ManaBarColor);
            PanelBuilder.SetAnchored(_manaText.GetComponent<RectTransform>(), 0, 0.00f, 1, 0.20f);

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

            if (_isDead) return;

            // HP line — append shield value if active
            StatusEffectInstance shield = _character.GetEffect(StatusEffect.Shield);
            if (shield != null)
                _hpText.text = $"HP {_character.CurrentHP}/{_character.MaxHP} +{shield.Value}";
            else
                _hpText.text = $"HP {_character.CurrentHP}/{_character.MaxHP}";

            _energyText.text = $"EN {_character.CurrentEnergy}/{_character.MaxEnergy}";
            _manaText.text = $"MP {_character.CurrentMana}/{_character.MaxMana}";

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
        }

        private string BuildStatusIndicators()
        {
            string s = "";
            if (_character.HasEffect(StatusEffect.Shield)) s += "[S]";
            if (_character.HasEffect(StatusEffect.Mark)) s += "[M]";
            if (_character.HasEffect(StatusEffect.Protect)) s += "[P]";
            if (_character.HasEffect(StatusEffect.Hide)) s += "[H]";
            return s;
        }

        private void SetBorderColor(Color color)
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
    }
}
