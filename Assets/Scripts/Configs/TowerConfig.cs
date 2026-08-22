using UnityEngine;

namespace Configs
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

        public GameObject[] GetSectionOrder(int num)
        {
            if (num < 2)
            {
                Debug.Log("Количество секций в акте не может быть меньше двух");
                return null;
            }

            GameObject[] sectionOrder = new GameObject[num];
            sectionOrder[0] = transitionStart;

            int remainingSlots = num - 1;

            ShuffleArray(poolA);

            int index = 1;

            int uniqueCount = Mathf.Min(poolA.Length, remainingSlots);
            for (int i = 0; i < uniqueCount; i++)
            {
                sectionOrder[index] = poolA[i];
                index++;
            }

            while (index < num)
            {
                sectionOrder[index] = poolA[Random.Range(0, poolA.Length)];
                index++;
            }

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

