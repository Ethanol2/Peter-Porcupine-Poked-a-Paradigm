using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Space]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool movingLeft = false;

    [Space]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Collider platform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = startPosition.position;
        movingLeft = Random.Range(0f, 1f) > 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * platform.transform.right * (movingLeft ? -1f : 1f) * Time.deltaTime;
        if (!platform.bounds.Contains(transform.position))
        {
            movingLeft = !movingLeft;
        }
    }

    public void On3DChange(bool value)
    {
        //do something
    }
}
