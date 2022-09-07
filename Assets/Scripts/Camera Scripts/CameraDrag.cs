using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 touchStart;

    public float zoomMin = 1;
    public float zoomMax = 100;
    public float zoomSpeed = 7;

    void Update()
    {
        if (UIController.Instance.gameIsPaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            var difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);
        }
        else if (Input.GetMouseButton(1))
        {
            var direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
