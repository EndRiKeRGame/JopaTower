using PrimeTween;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "TweenConfig", menuName = "Configs/TweenConfig", order = 0)]
    public class TweenConfig : ScriptableObject
    {
        [field: SerializeField]
        private float _defaultAnimationDuration = 0.2f;
        
        [field: SerializeField]
        public TweenSettings DefaultSettings { get; private set; } =
            new (0.2f);
    }
}

