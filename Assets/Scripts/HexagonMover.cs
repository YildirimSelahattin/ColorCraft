using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HexagonMover : MonoBehaviour
{
    [SerializeField] private float moveDuration = 20f;
    [SerializeField] private Button enqueueRedButton;
    [SerializeField] private Button enqueueGreenPatternButton;
    [SerializeField] private Button enqueueBluePatternButton;
    [SerializeField] private GameObject startPatternSprite;
    
    private bool isMoving = false;
    private bool isMouseDownOnStartPattern = false;
    private bool isProcessingMoveQueue = false;
    
    private Queue<Vector3> moveQueue = new Queue<Vector3>();
    private int moveQueueIndex = 0;
    
    [SerializeField] private LineRenderer lineRenderer;

    private bool shouldMove = false;

    private float elapsedTime;
    private float rotationElapsedTime;

    private float movementTimer = 0f; // hareket süresini kontrol etmek için yeni bir değişken

    [SerializeField] private float rotationSpeed = 60f;

    private float movementProgress = 0f;

    private void Update()
    {
        if (isMouseDownOnStartPattern)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }
    
    private void Start()
    {
        enqueueRedButton.onClick.AddListener(EnqueueRedMoves);
        enqueueGreenPatternButton.onClick.AddListener(EnqueueGreenMoves);
        enqueueBluePatternButton.onClick.AddListener(EnqueueBlueMoves);
        
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (isMouseDownOnStartPattern && moveQueue.Count > 0)
        {
            movementTimer += Time.fixedDeltaTime;

            if (movementTimer >= moveDuration)
            {
                Vector3 direction = moveQueue.Dequeue();
                MoveToNextHexagon(direction);
                movementTimer = 0f;
            }
        }
    }
    
    public void OnStartPatternButtonPress()
    {
            isMouseDownOnStartPattern = !isMouseDownOnStartPattern;
    }

    private void MoveToNextHexagon(Vector3 direction)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + GetHexagonOffset(direction);

        float progress = movementTimer / moveDuration;
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
        transform.position = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

        // LineRenderer'a yeni pozisyonu ekle
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);

        if (progress >= 1f)
        {
            PrintCurrentHexagonColor();
        }
    }
    
    private Vector3 GetHexagonOffset(Vector3 direction)
    {
        if (direction == Vector3.up) return new Vector3(0f, 0.55f, 0f);
        if (direction == Vector3.down) return new Vector3(0f, -0.55f, 0f);

        float x = direction.x > 0 ? 0.475f : -0.475f;
        float y = direction.y > 0 ? 0.275f : -0.275f;
        return new Vector3(x, y, 0f);
    }
    
    private void PrintCurrentHexagonColor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,        Vector2.zero);
        if (hit.collider != null)
        {
            Hexagon hexagon = hit.collider.GetComponent<Hexagon>();
            if (hexagon != null)
            {
                CaldronManager.Instance.water.color = hexagon.color;
                Debug.Log("Current hexagon color: " + hexagon.color);
            }
        }
    }
    
    private void EnqueueDirections(Vector3[] directions)
    {
        foreach (Vector3 direction in directions)
        {
            moveQueue.Enqueue(direction);
        }
        PrintMoveQueue();
    }

    public void EnqueueRedMoves()
    {
        if (isMoving) return;

        Vector3[] patternDirections = {
            new Vector3(-1, 1, 0),  // Left-Up
            new Vector3(1, 1, 0),   // Right-Up
            new Vector3(1, -1, 0),  // Right-Down
            new Vector3(0, 1, 0),   // Up
            new Vector3(-1, 1, 0)   // Left-Up
        };

        EnqueueDirections(patternDirections);
    }
    
    public void EnqueueGreenMoves()
    {
        if (isMoving) return;

        Vector3[] secondPatternDirections = {
            new Vector3(1, -1, 0),  // Right-Down
            new Vector3(1, 1, 0),   // Right-Up
            new Vector3(0, -1, 0)   // Down
        };

        EnqueueDirections(secondPatternDirections);
    }
    
    public void EnqueueBlueMoves()
    {
        if (isMoving) return;

        Vector3[] secondPatternDirections = {
            new Vector3(0, -1, 0),   // Down
            new Vector3(0, -1, 0),   // Down
            new Vector3(-1, 1, 0),   // Left-Up
            new Vector3(-1, 1, 0)   // Left-Up
        };

        EnqueueDirections(secondPatternDirections);
    }
    
    

    private void PrintMoveQueue()
    {
        Debug.Log("Move Queue:");
        foreach (Vector3 direction in moveQueue)
        {
            Debug.Log(direction);
        }
    }
}
