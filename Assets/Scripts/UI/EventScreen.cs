using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class EventScreen : IScreen
    {
        private GameObject _root;
        private RectTransform _rootRect;
        private bool _done;

        public bool Done => _done;

        private EventData _eventData;
        private RunData _runData;
        private EventChoice _pendingChoice;

        // Phase containers (toggled via SetActive)
        private RectTransform _choiceContainer;
        private RectTransform _pickContainer;
        private RectTransform _outcomeContainer;

        public EventScreen(EventData eventData, RunData runData)
        {
            _eventData = eventData;
            _runData = runData;
        }

        public void Build(Transform canvasParent)
        {
            _done = false;

            _root = new GameObject("EventScreen");
            _rootRect = _root.AddComponent<RectTransform>();
            _rootRect.SetParent(canvasParent, false);
            PanelBuilder.SetFill(_rootRect);

            // --- Header ---
            string headerLabel = _eventData.Id == "rest_site" ? "REST SITE" : "MYSTERY";
            Color headerColor = _eventData.Id == "rest_site"
                ? UIStyleConfig.AccentCyan
                : UIStyleConfig.AccentMagenta;

            TextMeshProUGUI header = PanelBuilder.CreateText("Header", _rootRect,
                headerLabel, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.MidlineLeft, headerColor);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(headerRect, 0.04f, 0.91f, 0.55f, 0.98f);

            // Act/Floor info
            TextMeshProUGUI infoText = PanelBuilder.CreateText("Info", _rootRect,
                $"Act {_runData.CurrentAct}  Floor {_runData.CurrentFloor}",
                UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.MidlineRight, UIStyleConfig.TextDimmed);
            RectTransform infoRect = infoText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(infoRect, 0.55f, 0.91f, 0.96f, 0.98f);

            // --- Title ---
            TextMeshProUGUI title = PanelBuilder.CreateText("Title", _rootRect,
                _eventData.Title, UIStyleConfig.FontSizeLarge,
                TextAlignmentOptions.Center, headerColor);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(titleRect, 0.05f, 0.80f, 0.95f, 0.90f);

            // --- Narrative (wrapping enabled) ---
            TextMeshProUGUI narrative = PanelBuilder.CreateText("Narrative", _rootRect,
                _eventData.Narrative, UIStyleConfig.FontSizeTiny,
                TextAlignmentOptions.TopLeft, UIStyleConfig.TextDimmed);
            narrative.textWrappingMode = TextWrappingModes.Normal;
            RectTransform narrativeRect = narrative.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(narrativeRect, 0.08f, 0.55f, 0.92f, 0.78f);

            // --- Choice container ---
            _choiceContainer = PanelBuilder.CreateContainer("Choices", _rootRect);
            PanelBuilder.SetAnchored(_choiceContainer, 0.10f, 0.05f, 0.90f, 0.52f);
            BuildChoiceButtons();

            // --- Character pick container (hidden initially) ---
            _pickContainer = PanelBuilder.CreateContainer("CharPick", _rootRect);
            PanelBuilder.SetAnchored(_pickContainer, 0.10f, 0.05f, 0.90f, 0.52f);
            _pickContainer.gameObject.SetActive(false);

            // --- Outcome container (hidden initially) ---
            _outcomeContainer = PanelBuilder.CreateContainer("Outcome", _rootRect);
            PanelBuilder.SetAnchored(_outcomeContainer, 0.10f, 0.05f, 0.90f, 0.52f);
            _outcomeContainer.gameObject.SetActive(false);
        }

        private void BuildChoiceButtons()
        {
            List<EventChoice> choices = _eventData.Choices;

            // Each choice block: button + description + optional condition label
            bool anyHasDesc = choices.Exists(c => !string.IsNullOrEmpty(c.Description));
            float blockHeight = anyHasDesc ? 0.28f : 0.20f;
            blockHeight = Mathf.Min(blockHeight, 0.90f / choices.Count);
            float gap = 0.02f;
            float totalHeight = choices.Count * blockHeight + (choices.Count - 1) * gap;
            float startY = 0.5f + totalHeight / 2f;

            for (int i = 0; i < choices.Count; i++)
            {
                EventChoice choice = choices[i];
                float blockTop = startY - i * (blockHeight + gap);
                float blockBottom = blockTop - blockHeight;

                bool available = choice.Condition == null || choice.Condition(_runData);
                Color btnColor = available ? UIStyleConfig.TextPrimary : UIStyleConfig.TextDimmed;

                // Button takes upper portion of the block
                float btnBottom = blockBottom;
                if (!string.IsNullOrEmpty(choice.Description) || !string.IsNullOrEmpty(choice.ConditionLabel))
                    btnBottom = blockBottom + blockHeight * 0.40f;

                Button btn = PanelBuilder.CreateButton($"Choice_{i}", _choiceContainer,
                    choice.Label, btnColor, UIStyleConfig.FontSizeTiny);
                RectTransform btnRect = btn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(btnRect, 0f, btnBottom, 1f, blockTop);
                btn.interactable = available;

                float descTop = btnBottom;

                // Description text (effect preview)
                if (!string.IsNullOrEmpty(choice.Description))
                {
                    float descBottom = !string.IsNullOrEmpty(choice.ConditionLabel)
                        ? blockBottom + blockHeight * 0.18f
                        : blockBottom;

                    TextMeshProUGUI descText = PanelBuilder.CreateText($"Desc_{i}", _choiceContainer,
                        choice.Description, UIStyleConfig.FontSizeTiny * 0.85f,
                        TextAlignmentOptions.Center,
                        available ? UIStyleConfig.AccentCyan : UIStyleConfig.TextDimmed);
                    descText.textWrappingMode = TextWrappingModes.Normal;
                    RectTransform descRect = descText.GetComponent<RectTransform>();
                    PanelBuilder.SetAnchored(descRect, 0.05f, descBottom, 0.95f, descTop);
                }

                // Condition label
                if (!string.IsNullOrEmpty(choice.ConditionLabel))
                {
                    TextMeshProUGUI condText = PanelBuilder.CreateText($"Cond_{i}", _choiceContainer,
                        choice.ConditionLabel, UIStyleConfig.FontSizeTiny * 0.8f,
                        TextAlignmentOptions.Center,
                        available ? UIStyleConfig.AccentYellow : UIStyleConfig.TextDimmed);
                    RectTransform condRect = condText.GetComponent<RectTransform>();
                    PanelBuilder.SetAnchored(condRect, 0.05f, blockBottom, 0.95f, blockBottom + blockHeight * 0.18f);
                }

                int capturedIdx = i;
                btn.onClick.AddListener(() => OnChoiceSelected(capturedIdx));
            }
        }

        private void OnChoiceSelected(int choiceIndex)
        {
            EventChoice choice = _eventData.Choices[choiceIndex];

            if (choice.NeedsCharacterPick)
            {
                _pendingChoice = choice;
                TransitionToCharacterPick();
            }
            else
            {
                ResolveChoice(choice, -1);
            }
        }

        private void TransitionToCharacterPick()
        {
            _choiceContainer.gameObject.SetActive(false);
            _pickContainer.gameObject.SetActive(true);

            // Clear any existing content
            for (int i = _pickContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _pickContainer.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }

            // Prompt text
            string prompt = _pendingChoice.CharacterPickPrompt ?? "Choose a character.";
            TextMeshProUGUI promptText = PanelBuilder.CreateText("Prompt", _pickContainer,
                prompt, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.TextPrimary);
            promptText.textWrappingMode = TextWrappingModes.Normal;
            RectTransform promptRect = promptText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(promptRect, 0.05f, 0.75f, 0.95f, 0.95f);

            // Character buttons
            int count = _runData.Party.Count;
            float btnHeight = Mathf.Min(0.15f, 0.70f / count);
            float gap = 0.03f;
            float startY = 0.72f;

            for (int i = 0; i < count; i++)
            {
                CharacterData c = _runData.Party[i];
                Color classColor = UIFormatUtil.GetClassColor(c.Class);
                float top = startY - i * (btnHeight + gap);
                float bottom = top - btnHeight;

                Button charBtn = PanelBuilder.CreateButton($"Char_{i}", _pickContainer,
                    $"{c.Name} ({c.Class})", classColor, UIStyleConfig.FontSizeTiny);
                RectTransform charRect = charBtn.GetComponent<RectTransform>();
                PanelBuilder.SetAnchored(charRect, 0.05f, bottom, 0.95f, top);

                int capturedIdx = i;
                charBtn.onClick.AddListener(() => OnCharacterPicked(capturedIdx));
            }
        }

        private void OnCharacterPicked(int partyIndex)
        {
            ResolveChoice(_pendingChoice, partyIndex);
        }

        private void ResolveChoice(EventChoice choice, int partyIndex)
        {
            // Mark unique events as seen
            if (_eventData.IsUnique)
                _runData.SeenEvents.Add(_eventData.Id);

            // Apply the outcome
            choice.Apply(_runData, partyIndex);

            // Get outcome description
            string outcomeText = choice.OutcomeDescription != null
                ? choice.OutcomeDescription(_runData)
                : "";

            TransitionToOutcome(outcomeText);
        }

        private void TransitionToOutcome(string outcomeText)
        {
            _choiceContainer.gameObject.SetActive(false);
            _pickContainer.gameObject.SetActive(false);
            _outcomeContainer.gameObject.SetActive(true);

            // Clear any existing content
            for (int i = _outcomeContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _outcomeContainer.GetChild(i).gameObject;
                child.SetActive(false);
                Object.Destroy(child);
            }

            // Outcome text
            TextMeshProUGUI resultText = PanelBuilder.CreateText("Result", _outcomeContainer,
                outcomeText, UIStyleConfig.FontSizeSmall,
                TextAlignmentOptions.Center, UIStyleConfig.AccentGreen);
            resultText.textWrappingMode = TextWrappingModes.Normal;
            RectTransform resultRect = resultText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(resultRect, 0.05f, 0.40f, 0.95f, 0.90f);

            // Continue button
            Button continueBtn = PanelBuilder.CreateButton("ContinueBtn", _outcomeContainer,
                "CONTINUE", UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeMedium);
            RectTransform btnRect = continueBtn.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(btnRect, 0.20f, 0.05f, 0.80f, 0.30f);
            continueBtn.onClick.AddListener(() => _done = true);
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
