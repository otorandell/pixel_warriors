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

        private RunData _runData;
        private List<RoomType> _choices;

        public RoomType? SelectedRoom => _selectedRoom;

        public RoomChoiceScreen(RunData runData, List<RoomType> choices)
        {
            _runData = runData;
            _choices = choices;
        }

        public void Build(Transform canvasParent)
        {
            _selectedRoom = null;

            _root = new GameObject("RoomChoiceScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            string headerText = $"Act {_runData.CurrentAct} - Floor {_runData.CurrentFloor}/{RunConfig.FloorsPerAct}";
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

            // --- "Choose your path" ---
            TextMeshProUGUI promptText = PanelBuilder.CreateText("Prompt", rootRect,
                "Choose your path:", UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            RectTransform promptRect = promptText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(promptRect, 0.1f, 0.65f, 0.9f, 0.73f);

            // --- Room cards ---
            if (_choices.Count == 1)
            {
                BuildRoomCard(rootRect, _choices[0], 0.25f, 0.15f, 0.75f, 0.63f);
            }
            else
            {
                float gap = 0.02f;
                float cardWidth = (0.9f - gap) / 2f;
                float left = 0.05f;

                BuildRoomCard(rootRect, _choices[0],
                    left, 0.15f, left + cardWidth, 0.63f);
                BuildRoomCard(rootRect, _choices[1],
                    left + cardWidth + gap, 0.15f, left + cardWidth * 2 + gap, 0.63f);
            }
        }

        private void BuildRoomCard(RectTransform parent, RoomType roomType,
            float xMin, float yMin, float xMax, float yMax)
        {
            Color roomColor = FloorGenerator.GetRoomColor(roomType);
            string roomName = FloorGenerator.GetRoomName(roomType);
            string roomDesc = FloorGenerator.GetRoomDescription(roomType);

            // Card panel
            RectTransform card = PanelBuilder.CreatePanel("Card_" + roomType, parent);
            PanelBuilder.SetAnchored(card, xMin, yMin, xMax, yMax);

            // Make it a button
            Button cardButton = card.gameObject.AddComponent<Button>();
            ColorBlock colors = cardButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = UIStyleConfig.AccentCyan;
            colors.pressedColor = UIStyleConfig.AccentGreen;
            cardButton.colors = colors;
            cardButton.onClick.AddListener(() => _selectedRoom = roomType);

            // Room type icon/symbol
            string symbol = GetRoomSymbol(roomType);
            TextMeshProUGUI symbolText = PanelBuilder.CreateText("Symbol", card,
                symbol, UIStyleConfig.FontSizeLarge * 1.5f,
                TextAlignmentOptions.Center, roomColor);
            RectTransform symbolRect = symbolText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(symbolRect, 0.05f, 0.55f, 0.95f, 0.90f);

            // Room name
            TextMeshProUGUI nameText = PanelBuilder.CreateText("Name", card,
                roomName, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, roomColor);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(nameRect, 0.05f, 0.38f, 0.95f, 0.55f);

            // Description
            TextMeshProUGUI descText = PanelBuilder.CreateText("Desc", card,
                roomDesc, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            RectTransform descRect = descText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(descRect, 0.05f, 0.05f, 0.95f, 0.38f);
            descText.textWrappingMode = TextWrappingModes.Normal;
        }

        private static string GetRoomSymbol(RoomType roomType)
        {
            return roomType switch
            {
                RoomType.Battle => "X",
                RoomType.EliteBattle => "XX",
                RoomType.BossBattle => "!!!",
                RoomType.Shop => "$",
                RoomType.Rest => "+",
                RoomType.Event => "?",
                RoomType.Recruit => "@",
                _ => "?"
            };
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
