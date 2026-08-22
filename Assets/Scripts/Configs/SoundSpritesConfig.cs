using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "SoundSpritesConfig", menuName = "Configs/SoundSpritesConfig", order = 0)]
    public class SoundSpritesConfig : ScriptableObject
    {
        [field: SerializeField]
        public Sprite SoundOn { get; set; }
        
        [field: SerializeField]
        public Sprite SoundOff { get; set; }
    }
}

