using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/TowerConfig", order = 0)]
    public class TowerConfig : ScriptableObject
    {
        [field: SerializeField]
        public GameObject WallsPrefab { get; set; }
        
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
        public GameObject transitionStart;
        public GameObject[] poolA;
        public GameObject[] poolB;
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
            if (poolA.Length < half || poolB.Length < half)
            {
                Debug.Log("poolA or poolB small for half");
                return null;
            }
            
            ShuffleArray(poolA);
            for (int i = 1, j = 0; i < half; i++, j++)
                sectionOrder[i] = poolA[j];
            
            ShuffleArray(poolB);
            for (int i = half, j = 0; i < num - 1; i++, j++)
                sectionOrder[i] = poolB[j];
            
            return sectionOrder;
        }
        
        void ShuffleArray(GameObject[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                // Выбираем случайный индекс от 0 до i включительно
                int randomIndex = Random.Range(0, i + 1);
        
                // Меняем местами элементы
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
        }
        
        public GameObject[] GetSectionOrderForPrologue()
        {
            return poolA;
        }
        
        public GameObject[] GetSectionOrderForFinale()
        {
            return new [] {transitionStart};
        }
    }
}

