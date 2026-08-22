using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "DialogueSpritesConfig", menuName = "Configs/DialogueSpritesConfig", order = 0)]
    public class DialogueSpritesConfig : ScriptableObject
    {
        [field: SerializeField]
        public Sprite Chill { get; set; }
        
        [field: SerializeField]
        public Sprite Shock { get; set; }
        
        [field: SerializeField]
        public Sprite Sad { get; set; }
        
        [field: SerializeField]
        public Sprite Happy { get; set; }
        
        [field: SerializeField]
        public Sprite Toilet { get; set; }
        
        [field: SerializeField]
        public Sprite Shards { get; set; }
    }
}

