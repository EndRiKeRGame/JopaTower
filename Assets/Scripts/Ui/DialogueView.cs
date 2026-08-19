using System;
using Enums;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Ui
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _dialogueCG;
        [SerializeField] private Button _skipButton;
        [SerializeField] private Image _jackImage;
        [SerializeField] private TMP_Text _dialogueText;
        [SerializeField] private DialogueSpritesConfig _dialogueSpritesConfig;

        private FullDialogueAsset _currentDialogue;
        private int _currentIndex;
        private Action _onDialogueComplete;

        private void Awake()
        {
            _skipButton.onClick.AddListener(ShowNextDialogue);

            _dialogueCG.alpha = 0f;
            _dialogueCG.interactable = false;
            _dialogueCG.blocksRaycasts = false;
        }

        public void ShowDialogue(FullDialogueAsset asset, Action onComplete = null)
        {
            _currentDialogue = asset;
            _currentIndex = 0;
            _onDialogueComplete = onComplete;

            Tween.Alpha(_dialogueCG, 1f, 0.2f);
            _dialogueCG.interactable = true;
            _dialogueCG.blocksRaycasts = true;

            UpdateDialogue(_currentDialogue.Dialogue[_currentIndex]);
        }

        private void ShowNextDialogue()
        {
            if (_currentDialogue == null) return;

            _currentIndex++;
            if (_currentIndex < _currentDialogue.Dialogue.Length)
            {
                UpdateDialogue(_currentDialogue.Dialogue[_currentIndex]);
            }
            else
            {
                HideDialogue();
                _onDialogueComplete?.Invoke();

                _onDialogueComplete = null;
                _currentDialogue = null;
            }
        }

        private void HideDialogue()
        {
            Tween.Alpha(_dialogueCG, 0f, 0.2f);
            _dialogueCG.interactable = false;
            _dialogueCG.blocksRaycasts = false;
        }

        private void UpdateDialogue(DialogueAsset dialogue)
        {
            _dialogueText.text = dialogue.DialogueText;
            _jackImage.sprite = dialogue.CurEmote switch
            {
                EmoteEnum.Chill => _dialogueSpritesConfig.Chill,
                EmoteEnum.Sad => _dialogueSpritesConfig.Sad,
                EmoteEnum.Shock => _dialogueSpritesConfig.Shock,
                EmoteEnum.Happy => _dialogueSpritesConfig.Happy,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}