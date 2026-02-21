using UnityEngine;
using UnityEngine.InputSystem;


public class characterController : MonoBehaviour
{
    public Rigidbody rb;
    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerShoot;
    public InputAction playerDash;

    private Vector3 inputMove;
    private float inputJump;
    private float inputShoot;
    private float inputDash;
    private Vector3 verVel;

    public float moveSpeed;
    public float jumpForce;
    public float raycastLength;
    public float dashForce;


    public float dashCooldown;
    private float dashTime = 0;
    

    bool isGrounded = false;

    

    public bool is3D;
    public void OnEnable()
    {
        playerMove.Enable();
        playerJump.Enable();
        playerShoot.Enable();
        playerDash.Enable();
    }

    public void OnDisable()
    {
        playerMove.Disable();
        playerJump.Disable();
        playerShoot.Disable();
        playerDash.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        verVel = transform.up * jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        dashTime -= Time.deltaTime;
        inputMove = playerMove.ReadValue<Vector3>();
        inputJump = playerJump.ReadValue<float>();
        inputShoot = playerShoot.ReadValue<float>();
        inputDash = playerDash.ReadValue<float>();
    }

    private void FixedUpdate()
    {


        transform.Translate(inputMove * moveSpeed * Time.fixedDeltaTime);
        

        if (getIsGrounded() && inputJump > 0)
        {
            rb.AddForce(verVel, ForceMode.Force);
            
        }

        if (inputDash > 0 && dashTime <= 0)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.Force);
            dashTime = dashCooldown;
        }

        
        Debug.DrawRay(transform.position, Vector3.down * raycastLength, Color.red);
      
    }

    private bool getIsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, raycastLength, LayerMask.GetMask("Floor"));
    }


}
