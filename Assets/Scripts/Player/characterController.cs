using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class characterController : MonoBehaviour
{
    [SerializeField] private Camera cam;
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

    private bool is3Drot;

    public float moveSpeed;
    public float jumpForce;
    public float raycastLength;
    public float dashForce;


    [Header("Shooting")]
    public GameObject bullet;
    [SerializeField] private Transform shootSpawn;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private AnimationClip shootAnim;
    [SerializeField] private bool canShoot = true;

    [Header("Drop Shadow")]
    [SerializeField] private Transform dropShadow;
    [SerializeField] private float dropShadowMaxDist = 5f;

    public float dashCooldown;
    private float dashTime = 0;


    public float shootCooldown;
    private float shootTime = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;


    public static Vector3 lookDirection;

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
        shootTime -= Time.deltaTime;
        inputMove = playerMove.ReadValue<Vector3>();
        inputJump = playerJump.ReadValue<float>();
        inputShoot = playerShoot.ReadValue<float>();
        inputDash = playerDash.ReadValue<float>();

        if (!GameManager.Instance.Is3D)
        {
            inputMove.z = 0f;
        }

        float absXInput = Mathf.Abs(inputMove.sqrMagnitude);
        animator.SetFloat("move", absXInput);
        if (absXInput > 0.1f && !Mathf.Approximately(inputMove.x, 0f))
        {
            spriteRenderer.flipX = inputMove.x < 0f;
        }
    }

    private void FixedUpdate()
    {

        if (_perspectiveChanging)
        {
            return;
        }

        transform.Translate(inputMove * moveSpeed * Time.fixedDeltaTime);
     
        if (is3Drot == false && (inputMove.x != 0 || inputMove.z != 0))

        {
            lookDirection = inputMove;

        }

        if (is3Drot == true && (inputMove.x != 0 || inputMove.z != 0))
        {
            lookDirection = Quaternion.Euler(0, -45, 0) * inputMove;
        }
        Debug.DrawRay(transform.position, lookDirection * 2f, Color.cornflowerBlue);
        //Debug.DrawRay(transform.position, inputMove * 2f, Color.cornflowerBlue);

        if (dropShadow)
            SetDropShadow();


        if (getIsGrounded() && inputJump > 0)
        {
            rb.AddForce(verVel, ForceMode.Force);
            animator.SetTrigger("Jump");
        }


        if (inputShoot > 0 && shootTime < 0)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            shootTime = shootCooldown;

        }
        if (inputShoot > 0 && canShoot)
        {
            //Instantiate(bullet, transform.position, transform.rotation);
            animator.SetTrigger("Shoot");
            StartCoroutine(CooldownShoot());

        }

        if (inputDash > 0 && dashTime <= 0 && (inputMove.x != 0 || inputMove.z != 0))
        {
            rb.AddForce(lookDirection * dashForce, ForceMode.Force);
            dashTime = dashCooldown;
            
        }
        //Debug.Log(dashTime);

    }
    private bool getIsGrounded()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, raycastLength, LayerMask.GetMask("Floor"));
        if (grounded && !isGrounded)
        {
            animator.SetTrigger("Land");
        }
        return isGrounded = grounded;
    }
    private void SetDropShadow()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, dropShadowMaxDist, LayerMask.GetMask("Floor")))
        {
            dropShadow.position = hit.point;
            dropShadow.gameObject.SetActive(true);
            dropShadow.localScale = Vector3.one * (1f - (hit.distance / dropShadowMaxDist));

            Debug.Log(hit.distance);
        }
        else
        {
            dropShadow.gameObject.SetActive(false);
        }
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
    private IEnumerator CooldownShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootAnim.length);
        canShoot = true;
    }


}
