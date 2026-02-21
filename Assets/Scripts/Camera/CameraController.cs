using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform camParent;
    [SerializeField] private Camera cam;

    [Header("Camera Parent Positions")]
    [SerializeField] private Transform _2DPosition;
    [SerializeField] private Transform _3DPosition;

    [Header("Camera Positions")]
    [SerializeField] private Vector3 _cam2DPos = new Vector3(0f, 0f, -40f);
    [SerializeField] private Vector3 _cam3DPos = new Vector3(0f, 0f, -8f);

    [Space]
    [SerializeField] private float _2DFOV = 10f;
    [SerializeField] private float _3DFOV = 60f;

    [Header("Animation Settings")]
    [Tooltip("Relative to the 2D transition")]
    [SerializeField] private float _orthoChangeTime = 0.2f;
    [SerializeField] private AnimationCurve _fovCurve;
    [SerializeField] private AnimationCurve _distanceCurve;
    [SerializeField] private AnimationCurve _parentCurve;

    void Start()
    {
        if (GameManager.Instance.Is3D)
            SnapTo3D();
        else
            SnapTo2D();
    }

    public void OnPerspectiveChange(bool is3D)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionCamera(is3D));
    }

    [ContextMenu("Snap to 2D View")]
    public void SnapTo2D()
    {
        camParent.localRotation = _2DPosition.localRotation;
        cam.transform.localPosition = _cam2DPos;
        cam.fieldOfView = _2DFOV;
        cam.orthographic = true;
    }
    [ContextMenu("Snap to 3D View")]
    public void SnapTo3D()
    {
        camParent.localRotation = _3DPosition.localRotation;
        cam.transform.localPosition = _cam3DPos;
        cam.fieldOfView = _3DFOV;
        cam.orthographic = false;
    }

    private IEnumerator TransitionCamera(bool is3D)
    {
        float duration = GameManager.Instance.TransitionTime / 2f;
        if (is3D)
        {
            yield return MoveCamera(_cam2DPos, _cam3DPos, _2DFOV, _3DFOV, duration, 1f - _orthoChangeTime, true);
            yield return MoveParent(_2DPosition.localRotation, _3DPosition.localRotation, duration);
        }
        else
        {
            yield return MoveParent(_3DPosition.localRotation, _2DPosition.localRotation, duration);
            yield return MoveCamera(_cam3DPos, _cam2DPos, _3DFOV, _2DFOV, duration, _orthoChangeTime, false);
        }
    }

    private IEnumerator MoveCamera(Vector3 start, Vector3 end, float fovStart, float fovEnd, float duration, float orthoToggleTime, bool isOrthographic)
    {
        bool orthoToggled = false;
        float t = Mathf.InverseLerp(start.z, end.z, cam.transform.localPosition.z);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            cam.transform.localPosition = Vector3.Lerp(start, end, _distanceCurve.Evaluate(t));
            cam.fieldOfView = Mathf.Lerp(fovStart, fovEnd, _distanceCurve.Evaluate(t));

            if (!orthoToggled && t > orthoToggleTime)
            {
                cam.orthographic = !isOrthographic;
            }

            yield return null;
        }

        cam.transform.localPosition = end;
        cam.fieldOfView = fovEnd;
    }

    private IEnumerator MoveParent(Quaternion start, Quaternion end, float duration)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            camParent.localRotation = Quaternion.Lerp(start, end, _parentCurve.Evaluate(t));

            yield return null;
        }

        camParent.localRotation = end;
    }
}
