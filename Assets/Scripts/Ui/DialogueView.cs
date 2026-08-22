using System;
using System.Collections.Generic;
using Configs;
using Enums;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
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

        // Действия, которые нужно выполнить при показе реплики с определённым индексом
        private Dictionary<int, Action> _positionActions = new Dictionary<int, Action>();

        private void Awake()
        {
            _skipButton.onClick.AddListener(ShowNextDialogue);

            _dialogueCG.alpha = 0f;
            _dialogueCG.interactable = false;
            _dialogueCG.blocksRaycasts = false;
        }
        
        public void ShowDialogue(FullDialogueAsset asset, Action onComplete = null,
            Dictionary<int, Action> positionActions = null)
        {
            _currentDialogue = asset;
            _onDialogueComplete = onComplete;
            _positionActions = positionActions ?? new Dictionary<int, Action>();

            Tween.Alpha(_dialogueCG, 1f, 0.2f);
            _dialogueCG.interactable = true;
            _dialogueCG.blocksRaycasts = true;

            GoToIndex(0);
        }

        private void ShowNextDialogue()
        {
            if (_currentDialogue == null) return;

            int nextIndex = _currentIndex + 1;
            if (nextIndex < _currentDialogue.Dialogue.Length)
            {
                GoToIndex(nextIndex);
            }
            else
            {
                HideDialogue();
                _onDialogueComplete?.Invoke();

                _onDialogueComplete = null;
                _currentDialogue = null;
                _positionActions.Clear();
            }
        }

        private void GoToIndex(int index)
        {
            _currentIndex = index;
            UpdateDialogue(_currentDialogue.Dialogue[_currentIndex]);

            if (_positionActions.TryGetValue(_currentIndex, out Action action))
                action?.Invoke();
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
                EmoteEnum.Toilet => _dialogueSpritesConfig.Toilet,
                EmoteEnum.Shards => _dialogueSpritesConfig.Shards,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}