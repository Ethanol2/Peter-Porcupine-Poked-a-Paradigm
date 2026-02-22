using UnityEngine;

public class playerProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float projectileSpeed;
    public GameObject player;
    public GameObject projectile;

    float projectileTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.AddForce(characterController.lookDirection * projectileSpeed, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        projectileTime -= Time.deltaTime;

        if (projectileTime <= 0f)
        {
            Destroy(projectile);
        }
    }

   

}
