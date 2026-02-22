using System.Collections;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformEnemy : Enemy
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _turnAroundWaitTime = 1f;

    [Space]
    [SerializeField] private float _deathAnimTime = 1f;
    [SerializeField] private AnimationCurve _deathAnimCurve;
    [SerializeField] private bool _destroyAfterDeath = true;

    private float _turnTimer = 0f;

    void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.On3DChange.AddListener(OnPerspectiveChange);
    }
    void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.On3DChange.RemoveListener(OnPerspectiveChange);
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
                _body.forward = GameManager.Instance.Is3D ? _movementDirection : Vector3.Scale(_movementDirection, Vector3.right);
            }
            return;
        }

        Move(Time.deltaTime * _speed * _movementDirection);

        if (AtBoundsEdgeX(_body.localPosition.x))
        {
            Vector3 pos = _body.localPosition;
            pos.x = _patrolArea.bounds.extents.x * _movementDirection.x;
            _body.localPosition = pos;

            _movementDirection.x *= -1f;
            _body.forward = GameManager.Instance.Is3D ? _movementDirection : Vector3.Scale(_movementDirection, Vector3.right);
            _turnTimer = _turnAroundWaitTime;
        }
        if (AtBoundsEdgeZ(_3DPatrolDepth))
        {
            _3DPatrolDepth = _patrolArea.bounds.extents.z * _movementDirection.z;
            _movementDirection.z *= -1f;
            _body.forward = _movementDirection;
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

    private void Move(Vector3 dir)
    {
        _3DPatrolDepth += dir.z;
        Vector3 position = _body.localPosition + dir;
        position.z = GameManager.Instance.Is3D ? position.z : _2DPatrolDepth;
        _body.localPosition = position;
    }
}
