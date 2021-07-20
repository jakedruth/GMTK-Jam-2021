using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera _camera;
    public PostProcess postProcess { get; private set; }
    private Vector2Int _resolution;

    private Vector3 _startPos;
    private float _startZoom;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        postProcess = GetComponent<PostProcess>();
        postProcess.material.SetFloat("_FillAmount", 1.1f);

        _startPos = transform.position;
        _startZoom = _camera.orthographicSize;

        Debug.Log($"Width: {Screen.width}\t Height: {Screen.height}");
    }


    void Update()
    {
        _resolution.x = Screen.width;
        _resolution.y = Screen.height;

        if (_resolution.x != (int)postProcess.material.GetFloat("_ScreenWidth") || 
            _resolution.y != (int)postProcess.material.GetFloat("_ScreenHeight"))
        {
            postProcess.material.SetFloat("_ScreenWidth", _resolution.x);
            postProcess.material.SetFloat("_ScreenHeight", _resolution.y);

            
        }
    }

    public void MoveTo(Vector3 point, float time, EasingFunction easing)
    {
        point.z = transform.position.z;
        StartCoroutine(MoveToPoint(point, time, easing));
    }

    public void SetMove(Vector3 point)
    {
        transform.position = point;
    }

    private IEnumerator MoveToPoint(Vector3 point, float time, EasingFunction easing)
    {
        Vector3 startPoint = transform.position;
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;

            float t = easing.Invoke(timer / time);

            transform.position = Vector3.Lerp(startPoint, point, t);

            yield return null;
        }
    }

    public void ZoomTo(float zoomLevel, float time, EasingFunction easing)
    {
        StartCoroutine(ZoomToLevel(zoomLevel, time, easing));
    }

    public void SetZoom(float zoomLevel)
    {
        _camera.orthographicSize = zoomLevel;
    }

    private IEnumerator ZoomToLevel(float zoomLevel, float time, EasingFunction easing)
    {
        float startZoom = _camera.orthographicSize;
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;

            float t = easing.Invoke(timer / time);

            _camera.orthographicSize = Mathf.Lerp(startZoom, zoomLevel, t);

            yield return null;
        }
    }

    public void FillScreenTo(float value, float time, EasingFunction easing)
    {
        StartCoroutine(FillScreenToValue(value, time, easing));
    }

    public void SetFillScreen(float value)
    {
        postProcess.material.SetFloat("_FillAmount", value);
    }

    private IEnumerator FillScreenToValue(float value, float time, EasingFunction easing)
    {
        float startValue = postProcess.material.GetFloat("_FillAmount");
        float timer = 0;
        while (timer <= time)
        {
            timer += Time.deltaTime;

            float t = easing.Invoke(timer / time);

            postProcess.material.SetFloat("_FillAmount",  Mathf.Lerp(startValue, value, t));

            yield return null;
        }
    }

    public void Reset()
    {
        transform.position = _startPos;
        _camera.orthographicSize = _startZoom;
    }
}
