using Enums;
using UnityEngine;

/// <summary>
/// Movement Left and Right
/// Arms Movement
/// </summary>

public class PlayerMovementSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement _body;
    
    [SerializeField]
    private HandGrip _leftHand;

    [SerializeField]
    private HandGrip _rightHand;

    void Update()
    {
        // upd body pos
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
