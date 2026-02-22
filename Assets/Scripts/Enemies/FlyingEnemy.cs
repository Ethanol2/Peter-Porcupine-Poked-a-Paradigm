using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] private Vector3 _movementSpeed = Vector3.one * 2f;
    [SerializeField] private float _waveCount = 5f;
    [SerializeField, Range(0f, 1f)] private float _followEase = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _forwardEase = 0.5f;
    [SerializeField] private float _turnAroundWaitTime = 2f;

    private Vector3 _followTarget;
    private float _floatPosition = 0f;
    private float _turnTimer;
    private float _sineTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (_transitioning)
        {
            return;
        }

        _sineTimer += Time.deltaTime;
        _floatPosition = Mathf.Sin(2f * Mathf.PI * _sineTimer * _waveCount / _patrolArea.bounds.size.x) * _patrolArea.bounds.size.y;
        _followTarget = _followTarget + GetMoveDir(_movementDirection * Time.deltaTime);
        _followTarget.y = _floatPosition;

        Vector3 pos = Vector3.Lerp(_body.localPosition, _followTarget, _followEase);
        _3DPatrolDepth = pos.z;
        pos.z = GameManager.Instance.Is3D ? pos.z : _2DPatrolDepth;
        _body.localPosition = pos;

        HandleForward();

        Debug.DrawLine(_body.position, this.transform.TransformPoint(_followTarget), Color.red);


        if (AtBoundsEdgeX(_body.localPosition.x))
        {
            pos = _body.localPosition;
            pos.x = _patrolArea.bounds.extents.x * _movementDirection.x;
            _body.localPosition = pos;

            _movementDirection.x *= -1f;
            _turnTimer = _turnAroundWaitTime;
            _followTarget = _body.localPosition + (GetMoveDir(_movementDirection) * Time.deltaTime * 10f);
        }
        if (AtBoundsEdgeZ(_3DPatrolDepth))
        {
            _3DPatrolDepth = _patrolArea.bounds.extents.z * _movementDirection.z;
            _movementDirection.z *= -1f;
            _followTarget = _body.localPosition + (GetMoveDir(_movementDirection) * Time.deltaTime * 10f);
        }
    }

    private Vector3 GetMoveDir(Vector3 rawMove)
    {
        Vector3 moveDir = Vector3.Scale(rawMove, _movementSpeed);
        return moveDir;
    }

    private void HandleForward()
    {
        Vector3 targetF = _followTarget - _body.localPosition;
        if (!GameManager.Instance.Is3D)
        {
            targetF.z = 0f;
        }

        targetF = Vector3.ProjectOnPlane(targetF, Vector3.forward);
        _body.forward = Vector3.Lerp(_body.forward, targetF, _forwardEase);
    }
}
