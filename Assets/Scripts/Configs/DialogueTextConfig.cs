using System;
using Enums;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "DialogueTextConfig", menuName = "Configs/DialogueTextConfig", order = 0)]
    public class DialogueTextConfig : ScriptableObject
    {
        [field: SerializeField]
        public FullDialogueAsset Prologue { get; set; }
        
        [field: SerializeField]
        public FullDialogueAsset Act1 { get; set; }
        
        [field: SerializeField]
        public FullDialogueAsset Act2 { get; set; }
        
        [field: SerializeField]
        public FullDialogueAsset Act3 { get; set; }
        
        [field: SerializeField]
        public FullDialogueAsset Finale { get; set; }
    }
    
    [Serializable]
    public class FullDialogueAsset
    {
        public DialogueAsset[] Dialogue;
    }

    [Serializable]
    public class DialogueAsset
    {
        public string DialogueText;
        public EmoteEnum CurEmote;
    }
}

