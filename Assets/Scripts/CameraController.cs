using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;

    private Vector2 touchStart;
    private Camera cam;
    
    [SerializeField] private GameObject player;
    [SerializeField] private float initialZoom;
    private Vector3 initialPosition;
    private bool isDoubleTapped;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initialZoom = cam.orthographicSize;
        initialPosition = transform.position;
        isDoubleTapped = false;
    }

    private void Update()
    {
        // Double Click Controle
        if (Input.GetMouseButtonDown(0))
        {
            if (isDoubleTapped)
            {
                cam.orthographicSize = initialZoom;
                transform.position = initialPosition;
                isDoubleTapped = false;
            }
            else
            {
                isDoubleTapped = true;
                Invoke("ResetDoubleTap", 0.3f); // 0.3 saniye içinde 2. tıklama yapılmazsa çift tıklama sayılmayacak
            }
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 direction = touchStart - (Vector2)cam.ScreenToWorldPoint(touch.position);
                transform.position += (Vector3)direction * panSpeed;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMagnitude = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMagnitude - touchDeltaMagnitude;

            Zoom(deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
        }

        // Mouse scroll wheel zoom
        float scrollWheelZoom = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelZoom != 0)
        {
            Zoom(-scrollWheelZoom * zoomSpeed);
        }
    }

    private void Zoom(float deltaZoom)
    {
        cam.orthographicSize += deltaZoom;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
    
    private void ResetDoubleTap()
    {
        isDoubleTapped = false;
    }
}