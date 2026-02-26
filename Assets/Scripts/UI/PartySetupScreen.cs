using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class PartySetupScreen : IScreen
    {
        private GameObject _root;
        private bool _done;
        private List<CharacterClass> _selectedClasses = new();

        public bool Done => _done;
        public List<CharacterClass> SelectedClasses => _selectedClasses;

        private static readonly CharacterClass[] AllClasses =
        {
            CharacterClass.Warrior, CharacterClass.Rogue, CharacterClass.Ranger,
            CharacterClass.Priest, CharacterClass.Elementalist, CharacterClass.Warlock
        };

        private Dictionary<CharacterClass, Image> _cardBackgrounds = new();
        private Dictionary<CharacterClass, Image[]> _cardBorderImages = new();
        private TextMeshProUGUI _counterText;
        private Button _beginButton;
        private TextMeshProUGUI _beginButtonText;

        public void Build(Transform canvasParent)
        {
            _done = false;
            _selectedClasses.Clear();

            _root = new GameObject("PartySetupScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                "CHOOSE YOUR PARTY", UIStyleConfig.FontSizeLarge,
                TextAlignmentOptions.Center, UIStyleConfig.AccentCyan);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.05f, 0.90f, 0.95f, 0.98f);

            // --- Subtitle ---
            TextMeshProUGUI subtitle = PanelBuilder.CreateText("Subtitle", rootRect,
                $"Select {RunConfig.StartingPartySize} classes", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform subRect = subtitle.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(subRect, 0.05f, 0.85f, 0.95f, 0.90f);

            // --- 3x2 grid of class cards ---
            float cardWidth = 0.29f;
            float cardHeight = 0.34f;
            float gapX = 0.03f;
            float gapY = 0.03f;
            float gridLeft = (1f - (3f * cardWidth + 2f * gapX)) / 2f;
            float gridTop = 0.83f;

            for (int i = 0; i < AllClasses.Length; i++)
            {
                int col = i % 3;
                int row = i / 3;

                float left = gridLeft + col * (cardWidth + gapX);
                float right = left + cardWidth;
                float top = gridTop - row * (cardHeight + gapY);
                float bottom = top - cardHeight;

                BuildClassCard(rootRect, AllClasses[i], left, bottom, right, top);
            }

            // --- Counter ---
            _counterText = PanelBuilder.CreateText("Counter", rootRect,
                $"Selected: 0/{RunConfig.StartingPartySize}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform counterRect = _counterText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(counterRect, 0.25f, 0.10f, 0.75f, 0.16f);

            // --- Begin button ---
            _beginButton = PanelBuilder.CreateButton("BeginButton", rootRect,
                "BEGIN", UIStyleConfig.TextDimmed, UIStyleConfig.FontSizeMedium);
            RectTransform btnRect = _beginButton.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.30f, 0.02f, 0.70f, 0.10f);
            _beginButton.interactable = false;
            _beginButtonText = _beginButton.GetComponentInChildren<TextMeshProUGUI>();
            _beginButton.onClick.AddListener(OnBeginPressed);
        }

        private void BuildClassCard(RectTransform parent, CharacterClass characterClass,
            float left, float bottom, float right, float top)
        {
            RectTransform card = PanelBuilder.CreatePanel("Card_" + characterClass, parent);
            PanelBuilder.SetAnchored(card, left, bottom, right, top);

            Image bgImage = card.GetComponent<Image>();
            _cardBackgrounds[characterClass] = bgImage;

            // Collect border edge images for color changes
            List<Image> borders = new();
            foreach (Transform child in card)
            {
                if (child.name.StartsWith("Border_"))
                    borders.Add(child.GetComponent<Image>());
            }
            _cardBorderImages[characterClass] = borders.ToArray();

            Color classColor = UIFormatUtil.GetClassColor(characterClass);

            // Class name
            TextMeshProUGUI nameText = PanelBuilder.CreateText("ClassName", card,
                characterClass.ToString(), UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.Center, classColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.04f, 0.82f, 0.96f, 0.96f);

            // Key stats
            CharacterStats stats = ClassDefinitions.GetBaseStats(characterClass);
            string statLine1 = $"END:{stats.Endurance} STR:{stats.Strength}";
            string statLine2 = $"DEX:{stats.Dexterity} WIL:{stats.Willpower}";
            string statLine3 = $"ARM:{stats.Armor} INI:{stats.Initiative}";

            BuildStatLine(card, "Stat0", statLine1, 0.72f);
            BuildStatLine(card, "Stat1", statLine2, 0.62f);
            BuildStatLine(card, "Stat2", statLine3, 0.52f);

            // Starting ability
            string abilityName = GetStartingAbilityName(characterClass);
            TextMeshProUGUI abilText = PanelBuilder.CreateText("Ability", card,
                abilityName, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform abilRect = abilText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(abilRect, 0.06f, 0.30f, 0.96f, 0.42f);

            // Starting passive
            string passiveName = GetStartingPassiveName(characterClass);
            TextMeshProUGUI passiveText = PanelBuilder.CreateText("Passive", card,
                $"{passiveName} (P)", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
            RectTransform passiveRect = passiveText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(passiveRect, 0.06f, 0.18f, 0.96f, 0.30f);

            // Clickable button overlay
            Button btn = card.gameObject.AddComponent<Button>();
            btn.targetGraphic = bgImage;
            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            btn.colors = colors;

            CharacterClass captured = characterClass;
            btn.onClick.AddListener(() => ToggleSelection(captured));
        }

        private static void BuildStatLine(RectTransform parent, string name, string text, float top)
        {
            TextMeshProUGUI tmp = PanelBuilder.CreateText(name, parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0.06f, top - 0.10f, 0.96f, top);
        }

        private void ToggleSelection(CharacterClass characterClass)
        {
            if (_selectedClasses.Contains(characterClass))
            {
                _selectedClasses.Remove(characterClass);
                UpdateCardVisual(characterClass, false);
            }
            else
            {
                if (_selectedClasses.Count >= RunConfig.StartingPartySize)
                    return;

                _selectedClasses.Add(characterClass);
                UpdateCardVisual(characterClass, true);
            }

            UpdateUI();
        }

        private void UpdateCardVisual(CharacterClass characterClass, bool selected)
        {
            if (_cardBorderImages.TryGetValue(characterClass, out Image[] borderImages))
            {
                Color borderColor = selected ? UIStyleConfig.AccentGreen : UIStyleConfig.PanelBorder;
                foreach (Image img in borderImages)
                    img.color = borderColor;
            }

            if (_cardBackgrounds.TryGetValue(characterClass, out Image bg))
            {
                bg.color = selected
                    ? new Color(0f, 0.15f, 0f, 1f)
                    : UIStyleConfig.PanelBackground;
            }
        }

        private void UpdateUI()
        {
            int count = _selectedClasses.Count;
            int required = RunConfig.StartingPartySize;

            _counterText.text = $"Selected: {count}/{required}";
            _counterText.color = count == required ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;

            bool canBegin = count == required;
            _beginButton.interactable = canBegin;
            if (_beginButtonText != null)
                _beginButtonText.color = canBegin ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
        }

        private void OnBeginPressed()
        {
            if (_selectedClasses.Count == RunConfig.StartingPartySize)
                _done = true;
        }

        private static string GetStartingAbilityName(CharacterClass characterClass)
        {
            List<AbilityData> abilities = ClassDefinitions.GetClassAbilityList(characterClass);
            return abilities.Count > 0 ? abilities[0].Name : "None";
        }

        private static string GetStartingPassiveName(CharacterClass characterClass)
        {
            AbilityData passive = ClassDefinitions.GetBasicPassive(characterClass);
            return passive.Name;
        }

        public void Show()
        {
            _done = false;
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
