using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    [SerializeField] private Collider _walkArea;
    [SerializeField] private Rigidbody _rb;

    [Header("Behaviour")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _turnAroundWaitTime = 1f;
    [SerializeField] private Vector3 _movementDirection = Vector3.zero;

    [Header("Spawning")]
    [Tooltip("Absolute values (0.5 is halfway)")]
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private bool _randomXStart;
    [SerializeField] private bool _randomZStart;

    [Space]
    [SerializeField] private bool _randomMovementDirection;

    private Vector3 _platformLocalPos;
    private float _turnTimer = 0f;

    void OnValidate()
    {
        if (!_rb)
        {
            _rb = this.GetComponent<Rigidbody>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Start")]
    void Start()
    {
        _rb.isKinematic = true;

        if (_randomXStart)
            _startPosition.x = Random.Range(0f, 1f);
        if (_randomZStart)
            _startPosition.y = Random.Range(0f, 1f);

        Vector3 pos = Vector3.zero;
        pos.x = (_startPosition.x - 0.5f) * _walkArea.bounds.size.x;

        if (GameManager.Instance.Is3D)
        {
            pos.z = (_startPosition.y - 0.5f) * _walkArea.bounds.size.z;
        }
        else
        {
            pos.z = -0.5f * _walkArea.bounds.size.z;
        }

        this.transform.position += pos;
        _platformLocalPos = pos;

        if (_randomMovementDirection)
        {
            _movementDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_turnTimer > 0f)
        {
            _turnTimer -= Time.deltaTime;
            if (_turnTimer < 0f)
            {
                this.transform.forward = _movementDirection;
            }
            return;
        }

        Move(Time.deltaTime * _speed * Vector3.Scale(_movementDirection, GameManager.Instance.Is3D ? Vector3.one : Vector3.right));

        if (AtBoundsEdgeX())
        {
            _movementDirection.x *= -1f;
            _turnTimer = _turnAroundWaitTime;
            Move(_movementDirection * 0.01f);
        }
        if (AtBoundsEdgeZ())
        {
            _movementDirection.z *= -1f;
            _turnTimer = _turnAroundWaitTime;
            Move(_movementDirection * 0.01f);
        }
    }

    public void On3DChange(bool value)
    {
        //do something
    }

    private bool AtBoundsEdgeX() => Mathf.Abs(_platformLocalPos.x) > _walkArea.bounds.extents.x;
    private bool AtBoundsEdgeZ() => Mathf.Abs(_platformLocalPos.z) > _walkArea.bounds.extents.z;
    private void Move(Vector3 dir)
    {
        _platformLocalPos += dir;
        this.transform.position += dir;
    }
}
