using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool is3D = false;

    public UnityEvent<bool> On3DChange;

    public bool Is3D => is3D;

    void Start()
    {
        On3DChange.Invoke(is3D);
    }

    [ContextMenu("Toggle 3D")]
    public void Toggle3D() => Toggle3D(!is3D);
    public void Toggle3D(bool value)
    {
        is3D = value;
        On3DChange.Invoke(is3D);
    }
}
