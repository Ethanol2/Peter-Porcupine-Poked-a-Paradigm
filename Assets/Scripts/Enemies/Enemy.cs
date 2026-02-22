using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected Collider _patrolArea;
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected Transform _body;

    [Header("Stats")]
    [SerializeField] protected int _health;

    [Header("Spawning")]
    [SerializeField] protected Vector2 _startPosition;
    [SerializeField] protected bool _randomStartPosition;

    [Space]
    [SerializeField] protected bool _randomMovementDirection;

    [Header("Behaviour")]
    [SerializeField] protected Vector3 _movementDirection = Vector3.zero;

    protected float _2DPatrolDepth;
    protected float _3DPatrolDepth;
    protected bool _transitioning = false;

    protected virtual void OnValidate()
    {
        if (!_rb)
        {
            _rb = this.GetComponent<Rigidbody>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Start")]
    protected virtual void Start()
    {
        _rb.isKinematic = true;
        this.transform.position = _patrolArea.transform.position;

        if (_randomStartPosition)
        {
            _startPosition.x = Random.Range(0f, 1f);
            _startPosition.y = Random.Range(0f, 1f);
        }

        Vector3 pos = Vector3.zero;
        pos.x = (_startPosition.x - 0.5f) * _patrolArea.bounds.size.x;
        pos.z = -0.5f * _patrolArea.bounds.size.z;

        _body.localPosition = pos;

        _2DPatrolDepth = -_patrolArea.bounds.extents.z;
        _3DPatrolDepth = 0f;

        if (_randomMovementDirection)
        {
            _movementDirection = new Vector3(Random.Range(0f, 1f) > 0.5f ? 1f : -1f, 0f, Random.Range(0, 1f) > 0.5f ? 1f : -1f);
        }
    }

    protected virtual void OnPerspectiveChange(bool is3D)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(is3D));
    }
    private IEnumerator Transition(bool is3D)
    {
        _transitioning = true;

        Vector3 start, end;
        start = end = _body.localPosition;
        if (is3D)
        {
            start.z = _2DPatrolDepth;
            end.z = _3DPatrolDepth;
        }
        else
        {
            start.z = _3DPatrolDepth;
            end.z = _2DPatrolDepth;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / GameManager.Instance.TransitionTime;
            _body.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        _body.localPosition = end;

        _transitioning = false;
    }

    protected bool AtBoundsEdgeX(float x) => Mathf.Abs(x) > _patrolArea.bounds.extents.x;
    protected bool AtBoundsEdgeZ(float z) => Mathf.Abs(z) > _patrolArea.bounds.extents.z;
}
