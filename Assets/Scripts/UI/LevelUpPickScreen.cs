using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class LevelUpPickScreen : IScreen
    {
        private GameObject _root;
        private bool _done;
        private LevelingSystem.LevelUpResult _result;

        public bool Done => _done;
        public LevelingSystem.LevelUpResult Result => _result;

        private List<CharacterData> _party;
        private List<int> _candidateIndices;

        public LevelUpPickScreen(List<CharacterData> party, List<int> candidateIndices)
        {
            _party = party;
            _candidateIndices = candidateIndices;
        }

        public void Build(Transform canvasParent)
        {
            _done = false;

            _root = new GameObject("LevelUpPickScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                "LEVEL UP", UIStyleConfig.FontSizeLarge * 1.2f,
                TextAlignmentOptions.Center, UIStyleConfig.AccentGreen);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.1f, 0.88f, 0.9f, 0.96f);

            TextMeshProUGUI subHeader = PanelBuilder.CreateText("SubHeader", rootRect,
                "Choose a character to level up", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform subRect = subHeader.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(subRect, 0.1f, 0.83f, 0.9f, 0.88f);

            // --- Candidate cards ---
            int count = _candidateIndices.Count;
            float cardWidth = 0.42f;
            float cardGap = 0.04f;
            float totalWidth = count * cardWidth + (count - 1) * cardGap;
            float startX = (1f - totalWidth) / 2f;

            for (int i = 0; i < count; i++)
            {
                float left = startX + i * (cardWidth + cardGap);
                float right = left + cardWidth;
                int partyIndex = _candidateIndices[i];
                BuildCandidateCard(rootRect, _party[partyIndex], partyIndex, left, right);
            }
        }

        private void BuildCandidateCard(RectTransform parent, CharacterData character, int partyIndex,
            float left, float right)
        {
            RectTransform card = PanelBuilder.CreatePanel("Candidate_" + partyIndex, parent);
            PanelBuilder.SetAnchored(card, left, 0.10f, right, 0.80f);

            Color classColor = UIFormatUtil.GetClassColor(character.Class);

            // Name + Class + Level
            TextMeshProUGUI nameText = PanelBuilder.CreateText("Name", card,
                character.Name, UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.Center, classColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.04f, 0.88f, 0.96f, 0.97f);

            TextMeshProUGUI classText = PanelBuilder.CreateText("Class", card,
                $"{character.Class} Lv.{character.Level}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, classColor);
            RectTransform classRect = classText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(classRect, 0.04f, 0.80f, 0.96f, 0.88f);

            // --- Current stats ---
            CharacterStats stats = character.GetTotalStats();
            float y = 0.73f;

            BuildStatLine(card, "Stat0", $"END:{stats.Endurance}  STR:{stats.Strength}", y); y -= 0.07f;
            BuildStatLine(card, "Stat1", $"STA:{stats.Stamina}  DEX:{stats.Dexterity}", y); y -= 0.07f;
            BuildStatLine(card, "Stat2", $"INT:{stats.Intellect}  WIL:{stats.Willpower}", y); y -= 0.07f;
            BuildStatLine(card, "Stat3", $"ARM:{stats.Armor}  MRS:{stats.MagicResist}", y); y -= 0.07f;

            // --- HP/EN/MP ---
            int maxHP = StatCalculator.CalculateMaxHP(stats);
            int maxEN = StatCalculator.CalculateMaxEnergy(stats);
            int maxMP = StatCalculator.CalculateMaxMana(stats);
            BuildStatLine(card, "Resources",
                $"HP:{character.CurrentHP}/{maxHP}  EN:{maxEN}  MP:{maxMP}", y);
            y -= 0.09f;

            // --- Preview: next ability/passive unlock ---
            string preview = GetNextUnlockPreview(character);
            if (preview != null)
            {
                TextMeshProUGUI previewText = PanelBuilder.CreateText("Preview", card,
                    preview, UIStyleConfig.FontSizeTiny,
                    TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentMagenta);
                RectTransform previewRect = previewText.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(previewRect, 0.06f, y - 0.08f, 0.96f, y);
                y -= 0.10f;
            }

            // --- Level Up button ---
            Button levelBtn = PanelBuilder.CreateButton("LevelBtn", card,
                $"LEVEL UP  > {character.Level + 1}", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
            RectTransform levelRect = levelBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(levelRect, 0.08f, 0.03f, 0.92f, 0.14f);

            int capturedIndex = partyIndex;
            levelBtn.onClick.AddListener(() => OnLevelUp(capturedIndex));
        }

        private static void BuildStatLine(RectTransform parent, string name, string text, float top)
        {
            TextMeshProUGUI tmp = PanelBuilder.CreateText(name, parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0.06f, top - 0.06f, 0.96f, top);
        }

        private static string GetNextUnlockPreview(CharacterData character)
        {
            int nextLevel = character.Level + 1;

            // Check ability unlock at next level
            List<AbilityData> classAbilities = ClassDefinitions.GetClassAbilityList(character.Class);
            int currentAbilityCount = LevelingSystem.GetAbilityCountAtLevel(character.Level);
            int nextAbilityCount = LevelingSystem.GetAbilityCountAtLevel(nextLevel);

            if (nextAbilityCount > currentAbilityCount && currentAbilityCount < classAbilities.Count)
                return $"Unlocks: {classAbilities[currentAbilityCount].Name}";

            // Check passive unlock at next level
            if (nextLevel % 5 == 0)
            {
                List<AbilityData> classPassives = ClassDefinitions.GetClassPassiveList(character.Class);
                int passiveIndex = nextLevel / 5 - 1;
                if (passiveIndex >= 0 && passiveIndex < classPassives.Count)
                    return $"Unlocks: {classPassives[passiveIndex].Name} (P)";
            }

            return null;
        }

        private void OnLevelUp(int partyIndex)
        {
            if (_done) return;
            _result = LevelingSystem.LevelUp(_party[partyIndex]);
            _done = true;
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
