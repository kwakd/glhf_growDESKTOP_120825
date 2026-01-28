using UnityEngine;

public class cameraControllerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Boundary Settings")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    
    [Header("Zoom Level 1 Boundary Settings (1.25x)")]
    public float zoom1MinX = -7.5f;
    public float zoom1MaxX = 7.5f;
    public float zoom1MinY = -7.5f;
    public float zoom1MaxY = 7.5f;
    
    [Header("Zoom Level 2 Boundary Settings (1.5x)")]
    public float zoom2MinX = -5f;
    public float zoom2MaxX = 5f;
    public float zoom2MinY = -5f;
    public float zoom2MaxY = 5f;
    
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float defaultZoom = 5f;      // Normal view
    public float zoom1Size = 4f;        // 1.25x zoomed in
    public float zoom2Size = 3.33f;     // 1.5x zoomed in
    
    private Camera cam;
    private float targetZoom;
    private int zoomLevel = 0; // 0 = normal, 1 = 1.25x, 2 = 1.5x

    void Start()
    {
        cam = GetComponent<Camera>();
        
        if (cam == null)
        {
            Debug.LogError("CameraController requires a Camera component!");
        }
        
        // Set default zoom
        if (cam.orthographic)
        {
            cam.orthographicSize = defaultZoom;
            targetZoom = defaultZoom;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        // Get input from arrow keys
        float horizontalInput = 0f;
        float verticalInput = 0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalInput = -1f;
        }
        
        // Calculate new position
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);
        Vector3 newPosition = transform.position + movement * moveSpeed * Time.deltaTime;
        
        // Use different boundaries based on zoom level
        float currentMinX = minX;
        float currentMaxX = maxX;
        float currentMinY = minY;
        float currentMaxY = maxY;
        
        if (zoomLevel == 1)
        {
            currentMinX = zoom1MinX;
            currentMaxX = zoom1MaxX;
            currentMinY = zoom1MinY;
            currentMaxY = zoom1MaxY;
        }
        else if (zoomLevel == 2)
        {
            currentMinX = zoom2MinX;
            currentMaxX = zoom2MaxX;
            currentMinY = zoom2MinY;
            currentMaxY = zoom2MaxY;
        }
        
        // Clamp position within boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, currentMinX, currentMaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, currentMinY, currentMaxY);
        
        // Apply new position
        transform.position = newPosition;
    }

    void HandleZoom()
    {
        if (cam == null || !cam.orthographic) return;
        
        // Cycle through zoom levels when Z is pressed
        if (Input.GetKeyDown(KeyCode.Z))
        {
            zoomLevel++;
            
            // Cycle back to normal after max zoom
            if (zoomLevel > 2)
            {
                zoomLevel = 0;
            }
            
            // Set target zoom based on level
            switch (zoomLevel)
            {
                case 0:
                    targetZoom = defaultZoom; // Normal
                    Debug.Log("Zoom: Normal");
                    break;
                case 1:
                    targetZoom = zoom1Size; // 1.25x
                    Debug.Log("Zoom: 1.25x");
                    break;
                case 2:
                    targetZoom = zoom2Size; // 1.5x
                    Debug.Log("Zoom: 1.5x");
                    break;
            }
        }
        
        // Smoothly interpolate to target zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }
    
    // Optional: Method to reset camera to center
    public void ResetCamera()
    {
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        transform.position = new Vector3(centerX, centerY, transform.position.z);
        
        zoomLevel = 0;
        targetZoom = defaultZoom;
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = defaultZoom;
        }
    }
}