using UnityEngine;

public class TriggerRoom : TowerPart
{
    [SerializeField]
    private GameObject _trigger;

    public float GetTriggerYPosition()
    {
        return _trigger.transform.position.y;
    }
}