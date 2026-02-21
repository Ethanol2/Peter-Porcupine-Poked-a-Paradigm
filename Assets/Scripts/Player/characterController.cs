using System.Collections;
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

    [SerializeField] private Camera cam;


    public float dashCooldown;
    private float dashTime = 0;


    [SerializeField] private bool isGrounded = false;

    private bool _perspectiveChanging = false;

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
        GameManager.Instance.On3DChange.AddListener(On3DChange);
        cam = Camera.main;
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
        if (_perspectiveChanging)
        {
            return;
        }

        if (!GameManager.Instance.Is3D)
        {
            inputMove.z = 0f;
        }

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
        return isGrounded = Physics.Raycast(transform.position, Vector3.down, raycastLength, LayerMask.GetMask("Floor"));
    }

    private void On3DChange(bool is3d)
    {
        if (!is3d)
        {
            StartCoroutine(MovePlayerTo2DPlane());
        }
        StartCoroutine(FollowCameraForward());
    }
    private IEnumerator MovePlayerTo2DPlane()
    {
        Vector3 start = this.transform.localPosition;
        Vector3 end = start;
        end.z = GameManager.Instance.ZDepth2D;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / GameManager.Instance.TransitionTime;
            this.transform.localPosition = Vector3.Lerp(start, end, t);

            yield return null;
        }

        this.transform.localPosition = end;
    }
    private IEnumerator FollowCameraForward()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / GameManager.Instance.TransitionTime;

            Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, this.transform.up);
            this.transform.forward = forward;

            yield return null;
        }
    }


}
