using UnityEngine;
using UnityEngine.InputSystem;


public class characterController : MonoBehaviour
{
    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerShoot;

    public Vector3 inputMove;
    public float inputJump;
    public float inputShoot;

    public bool is3D = false;


    
    public void OnEnable()
    {
        playerMove.Enable();
        playerJump.Enable();
        playerShoot.Enable();
    }

    public void OnDisable()
    {
        playerMove.Disable();
        playerJump.Disable();
        playerShoot.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
