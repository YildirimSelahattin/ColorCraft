using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ColorWheelController colorWheelController;
    
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
            Vector2 deltaPosition = touch.deltaPosition * Time.deltaTime * panSpeed;

            float radius = colorWheelController.GetOuterRadius();
            Vector2 boundary = new Vector2(radius, radius) * 1.7f; // Çarpanı 1.7f olarak güncelledik.
            Vector3 newPosition = transform.position - new Vector3(deltaPosition.x, deltaPosition.y, 0f);

            newPosition.x = Mathf.Clamp(newPosition.x, -boundary.x, boundary.x);
            newPosition.y = Mathf.Clamp(newPosition.y, -boundary.y, boundary.y + 2f); // Yukarı doğru ekstra 2 birim ekledik.

            transform.position = newPosition;
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