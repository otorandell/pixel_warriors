using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class ActionBarUI
    {
        public RectTransform Root { get; private set; }
        public CombatLogUI CombatLog { get; private set; }

        private Button _cancelButton;
        private Button _confirmButton;
        private TextMeshProUGUI _promptText;
        private string _stagedActionDescription;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("ActionBar", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            float btnWidth = UIStyleConfig.ActionBarButtonWidthRatio;

            // Cancel button (left)
            _cancelButton = PanelBuilder.CreateButton("CancelBtn", content, "<<",
                UIStyleConfig.AccentRed, UIStyleConfig.FontSizeTiny);
            RectTransform cancelRect = _cancelButton.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(cancelRect, 0, 0, btnWidth, 1, 0, 0, -2, 0);
            _cancelButton.onClick.AddListener(GameEvents.RaiseActionCancelled);
            _cancelButton.interactable = false;

            // Confirm button (right)
            _confirmButton = PanelBuilder.CreateButton("ConfirmBtn", content, "OK",
                UIStyleConfig.AccentGreen, UIStyleConfig.FontSizeTiny);
            RectTransform confirmRect = _confirmButton.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(confirmRect, 1 - btnWidth, 0, 1, 1, 2, 0, 0, 0);
            _confirmButton.onClick.AddListener(GameEvents.RaiseActionConfirmed);
            _confirmButton.interactable = false;

            // Middle area for combat log + prompt
            RectTransform logArea = PanelBuilder.CreateContainer("LogArea", content);
            PanelBuilder.SetAnchored(logArea, btnWidth, 0, 1 - btnWidth, 1, 4, 0, -4, 0);

            // Combat log
            CombatLog = new CombatLogUI();
            CombatLog.Build(logArea);
            PanelBuilder.SetFill(CombatLog.Root);

            // Prompt text (overlays combat log, hidden by default)
            _promptText = PanelBuilder.CreateText("PromptText", logArea, "",
                UIStyleConfig.FontSizeSmall, TextAlignmentOptions.Center, UIStyleConfig.AccentCyan);
            RectTransform promptRect = _promptText.GetComponent<RectTransform>();
            PanelBuilder.SetFill(promptRect);
            _promptText.gameObject.SetActive(false);

            GameEvents.OnPlayerInputPhaseChanged += HandleInputPhaseChanged;
            GameEvents.OnBattleStateChanged += HandleBattleStateChanged;
            GameEvents.OnStagedActionChanged += HandleStagedActionChanged;
        }

        private void HandleInputPhaseChanged(PlayerInputPhase phase)
        {
            switch (phase)
            {
                case PlayerInputPhase.SelectingAbility:
                    _cancelButton.interactable = false;
                    _confirmButton.interactable = false;
                    ShowPrompt("Select an ability");
                    break;

                case PlayerInputPhase.SelectingTarget:
                    _cancelButton.interactable = true;
                    _confirmButton.interactable = false;
                    ShowPrompt("Select a target");
                    break;

                case PlayerInputPhase.AwaitingConfirmation:
                    _cancelButton.interactable = true;
                    _confirmButton.interactable = true;
                    ShowPrompt(_stagedActionDescription ?? "Confirm?");
                    break;
            }
        }

        private void HandleBattleStateChanged(BattleState state)
        {
            if (state != BattleState.AwaitingInput)
            {
                _cancelButton.interactable = false;
                _confirmButton.interactable = false;
                HidePrompt();
            }
        }

        private void HandleStagedActionChanged(string description)
        {
            _stagedActionDescription = description;
        }

        private void ShowPrompt(string text)
        {
            _promptText.text = text;
            _promptText.gameObject.SetActive(true);
            CombatLog.Root.gameObject.SetActive(false);
        }

        private void HidePrompt()
        {
            _promptText.gameObject.SetActive(false);
            CombatLog.Root.gameObject.SetActive(true);
        }
    }
}
