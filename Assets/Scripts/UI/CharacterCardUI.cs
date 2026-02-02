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
        private Image _hpFill;
        private Image _energyFill;
        private Image _manaFill;
        private RectTransform _hpBarBg;
        private RectTransform _energyBarBg;
        private RectTransform _manaBarBg;
        private BattleCharacter _character;
        private Button _button;
        private Image[] _borderImages;

        public void Build(Transform parent, BattleCharacter character)
        {
            _character = character;

            Root = PanelBuilder.CreatePanel("Card_" + character.Data.Name, parent);

            // Add button component (disabled by default)
            _button = Root.gameObject.AddComponent<Button>();
            _button.interactable = false;

            // Long press detection (works regardless of button interactable state)
            LongPressHandler longPress = Root.gameObject.AddComponent<LongPressHandler>();
            longPress.OnLongPress += () => GameEvents.RaiseCharacterDetailRequested(_character);

            _button.onClick.AddListener(() =>
            {
                if (longPress.WasLongPress) return;
                OnCardClicked?.Invoke(_character);
            });

            // Cache border images for highlighting
            _borderImages = Root.GetComponentsInChildren<Image>();

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            // Name label
            _nameText = PanelBuilder.CreateText("Name", content, character.Data.Name,
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.TopLeft, UIFormatUtil.GetClassColor(character.Data.Class));
            RectTransform nameRect = _nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0, 0.65f, 1, 1);

            // HP bar
            _hpFill = PanelBuilder.CreateBar("HP", content,
                UIStyleConfig.HPBarColor, UIStyleConfig.HPBarBackground);
            _hpBarBg = _hpFill.transform.parent.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(_hpBarBg, 0, 0.45f, 1, 0.60f);

            // Energy bar
            _energyFill = PanelBuilder.CreateBar("Energy", content,
                UIStyleConfig.EnergyBarColor, UIStyleConfig.EnergyBarBackground);
            _energyBarBg = _energyFill.transform.parent.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(_energyBarBg, 0, 0.25f, 1, 0.40f);

            // Mana bar
            _manaFill = PanelBuilder.CreateBar("Mana", content,
                UIStyleConfig.ManaBarColor, UIStyleConfig.ManaBarBackground);
            _manaBarBg = _manaFill.transform.parent.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(_manaBarBg, 0, 0.05f, 1, 0.20f);

            Refresh();
        }

        public void Refresh()
        {
            if (_character == null) return;

            SetBarFill(_hpFill, _character.CurrentHP, _character.MaxHP);
            SetBarFill(_energyFill, _character.CurrentEnergy, _character.MaxEnergy);
            SetBarFill(_manaFill, _character.CurrentMana, _character.MaxMana);
        }

        public void SetTargetable(bool targetable)
        {
            _button.interactable = targetable;
            SetBorderColor(targetable ? UIStyleConfig.TargetHighlight : UIStyleConfig.PanelBorder);
        }

        public void SetHighlight(bool highlighted)
        {
            SetBorderColor(highlighted ? UIStyleConfig.ActiveTurnHighlight : UIStyleConfig.PanelBorder);
        }

        public void SetStagedHighlight(bool staged)
        {
            SetBorderColor(staged ? UIStyleConfig.StagedHighlight : UIStyleConfig.PanelBorder);
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

        private void SetBarFill(Image fill, int current, int max)
        {
            float ratio = max > 0 ? (float)current / max : 0f;
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMax = new Vector2(ratio, 1f);
        }

    }
}
