using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Shift Settings")]
    [SerializeField] private bool _is3D = false;
    [SerializeField] private float _transitionTime = 2f;
    [SerializeField] private bool _devMode = false;

    [Header("World Settings")]
    [SerializeField] private float _2DZDepth = -4.85f;

    [Header("Stats")]
    [SerializeField] private float _insanitySeconds = 10f;
    [SerializeField] private float _insanity = 10f;

    [Space]
    [SerializeField] private int _totalHitPoints = 5;
    [SerializeField] private int _hitPoints = 5;
    [SerializeField] private float _stunTime = 2f;
    [SerializeField] private float _stunTimer = 0f;

    [Space]
    [SerializeField] private int _points = 0;

    public UnityEvent<bool> On3DChange;
    public UnityEvent<int> OnHealthChange;
    public UnityEvent<int> OnPointsChange;
    public UnityEvent OnDead;
    public UnityEvent OnStunned;
    public UnityEvent OnNotStunned;

    public static GameManager Instance { get { if (_instance == null) { Debug.Log("No Game Manager in the Scene!"); return null; } return _instance; } }
    public bool Is3D => _is3D;
    public float TransitionTime => _transitionTime;
    public float ZDepth2D => _2DZDepth;
    public float InsanityNormalized => _insanity / _insanitySeconds;
    public int MaxHitPoints => _totalHitPoints;
    public bool Stunned => _stunTimer > 0f;

    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Only one Game Manager alowed in the scene at once. Destroying copy");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        _insanity = _insanitySeconds;
        _hitPoints = _totalHitPoints;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Toggle3D();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ChangeHitpointsBy(-1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ChangeHitpointsBy(1);
        }
#endif

        if (_stunTimer > 0f)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0f)
            {
                OnNotStunned.Invoke();
            }
        }

        if (_devMode) return;

        _insanity = Mathf.Clamp(_insanity + Time.deltaTime * (_is3D ? -1f : 1f), 0f, _insanitySeconds);
        if (_insanity <= 0f)
        {
            Toggle3D(false);
        }
    }

    [ContextMenu("Toggle 3D")]
    public void Toggle3D() => Toggle3D(!_is3D);
    public void Toggle3D(bool is3D)
    {
        _is3D = is3D;
        On3DChange.Invoke(_is3D);
    }

    public void ChangeHitpointsBy(int count = 1)
    {
        if (Stunned) return;

        _hitPoints = Mathf.Clamp(_hitPoints + count, 0, _totalHitPoints);
        _stunTimer = _stunTime;
        OnStunned.Invoke();
        OnHealthChange.Invoke(_hitPoints);
        if (_hitPoints <= 0)
        {
            OnDead.Invoke();
            SceneManager.LoadScene(2);
        }
    }

    public void ChangePointsBy(int count = 1)
    {
        _points += count;
        OnPointsChange.Invoke(_points);
    }
}
