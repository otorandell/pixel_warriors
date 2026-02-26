using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class ShopScreen : IScreen
    {
        private enum ShopTab { BuyGear, BuyItems, Sell }

        private GameObject _root;
        private RectTransform _rootRect;
        private RunData _runData;
        private ShopStock _stock;

        private ShopTab _activeTab = ShopTab.BuyGear;
        private RectTransform _tabContainer;
        private RectTransform _contentPanel;
        private TextMeshProUGUI _goldText;

        private bool _exitRequested;
        private bool _inventoryRequested;

        public bool ExitRequested => _exitRequested;
        public bool InventoryRequested => _inventoryRequested;

        public ShopScreen(RunData runData, ShopStock stock)
        {
            _runData = runData;
            _stock = stock;
        }

        public void Build(Transform canvasParent)
        {
            _exitRequested = false;
            _inventoryRequested = false;

            _root = new GameObject("ShopScreen");
            _rootRect = _root.AddComponent<RectTransform>();
            _rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(_rootRect);

            // --- Header bar ---
            TextMeshProUGUI titleTmp = PanelBuilder.CreateText("Title", _rootRect,
                "SHOP", UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.AccentGreen);
            RectTransform titleRect = titleTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(titleRect, 0.02f, 0.91f, 0.25f, 0.98f, 4f);

            _goldText = PanelBuilder.CreateText("Gold", _rootRect,
                $"Gold: {_runData.Gold}", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.AccentYellow);
            RectTransform goldRect = _goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.30f, 0.91f, 0.65f, 0.98f);

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

            string[] labels = { "BUY GEAR", "BUY ITEMS", "SELL" };
            ShopTab[] tabs = { ShopTab.BuyGear, ShopTab.BuyItems, ShopTab.Sell };
            float tabWidth = 0.22f;

            for (int i = 0; i < tabs.Length; i++)
            {
                bool isActive = tabs[i] == _activeTab;
                Color color = isActive ? UIStyleConfig.AccentCyan : UIStyleConfig.TextDimmed;

                Button tab = PanelBuilder.CreateButton($"Tab_{labels[i]}", _tabContainer,
                    labels[i], color, UIStyleConfig.FontSizeTiny);
                RectTransform tabRect = tab.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(tabRect, tabWidth * i, 0f, tabWidth * (i + 1), 1f);

                ShopTab capturedTab = tabs[i];
                tab.onClick.AddListener(() =>
                {
                    _activeTab = capturedTab;
                    RebuildAll();
                });
            }

            // Reroll button (right side)
            int rerollCost = ShopGenerator.CalculateRerollCost(_runData.CurrentAct);
            bool canReroll = _runData.Gold >= rerollCost;
            Color rerollColor = canReroll ? UIStyleConfig.AccentMagenta : UIStyleConfig.TextDimmed;

            Button rerollBtn = PanelBuilder.CreateButton("RerollBtn", _tabContainer,
                $"REROLL {rerollCost}g", rerollColor, UIStyleConfig.FontSizeTiny);
            RectTransform rerollRect = rerollBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(rerollRect, 0.74f, 0f, 1f, 1f);
            rerollBtn.interactable = canReroll;
            rerollBtn.onClick.AddListener(() =>
            {
                if (_runData.Gold >= rerollCost)
                {
                    _runData.Gold -= rerollCost;
                    _stock = ShopGenerator.GenerateShopStock(_runData);
                    RebuildAll();
                }
            });
        }

        private void BuildContent()
        {
            // Clear existing content (except borders)
            for (int i = _contentPanel.childCount - 1; i >= 0; i--)
            {
                Transform child = _contentPanel.GetChild(i);
                if (child.name.StartsWith("Border_")) continue;
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }

            // Scrollable list
            RectTransform scrollArea = PanelBuilder.CreateContainer("ScrollArea", _contentPanel);
            PanelBuilder.SetAnchored(scrollArea, 0.01f, 0.01f, 0.99f, 0.99f);

            var (scrollRect, content) = PanelBuilder.CreateVerticalScrollView("ShopScroll", scrollArea);

            VerticalLayoutGroup vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = UIStyleConfig.ElementSpacing;
            vlg.padding = new RectOffset(4, 4, 2, 2);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;

            switch (_activeTab)
            {
                case ShopTab.BuyGear:
                    BuildBuyGearContent(content);
                    break;
                case ShopTab.BuyItems:
                    BuildBuyItemsContent(content);
                    break;
                case ShopTab.Sell:
                    BuildSellContent(content);
                    break;
            }
        }

        // --- Buy Gear Tab ---

        private void BuildBuyGearContent(RectTransform content)
        {
            for (int i = 0; i < _stock.Equipment.Count; i++)
            {
                ShopEquipmentEntry entry = _stock.Equipment[i];
                BuildGearRow(content, entry, i);
            }

            if (_stock.Equipment.Count == 0)
                BuildEmptyRow(content, "No gear available");
        }

        private void BuildGearRow(RectTransform parent, ShopEquipmentEntry entry, int index)
        {
            RectTransform row = PanelBuilder.CreateContainer("Gear_" + index, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 58f;

            EquipmentData item = entry.Item;
            bool sold = entry.Sold;
            bool canAfford = _runData.Gold >= entry.Price && !sold;

            // Item name
            Color nameColor = sold ? UIStyleConfig.TextDimmed : UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                sold ? $"{item.Name} SOLD" : item.Name,
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.MidlineLeft, nameColor);
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

            if (!sold)
            {
                // Buy button
                Color buyColor = canAfford ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
                Button buyBtn = PanelBuilder.CreateButton("Buy", row,
                    $"BUY {entry.Price}g", buyColor, UIStyleConfig.FontSizeTiny);
                RectTransform buyRect = buyBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(buyRect, 0.74f, 0.15f, 0.99f, 0.85f);
                buyBtn.interactable = canAfford;

                int capturedIndex = index;
                buyBtn.onClick.AddListener(() => OnBuyGear(capturedIndex));
            }
        }

        private void OnBuyGear(int index)
        {
            ShopEquipmentEntry entry = _stock.Equipment[index];
            if (entry.Sold || _runData.Gold < entry.Price) return;
            if (_runData.Inventory.Count >= LootConfig.MaxInventorySize) return;

            _runData.Gold -= entry.Price;
            _runData.Inventory.Add(entry.Item);
            entry.Sold = true;
            RebuildAll();
        }

        // --- Buy Items Tab ---

        private void BuildBuyItemsContent(RectTransform content)
        {
            for (int i = 0; i < _stock.Consumables.Count; i++)
            {
                ShopConsumableEntry entry = _stock.Consumables[i];
                BuildConsumableRow(content, entry, i);
            }

            // Book slot
            if (_stock.Book != null)
            {
                BuildBookRow(content, _stock.Book);
            }

            if (_stock.Consumables.Count == 0 && _stock.Book == null)
                BuildEmptyRow(content, "No items available");
        }

        private void BuildConsumableRow(RectTransform parent, ShopConsumableEntry entry, int index)
        {
            RectTransform row = PanelBuilder.CreateContainer("Consumable_" + index, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 48f;

            bool outOfStock = entry.Stock <= 0;
            bool canAfford = _runData.Gold >= entry.Price && !outOfStock;

            // Name + stock
            Color nameColor = outOfStock ? UIStyleConfig.TextDimmed : UIStyleConfig.TextPrimary;
            string nameStr = outOfStock
                ? $"{entry.Consumable.Name} (sold out)"
                : $"{entry.Consumable.Name} x{entry.Stock}";
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                nameStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.5f, 0.72f, 1f, 2f);

            // Description
            TextMeshProUGUI descTmp = PanelBuilder.CreateText("Desc", row,
                entry.Consumable.Description, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform descRect = descTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.0f, 0.0f, 0.72f, 0.5f, 2f);

            if (!outOfStock)
            {
                Color buyColor = canAfford ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
                Button buyBtn = PanelBuilder.CreateButton("Buy", row,
                    $"BUY {entry.Price}g", buyColor, UIStyleConfig.FontSizeTiny);
                RectTransform buyRect = buyBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(buyRect, 0.74f, 0.15f, 0.99f, 0.85f);
                buyBtn.interactable = canAfford;

                int capturedIndex = index;
                buyBtn.onClick.AddListener(() => OnBuyConsumable(capturedIndex));
            }
        }

        private void BuildBookRow(RectTransform parent, ShopBookEntry entry)
        {
            RectTransform row = PanelBuilder.CreateContainer("Book", parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 48f;

            bool sold = entry.Sold;
            bool canAfford = _runData.Gold >= entry.Price && !sold;

            Color nameColor = sold ? UIStyleConfig.TextDimmed : UIStyleConfig.AccentMagenta;
            string nameStr = sold ? $"{entry.Book.Name} SOLD" : entry.Book.Name;
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                nameStr, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
            RectTransform nameRect = nameTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.0f, 0.5f, 0.72f, 1f, 2f);

            TextMeshProUGUI descTmp = PanelBuilder.CreateText("Desc", row,
                entry.Book.Description, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform descRect = descTmp.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.0f, 0.0f, 0.72f, 0.5f, 2f);

            if (!sold)
            {
                Color buyColor = canAfford ? UIStyleConfig.AccentGreen : UIStyleConfig.TextDimmed;
                Button buyBtn = PanelBuilder.CreateButton("Buy", row,
                    $"BUY {entry.Price}g", buyColor, UIStyleConfig.FontSizeTiny);
                RectTransform buyRect = buyBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(buyRect, 0.74f, 0.15f, 0.99f, 0.85f);
                buyBtn.interactable = canAfford;

                buyBtn.onClick.AddListener(OnBuyBook);
            }
        }

        private void OnBuyConsumable(int index)
        {
            ShopConsumableEntry entry = _stock.Consumables[index];
            if (entry.Stock <= 0 || _runData.Gold < entry.Price) return;

            _runData.Gold -= entry.Price;
            _runData.AddConsumable(entry.Consumable.Id);
            entry.Stock--;
            RebuildAll();
        }

        private void OnBuyBook()
        {
            if (_stock.Book == null || _stock.Book.Sold) return;
            if (_runData.Gold < _stock.Book.Price) return;

            _runData.Gold -= _stock.Book.Price;
            _runData.AddConsumable(_stock.Book.Book.Id);
            _stock.Book.Sold = true;
            RebuildAll();
        }

        // --- Sell Tab ---

        private void BuildSellContent(RectTransform content)
        {
            if (_runData.Inventory.Count == 0)
            {
                BuildEmptyRow(content, "No items to sell");
                return;
            }

            for (int i = 0; i < _runData.Inventory.Count; i++)
            {
                BuildSellRow(content, _runData.Inventory[i], i);
            }
        }

        private void BuildSellRow(RectTransform parent, EquipmentData item, int inventoryIndex)
        {
            RectTransform row = PanelBuilder.CreateContainer("Sell_" + inventoryIndex, parent);
            LayoutElement le = row.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 52f;

            int sellPrice = ShopGenerator.CalculateEquipmentSellPrice(item);

            // Item name
            Color nameColor = UIFormatUtil.GetItemNameColor(item);
            TextMeshProUGUI nameTmp = PanelBuilder.CreateText("Name", row,
                item.Name, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, nameColor);
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

            // Sell button
            Button sellBtn = PanelBuilder.CreateButton("Sell", row,
                $"SELL {sellPrice}g", UIStyleConfig.AccentYellow, UIStyleConfig.FontSizeTiny);
            RectTransform sellRect = sellBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(sellRect, 0.74f, 0.15f, 0.99f, 0.85f);

            int capturedIdx = inventoryIndex;
            sellBtn.onClick.AddListener(() => OnSellItem(capturedIdx));
        }

        private void OnSellItem(int inventoryIndex)
        {
            if (inventoryIndex >= _runData.Inventory.Count) return;

            EquipmentData item = _runData.Inventory[inventoryIndex];
            int sellPrice = ShopGenerator.CalculateEquipmentSellPrice(item);

            _runData.Gold += sellPrice;
            _runData.Inventory.RemoveAt(inventoryIndex);
            RebuildAll();
        }

        // --- Helpers ---

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
