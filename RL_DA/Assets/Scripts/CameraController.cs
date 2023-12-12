using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float zoom;
    private float zoomMultiplyer = 4f;
    private float minZoom = 2f;
    private float maxZoom = 12f;
    private float vel = 0f;
    private float smoothTime = .25f;

    private void Start()
    {
        zoom = Camera.main.orthographicSize;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplyer;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, zoom, ref vel, smoothTime);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }

}
