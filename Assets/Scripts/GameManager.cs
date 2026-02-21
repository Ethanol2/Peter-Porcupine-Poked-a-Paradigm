using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Shift Settings")]
    [SerializeField] private bool _is3D = false;
    [SerializeField] private float _transitionTime = 2f;

    [Header("World Settings")]
    [SerializeField] private float _2DZDepth = -4.85f;

    public UnityEvent<bool> On3DChange;

    public static GameManager Instance { get { if (_instance == null) { Debug.Log("No Game Manager in the Scene!"); return null; } return _instance; } }
    public bool Is3D => _is3D;
    public float TransitionTime => _transitionTime;
    public float ZDepth2D => _2DZDepth;

    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Only one Game Manager alowed in the scene at once. Destroying copy");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Toggle3D();
        }
    }
#endif

    [ContextMenu("Toggle 3D")]
    public void Toggle3D() => Toggle3D(!_is3D);
    public void Toggle3D(bool value)
    {
        _is3D = value;
        On3DChange.Invoke(_is3D);
    }
}
