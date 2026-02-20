using UnityEngine;
using UnityEngine.InputSystem;


public class characterController : MonoBehaviour
{
    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerShoot;

    public bool is3D = false;


    
    public void OnEnable()
    {
        playerMove.Enable();
        playerJump.Enable();
        playerShoot.Enable();
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
