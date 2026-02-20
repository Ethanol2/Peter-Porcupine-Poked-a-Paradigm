using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool is3D = false;

    public bool Is3D => is3D;

    public void Toggle3D(bool value)
    {
        is3D = value;
    }
}
