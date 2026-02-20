using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameManager.Is3D)
        {
            gameManager.Toggle3D(!gameManager.Is3D);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
