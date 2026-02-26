using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class PostBattleScreen : IScreen
    {
        private GameObject _root;
        private RectTransform _rootRect;
        private RectTransform _contentRect;
        private bool _continuePressed;
        private bool _inventoryRequested;

        public bool ContinuePressed => _continuePressed;
        public bool InventoryRequested => _inventoryRequested;

        private PostBattleResult _result;
        private List<CharacterData> _party;
        private RunData _runData;

        // Track loot card state: null = unhandled, string = action taken
        private List<string> _lootStates;
        private List<RectTransform> _lootCards;

        public PostBattleScreen(PostBattleResult result, List<CharacterData> party, RunData runData)
        {
            _result = result;
            _party = party;
            _runData = runData;
        }

        public void Build(Transform canvasParent)
        {
            _continuePressed = false;
            _inventoryRequested = false;

            _root = new GameObject("PostBattleScreen");
            _rootRect = _root.AddComponent<RectTransform>();
            _rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(_rootRect);

            // --- Header: VICTORY ---
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", _rootRect,
                "VICTORY", UIStyleConfig.FontSizeLarge * 1.2f,
                TextAlignmentOptions.Center, UIStyleConfig.AccentGreen);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.1f, 0.88f, 0.9f, 0.96f);

            // --- Gold earned ---
            TextMeshProUGUI goldText = PanelBuilder.CreateText("GoldEarned", _rootRect,
                $"+{_result.GoldEarned} Gold", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentYellow);
            RectTransform goldRect = goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.1f, 0.82f, 0.9f, 0.88f);

            // --- Scrollable content area for character results ---
            RectTransform scrollPanel = PanelBuilder.CreateContainer("ScrollArea", _rootRect);
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

            // --- Loot section ---
            if (_result.LootDrops != null && _result.LootDrops.Count > 0)
            {
                BuildLootSection();
            }

            // --- Bottom buttons: INVENTORY + CONTINUE ---
            Button inventoryBtn = PanelBuilder.CreateButton("InventoryButton", _rootRect,
                "INVENTORY", UIStyleConfig.AccentCyan, UIStyleConfig.FontSizeMedium);
            RectTransform invBtnRect = inventoryBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(invBtnRect, 0.05f, 0.03f, 0.42f, 0.12f);
            inventoryBtn.onClick.AddListener(() => _inventoryRequested = true);

            Button continueBtn = PanelBuilder.CreateButton("ContinueButton", _rootRect,
                "CONTINUE", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
            RectTransform btnRect = continueBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.58f, 0.03f, 0.95f, 0.12f);
            continueBtn.onClick.AddListener(OnContinuePressed);
        }

        // --- Loot UI ---

        private void BuildLootSection()
        {
            _lootStates = new List<string>();
            _lootCards = new List<RectTransform>();

            // Loot header
            RectTransform lootHeader = PanelBuilder.CreateContainer("LootHeader", _contentRect);
            LayoutElement headerLe = lootHeader.gameObject.AddComponent<LayoutElement>();
            headerLe.preferredHeight = 24f;
            PanelBuilder.CreateText("LootTitle", lootHeader,
                "-- LOOT --", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentYellow);

            for (int i = 0; i < _result.LootDrops.Count; i++)
            {
                _lootStates.Add(null);
                BuildLootCard(i, _result.LootDrops[i]);
            }
        }

        private void BuildLootCard(int index, EquipmentData item)
        {
            // Calculate height based on content
            float height = 70f;
            if (item.IsUnique && !string.IsNullOrEmpty(item.FlavorText)) height += 18f;

            RectTransform container = PanelBuilder.CreatePanel("Loot_" + index, _contentRect);
            LayoutElement le = container.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            _lootCards.Add(container);

            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            string slotLabel = UIFormatUtil.FormatSlotName(item.Slot);
            if (item.WeaponType != WeaponType.None && item.WeaponType != WeaponType.Shield)
                slotLabel += $" ({item.WeaponType})";

            // Item name (left)
            TextMeshProUGUI nameText = PanelBuilder.CreateText("ItemName", container,
                item.Name, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.02f, 0.78f, 0.62f, 0.98f, 4f);

            // Slot label (right)
            TextMeshProUGUI slotText = PanelBuilder.CreateText("SlotLabel", container,
                slotLabel, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform slotRect = slotText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(slotRect, 0.55f, 0.78f, 0.98f, 0.98f, 0, 0, -4f);

            // Stats line
            string statsStr = UIFormatUtil.FormatItemStats(item);
            TextMeshProUGUI statsText = PanelBuilder.CreateText("Stats", container,
                statsStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform statsRect = statsText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsRect, 0.02f, 0.55f, 0.98f, 0.78f, 4f);

            float buttonTop = 0.50f;

            // Flavor text for uniques
            if (item.IsUnique && !string.IsNullOrEmpty(item.FlavorText))
            {
                TextMeshProUGUI flavorText = PanelBuilder.CreateText("Flavor", container,
                    $"\"{item.FlavorText}\"", UIStyleConfig.FontSizeTiny,
                    TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
                RectTransform flavorRect = flavorText.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(flavorRect, 0.02f, 0.35f, 0.98f, 0.55f, 4f);
                buttonTop = 0.32f;
            }

            // Stash button (full width)
            bool canStash = _runData.Inventory.Count < LootConfig.MaxInventorySize;
            Button stashBtn = PanelBuilder.CreateButton("Stash", container,
                canStash ? "Stash" : "Inventory Full",
                canStash ? UIStyleConfig.TextDimmed : UIStyleConfig.DeathTextColor,
                UIStyleConfig.FontSizeTiny);
            RectTransform stashRect = stashBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(stashRect, 0.02f, 0.02f, 0.98f, buttonTop);
            stashBtn.interactable = canStash;
            int stashIdx = index;
            stashBtn.onClick.AddListener(() => OnStashItem(stashIdx));
        }

        private void OnStashItem(int lootIndex)
        {
            if (_lootStates[lootIndex] != null) return;
            if (_runData.Inventory.Count >= LootConfig.MaxInventorySize) return;

            _runData.Inventory.Add(_result.LootDrops[lootIndex]);
            _lootStates[lootIndex] = "Stashed";
            GreyOutLootCard(lootIndex, "Stashed");
        }

        private void GreyOutLootCard(int index, string statusText)
        {
            RectTransform card = _lootCards[index];

            // Disable all buttons
            foreach (Button btn in card.GetComponentsInChildren<Button>())
                btn.interactable = false;

            // Dim all text
            foreach (TextMeshProUGUI txt in card.GetComponentsInChildren<TextMeshProUGUI>())
                txt.color = UIStyleConfig.TextDimmed;

            // Add status overlay text
            TextMeshProUGUI statusTmp = PanelBuilder.CreateText("Status", card,
                statusText, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentGreen);
            RectTransform statusRect = statusTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statusRect, 0.1f, 0.3f, 0.9f, 0.7f);
        }

        /// <summary>
        /// Refreshes stash button availability on loot cards (e.g. after returning from inventory).
        /// </summary>
        public void RefreshLoot()
        {
            _inventoryRequested = false;

            if (_lootCards == null) return;

            bool canStash = _runData.Inventory.Count < LootConfig.MaxInventorySize;

            for (int i = 0; i < _lootCards.Count; i++)
            {
                if (_lootStates[i] != null) continue; // already handled

                RectTransform card = _lootCards[i];
                Button[] buttons = card.GetComponentsInChildren<Button>();
                foreach (Button btn in buttons)
                {
                    btn.interactable = canStash;
                    // Update button text
                    TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null)
                    {
                        btnText.text = canStash ? "Stash" : "Inventory Full";
                        btnText.color = canStash ? UIStyleConfig.TextDimmed : UIStyleConfig.DeathTextColor;
                    }
                }
            }
        }

        private void OnContinuePressed()
        {
            // Auto-stash any unhandled loot
            if (_lootStates != null)
            {
                for (int i = 0; i < _lootStates.Count; i++)
                {
                    if (_lootStates[i] != null) continue;
                    if (_runData.Inventory.Count < LootConfig.MaxInventorySize)
                    {
                        _runData.Inventory.Add(_result.LootDrops[i]);
                    }
                    // If inventory full, item is lost (could log a warning)
                }
            }

            _continuePressed = true;
        }

        // --- Character Results (existing) ---

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
            _inventoryRequested = false;
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
