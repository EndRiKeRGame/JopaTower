using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/TowerConfig", order = 0)]
    public class TowerConfig : ScriptableObject
    {
        [field: SerializeField]
        public ActConfig Prologue { get; private set; }
        
        [field: SerializeField]
        public ActConfig Act1 { get; private set; }
        
        [field: SerializeField]
        public ActConfig Act2 { get; private set; }
        
        [field: SerializeField]
        public ActConfig Act3 { get; private set; }
        
        [field: SerializeField]
        public ActConfig Finale { get; private set; }
    }
    
    [System.Serializable]
    public class ActConfig
    {
        [Header("Секция 1 (переход в акт)")]
        public GameObject transitionStart;

        [Header("Секции 2-5 (пул A)")]
        public GameObject[] poolA;

        [Header("Секции 6-9 (пул B)")]
        public GameObject[] poolB;

        [Header("Секция 10 (переход из акта)")]
        public GameObject transitionEnd;

        public GameObject[] GetSectionOrder(int num)
        {
            if (num < 2)
            {
                Debug.Log("Количество cекций в акте не может быть меньше двух");
                return null;
            }
            
            GameObject[] sectionOrder = new GameObject[num];
            sectionOrder[0] = transitionStart;
            sectionOrder[^1] = transitionEnd;

            int half = num / 2;

            for (int i = 1; i < half; i++)
                sectionOrder[i] = poolA[Random.Range(0, poolA.Length)];
            
            for (int i = half; i < num - 1; i++)
                sectionOrder[i] = poolB[Random.Range(0, poolB.Length)];
            
            return sectionOrder;
        }
    }
}

