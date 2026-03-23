using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class RoomChoiceScreen : IScreen
    {
        private GameObject _root;
        private RoomType? _selectedRoom;
        private bool _inventoryRequested;

        private RunData _runData;
        private FloorPlanEntry _floor;

        public RoomType? SelectedRoom => _selectedRoom;
        public bool InventoryRequested => _inventoryRequested;

        public RoomChoiceScreen(RunData runData, FloorPlanEntry floor)
        {
            _runData = runData;
            _floor = floor;
        }

        public void Build(Transform canvasParent)
        {
            _selectedRoom = null;
            _inventoryRequested = false;

            _root = new GameObject("RoomChoiceScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            string headerText = $"Act {_runData.CurrentAct} - Floor {_runData.CurrentFloor}/{RunConfig.BossFloor}";
            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                headerText, UIStyleConfig.FontSizeMedium,
                TextAlignmentOptions.Center, UIStyleConfig.AccentCyan);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.1f, 0.85f, 0.9f, 0.95f);

            // --- Gold display ---
            TextMeshProUGUI goldText = PanelBuilder.CreateText("Gold", rootRect,
                $"Gold: {_runData.Gold}", UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.AccentYellow);
            RectTransform goldRect = goldText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(goldRect, 0.1f, 0.79f, 0.9f, 0.85f);

            // --- Party summary ---
            string partySummary = "";
            foreach (CharacterData c in _runData.Party)
            {
                if (partySummary.Length > 0) partySummary += "  ";
                partySummary += $"{c.Name} L{c.Level}";
            }
            TextMeshProUGUI partyText = PanelBuilder.CreateText("Party", rootRect,
                partySummary, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform partyRect = partyText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(partyRect, 0.05f, 0.73f, 0.95f, 0.79f);

            // --- Build content based on floor category ---
            if (_floor.Category == FloorCategory.Battle)
            {
                BuildBattleFloor(rootRect);
            }
            else if (_floor.Category == FloorCategory.Bonus)
            {
                BuildBonusFloor(rootRect);
            }

            // --- Inventory button ---
            Button invBtn = PanelBuilder.CreateButton("InventoryBtn", rootRect,
                "INVENTORY", UIStyleConfig.AccentYellow, UIStyleConfig.FontSizeTiny);
            RectTransform invRect = invBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(invRect, 0.65f, 0.03f, 0.95f, 0.12f);
            invBtn.onClick.AddListener(() => _inventoryRequested = true);
        }

        private void BuildBattleFloor(RectTransform rootRect)
        {
            RoomType battleType = _floor.IsElite ? RoomType.EliteBattle : RoomType.Battle;

            // Single centered card
            BuildRoomCard(rootRect, battleType, 0.25f, 0.15f, 0.75f, 0.63f, battleType);

            TextMeshProUGUI prompt = PanelBuilder.CreateText("Prompt", rootRect,
                "Enemies block the path.", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            RectTransform promptRect = prompt.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(promptRect, 0.1f, 0.65f, 0.9f, 0.73f);
        }

        private void BuildBonusFloor(RectTransform rootRect)
        {
            TextMeshProUGUI prompt = PanelBuilder.CreateText("Prompt", rootRect,
                "Choose your path:", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            RectTransform promptRect = prompt.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(promptRect, 0.1f, 0.65f, 0.9f, 0.73f);

            float gap = 0.02f;
            float cardWidth = (0.9f - gap) / 2f;
            float left = 0.05f;

            // Bonus card (left)
            BuildRoomCard(rootRect, _floor.BonusType,
                left, 0.15f, left + cardWidth, 0.63f, _floor.BonusType);

            // Fight instead card (right)
            BuildRoomCard(rootRect, RoomType.Battle,
                left + cardWidth + gap, 0.15f, left + cardWidth * 2 + gap, 0.63f,
                RoomType.Battle);
        }

        private void BuildRoomCard(RectTransform parent, RoomType roomType,
            float xMin, float yMin, float xMax, float yMax, RoomType selectAs)
        {
            Color roomColor = FloorGenerator.GetRoomColor(roomType);
            string roomName = FloorGenerator.GetRoomName(roomType);
            string roomDesc = FloorGenerator.GetRoomDescription(roomType);
            string symbol = FloorGenerator.GetRoomSymbol(roomType);

            RectTransform card = PanelBuilder.CreatePanel("Card_" + roomType, parent);
            PanelBuilder.SetAnchored(card, xMin, yMin, xMax, yMax);

            Button cardButton = card.gameObject.AddComponent<Button>();
            ColorBlock colors = cardButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = UIStyleConfig.AccentCyan;
            colors.pressedColor = UIStyleConfig.AccentGreen;
            cardButton.colors = colors;
            cardButton.onClick.AddListener(() => _selectedRoom = selectAs);

            TextMeshProUGUI symbolText = PanelBuilder.CreateText("Symbol", card,
                symbol, UIStyleConfig.FontSizeLarge * 1.5f,
                TextAlignmentOptions.Center, roomColor);
            RectTransform symbolRect = symbolText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(symbolRect, 0.05f, 0.55f, 0.95f, 0.90f);

            TextMeshProUGUI nameText = PanelBuilder.CreateText("Name", card,
                roomName, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, roomColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.05f, 0.38f, 0.95f, 0.55f);

            TextMeshProUGUI descText = PanelBuilder.CreateText("Desc", card,
                roomDesc, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform descRect = descText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.05f, 0.05f, 0.95f, 0.38f);
            descText.textWrappingMode = TextWrappingModes.Normal;
        }

        public void Show()
        {
            _selectedRoom = null;
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
