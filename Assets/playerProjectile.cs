using UnityEngine;

public class playerProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float projectileSpeed;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.AddForce(characterController.lookDirection * projectileSpeed, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

}
