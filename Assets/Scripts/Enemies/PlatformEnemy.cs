using System.Collections;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformEnemy : MonoBehaviour
{
    [SerializeField] private Collider _walkArea;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _body;

    [Header("Stats")]
    [SerializeField] private int _health;

    [Header("Behaviour")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _turnAroundWaitTime = 1f;
    [SerializeField] private Vector3 _movementDirection = Vector3.zero;

    [Space]
    [SerializeField] private float _deathAnimTime = 1f;
    [SerializeField] private AnimationCurve _deathAnimCurve;
    [SerializeField] private bool _destroyAfterDeath = true;

    [Header("Spawning")]
    [Tooltip("Absolute values (0.5 is halfway)")]
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private bool _randomXStart;
    [SerializeField] private bool _randomZStart;

    [Space]
    [SerializeField] private bool _randomMovementDirection;

    private Vector3 _platformLocalPos;
    private float _turnTimer = 0f;
    private bool _transitioning = false;
    private float _platformEdge;
    private float _zPos;

    public Vector3 MovementDirection => _movementDirection;

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
        pos.z = -0.5f * _walkArea.bounds.size.z;

        this.transform.position += pos + _walkArea.transform.position;
        _platformLocalPos = pos;

        _platformEdge = _walkArea.transform.position.z + (-0.5f * _walkArea.bounds.size.z);
        _zPos = this.transform.position.z;

        if (_randomMovementDirection)
        {
            _movementDirection = new Vector3(Random.Range(0f, 1f) > 0.5f ? 1f : -1f, 0f, Random.Range(0, 1f) > 0.5f ? 1f : -1f);
        }

    }
    void OnEnable()
    {
        GameManager.Instance.On3DChange.AddListener(On3DChange);
    }
    void OnDisable()
    {
        GameManager.Instance.On3DChange.RemoveListener(On3DChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (_transitioning)
        {
            return;
        }

        if (_turnTimer > 0f)
        {
            _turnTimer -= Time.deltaTime;
            if (_turnTimer < 0f)
            {
                this.transform.forward = _movementDirection;
            }
            return;
        }

        Move(Time.deltaTime * _speed * _movementDirection);

        if (AtBoundsEdgeX())
        {
            _movementDirection.x *= -1f;
            _turnTimer = _turnAroundWaitTime;
            Move(_movementDirection * 0.01f);
        }
        if (AtBoundsEdgeZ())
        {
            _movementDirection.z *= -1f;
            this.transform.forward = _movementDirection;

            Move(_movementDirection * 0.01f);
        }
    }

    public void TakeHit()
    {
        _health--;
        if (_health <= 0)
        {
            StartCoroutine(DeathAnimation());
        }
    }
    [ContextMenu("Kill")]
    public void Kill()
    {
        StartCoroutine(DeathAnimation());
    }
    private IEnumerator DeathAnimation()
    {
        _movementDirection = Vector3.zero;
        yield return new WaitWhile(() => _transitioning);

        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(1f, 0f, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _deathAnimTime;
            _body.localScale = Vector3.LerpUnclamped(startScale, endScale, _deathAnimCurve.Evaluate(t));
            yield return null;
        }

        this.gameObject.SetActive(false);
        if (_destroyAfterDeath)
        {
            Destroy(this.gameObject);
        }
    }

    public void On3DChange(bool is3D)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(is3D));
    }
    private IEnumerator Transition(bool is3D)
    {
        _transitioning = true;

        Vector3 start, end;
        start = end = this.transform.position;
        if (is3D)
        {
            start.z = _platformEdge;
            end.z = _zPos;
        }
        else
        {
            start.z = _zPos;
            end.z = _platformEdge;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / GameManager.Instance.TransitionTime;
            this.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        this.transform.position = end;

        _transitioning = false;
    }

    private bool AtBoundsEdgeX() => Mathf.Abs(_platformLocalPos.x) > _walkArea.bounds.extents.x;
    private bool AtBoundsEdgeZ() => Mathf.Abs(_platformLocalPos.z) > _walkArea.bounds.extents.z;
    private void Move(Vector3 dir)
    {
        _platformLocalPos += dir;
        _zPos += dir.z;
        Vector3 position = this.transform.position + dir;
        position.z = GameManager.Instance.Is3D ? position.z : _platformEdge;
        this.transform.position = position;
    }
}
