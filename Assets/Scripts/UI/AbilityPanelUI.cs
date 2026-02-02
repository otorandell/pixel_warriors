using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class AbilityPanelUI
    {
        public RectTransform Root { get; private set; }

        private readonly List<Button> _tabButtons = new();
        private readonly List<Button> _abilityButtons = new();
        private readonly List<AbilityData> _abilityDataForButtons = new();
        private RectTransform _tabBar;
        private RectTransform _abilityListContent;
        private ScrollRect _abilityScrollRect;
        private BattleCharacter _activeCharacter;
        private AbilityTab _activeTab = AbilityTab.Attacks;
        private AbilityData _stagedAbility;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("AbilityPanel", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            // Tab bar at top
            _tabBar = PanelBuilder.CreateContainer("TabBar", content);
            PanelBuilder.SetAnchored(_tabBar, 0, 0.90f, 1, 1);
            BuildTabs();

            // Scrollable ability list below
            RectTransform scrollArea = PanelBuilder.CreateContainer("AbilityListArea", content);
            PanelBuilder.SetAnchored(scrollArea, 0, 0, 1, 0.88f);
            (_abilityScrollRect, _abilityListContent) = PanelBuilder.CreateVerticalScrollView("AbilityScroll", scrollArea);

            VerticalLayoutGroup layoutGroup = _abilityListContent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 2;
            layoutGroup.padding = new RectOffset(2, 2, 0, 0);
        }

        private void BuildTabs()
        {
            AbilityTab[] tabs = { AbilityTab.Attacks, AbilityTab.Skills, AbilityTab.Spells, AbilityTab.Items, AbilityTab.Generic };
            string[] labels = { "ATK", "SKL", "SPL", "ITM", "GEN" };

            for (int i = 0; i < tabs.Length; i++)
            {
                float slotWidth = 1f / tabs.Length;
                float xMin = i * slotWidth;
                float xMax = xMin + slotWidth;

                Button tab = PanelBuilder.CreateButton("Tab_" + labels[i], _tabBar, labels[i],
                    fontSize: UIStyleConfig.FontSizeTiny);
                RectTransform tabRect = tab.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(tabRect, xMin, 0, xMax, 1, 1, 0, -1, 0);

                AbilityTab capturedTab = tabs[i];
                tab.onClick.AddListener(() => SetActiveTab(capturedTab));

                _tabButtons.Add(tab);
            }
        }

        public void SetCharacter(BattleCharacter character)
        {
            _activeCharacter = character;
            _activeTab = AbilityTab.Attacks;
            RefreshAbilities();
        }

        public void SetActiveTab(AbilityTab tab)
        {
            _activeTab = tab;
            RefreshAbilities();
        }

        public void RefreshAbilities()
        {
            // Clear existing ability buttons
            foreach (Button btn in _abilityButtons)
            {
                Object.Destroy(btn.gameObject);
            }
            _abilityButtons.Clear();
            _abilityDataForButtons.Clear();

            if (_activeCharacter == null) return;

            List<AbilityData> abilities = _activeCharacter.Data.Abilities.FindAll(a => a.Tab == _activeTab && !a.IsPassive);
            float btnHeight = UIStyleConfig.AbilityButtonHeight;

            for (int i = 0; i < abilities.Count; i++)
            {
                AbilityData ability = abilities[i];

                string costText = UIFormatUtil.FormatAbilityCost(ability);
                string label = ability.Name + (costText.Length > 0 ? " " + costText : "");

                Color textColor = _activeCharacter.CanUseAbility(ability)
                    ? UIStyleConfig.TextPrimary
                    : UIStyleConfig.TextDimmed;

                Button btn = PanelBuilder.CreateButton("Ability_" + ability.Name, _abilityListContent, label,
                    textColor, UIStyleConfig.FontSizeTiny);

                LayoutElement layout = btn.gameObject.AddComponent<LayoutElement>();
                layout.preferredHeight = btnHeight;

                btn.interactable = _activeCharacter.CanUseAbility(ability);

                AbilityData capturedAbility = ability;
                LongPressHandler longPress = btn.gameObject.AddComponent<LongPressHandler>();
                longPress.OnLongPress += () => GameEvents.RaiseAbilityDetailRequested(capturedAbility);

                btn.onClick.AddListener(() =>
                {
                    if (longPress.WasLongPress) return;
                    GameEvents.RaiseAbilitySelected(capturedAbility);
                });

                _abilityButtons.Add(btn);
                _abilityDataForButtons.Add(ability);
            }

            // Reset scroll to top
            LayoutRebuilder.ForceRebuildLayoutImmediate(_abilityListContent);
            _abilityScrollRect.verticalNormalizedPosition = 1f;

            ApplyStagedHighlight();
            UpdateTabHighlights();
        }

        private void UpdateTabHighlights()
        {
            AbilityTab[] tabs = { AbilityTab.Attacks, AbilityTab.Skills, AbilityTab.Spells, AbilityTab.Items, AbilityTab.Generic };

            for (int i = 0; i < _tabButtons.Count; i++)
            {
                TextMeshProUGUI label = _tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.color = tabs[i] == _activeTab ? UIStyleConfig.AccentCyan : UIStyleConfig.TextDimmed;
                }
            }
        }

        public void SetStagedHighlight(AbilityData ability)
        {
            _stagedAbility = ability;
            ApplyStagedHighlight();
        }

        public void ClearStagedHighlight()
        {
            _stagedAbility = null;
            ApplyStagedHighlight();
        }

        private void ApplyStagedHighlight()
        {
            for (int i = 0; i < _abilityButtons.Count; i++)
            {
                TextMeshProUGUI label = _abilityButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label == null) continue;

                bool isStaged = _stagedAbility != null && i < _abilityDataForButtons.Count
                    && _abilityDataForButtons[i] == _stagedAbility;

                if (isStaged)
                {
                    label.color = UIStyleConfig.StagedHighlight;
                }
                else
                {
                    label.color = _activeCharacter != null && _activeCharacter.CanUseAbility(_abilityDataForButtons[i])
                        ? UIStyleConfig.TextPrimary
                        : UIStyleConfig.TextDimmed;
                }
            }
        }

    }
}
