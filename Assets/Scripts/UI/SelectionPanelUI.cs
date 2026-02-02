using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PixelWarriors
{
    public class SelectionPanelUI
    {
        public RectTransform Root { get; private set; }

        private Button _cancelButton;
        private Button _confirmButton;
        private TextMeshProUGUI _phaseText;
        private TextMeshProUGUI _stagedText;
        private string _stagedActionDescription;

        public void Build(Transform parent)
        {
            Root = PanelBuilder.CreatePanel("SelectionPanel", parent);

            float padding = UIStyleConfig.PanelPadding;
            RectTransform content = PanelBuilder.CreateContainer("Content", Root);
            PanelBuilder.SetFill(content, padding);

            float btnWidth = UIStyleConfig.SelectionButtonWidthRatio;

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

            // Info area (middle)
            RectTransform infoArea = PanelBuilder.CreateContainer("InfoArea", content);
            PanelBuilder.SetAnchored(infoArea, btnWidth, 0, 1 - btnWidth, 1, 4, 0, -4, 0);

            // Phase text (top half)
            _phaseText = PanelBuilder.CreateText("PhaseText", infoArea, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.AccentCyan);
            RectTransform phaseRect = _phaseText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(phaseRect, 0, 0.5f, 1, 1);

            // Staged action text (bottom half)
            _stagedText = PanelBuilder.CreateText("StagedText", infoArea, "",
                UIStyleConfig.FontSizeTiny, TextAlignmentOptions.Center, UIStyleConfig.StagedHighlight);
            RectTransform stagedRect = _stagedText.GetComponent<RectTransform>();
            PanelBuilder.SetAnchored(stagedRect, 0, 0, 1, 0.5f);
            _stagedText.overflowMode = TextOverflowModes.Ellipsis;

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
                    _phaseText.text = "Select an ability";
                    _stagedText.text = "";
                    break;

                case PlayerInputPhase.SelectingTarget:
                    _cancelButton.interactable = true;
                    _confirmButton.interactable = false;
                    _phaseText.text = "Select a target";
                    _stagedText.text = "";
                    break;

                case PlayerInputPhase.AwaitingConfirmation:
                    _cancelButton.interactable = true;
                    _confirmButton.interactable = true;
                    _phaseText.text = "Confirm?";
                    _stagedText.text = _stagedActionDescription ?? "";
                    break;
            }
        }

        private void HandleBattleStateChanged(BattleState state)
        {
            if (state != BattleState.AwaitingInput)
            {
                _cancelButton.interactable = false;
                _confirmButton.interactable = false;
                _phaseText.text = "";
                _stagedText.text = "";
            }
        }

        private void HandleStagedActionChanged(string description)
        {
            _stagedActionDescription = description;
            _stagedText.text = description;
        }
    }
}
