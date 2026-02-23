using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class PostBattleScreen : IScreen
    {
        private GameObject _root;
        private RectTransform _contentRect;
        private bool _continuePressed;

        public bool ContinuePressed => _continuePressed;

        private PostBattleResult _result;
        private List<CharacterData> _party;

        public PostBattleScreen(PostBattleResult result, List<CharacterData> party)
        {
            _result = result;
            _party = party;
        }

        public void Build(Transform canvasParent)
        {
            _continuePressed = false;

            _root = new GameObject("PostBattleScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header: VICTORY ---
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                "VICTORY", UIStyleConfig.FontSizeLarge * 1.2f,
                TextAlignmentOptions.Center, UIStyleConfig.AccentGreen);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.1f, 0.88f, 0.9f, 0.96f);

            // --- Gold earned ---
            TextMeshProUGUI goldText = PanelBuilder.CreateText("GoldEarned", rootRect,
                $"+{_result.GoldEarned} Gold", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentYellow);
            RectTransform goldRect = goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.1f, 0.82f, 0.9f, 0.88f);

            // --- Scrollable content area for character results ---
            RectTransform scrollPanel = PanelBuilder.CreateContainer("ScrollArea", rootRect);
            PanelBuilder.SetAnchored(scrollPanel, 0.05f, 0.14f, 0.95f, 0.80f);

            var (scrollRect, content) = PanelBuilder.CreateVerticalScrollView("ResultsScroll", scrollPanel);
            _contentRect = content;

            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = UIStyleConfig.ElementSpacing * 2;
            layout.padding = new RectOffset(8, 8, 4, 4);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            // --- Character results ---
            for (int i = 0; i < _party.Count; i++)
            {
                BuildCharacterResult(_party[i], _result.LevelUpResults[i]);
            }

            // --- Fallen characters ---
            foreach (CharacterData fallen in _result.FallenCharacters)
            {
                BuildFallenEntry(fallen);
            }

            // --- Continue button ---
            Button continueBtn = PanelBuilder.CreateButton("ContinueButton", rootRect,
                "CONTINUE", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
            RectTransform btnRect = continueBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.3f, 0.03f, 0.7f, 0.12f);
            continueBtn.onClick.AddListener(() => _continuePressed = true);
        }

        private void BuildCharacterResult(CharacterData character, LevelingSystem.LevelUpResult levelUp)
        {
            // Container with fixed height
            RectTransform container = PanelBuilder.CreatePanel("CharResult_" + character.Name, _contentRect);
            LayoutElement le = container.gameObject.AddComponent<LayoutElement>();

            bool leveledUp = levelUp.NewLevel > levelUp.OldLevel;
            bool hasNewAbilities = levelUp.NewAbilities.Count > 0 || levelUp.NewPassives.Count > 0;
            float height = 60f;
            if (leveledUp) height += 30f;
            if (hasNewAbilities) height += 20f * (levelUp.NewAbilities.Count + levelUp.NewPassives.Count);
            le.preferredHeight = height;

            Color classColor = UIFormatUtil.GetClassColor(character.Class);

            // Name + class
            TextMeshProUGUI nameText = PanelBuilder.CreateText("Name", container,
                $"{character.Name} ({character.Class})", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, classColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.02f, 0.75f, 0.65f, 1f, 4f);

            // XP info
            int xpNeeded = StatCalculator.CalculateXPToLevel(character.Level);
            string xpString = $"+{_result.XPPerCharacter}XP  [{character.CurrentXP}/{xpNeeded}]";
            TextMeshProUGUI xpText = PanelBuilder.CreateText("XP", container,
                xpString, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.AccentCyan);
            RectTransform xpRect = xpText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(xpRect, 0.50f, 0.75f, 0.98f, 1f, 0, 0, -4f);

            // XP bar
            Image xpBar = PanelBuilder.CreateBar("XPBar", container,
                UIStyleConfig.AccentCyan, new Color(0.05f, 0.15f, 0.2f, 1f));
            RectTransform barBgRect = xpBar.transform.parent.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(barBgRect, 0.02f, 0.60f, 0.98f, 0.72f, 4f, 0, -4f);

            float xpFill = xpNeeded > 0 ? (float)character.CurrentXP / xpNeeded : 0f;
            RectTransform fillRect = xpBar.GetComponent<RectTransform>();
            fillRect.anchorMax = new Vector2(Mathf.Clamp01(xpFill), 1f);

            float yOffset = 0.55f;

            // Level up notification
            if (leveledUp)
            {
                string lvlText = $"LEVEL UP! {levelUp.OldLevel} > {levelUp.NewLevel}";
                TextMeshProUGUI lvl = PanelBuilder.CreateText("LevelUp", container,
                    lvlText, UIStyleConfig.FontSizeSmall,
                    TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentGreen);
                RectTransform lvlRect = lvl.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(lvlRect, 0.02f, yOffset - 0.15f, 0.5f, yOffset, 4f);

                // Stat gains summary
                string gains = FormatStatGains(levelUp.StatGains);
                if (gains.Length > 0)
                {
                    TextMeshProUGUI gainText = PanelBuilder.CreateText("StatGains", container,
                        gains, UIStyleConfig.FontSizeTiny,
                        TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
                    RectTransform gainRect = gainText.GetComponent<RectTransform>();
                    PanelBuilder.SetAnchored(gainRect, 0.45f, yOffset - 0.15f, 0.98f, yOffset, 0, 0, -4f);
                }

                yOffset -= 0.18f;
            }

            // New abilities
            foreach (AbilityData ability in levelUp.NewAbilities)
            {
                TextMeshProUGUI abilityText = PanelBuilder.CreateText("NewAbility", container,
                    $"+ {ability.Name}", UIStyleConfig.FontSizeTiny,
                    TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentMagenta);
                RectTransform abilityRect = abilityText.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(abilityRect, 0.04f, yOffset - 0.12f, 0.98f, yOffset, 4f);
                yOffset -= 0.14f;
            }

            // New passives
            foreach (AbilityData passive in levelUp.NewPassives)
            {
                TextMeshProUGUI passiveText = PanelBuilder.CreateText("NewPassive", container,
                    $"+ {passive.Name} (Passive)", UIStyleConfig.FontSizeTiny,
                    TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
                RectTransform passiveRect = passiveText.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(passiveRect, 0.04f, yOffset - 0.12f, 0.98f, yOffset, 4f);
                yOffset -= 0.14f;
            }
        }

        private void BuildFallenEntry(CharacterData fallen)
        {
            RectTransform container = PanelBuilder.CreatePanel("Fallen_" + fallen.Name, _contentRect);
            LayoutElement le = container.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 36f;

            // Change border color to death color
            foreach (Image img in container.GetComponentsInChildren<Image>())
            {
                if (img.gameObject != container.gameObject)
                    img.color = UIStyleConfig.DeathBorderColor;
            }

            Color classColor = UIFormatUtil.GetClassColor(fallen.Class);
            TextMeshProUGUI nameText = PanelBuilder.CreateText("FallenName", container,
                $"{fallen.Name} ({fallen.Class}) - FALLEN", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.DeathTextColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetFill(nameRect, 4f);
        }

        private static string FormatStatGains(CharacterStats gains)
        {
            List<string> parts = new();
            if (gains.Endurance > 0) parts.Add($"END+{gains.Endurance}");
            if (gains.Stamina > 0) parts.Add($"STA+{gains.Stamina}");
            if (gains.Intellect > 0) parts.Add($"INT+{gains.Intellect}");
            if (gains.Strength > 0) parts.Add($"STR+{gains.Strength}");
            if (gains.Dexterity > 0) parts.Add($"DEX+{gains.Dexterity}");
            if (gains.Willpower > 0) parts.Add($"WIL+{gains.Willpower}");
            if (gains.Armor > 0) parts.Add($"ARM+{gains.Armor}");
            if (gains.MagicResist > 0) parts.Add($"MRS+{gains.MagicResist}");
            if (gains.Initiative > 0) parts.Add($"INI+{gains.Initiative}");
            return string.Join(" ", parts);
        }

        public void Show()
        {
            _continuePressed = false;
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        public void Destroy()
        {
            if (_root != null) Object.Destroy(_root);
        }
    }
}
