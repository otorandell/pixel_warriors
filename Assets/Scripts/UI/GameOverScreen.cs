using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class GameOverScreen : IScreen
    {
        private GameObject _root;
        private bool _menuPressed;

        public bool MenuPressed => _menuPressed;

        private RunData _runData;
        private bool _victory;

        public GameOverScreen(RunData runData, bool victory)
        {
            _runData = runData;
            _victory = victory;
        }

        public void Build(Transform canvasParent)
        {
            _menuPressed = false;

            _root = new GameObject("GameOverScreen");
            RectTransform rootRect = _root.AddComponent<RectTransform>();
            rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(rootRect);

            // --- Header ---
            string headerText = _victory ? "VICTORY" : "DEFEAT";
            Color headerColor = _victory ? UIStyleConfig.AccentGreen : UIStyleConfig.AccentRed;

            TextMeshProUGUI header = PanelBuilder.CreateText("Header", rootRect,
                headerText, UIStyleConfig.FontSizeLarge * 1.5f,
                TextAlignmentOptions.Center, headerColor);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.1f, 0.85f, 0.9f, 0.96f);

            // --- Subtitle ---
            string subtitle = _victory
                ? "Your party has conquered the dungeon!"
                : "Your party has fallen...";
            TextMeshProUGUI sub = PanelBuilder.CreateText("Subtitle", rootRect,
                subtitle, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.Center, UIStyleConfig.TextDimmed);
            sub.textWrappingMode = TextWrappingModes.Normal;
            RectTransform subRect = sub.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(subRect, 0.1f, 0.79f, 0.9f, 0.85f);

            // --- Scrollable content ---
            RectTransform scrollPanel = PanelBuilder.CreateContainer("ScrollArea", rootRect);
            PanelBuilder.SetAnchored(scrollPanel, 0.08f, 0.14f, 0.92f, 0.78f);

            var (scrollRect, contentRect) = PanelBuilder.CreateVerticalScrollView("Scroll", scrollPanel);
            PanelBuilder.SetFill(scrollRect.GetComponent<RectTransform>());

            float yOffset = 0f;

            // --- Run Statistics ---
            yOffset = AddSectionHeader(contentRect, "-- STATISTICS --", UIStyleConfig.TextPrimary, yOffset);

            int floorReached = _runData.IsBossFloor || _runData.CurrentFloor == RunConfig.FloorsPerAct
                ? _runData.CurrentFloor
                : _runData.CurrentFloor;
            yOffset = AddStatLine(contentRect, $"Reached: Act {_runData.CurrentAct} - Floor {floorReached}", yOffset);
            yOffset = AddStatLine(contentRect, $"Battles: {_runData.TotalBattles}", yOffset);
            yOffset = AddStatLine(contentRect, $"Enemies Slain: {_runData.TotalKills}", yOffset);
            yOffset = AddStatLine(contentRect, $"Gold: {_runData.Gold}", yOffset);
            yOffset += 10f;

            // --- Surviving Party ---
            if (_runData.Party.Count > 0)
            {
                yOffset = AddSectionHeader(contentRect, "-- SURVIVORS --", UIStyleConfig.AccentGreen, yOffset);

                foreach (CharacterData c in _runData.Party)
                {
                    Color classColor = UIFormatUtil.GetClassColor(c.Class);
                    yOffset = AddCharacterLine(contentRect, c.Name, c.Class, c.Level, classColor, yOffset);
                }
                yOffset += 10f;
            }

            // --- Fallen Memorial ---
            if (_runData.Fallen.Count > 0)
            {
                yOffset = AddSectionHeader(contentRect, "-- FALLEN --", UIStyleConfig.DeathTextColor, yOffset);

                foreach (CharacterData c in _runData.Fallen)
                {
                    yOffset = AddCharacterLine(contentRect, c.Name, c.Class, c.Level,
                        UIStyleConfig.DeathTextColor, yOffset);
                }
                yOffset += 10f;
            }

            // Set content height
            contentRect.sizeDelta = new Vector2(0, yOffset);

            // --- Return to Menu button ---
            Button menuBtn = PanelBuilder.CreateButton("MenuButton", rootRect,
                "RETURN TO MENU", UIStyleConfig.TextPrimary, UIStyleConfig.FontSizeSmall);
            RectTransform btnRect = menuBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.25f, 0.03f, 0.75f, 0.11f);
            menuBtn.onClick.AddListener(() => _menuPressed = true);
        }

        private float AddSectionHeader(RectTransform parent, string text, Color color, float yOffset)
        {
            float height = 22f;
            TextMeshProUGUI tmp = PanelBuilder.CreateText("Section", parent,
                text, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, color);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -yOffset);
            rect.sizeDelta = new Vector2(0, height);
            return yOffset + height + 4f;
        }

        private float AddStatLine(RectTransform parent, string text, float yOffset)
        {
            float height = 18f;
            TextMeshProUGUI tmp = PanelBuilder.CreateText("Stat", parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, UIStyleConfig.TextDimmed);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(10f, -yOffset);
            rect.sizeDelta = new Vector2(-20f, height);
            return yOffset + height + 2f;
        }

        private float AddCharacterLine(RectTransform parent, string name, CharacterClass cls,
            int level, Color color, float yOffset)
        {
            float height = 18f;
            string text = $"{name} - {cls} Lv{level}";
            TextMeshProUGUI tmp = PanelBuilder.CreateText("Char", parent,
                text, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineLeft, color);
            RectTransform rect = tmp.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(10f, -yOffset);
            rect.sizeDelta = new Vector2(-20f, height);
            return yOffset + height + 2f;
        }

        public void Show()
        {
            _menuPressed = false;
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
