using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryObject : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody)
        {
            if (collision.rigidbody.tag == "Player")
            {
                SceneManager.LoadScene(3);
            }
        }
    }
}
