using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _height = 0.5f;
    [SerializeField] private float _heightOffset = 0.5f;

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = new Vector3(
            0f,
            (Mathf.Sin(Time.timeSinceLevelLoad * _speed) * _height) + _heightOffset,
            0f
        );
    }
}
