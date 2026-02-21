using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
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
    [SerializeField] private float transitionTime = 2f;

    public void OnPerspectiveChange(bool is3D)
    {
        if (is3D)
            StartCoroutine(MoveToPosition(_3DPosition, _cam3DPos, _3DFOV));
        else
            StartCoroutine(MoveToPosition(_2DPosition, _cam2DPos, _2DFOV));
    }

    private IEnumerator MoveToPosition(Transform parentTarget, Vector3 camTarget, float fovTarget)
    {
        Quaternion startRot = this.transform.localRotation;
        Quaternion endRot = parentTarget.localRotation;

        Vector3 camStart = cam.transform.localPosition;
        float fovStart = cam.fieldOfView;

        float t = 0f;
        while (t < 1f) 
        {
            t += Time.deltaTime / transitionTime;

            this.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);

            cam.transform.localPosition = Vector3.Lerp(camStart, camTarget, t);
            cam.fieldOfView = Mathf.Lerp(fovStart, fovTarget, t);

            yield return null;
        }

        this.transform.localRotation = endRot;
        cam.transform.localPosition = camTarget;
        cam.fieldOfView = fovTarget;
    }
}
