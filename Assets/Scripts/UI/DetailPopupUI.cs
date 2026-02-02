using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class DetailPopupUI
    {
        private RectTransform _root;
        private RectTransform _popupPanel;
        private RectTransform _contentArea;
        private readonly List<GameObject> _contentChildren = new();

        public bool IsVisible => _root != null && _root.gameObject.activeSelf;

        public void Build(Transform canvasTransform)
        {
            // Root fills canvas, hidden by default
            GameObject rootGo = new GameObject("DetailPopup");
            _root = rootGo.AddComponent<RectTransform>();
            _root.SetParent(canvasTransform, false);
            PanelBuilder.SetFill(_root);
            _root.gameObject.SetActive(false);

            // Blocker (full-screen semi-transparent, catches taps to dismiss)
            GameObject blockerGo = new GameObject("Blocker");
            RectTransform blockerRect = blockerGo.AddComponent<RectTransform>();
            blockerRect.SetParent(_root, false);
            PanelBuilder.SetFill(blockerRect);
            Image blockerImage = blockerGo.AddComponent<Image>();
            blockerImage.color = UIStyleConfig.PopupBlockerColor;
            Button blockerButton = blockerGo.AddComponent<Button>();
            blockerButton.transition = Selectable.Transition.None;
            blockerButton.onClick.AddListener(Hide);

            // Popup panel (centered)
            float w = UIStyleConfig.PopupWidthRatio;
            float h = UIStyleConfig.PopupHeightRatio;
            float xMin = (1f - w) / 2f;
            float yMin = (1f - h) / 2f;

            _popupPanel = PanelBuilder.CreatePanel("PopupPanel", _root);
            PanelBuilder.SetAnchored(_popupPanel, xMin, yMin, xMin + w, yMin + h);

            // Popup also dismisses on tap
            Button popupButton = _popupPanel.gameObject.AddComponent<Button>();
            popupButton.transition = Selectable.Transition.None;
            popupButton.onClick.AddListener(Hide);

            // Content area (padded interior)
            float padding = UIStyleConfig.PopupPadding;
            _contentArea = PanelBuilder.CreateContainer("Content", _popupPanel);
            PanelBuilder.SetFill(_contentArea, padding);
        }

        public void ShowCharacterPopup(BattleCharacter character)
        {
            ClearContent();

            CharacterData data = character.Data;
            CharacterStats stats = character.EffectiveStats;

            // Header: "Name - Lv1 Warrior"
            string header = $"{data.Name} - Lv{data.Level} {data.Class}";
            Color classColor = GetClassColor(data.Class);
            AddText(header, UIStyleConfig.FontSizeSmall, classColor, 0.88f, 1f);

            // Resources
            AddText($"HP: {character.CurrentHP}/{character.MaxHP}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentRed, 0.78f, 0.86f);
            AddText($"EN: {character.CurrentEnergy}/{character.MaxEnergy}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentYellow, 0.70f, 0.78f);
            AddText($"MP: {character.CurrentMana}/{character.MaxMana}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentCyan, 0.62f, 0.70f);

            // Stats grid (3 rows)
            AddText($"END:{stats.Endurance}  STA:{stats.Stamina}  INT:{stats.Intellect}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.52f, 0.60f);
            AddText($"STR:{stats.Strength}  DEX:{stats.Dexterity}  WIL:{stats.Willpower}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.44f, 0.52f);
            AddText($"ARM:{stats.Armor}  MRS:{stats.MagicResist}  INI:{stats.Initiative}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.36f, 0.44f);

            // Position + actions
            string posText = $"{character.Row}-{character.Column}  Actions: {character.LongActionsRemaining}L {character.ShortActionsRemaining}S";
            AddText(posText, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, 0.28f, 0.36f);

            // Abilities list
            AddText("-- Abilities --", UIStyleConfig.FontSizeTiny, UIStyleConfig.AccentMagenta, 0.20f, 0.28f);

            List<AbilityData> abilities = data.Abilities.FindAll(a => !a.IsPassive);
            float yTop = 0.20f;
            float lineHeight = 0.06f;
            int maxAbilities = Mathf.Min(abilities.Count, 4);

            for (int i = 0; i < maxAbilities; i++)
            {
                AbilityData ability = abilities[i];
                string cost = GetAbilityCostShort(ability);
                string line = $"{ability.Name} {cost}";
                float yMax = yTop - i * lineHeight;
                float yMinLine = yMax - lineHeight;
                AddText(line, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, yMinLine, yMax);
            }

            Show();
        }

        public void ShowAbilityPopup(AbilityData ability)
        {
            ClearContent();

            // Header: ability name
            AddText(ability.Name, UIStyleConfig.FontSizeSmall, UIStyleConfig.AccentCyan, 0.88f, 1f);

            // Description (wrapped)
            TextMeshProUGUI descText = AddText(ability.Description, UIStyleConfig.FontSizeTiny,
                UIStyleConfig.TextPrimary, 0.62f, 0.86f);
            descText.textWrappingMode = TextWrappingModes.Normal;
            descText.overflowMode = TextOverflowModes.Truncate;

            // Properties
            float y = 0.58f;
            float lineH = 0.07f;

            // Action cost
            string actionStr = ability.ActionCost == ActionPointType.Long
                ? $"Action: Long ({ability.LongPointCost} pt)"
                : $"Action: Quick ({ability.ShortPointCost} pt)";
            AddText(actionStr, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, y - lineH, y);
            y -= lineH;

            // Resource costs (only show non-zero)
            if (ability.EnergyCost > 0)
            {
                AddText($"Energy Cost: {ability.EnergyCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentYellow, y - lineH, y);
                y -= lineH;
            }

            if (ability.ManaCost > 0)
            {
                AddText($"Mana Cost: {ability.ManaCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentCyan, y - lineH, y);
                y -= lineH;
            }

            if (ability.HPCost > 0)
            {
                AddText($"HP Cost: {ability.HPCost}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentRed, y - lineH, y);
                y -= lineH;
            }

            // Damage/healing info
            if (ability.BasePower > 0)
            {
                AddText($"Power: {ability.BasePower}  Type: {ability.DamageType}",
                    UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, y - lineH, y);
                y -= lineH;
            }

            if (ability.Element != Element.None)
            {
                AddText($"Element: {ability.Element}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.AccentMagenta, y - lineH, y);
                y -= lineH;
            }

            if (ability.HitCount > 1)
            {
                AddText($"Hits: {ability.HitCount}", UIStyleConfig.FontSizeTiny,
                    UIStyleConfig.TextPrimary, y - lineH, y);
                y -= lineH;
            }

            // Target type
            string targetStr = FormatTargetType(ability.TargetType);
            AddText($"Target: {targetStr}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.TextDimmed, y - lineH, y);

            Show();
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        private void Show()
        {
            if (_root != null)
            {
                _root.SetAsLastSibling();
                _root.gameObject.SetActive(true);
            }
        }

        private void ClearContent()
        {
            foreach (GameObject child in _contentChildren)
            {
                Object.Destroy(child);
            }
            _contentChildren.Clear();
        }

        private TextMeshProUGUI AddText(string content, float fontSize, Color color,
            float anchorMinY, float anchorMaxY)
        {
            TextMeshProUGUI text = PanelBuilder.CreateText("PopupText", _contentArea, content,
                fontSize, TextAlignmentOptions.TopLeft, color);
            RectTransform rect = text.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0, anchorMinY, 1, anchorMaxY);
            _contentChildren.Add(text.gameObject);
            return text;
        }

        private static string GetAbilityCostShort(AbilityData ability)
        {
            if (ability.EnergyCost > 0) return $"[{ability.EnergyCost}E]";
            if (ability.ManaCost > 0) return $"[{ability.ManaCost}M]";
            if (ability.HPCost > 0) return $"[{ability.HPCost}HP]";
            if (ability.ActionCost == ActionPointType.Short) return "[Q]";
            return "";
        }

        private static string FormatTargetType(TargetType targetType)
        {
            return targetType switch
            {
                TargetType.SingleEnemy => "Single Enemy",
                TargetType.SingleAlly => "Single Ally",
                TargetType.Self => "Self",
                TargetType.AllEnemies => "All Enemies",
                TargetType.AllAllies => "All Allies",
                TargetType.All => "Everyone",
                _ => targetType.ToString()
            };
        }

        private static Color GetClassColor(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => UIStyleConfig.AccentRed,
                CharacterClass.Rogue => UIStyleConfig.AccentGreen,
                CharacterClass.Ranger => UIStyleConfig.AccentYellow,
                CharacterClass.Priest => UIStyleConfig.TextPrimary,
                CharacterClass.Wizard => UIStyleConfig.AccentCyan,
                CharacterClass.Warlock => UIStyleConfig.AccentMagenta,
                _ => UIStyleConfig.TextPrimary
            };
        }
    }
}
