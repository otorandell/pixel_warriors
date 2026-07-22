using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class SmithyScreen : IScreen
    {
        private enum SmithyTab { Upgrade, Craft, Dismantle, Imbue }

        private GameObject _root;
        private RectTransform _rootRect;
        private RunData _runData;

        private SmithyTab _activeTab = SmithyTab.Upgrade;
        private RectTransform _tabContainer;
        private RectTransform _contentPanel;
        private TextMeshProUGUI _goldText;
        private TextMeshProUGUI _materialsText;

        // Craft options generated on visit
        private List<EquipmentData> _craftOptions;

        private bool _exitRequested;
        private bool _inventoryRequested;

        public bool ExitRequested => _exitRequested;
        public bool InventoryRequested => _inventoryRequested;

        public SmithyScreen(RunData runData)
        {
            _runData = runData;
            _craftOptions = GenerateCraftOptions();
        }

        public void Build(Transform canvasParent)
        {
            _exitRequested = false;
            _inventoryRequested = false;

            _root = new GameObject("SmithyScreen");
            _rootRect = _root.AddComponent<RectTransform>();
            _rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(_rootRect);

            // --- Header bar ---
            TextMeshProUGUI titleTmp = PanelBuilder.CreateText("Title", _rootRect,
                "SMITHY", UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
            RectTransform titleRect = titleTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(titleRect, 0.02f, 0.91f, 0.18f, 0.98f, 4f);

            _goldText = PanelBuilder.CreateText("Gold", _rootRect,
                $"Gold: {_runData.Gold}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentYellow);
            RectTransform goldRect = _goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.18f, 0.91f, 0.38f, 0.98f);

            _materialsText = PanelBuilder.CreateText("Materials", _rootRect,
                FormatMaterials(), UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            RectTransform matRect = _materialsText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(matRect, 0.38f, 0.91f, 0.66f, 0.98f);

            // Leave button
            Button leaveBtn = PanelBuilder.CreateButton("LeaveBtn", _rootRect,
                "LEAVE", UIStyleConfig.AccentRed, UIStyleConfig.FontSizeSmall);
            RectTransform leaveBtnRect = leaveBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(leaveBtnRect, 0.82f, 0.91f, 0.97f, 0.98f);
            leaveBtn.onClick.AddListener(() => _exitRequested = true);

            // Inventory button
            Button invBtn = PanelBuilder.CreateButton("InvBtn", _rootRect,
                "INV", UIStyleConfig.AccentYellow, UIStyleConfig.FontSizeSmall);
            RectTransform invBtnRect = invBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(invBtnRect, 0.68f, 0.91f, 0.80f, 0.98f);
            invBtn.onClick.AddListener(() => _inventoryRequested = true);

            // --- Tab bar ---
            _tabContainer = PanelBuilder.CreateContainer("TabBar", _rootRect);
            PanelBuilder.SetAnchored(_tabContainer, 0.02f, 0.83f, 0.98f, 0.90f);
            BuildTabBar();

            // --- Content area ---
            _contentPanel = PanelBuilder.CreatePanel("ContentPanel", _rootRect);
            PanelBuilder.SetAnchored(_contentPanel, 0.02f, 0.03f, 0.98f, 0.82f);
            BuildContent();
        }

        private void BuildTabBar()
        {
            for (int i = _tabContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _tabContainer.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }

            string[] labels = { "UPGRADE", "CRAFT", "DISMANTLE", "IMBUE" };
            SmithyTab[] tabs = { SmithyTab.Upgrade, SmithyTab.Craft, SmithyTab.Dismantle, SmithyTab.Imbue };
            float tabWidth = 0.25f;

            for (int i = 0; i < tabs.Length; i++)
            {
                bool isActive = tabs[i] == _activeTab;
                Color color = isActive ? UIStyleConfig.AccentCyan : UIStyleConfig.TextDimmed;

                Button tab = PanelBuilder.CreateButton($"Tab_{labels[i]}", _tabContainer,
                    labels[i], color, UIStyleConfig.FontSizeTiny);
                RectTransform tabRect = tab.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(tabRect, tabWidth * i, 0f, tabWidth * (i + 1), 1f);

                SmithyTab capturedTab = tabs[i];
                tab.onClick.AddListener(() =>
                {
                    _activeTab = capturedTab;
                    RebuildAll();
                });
            }
        }

        private void BuildContent()
        {
            for (int i = _contentPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = _contentPanel.GetChild(i);
                if (child.name.StartsWith("Border_")) continue;
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }

            RectTransform scrollArea = PanelBuilder.CreateContainer("ScrollArea", _contentPanel);
            PanelBuilder.SetAnchored(scrollArea, 0.01f, 0.01f, 0.99f, 0.99f);

            var (scrollRect, content) = PanelBuilder.CreateVerticalScrollView("SmithyScroll", scrollArea);

            VerticalLayoutGroup vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = UIStyleConfig.ElementSpacing;
            vlg.padding = new RectOffset(4, 4, 2, 2);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;

            switch (_activeTab)
            {
                case SmithyTab.Upgrade:
                    BuildUpgradeContent(content);
                    break;
                case SmithyTab.Craft:
                    BuildCraftContent(content);
                    break;
                case SmithyTab.Dismantle:
                    BuildDismantleContent(content);
                    break;
                case SmithyTab.Imbue:
                    BuildImbueContent(content);
                    break;
            }
        }

        // ==============================
        // UPGRADE TAB
        // ==============================

        private void BuildUpgradeContent(RectTransform content)
        {
            List<EquipmentData> upgradeable = GetAllEquippedItems();
            upgradeable.AddRange(_runData.Inventory);

            // Filter: not max upgrade
            upgradeable.RemoveAll(item => item.UpgradeLevel >= SmithyConfig.MaxUpgradeLevel);

            if (upgradeable.Count == 0)
            {
                BuildEmptyRow(content, "No upgradeable equipment");
                return;
            }

            foreach (EquipmentData item in upgradeable)
            {
                BuildUpgradeRow(content, item);
            }
        }

        private void BuildUpgradeRow(RectTransform parent, EquipmentData item)
        {
            RectTransform row = PanelBuilder.CreateContainer("Upgrade_" + item.Name, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 58f;

            bool isMetal = SmithyConfig.YieldsMainlyMetal(item.Slot);
            int metalCost = isMetal ? SmithyConfig.GetUpgradeCost(item.UpgradeLevel, _runData.CurrentAct, true) : 0;
            int leatherCost = !isMetal ? SmithyConfig.GetUpgradeCost(item.UpgradeLevel, _runData.CurrentAct, false) : 0;
            bool canAfford = _runData.MetalScraps >= metalCost && _runData.LeatherScraps >= leatherCost;

            // Name + upgrade level
            string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                $"{item.Name}{upgradeStr}", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.55f, 0.55f, 1f, 2f);

            // Current stats
            string stats = UIFormatUtil.FormatItemStats(item);
            TextMeshProUGUI statsTmp = PanelBuilder.CreateText("Stats", row,
                stats, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform statsRect = statsTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsRect, 0.0f, 0.0f, 0.72f, 0.55f, 2f);

            // Cost text
            string costStr = metalCost > 0 ? $"{metalCost} Metal" : $"{leatherCost} Leather";
            Color costColor = canAfford ? UIStyleConfig.TextPrimary : UIStyleConfig.TextDimmed;
            TextMeshProUGUI costTmp = PanelBuilder.CreateText("Cost", row,
                costStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, costColor);
            RectTransform costRect = costTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(costRect, 0.55f, 0.55f, 0.72f, 1f);

            // Upgrade button
            Color btnColor = canAfford ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
            Button upgradeBtn = PanelBuilder.CreateButton("UpBtn", row,
                "UPGRADE", btnColor, UIStyleConfig.FontSizeTiny);
            RectTransform btnRect = upgradeBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.74f, 0.15f, 0.99f, 0.85f);
            upgradeBtn.interactable = canAfford;

            EquipmentData capturedItem = item;
            upgradeBtn.onClick.AddListener(() => OnUpgrade(capturedItem));
        }

        private void OnUpgrade(EquipmentData item)
        {
            if (item.UpgradeLevel >= SmithyConfig.MaxUpgradeLevel) return;

            bool isMetal = SmithyConfig.YieldsMainlyMetal(item.Slot);
            int metalCost = isMetal ? SmithyConfig.GetUpgradeCost(item.UpgradeLevel, _runData.CurrentAct, true) : 0;
            int leatherCost = !isMetal ? SmithyConfig.GetUpgradeCost(item.UpgradeLevel, _runData.CurrentAct, false) : 0;

            if (_runData.MetalScraps < metalCost || _runData.LeatherScraps < leatherCost) return;

            _runData.MetalScraps -= metalCost;
            _runData.LeatherScraps -= leatherCost;

            item.UpgradeLevel++;

            // Apply stat bonuses
            if (item.BaseDamage > 0)
                item.BaseDamage += SmithyConfig.UpgradeDamageBonus;
            if (item.BaseBlockChance > 0)
                item.BaseBlockChance += SmithyConfig.UpgradeBlockBonus;

            // Boost primary stat modifier by 1
            ApplyUpgradeStatBonus(item);

            RebuildAll();
        }

        private void ApplyUpgradeStatBonus(EquipmentData item)
        {
            CharacterStats s = item.StatModifiers;

            // Find the highest existing stat and boost it
            int maxVal = 0;
            int maxIdx = -1;
            int[] vals = { s.Endurance, s.Stamina, s.Intellect, s.Strength,
                           s.Dexterity, s.Willpower, s.Armor, s.MagicResist, s.Initiative };

            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[i] > maxVal)
                {
                    maxVal = vals[i];
                    maxIdx = i;
                }
            }

            if (maxIdx < 0) return;

            // Rebuild stats with the bonus
            item.StatModifiers = new CharacterStats(
                s.Endurance + (maxIdx == 0 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Stamina + (maxIdx == 1 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Intellect + (maxIdx == 2 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Strength + (maxIdx == 3 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Dexterity + (maxIdx == 4 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Willpower + (maxIdx == 5 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Armor + (maxIdx == 6 ? SmithyConfig.UpgradeStatBonus : 0),
                s.MagicResist + (maxIdx == 7 ? SmithyConfig.UpgradeStatBonus : 0),
                s.Initiative + (maxIdx == 8 ? SmithyConfig.UpgradeStatBonus : 0)
            );
        }

        // ==============================
        // CRAFT TAB
        // ==============================

        private void BuildCraftContent(RectTransform content)
        {
            if (_craftOptions.Count == 0)
            {
                BuildEmptyRow(content, "No craft options available");
                return;
            }

            // Materials cost header
            string costStr = $"Cost per item: {SmithyConfig.CraftMetalCost} Metal + {SmithyConfig.CraftLeatherCost} Leather";
            RectTransform headerRow = PanelBuilder.CreateContainer("CraftHeader", content);
            LayoutElement headerLe = headerRow.gameObject.AddComponent<LayoutElement>();
            headerLe.preferredHeight = 24f;
            PanelBuilder.CreateText("CraftCostText", headerRow, costStr,
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);

            bool canAfford = _runData.MetalScraps >= SmithyConfig.CraftMetalCost
                          && _runData.LeatherScraps >= SmithyConfig.CraftLeatherCost;

            for (int i = 0; i < _craftOptions.Count; i++)
            {
                if (_craftOptions[i] == null) continue;
                BuildCraftRow(content, i, canAfford);
            }
        }

        private void BuildCraftRow(RectTransform parent, int index, bool canAfford)
        {
            EquipmentData item = _craftOptions[index];

            RectTransform row = PanelBuilder.CreateContainer("Craft_" + index, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 58f;

            // Item name
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                item.Name, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.55f, 0.55f, 1f, 2f);

            // Slot
            string slotLabel = UIFormatUtil.FormatSlotName(item.Slot);
            TextMeshProUGUI slotTmp = PanelBuilder.CreateText("Slot", row,
                slotLabel, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform slotRect = slotTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(slotRect, 0.55f, 0.55f, 0.72f, 1f);

            // Stats
            string stats = UIFormatUtil.FormatItemStats(item);
            TextMeshProUGUI statsTmp = PanelBuilder.CreateText("Stats", row,
                stats, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform statsRect = statsTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsRect, 0.0f, 0.0f, 0.72f, 0.55f, 2f);

            // Craft button
            Color btnColor = canAfford ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
            Button craftBtn = PanelBuilder.CreateButton("CraftBtn", row,
                "CRAFT", btnColor, UIStyleConfig.FontSizeTiny);
            RectTransform btnRect = craftBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.74f, 0.15f, 0.99f, 0.85f);
            craftBtn.interactable = canAfford;

            int capturedIndex = index;
            craftBtn.onClick.AddListener(() => OnCraft(capturedIndex));
        }

        private void OnCraft(int index)
        {
            if (_runData.MetalScraps < SmithyConfig.CraftMetalCost) return;
            if (_runData.LeatherScraps < SmithyConfig.CraftLeatherCost) return;
            if (_runData.Inventory.Count >= LootConfig.MaxInventorySize) return;
            if (index >= _craftOptions.Count || _craftOptions[index] == null) return;

            _runData.MetalScraps -= SmithyConfig.CraftMetalCost;
            _runData.LeatherScraps -= SmithyConfig.CraftLeatherCost;
            _runData.Inventory.Add(_craftOptions[index]);
            _craftOptions[index] = null; // Mark as taken
            RebuildAll();
        }

        // ==============================
        // DISMANTLE TAB
        // ==============================

        private void BuildDismantleContent(RectTransform content)
        {
            if (_runData.Inventory.Count == 0)
            {
                BuildEmptyRow(content, "No equipment to dismantle");
                return;
            }

            for (int i = 0; i < _runData.Inventory.Count; i++)
            {
                BuildDismantleRow(content, _runData.Inventory[i], i);
            }
        }

        private void BuildDismantleRow(RectTransform parent, EquipmentData item, int index)
        {
            RectTransform row = PanelBuilder.CreateContainer("Dismantle_" + index, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 52f;

            bool mainlyMetal = SmithyConfig.YieldsMainlyMetal(item.Slot);
            int metalYield = mainlyMetal ? SmithyConfig.GetDismantleYield(item, true) : Mathf.Max(1, SmithyConfig.GetDismantleYield(item, true) / 2);
            int leatherYield = !mainlyMetal ? SmithyConfig.GetDismantleYield(item, false) : Mathf.Max(1, SmithyConfig.GetDismantleYield(item, false) / 2);

            // Item name
            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                item.Name, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.55f, 0.55f, 1f, 2f);

            // Stats
            string stats = UIFormatUtil.FormatItemStats(item);
            TextMeshProUGUI statsTmp = PanelBuilder.CreateText("Stats", row,
                stats, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform statsRect = statsTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsRect, 0.0f, 0.0f, 0.72f, 0.55f, 2f);

            // Yield text
            string yieldStr = $"+{metalYield}M +{leatherYield}L";
            TextMeshProUGUI yieldTmp = PanelBuilder.CreateText("Yield", row,
                yieldStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.AccentGreen);
            RectTransform yieldRect = yieldTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(yieldRect, 0.55f, 0.55f, 0.72f, 1f);

            // Dismantle button
            Button dismantleBtn = PanelBuilder.CreateButton("DismantleBtn", row,
                "SALVAGE", UIStyleConfig.AccentYellow, UIStyleConfig.FontSizeTiny);
            RectTransform btnRect = dismantleBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.74f, 0.15f, 0.99f, 0.85f);

            int capturedIndex = index;
            dismantleBtn.onClick.AddListener(() => OnDismantle(capturedIndex));
        }

        private void OnDismantle(int inventoryIndex)
        {
            if (inventoryIndex >= _runData.Inventory.Count) return;

            EquipmentData item = _runData.Inventory[inventoryIndex];
            bool mainlyMetal = SmithyConfig.YieldsMainlyMetal(item.Slot);
            int metalYield = mainlyMetal ? SmithyConfig.GetDismantleYield(item, true) : Mathf.Max(1, SmithyConfig.GetDismantleYield(item, true) / 2);
            int leatherYield = !mainlyMetal ? SmithyConfig.GetDismantleYield(item, false) : Mathf.Max(1, SmithyConfig.GetDismantleYield(item, false) / 2);

            _runData.MetalScraps += metalYield;
            _runData.LeatherScraps += leatherYield;
            _runData.Inventory.RemoveAt(inventoryIndex);
            RebuildAll();
        }

        // ==============================
        // IMBUE TAB
        // ==============================

        private void BuildImbueContent(RectTransform content)
        {
            if (_runData.Gems.Count == 0)
            {
                BuildEmptyRow(content, "No gems available");
                return;
            }

            // List gems with available equipment to imbue
            List<EquipmentData> imbueTargets = GetAllEquippedItems();
            imbueTargets.AddRange(_runData.Inventory);
            imbueTargets.RemoveAll(item => item.ImbuedGem != null); // Already imbued

            if (imbueTargets.Count == 0)
            {
                BuildEmptyRow(content, "No equipment without gems");
                return;
            }

            // Show each gem type the player has
            Dictionary<GemType, int> gemCounts = new();
            foreach (GemType gem in _runData.Gems)
            {
                if (!gemCounts.ContainsKey(gem)) gemCounts[gem] = 0;
                gemCounts[gem]++;
            }

            foreach (var kvp in gemCounts)
            {
                BuildGemSection(content, kvp.Key, kvp.Value, imbueTargets);
            }
        }

        private void BuildGemSection(RectTransform content, GemType gem, int count, List<EquipmentData> targets)
        {
            // Gem header
            RectTransform headerRow = PanelBuilder.CreateContainer($"GemHeader_{gem}", content);
            LayoutElement headerLe = headerRow.gameObject.AddComponent<LayoutElement>();
            headerLe.preferredHeight = 30f;

            string gemName = SmithyConfig.GetGemName(gem);
            int imbueCost = SmithyConfig.ImbueGoldCost * _runData.CurrentAct;
            PanelBuilder.CreateText("GemName", headerRow,
                $"{gemName} x{count}  (imbue cost: {imbueCost}g)",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentMagenta);

            bool canAfford = _runData.Gold >= imbueCost;

            // Show equipment targets
            foreach (EquipmentData item in targets)
            {
                BuildImbueRow(content, gem, item, canAfford);
            }
        }

        private void BuildImbueRow(RectTransform parent, GemType gem, EquipmentData item, bool canAfford)
        {
            RectTransform row = PanelBuilder.CreateContainer($"Imbue_{gem}_{item.Name}", parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 52f;

            bool isWeapon = SmithyConfig.IsWeaponSlot(item.Slot);
            string effectStr = isWeapon
                ? SmithyConfig.GetGemWeaponEffect(gem)
                : SmithyConfig.GetGemArmorEffect(gem);

            // Item name
            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                item.Name, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.55f, 0.40f, 1f, 2f);

            // Effect description
            TextMeshProUGUI effectTmp = PanelBuilder.CreateText("Effect", row,
                effectStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentGreen);
            RectTransform effectRect = effectTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(effectRect, 0.0f, 0.0f, 0.72f, 0.55f, 2f);

            // Slot
            string slotLabel = UIFormatUtil.FormatSlotName(item.Slot);
            TextMeshProUGUI slotTmp = PanelBuilder.CreateText("Slot", row,
                slotLabel, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform slotRect = slotTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(slotRect, 0.40f, 0.55f, 0.72f, 1f);

            // Imbue button
            Color btnColor = canAfford ? UIStyleConfig.AccentMagenta : UIStyleConfig.TextDimmed;
            Button imbueBtn = PanelBuilder.CreateButton("ImbueBtn", row,
                "IMBUE", btnColor, UIStyleConfig.FontSizeTiny);
            RectTransform btnRect = imbueBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.74f, 0.15f, 0.99f, 0.85f);
            imbueBtn.interactable = canAfford;

            GemType capturedGem = gem;
            EquipmentData capturedItem = item;
            imbueBtn.onClick.AddListener(() => OnImbue(capturedGem, capturedItem));
        }

        private void OnImbue(GemType gem, EquipmentData item)
        {
            int imbueCost = SmithyConfig.ImbueGoldCost * _runData.CurrentAct;
            if (_runData.Gold < imbueCost) return;
            if (item.ImbuedGem != null) return;

            // Remove one gem of this type
            int gemIdx = _runData.Gems.IndexOf(gem);
            if (gemIdx < 0) return;

            _runData.Gold -= imbueCost;
            _runData.Gems.RemoveAt(gemIdx);
            item.ImbuedGem = gem;

            // Apply stat bonuses for armor gems
            if (!SmithyConfig.IsWeaponSlot(item.Slot))
            {
                ApplyGemArmorStats(gem, item);
            }

            RebuildAll();
        }

        private void ApplyGemArmorStats(GemType gem, EquipmentData item)
        {
            CharacterStats s = item.StatModifiers;
            item.StatModifiers = gem switch
            {
                GemType.Ruby => new CharacterStats(s.Endurance, s.Stamina, s.Intellect, s.Strength,
                    s.Dexterity, s.Willpower, s.Armor + 3, s.MagicResist, s.Initiative),
                GemType.Sapphire => new CharacterStats(s.Endurance, s.Stamina, s.Intellect, s.Strength,
                    s.Dexterity, s.Willpower, s.Armor, s.MagicResist + 4, s.Initiative),
                GemType.Diamond => new CharacterStats(s.Endurance, s.Stamina, s.Intellect, s.Strength,
                    s.Dexterity, s.Willpower, s.Armor + 3, s.MagicResist + 2, s.Initiative),
                GemType.Emerald => new CharacterStats(s.Endurance + 2, s.Stamina, s.Intellect, s.Strength,
                    s.Dexterity, s.Willpower, s.Armor, s.MagicResist, s.Initiative),
                GemType.Onyx => new CharacterStats(s.Endurance, s.Stamina, s.Intellect, s.Strength,
                    s.Dexterity, s.Willpower, s.Armor, s.MagicResist, s.Initiative + 2),
                _ => s
            };
        }

        // ==============================
        // HELPERS
        // ==============================

        private List<EquipmentData> GetAllEquippedItems()
        {
            List<EquipmentData> items = new();
            foreach (CharacterData c in _runData.Party)
            {
                for (int i = 0; i < c.Equipment.Length; i++)
                {
                    if (c.Equipment[i] != null)
                        items.Add(c.Equipment[i]);
                }
            }
            return items;
        }

        private List<EquipmentData> GenerateCraftOptions()
        {
            List<EquipmentData> options = new();
            for (int i = 0; i < SmithyConfig.CraftOptions; i++)
            {
                options.Add(LootGenerator.GenerateProceduralItem(_runData.CurrentAct));
            }
            return options;
        }

        private string FormatMaterials()
        {
            string gemStr = _runData.Gems.Count > 0 ? $"  Gems:{_runData.Gems.Count}" : "";
            return $"Metal:{_runData.MetalScraps}  Leather:{_runData.LeatherScraps}{gemStr}";
        }

        private void BuildEmptyRow(RectTransform content, string message)
        {
            RectTransform emptyContainer = PanelBuilder.CreateContainer("Empty", content);
            LayoutElement emptyLe = emptyContainer.gameObject.AddComponent<LayoutElement>();
            emptyLe.preferredHeight = 30f;
            PanelBuilder.CreateText("EmptyText", emptyContainer,
                message, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
        }

        private void RebuildAll()
        {
            _goldText.text = $"Gold: {_runData.Gold}";
            _materialsText.text = FormatMaterials();
            BuildTabBar();
            BuildContent();
        }

        public void ClearInventoryFlag()
        {
            _inventoryRequested = false;
        }

        public void Show()
        {
            _exitRequested = false;
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
