using Enums;
using UnityEngine;

public class PlayerMovementSystem : MonoBehaviour
{
    [SerializeField]
    private BodyMovement _body;
    
    [SerializeField]
    private HandGrip _leftHand;

    [SerializeField]
    private HandGrip _rightHand;
    
    public bool IsMoveable = true;

    void Update()
    {
        if (!IsMoveable)
            return;
        
        float input = 0f;
        
        if (Input.GetKey(KeyCode.A))
            input -= 1f;
        
        if (Input.GetKey(KeyCode.D))
            input += 1f;

        _body.AddForce(input);
        
        // upd arm pos
        if (Input.GetMouseButtonDown((int)HandSide.Left))
        {
            _leftHand.StartRaising();
        }
        
        if (Input.GetMouseButtonUp((int)HandSide.Left))
        {
            _leftHand.StopRaising();
        }
        
        if (Input.GetMouseButtonDown((int)HandSide.Right))
        {
            _rightHand.StartRaising();
        }
        
        if (Input.GetMouseButtonUp((int)HandSide.Right))
        {
            _rightHand.StopRaising();
        }
    }
}
