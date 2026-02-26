using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class InventoryScreen : IScreen
    {
        private enum InventoryMode { Equipment, Items }

        private GameObject _root;
        private RectTransform _rootRect;
        private RunData _runData;

        private int _selectedCharIndex;
        private EquipmentSlot? _filterSlot;
        private InventoryMode _mode = InventoryMode.Equipment;

        // UI references for rebuilding panels
        private RectTransform _equipmentPanel;
        private RectTransform _inventoryPanel;
        private RectTransform _tabContainer;
        private RectTransform _modeContainer;
        private TextMeshProUGUI _goldText;

        private bool _closePressed;
        public bool ClosePressed => _closePressed;

        public InventoryScreen(RunData runData)
        {
            _runData = runData;
        }

        public void Build(Transform canvasParent)
        {
            _closePressed = false;
            _selectedCharIndex = 0;
            _filterSlot = null;

            _root = new GameObject("InventoryScreen");
            _rootRect = _root.AddComponent<RectTransform>();
            _rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(_rootRect);

            // --- Header bar ---
            PanelBuilder.CreateText("Title", _rootRect,
                "INVENTORY", UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentYellow);
            RectTransform titleRect = _root.transform.Find("Title").GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(titleRect, 0.02f, 0.90f, 0.35f, 0.98f, 4f);

            _goldText = PanelBuilder.CreateText("Gold", _rootRect,
                $"Gold: {_runData.Gold}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.AccentYellow);
            RectTransform goldRect = _goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.45f, 0.90f, 0.78f, 0.98f);

            Button closeBtn = PanelBuilder.CreateButton("CloseBtn", _rootRect,
                "X", UIStyleConfig.AccentRed, UIStyleConfig.FontSizeMedium);
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(closeBtnRect, 0.85f, 0.91f, 0.97f, 0.97f);
            closeBtn.onClick.AddListener(() => _closePressed = true);

            // --- Party tabs ---
            _tabContainer = PanelBuilder.CreateContainer("PartyTabs", _rootRect);
            PanelBuilder.SetAnchored(_tabContainer, 0.02f, 0.84f, 0.98f, 0.90f);
            BuildPartyTabs();

            // --- Mode toggle (Equipment / Items) ---
            _modeContainer = PanelBuilder.CreateContainer("ModeToggle", _rootRect);
            PanelBuilder.SetAnchored(_modeContainer, 0.02f, 0.77f, 0.98f, 0.83f);
            BuildModeToggle();

            // --- Equipment panel (left) ---
            _equipmentPanel = PanelBuilder.CreatePanel("EquipmentPanel", _rootRect);
            PanelBuilder.SetAnchored(_equipmentPanel, 0.02f, 0.03f, 0.46f, 0.76f);
            BuildEquipmentPanel();

            // --- Inventory panel (right) ---
            _inventoryPanel = PanelBuilder.CreatePanel("InventoryPanel", _rootRect);
            PanelBuilder.SetAnchored(_inventoryPanel, 0.48f, 0.03f, 0.98f, 0.76f);
            BuildInventoryPanel();
        }

        // --- Party Tabs ---

        private void BuildPartyTabs()
        {
            // Clear existing — deactivate first so destroyed objects don't render this frame
            for (int i = _tabContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _tabContainer.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }

            float tabWidth = 1f / Mathf.Max(_runData.Party.Count, 1);
            for (int i = 0; i < _runData.Party.Count; i++)
            {
                CharacterData c = _runData.Party[i];
                Color color = UIFormatUtil.GetClassColor(c.Class);
                bool isActive = i == _selectedCharIndex;

                Button tab = PanelBuilder.CreateButton($"Tab_{c.Name}", _tabContainer,
                    $"{c.Name}", isActive ? color : UIStyleConfig.TextDimmed,
                    UIStyleConfig.FontSizeTiny);
                RectTransform tabRect = tab.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(tabRect, tabWidth * i, 0f, tabWidth * (i + 1), 1f);

                int idx = i;
                tab.onClick.AddListener(() => SelectCharacter(idx));
            }
        }

        private void SelectCharacter(int index)
        {
            _selectedCharIndex = index;
            _filterSlot = null;
            RebuildPanels();
        }

        // --- Mode Toggle ---

        private void BuildModeToggle()
        {
            for (int i = _modeContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _modeContainer.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }

            string[] labels = { "EQUIPMENT", "ITEMS" };
            InventoryMode[] modes = { InventoryMode.Equipment, InventoryMode.Items };

            for (int i = 0; i < modes.Length; i++)
            {
                bool isActive = modes[i] == _mode;
                Color color = isActive ? UIStyleConfig.AccentCyan : UIStyleConfig.TextDimmed;

                Button btn = PanelBuilder.CreateButton($"Mode_{labels[i]}", _modeContainer,
                    labels[i], color, UIStyleConfig.FontSizeTiny);
                RectTransform btnRect = btn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(btnRect, 0.5f * i, 0f, 0.5f * (i + 1), 1f);

                InventoryMode capturedMode = modes[i];
                btn.onClick.AddListener(() =>
                {
                    _mode = capturedMode;
                    _filterSlot = null;
                    RebuildPanels();
                });
            }
        }

        // --- Equipment Panel (Left) ---

        private void BuildEquipmentPanel()
        {
            // Clear existing content — deactivate first so destroyed objects don't render this frame
            for (int i = _equipmentPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = _equipmentPanel.GetChild(i);
                if (child.name.StartsWith("Border_")) continue;
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }

            if (_runData.Party.Count == 0) return;
            CharacterData character = _runData.Party[_selectedCharIndex];

            // Character name/class header
            Color classColor = UIFormatUtil.GetClassColor(character.Class);
            TextMeshProUGUI charNameTmp = PanelBuilder.CreateText("CharName", _equipmentPanel,
                $"{character.Name} Lv{character.Level}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, classColor);
            RectTransform charNameRect = charNameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(charNameRect, 0.04f, 0.93f, 0.96f, 0.99f);

            // Equipment slots - compact layout
            EquipmentSlot[] slots = {
                EquipmentSlot.Hand1, EquipmentSlot.Offhand,
                EquipmentSlot.Head, EquipmentSlot.Body,
                EquipmentSlot.Trinket1, EquipmentSlot.Trinket2
            };

            float slotHeight = 0.06f;
            float slotGap = 0.012f;
            float startY = 0.91f;

            for (int i = 0; i < slots.Length; i++)
            {
                EquipmentSlot slot = slots[i];
                float top = startY - i * (slotHeight + slotGap);
                float bottom = top - slotHeight;
                EquipmentData equipped = character.Equipment[(int)slot];
                bool isFiltered = _filterSlot == slot;

                string slotName = UIFormatUtil.FormatSlotName(slot);
                string itemName = equipped != null ? equipped.Name : "(empty)";
                Color itemColor = equipped != null
                    ? (isFiltered ? UIStyleConfig.AccentCyan : UIFormatUtil.GetItemNameColor(equipped))
                    : UIStyleConfig.TextDimmed;

                Button slotBtn = PanelBuilder.CreateButton($"Slot_{slot}", _equipmentPanel,
                    $"{slotName}: {itemName}", itemColor, UIStyleConfig.FontSizeTiny);
                RectTransform slotRect = slotBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(slotRect, 0.02f, bottom, 0.98f, top);

                EquipmentSlot capturedSlot = slot;
                slotBtn.onClick.AddListener(() => OnSlotClicked(capturedSlot));
            }

            // --- Divider between equipment and stats ---
            float slotsBottom = startY - slots.Length * (slotHeight + slotGap);
            TextMeshProUGUI dividerTmp = PanelBuilder.CreateText("Divider", _equipmentPanel,
                "--- Stats ---", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform divRect = dividerTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(divRect, 0.04f, slotsBottom - 0.055f, 0.96f, slotsBottom - 0.01f);

            // --- Stats as individual rows ---
            CharacterStats total = character.GetTotalStats();
            int maxHP = StatCalculator.CalculateMaxHP(total);
            int maxEN = StatCalculator.CalculateMaxEnergy(total);
            int maxMP = StatCalculator.CalculateMaxMana(total);

            float statY = slotsBottom - 0.07f;
            float rowH = 0.048f;
            float rowGap = 0.006f;

            BuildStatRow("Stat_0", _equipmentPanel,
                $"END:{total.Endurance}  STR:{total.Strength}", statY, rowH, UIStyleConfig.TextDimmed);
            statY -= rowH + rowGap;

            BuildStatRow("Stat_1", _equipmentPanel,
                $"STA:{total.Stamina}  DEX:{total.Dexterity}", statY, rowH, UIStyleConfig.TextDimmed);
            statY -= rowH + rowGap;

            BuildStatRow("Stat_2", _equipmentPanel,
                $"INT:{total.Intellect}  WIL:{total.Willpower}", statY, rowH, UIStyleConfig.TextDimmed);
            statY -= rowH + rowGap;

            BuildStatRow("Stat_3", _equipmentPanel,
                $"ARM:{total.Armor}  MRS:{total.MagicResist}", statY, rowH, UIStyleConfig.TextDimmed);
            statY -= rowH + rowGap;

            BuildStatRow("Stat_4", _equipmentPanel,
                $"INI:{total.Initiative}", statY, rowH, UIStyleConfig.TextDimmed);
            statY -= rowH + rowGap;

            BuildStatRow("Stat_Res", _equipmentPanel,
                $"HP:{maxHP}  EN:{maxEN}  MP:{maxMP}", statY, rowH, UIStyleConfig.AccentCyan);
        }

        private static void BuildStatRow(string name, RectTransform parent,
            string text, float top, float height, Color color)
        {
            TextMeshProUGUI tmp = PanelBuilder.CreateText(name, parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, color);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rect, 0.06f, top - height, 0.96f, top);
        }

        private void OnSlotClicked(EquipmentSlot slot)
        {
            CharacterData character = _runData.Party[_selectedCharIndex];
            EquipmentData equipped = character.Equipment[(int)slot];

            if (equipped != null && _filterSlot == slot)
            {
                // Second click on same slot: unequip
                if (_runData.Inventory.Count < LootConfig.MaxInventorySize)
                {
                    _runData.Inventory.Add(equipped);
                    character.Equipment[(int)slot] = null;
                    _filterSlot = null;
                    RebuildPanels();
                }
                return;
            }

            // Toggle filter
            _filterSlot = _filterSlot == slot ? null : slot;
            RebuildPanels();
        }

        // --- Inventory Panel (Right) ---

        private void BuildInventoryPanel()
        {
            // Clear existing content (except borders) — deactivate first so destroyed objects don't render this frame
            for (int i = _inventoryPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = _inventoryPanel.GetChild(i);
                if (child.name.StartsWith("Border_")) continue;
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }

            if (_mode == InventoryMode.Items)
            {
                BuildConsumablesView();
                return;
            }

            // --- Equipment mode ---
            string headerText = _filterSlot.HasValue
                ? $"Inventory ({UIFormatUtil.FormatSlotName(_filterSlot.Value)})"
                : $"Inventory ({_runData.Inventory.Count}/{LootConfig.MaxInventorySize})";
            TextMeshProUGUI invHeaderTmp = PanelBuilder.CreateText("InvHeader", _inventoryPanel,
                headerText, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform headerRect = invHeaderTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.04f, 0.92f, 0.96f, 1f);

            // Scrollable item list
            RectTransform scrollArea = PanelBuilder.CreateContainer("ScrollArea", _inventoryPanel);
            PanelBuilder.SetAnchored(scrollArea, 0.02f, 0.02f, 0.98f, 0.91f);

            var (scrollRect, content) = PanelBuilder.CreateVerticalScrollView("InvScroll", scrollArea);

            VerticalLayoutGroup vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = UIStyleConfig.ElementSpacing;
            vlg.padding = new RectOffset(4, 4, 2, 2);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;

            // Filter items
            List<EquipmentData> filtered = new();
            List<int> filteredIndices = new();
            for (int i = 0; i < _runData.Inventory.Count; i++)
            {
                EquipmentData item = _runData.Inventory[i];
                if (_filterSlot.HasValue)
                {
                    bool matches = item.Slot == _filterSlot.Value;
                    // Trinkets: either slot matches either filter
                    if (!matches && (item.Slot == EquipmentSlot.Trinket1 || item.Slot == EquipmentSlot.Trinket2)
                        && (_filterSlot.Value == EquipmentSlot.Trinket1 || _filterSlot.Value == EquipmentSlot.Trinket2))
                        matches = true;
                    if (!matches) continue;
                }
                filtered.Add(item);
                filteredIndices.Add(i);
            }

            for (int f = 0; f < filtered.Count; f++)
            {
                BuildInventoryRow(content, filtered[f], filteredIndices[f]);
            }

            if (filtered.Count == 0)
            {
                RectTransform emptyContainer = PanelBuilder.CreateContainer("Empty", content);
                LayoutElement emptyLe = emptyContainer.gameObject.AddComponent<LayoutElement>();
                emptyLe.preferredHeight = 30f;
                PanelBuilder.CreateText("EmptyText", emptyContainer,
                    _filterSlot.HasValue ? "No items for this slot" : "Inventory empty",
                    UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            }
        }

        // --- Consumables View (Items mode) ---

        private void BuildConsumablesView()
        {
            TextMeshProUGUI headerTmp = PanelBuilder.CreateText("ItemsHeader", _inventoryPanel,
                "Consumables", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform headerRect = headerTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.04f, 0.92f, 0.96f, 1f);

            RectTransform scrollArea = PanelBuilder.CreateContainer("ScrollArea", _inventoryPanel);
            PanelBuilder.SetAnchored(scrollArea, 0.02f, 0.02f, 0.98f, 0.91f);

            var (scrollRect, content) = PanelBuilder.CreateVerticalScrollView("ItemsScroll", scrollArea);

            VerticalLayoutGroup vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = UIStyleConfig.ElementSpacing;
            vlg.padding = new RectOffset(4, 4, 2, 2);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;

            bool hasAny = false;

            foreach (ConsumableStack stack in _runData.Consumables)
            {
                if (stack.Quantity <= 0) continue;
                ConsumableData data = ConsumableCatalog.Get(stack.ConsumableId);
                if (data == null) continue;

                hasAny = true;

                if (data.Category == ConsumableCategory.Book)
                    BuildBookRow(content, data, stack);
                else
                    BuildConsumableInfoRow(content, data, stack);
            }

            if (!hasAny)
            {
                RectTransform emptyContainer = PanelBuilder.CreateContainer("Empty", content);
                LayoutElement emptyLe = emptyContainer.gameObject.AddComponent<LayoutElement>();
                emptyLe.preferredHeight = 30f;
                PanelBuilder.CreateText("EmptyText", emptyContainer,
                    "No consumables",
                    UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            }
        }

        private void BuildConsumableInfoRow(RectTransform parent, ConsumableData data, ConsumableStack stack)
        {
            RectTransform row = PanelBuilder.CreateContainer("C_" + data.Id, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 38f;

            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                $"{data.Name} x{stack.Quantity}", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextPrimary);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.5f, 1f, 1f, 2f);

            TextMeshProUGUI descTmp = PanelBuilder.CreateText("Desc", row,
                data.Description, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform descRect = descTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.0f, 0.0f, 1f, 0.5f, 2f);
        }

        private void BuildBookRow(RectTransform parent, ConsumableData book, ConsumableStack stack)
        {
            // Determine what ability this book teaches
            AbilityData bookAbility = ConsumableCatalog.GetBookAbility(book);
            string abilityName = bookAbility != null ? bookAbility.Name : "???";

            RectTransform row = PanelBuilder.CreateContainer("Book_" + book.Id, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();

            // Height depends on party size for teach buttons
            float baseHeight = 38f;
            float teachRowHeight = 24f;
            le.preferredHeight = baseHeight + _runData.Party.Count * teachRowHeight;

            // Book name
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                $"{book.Name} x{stack.Quantity}", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentMagenta);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.85f, 1f, 1f, 2f);

            // Description
            TextMeshProUGUI descTmp = PanelBuilder.CreateText("Desc", row,
                book.Description, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform descRect = descTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.0f, 0.70f, 1f, 0.85f, 2f);

            // Per-character teach buttons
            float yStep = 0.70f / Mathf.Max(_runData.Party.Count, 1);
            for (int i = 0; i < _runData.Party.Count; i++)
            {
                CharacterData character = _runData.Party[i];
                float yTop = 0.70f - i * yStep;
                float yBot = yTop - yStep;

                bool alreadyKnows = bookAbility != null
                    && character.Abilities.Exists(a => a.Name == bookAbility.Name);

                Color charColor = UIFormatUtil.GetClassColor(character.Class);
                string label = alreadyKnows ? $"{character.Name} (known)" : character.Name;
                Color labelColor = alreadyKnows ? UIStyleConfig.TextDimmed : charColor;

                TextMeshProUGUI charTmp = PanelBuilder.CreateText($"Char_{i}", row,
                    label, UIStyleConfig.FontSizeTiny,
                    TextAlignmentOptions.MidlineLeft, labelColor);
                RectTransform charRect = charTmp.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(charRect, 0.02f, yBot, 0.60f, yTop);

                if (!alreadyKnows && bookAbility != null)
                {
                    Button teachBtn = PanelBuilder.CreateButton($"Teach_{i}", row,
                        "TEACH", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeTiny);
                    RectTransform teachRect = teachBtn.GetComponent<RectTransform>();
                    PanelBuilder.SetAnchored(teachRect, 0.62f, yBot + 0.02f, 0.98f, yTop - 0.02f);

                    int capturedCharIdx = i;
                    string capturedBookId = book.Id;
                    teachBtn.onClick.AddListener(() => OnTeachBook(capturedCharIdx, capturedBookId));
                }
            }
        }

        private void OnTeachBook(int charIndex, string bookId)
        {
            if (charIndex >= _runData.Party.Count) return;

            ConsumableData book = ConsumableCatalog.Get(bookId);
            if (book == null) return;

            AbilityData ability = ConsumableCatalog.GetBookAbility(book);
            if (ability == null) return;

            CharacterData character = _runData.Party[charIndex];

            // Prevent duplicates
            if (character.Abilities.Exists(a => a.Name == ability.Name)) return;

            character.Abilities.Add(ability);
            _runData.RemoveConsumable(bookId);
            RebuildPanels();
        }

        private void BuildInventoryRow(RectTransform parent, EquipmentData item, int inventoryIndex)
        {
            RectTransform row = PanelBuilder.CreateContainer("Item_" + inventoryIndex, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 52f;

            // Item name
            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                item.Name, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.50f, 0.60f, 1f, 2f);

            // Slot label
            string slotLabel = UIFormatUtil.FormatSlotName(item.Slot);
            TextMeshProUGUI slotTmp = PanelBuilder.CreateText("Slot", row,
                slotLabel, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform slotRect = slotTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(slotRect, 0.60f, 0.50f, 1f, 1f, 0, 0, -2f);

            // Stats summary
            string stats = UIFormatUtil.FormatItemStats(item);
            TextMeshProUGUI statsTmp = PanelBuilder.CreateText("Stats", row,
                stats, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentCyan);
            RectTransform statsRect = statsTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(statsRect, 0.0f, 0.0f, 0.60f, 0.50f, 2f);

            // Equip button
            Button equipBtn = PanelBuilder.CreateButton("Equip", row,
                "Equip", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeTiny);
            RectTransform equipRect = equipBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(equipRect, 0.62f, 0.05f, 0.82f, 0.45f);

            int capturedIdx = inventoryIndex;
            equipBtn.onClick.AddListener(() => OnEquipFromInventory(capturedIdx));

            // Drop button
            Button dropBtn = PanelBuilder.CreateButton("Drop", row,
                "Drop", UIStyleConfig.AccentRed, UIStyleConfig.FontSizeTiny);
            RectTransform dropRect = dropBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(dropRect, 0.84f, 0.05f, 1f, 0.45f);

            int dropIdx = inventoryIndex;
            dropBtn.onClick.AddListener(() => OnDropItem(dropIdx));
        }

        private void OnEquipFromInventory(int inventoryIndex)
        {
            if (inventoryIndex >= _runData.Inventory.Count) return;

            EquipmentData item = _runData.Inventory[inventoryIndex];
            CharacterData character = _runData.Party[_selectedCharIndex];

            // Determine target slot
            EquipmentSlot targetSlot = item.Slot;
            if (targetSlot == EquipmentSlot.Trinket1 || targetSlot == EquipmentSlot.Trinket2)
            {
                if (character.Equipment[(int)EquipmentSlot.Trinket1] == null)
                    targetSlot = EquipmentSlot.Trinket1;
                else if (character.Equipment[(int)EquipmentSlot.Trinket2] == null)
                    targetSlot = EquipmentSlot.Trinket2;
                else
                    targetSlot = EquipmentSlot.Trinket1;
            }

            // Remove item from inventory
            _runData.Inventory.RemoveAt(inventoryIndex);

            // Swap with existing
            EquipmentData oldItem = character.Equipment[(int)targetSlot];
            if (oldItem != null)
                _runData.Inventory.Add(oldItem);

            character.Equipment[(int)targetSlot] = item;

            RebuildPanels();
        }

        private void OnDropItem(int inventoryIndex)
        {
            if (inventoryIndex >= _runData.Inventory.Count) return;
            _runData.Inventory.RemoveAt(inventoryIndex);
            RebuildPanels();
        }

        // --- Rebuild ---

        private void RebuildPanels()
        {
            _goldText.text = $"Gold: {_runData.Gold}";
            BuildPartyTabs();
            BuildModeToggle();
            BuildEquipmentPanel();
            BuildInventoryPanel();
        }

        public void Show()
        {
            _closePressed = false;
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
