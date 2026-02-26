using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class RecruitScreen : IScreen
    {
        private GameObject _root;
        private bool _done;
        private CharacterData _recruitedCharacter;

        public bool Done => _done;
        public CharacterData RecruitedCharacter => _recruitedCharacter;

        private List<CharacterData> _candidates;
        private RunData _runData;

        public RecruitScreen(List<CharacterData> candidates, RunData runData)
        {
            _candidates = candidates;
            _runData = runData;
        }

        public void Build(Transform canvasParent)
        {
            _done = false;
            _recruitedCharacter = null;

            _root = new GameObject("RecruitScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                "RECRUITMENT", UIStyleConfig.FontSizeLarge,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.04f, 0.90f, 0.60f, 0.98f);

            // Gold display
            TextMeshProUGUI goldText = PanelBuilder.CreateText("Gold", rootRect,
                $"Party: {_runData.Party.Count}/4", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform goldRect = goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.55f, 0.90f, 0.96f, 0.98f);

            // --- Flavor text ---
            TextMeshProUGUI flavorText = PanelBuilder.CreateText("Flavor", rootRect,
                "\"Wanderers offer to join your cause.\"",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform flavorRect = flavorText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(flavorRect, 0.05f, 0.84f, 0.95f, 0.90f);

            // --- Candidate cards ---
            float cardWidth = 0.44f;
            float cardGap = 0.04f;
            float startX = (1f - (_candidates.Count * cardWidth + (_candidates.Count - 1) * cardGap)) / 2f;

            for (int i = 0; i < _candidates.Count; i++)
            {
                float left = startX + i * (cardWidth + cardGap);
                float right = left + cardWidth;
                BuildCandidateCard(rootRect, _candidates[i], i, left, right);
            }

            // --- Skip button ---
            Button skipBtn = PanelBuilder.CreateButton("SkipButton", rootRect,
                "SKIP", UIStyleConfig.TextDimmed, UIStyleConfig.FontSizeMedium);
            RectTransform skipRect = skipBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(skipRect, 0.35f, 0.03f, 0.65f, 0.12f);
            skipBtn.onClick.AddListener(() => _done = true);
        }

        private void BuildCandidateCard(RectTransform parent, CharacterData candidate, int index,
            float left, float right)
        {
            RectTransform card = PanelBuilder.CreatePanel("Candidate_" + index, parent);
            PanelBuilder.SetAnchored(card, left, 0.14f, right, 0.82f);

            Color classColor = UIFormatUtil.GetClassColor(candidate.Class);

            // Name + Class
            TextMeshProUGUI nameText = PanelBuilder.CreateText("Name", card,
                candidate.Name, UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.Center, classColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.04f, 0.88f, 0.96f, 0.98f);

            TextMeshProUGUI classText = PanelBuilder.CreateText("Class", card,
                candidate.Class.ToString(), UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, classColor);
            RectTransform classRect = classText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(classRect, 0.04f, 0.80f, 0.96f, 0.88f);

            // --- Base stats ---
            TextMeshProUGUI statsHeader = PanelBuilder.CreateText("StatsHeader", card,
                "-- Stats --", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform statsHeaderRect = statsHeader.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsHeaderRect, 0.04f, 0.72f, 0.96f, 0.79f);

            CharacterStats stats = candidate.BaseStats;

            BuildStatLine(card, "Stat0", $"END:{stats.Endurance}  STR:{stats.Strength}", 0.65f);
            BuildStatLine(card, "Stat1", $"STA:{stats.Stamina}  DEX:{stats.Dexterity}", 0.59f);
            BuildStatLine(card, "Stat2", $"INT:{stats.Intellect}  WIL:{stats.Willpower}", 0.53f);
            BuildStatLine(card, "Stat3", $"ARM:{stats.Armor}  MRS:{stats.MagicResist}", 0.47f);
            BuildStatLine(card, "Stat4", $"INI:{stats.Initiative}", 0.41f);

            // --- Starting ability ---
            TextMeshProUGUI abilHeader = PanelBuilder.CreateText("AbilHeader", card,
                "-- Abilities --", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform abilHeaderRect = abilHeader.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(abilHeaderRect, 0.04f, 0.33f, 0.96f, 0.40f);

            // Find the class ability (first non-generic, non-passive)
            string abilityName = "None";
            foreach (AbilityData ability in candidate.Abilities)
            {
                if (!ability.IsPassive && ability.Tag != AbilityTag.None
                    && ability.Tag != AbilityTag.Swap
                    && ability.Tag != AbilityTag.Anticipate
                    && ability.Tag != AbilityTag.React
                    && ability.Tag != AbilityTag.Taunt
                    && ability.Tag != AbilityTag.Hide
                    && ability.Tag != AbilityTag.Pass)
                {
                    abilityName = ability.Name;
                    break;
                }
            }

            TextMeshProUGUI abilText = PanelBuilder.CreateText("Ability", card,
                abilityName, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform abilRect = abilText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(abilRect, 0.06f, 0.26f, 0.96f, 0.33f);

            // Passive
            string passiveName = candidate.Passives.Count > 0 ? candidate.Passives[0].Name : "None";
            TextMeshProUGUI passiveText = PanelBuilder.CreateText("Passive", card,
                $"{passiveName} (P)", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
            RectTransform passiveRect = passiveText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(passiveRect, 0.06f, 0.19f, 0.96f, 0.26f);

            // --- Recruit button ---
            Button recruitBtn = PanelBuilder.CreateButton("RecruitBtn", card,
                "RECRUIT", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
            RectTransform recruitRect = recruitBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(recruitRect, 0.08f, 0.03f, 0.92f, 0.16f);

            int capturedIndex = index;
            recruitBtn.onClick.AddListener(() => OnRecruit(capturedIndex));
        }

        private static void BuildStatLine(RectTransform parent, string name, string text, float top)
        {
            TextMeshProUGUI tmp = PanelBuilder.CreateText(name, parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0.06f, top - 0.06f, 0.96f, top);
        }

        private void OnRecruit(int index)
        {
            if (_done) return;
            _recruitedCharacter = _candidates[index];
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
