using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using Unity.VisualScripting;

public class HexagonMover : MonoBehaviour
{
    [SerializeField] private float moveDuration = 20f;
    [SerializeField] private Button enqueueRedButton;
    [SerializeField] private Button enqueueGreenPatternButton;
    [SerializeField] private Button enqueueBluePatternButton;
    [SerializeField] private GameObject startPatternSprite;
    public GameObject[] movingSprites;

    private bool isMoving = false;
    private bool isMouseDownOnStartPattern = false;
    private bool isProcessingMoveQueue = false;

    public Queue<Vector3> moveQueue = new Queue<Vector3>();
    private int moveQueueIndex = 0;

    [SerializeField] private LineRenderer lineRenderer;

    private bool shouldMove = false;

    private float elapsedTime;
    private float rotationElapsedTime;

    private float movementTimer = 0f; // hareket süresini kontrol etmek için yeni bir değişken

    [SerializeField] private float rotationSpeed = 60f;

    private float movementProgress = 0f;

    [SerializeField] private Button[] displayPatternButtons = new Button[3];
    public List<int> patternIndices = new List<int>();
    public List<int> patternLengths = new List<int>();
    private bool isCurrentPatternFinished = false;
    private int currentPatternMoveCount = 0;

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

        for (int i = 0; i < displayPatternButtons.Length; i++)
        {
            int index = i;
            displayPatternButtons[i].onClick.AddListener(() => RemovePatternAtIndex(index));
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
                currentPatternMoveCount--;

                if (currentPatternMoveCount <= 0 && patternIndices.Count > 0)
                {
                    patternIndices.RemoveAt(0);
                    patternLengths.RemoveAt(0); // Remove the length of the finished pattern from patternLengths
                    UpdatePatternDisplayButtons();

                    if (patternLengths.Count > 0)
                    {
                        currentPatternMoveCount = patternLengths[0]; // Update the currentPatternMoveCount
                    }
                }
            }
        }
    }


    private void Update()
    {
        if (isMouseDownOnStartPattern)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (moveQueue.Count == 0)
        {
            isMouseDownOnStartPattern = false;
        }
    }

    public void OnStartPatternButtonPress()
    {
        isMouseDownOnStartPattern = !isMouseDownOnStartPattern;
    }

    private void UpdateLineRendererPreview()
    {
        Vector3 currentPosition = transform.position;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        foreach (Vector3 direction in moveQueue)
        {
            Vector3 hexagonOffset = GetHexagonOffset(direction);
            currentPosition += hexagonOffset;
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);
        }
    }


    private void MoveToNextHexagon(Vector3 direction)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + GetHexagonOffset(direction);

        float progress = movementTimer / moveDuration;
        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
        transform.position = Vector3.Lerp(startPosition, targetPosition, smoothProgress);

        /*
        // LineRenderer'a yeni pozisyonu ekle
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
        */

        if (progress >= 1f)
        {
            PrintCurrentHexagonColor();
        }
    }

    private void EnqueueDirections(Vector3[] directions, int patternIndex, Button pressedButton)
    {
        if (patternIndices.Count >= 3)
        {
            Debug.Log("Pattern limit reached.");
            return;
        }

        if (patternIndices.Count < 3)
        {
            patternIndices.Add(patternIndex);
            patternLengths.Add(directions.Length); // Add the length of the pattern to patternLengths
            MoveToEmptySpot(movingSprites[patternIndex], displayPatternButtons[patternIndices.Count - 1], directions,
                pressedButton);

            foreach (Vector3 direction in directions)
            {
                moveQueue.Enqueue(direction);
            }

            if (moveQueue.Count > 0 && patternIndices.Count == 1)
            {
                currentPatternMoveCount = directions.Length;
            }
            else if (moveQueue.Count == 0)
            {
                currentPatternMoveCount = 0;
            }

            UpdateLineRendererPreview();
            PrintMoveQueue();
        }
    }


    private void RemovePatternAtIndex(int index)
    {
        if (index >= patternIndices.Count)
        {
            Debug.LogWarning("Invalid pattern index.");
            return;
        }

        int patternIndexToRemove = patternIndices[index];
        Vector3[] patternToRemove = null;

        switch (patternIndexToRemove)
        {
            case 0: // Red pattern
                patternToRemove = new Vector3[]
                {
                    new Vector3(-1, 1, 0), // Left-Up
                    new Vector3(1, 1, 0), // Right-Up
                    new Vector3(1, -1, 0), // Right-Down
                    new Vector3(0, 1, 0), // Up
                    new Vector3(-1, 1, 0) // Left-Up
                };

                break;
            case 1: // Green pattern
                patternToRemove = new Vector3[]
                {
                    new Vector3(1, -1, 0), // Right-Down
                    new Vector3(1, 1, 0), // Right-Up
                    new Vector3(0, -1, 0) // Down
                };
                break;
            case 2: // Blue pattern
                patternToRemove = new Vector3[]
                {
                    new Vector3(0, -1, 0), // Down
                    new Vector3(0, -1, 0), // Down
                    new Vector3(-1, 1, 0), // Left-Up
                    new Vector3(-1, 1, 0) // Left-Upk
                };
                break;
        }

        if (patternToRemove != null)
        {
            for (int i = 0; i < patternToRemove.Length; i++)
            {
                if (moveQueue.Count > 0)
                {
                    moveQueue.Dequeue();
                }
            }
        }

        //shift colors in deck
        for (int i = 0; i < patternIndices.Count; i++)
        {
            if (i > index)
            {
                displayPatternButtons[i].image.color = Color.white;
                int patternIndex = patternIndices[i];
                GameObject temp1 = Instantiate(movingSprites[patternIndices[i]], displayPatternButtons[i].transform);
                MoveAndColorWhenReached(temp1, i);
            }
        }

        //move removed one to 
        displayPatternButtons[index].image.color = Color.white;
        GameObject temp = Instantiate(movingSprites[patternIndices[index]], displayPatternButtons[index].transform);
        Vector3 aimedPos = new Vector3();
        if (patternIndices[index] == 0)
        {
            aimedPos = enqueueRedButton.transform.position;
        }

        if (patternIndices[index] == 1)
        {
            aimedPos = enqueueGreenPatternButton.transform.position;
        }

        if (patternIndices[index] == 2)
        {
            aimedPos = enqueueBluePatternButton.transform.position;
        }

        temp.transform.localRotation = new Quaternion(0, 0, 90, 0);
        temp.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);

        temp.transform.DOMove(aimedPos, 0.5f).OnComplete(() =>
        {
            Destroy(temp.gameObject);
            patternIndices.RemoveAt(index);
            patternLengths.RemoveAt(index);
            PrintMoveQueue();
            UpdateLineRendererPreview();
        });
    }

    public void MoveAndColorWhenReached(GameObject objectToMove, int index)
    {
        objectToMove.transform.DOMove(displayPatternButtons[index - 1].transform.position, 0.5f).OnComplete(() =>
        {
            Destroy(objectToMove.gameObject);
            switch (patternIndices[index])
            {
                case 0:
                    displayPatternButtons[index - 1].image.color = Color.red;
                    break;
                case 1:
                    displayPatternButtons[index - 1].image.color = Color.green;
                    break;
                case 2:
                    displayPatternButtons[index - 1].image.color = Color.blue;
                    break;
            }
        });
    }

    private void UpdatePatternDisplayButtonRemove()
    {
        // Set all button colors to white
        for (int i = 0; i < displayPatternButtons.Length; i++)
        {
            displayPatternButtons[i].image.color = Color.white;
        }

        for (int i = 0; i < patternIndices.Count; i++)
        {
            int patternIndex = patternIndices[i];
            switch (patternIndex)
            {
                case 0:
                    displayPatternButtons[i].image.color = Color.red;
                    break;
                case 1:
                    displayPatternButtons[i].image.color = Color.green;
                    break;
                case 2:
                    displayPatternButtons[i].image.color = Color.blue;
                    break;
            }
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
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


    public void MoveToEmptySpot(GameObject movingColor, Button targetButton, Vector3[] directions, Button pressedButton)
    {
        GameObject temp = Instantiate(movingColor, pressedButton.gameObject.transform);
        temp.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f);
        temp.transform.DOMove(targetButton.transform.position, 0.5f).OnComplete(() =>
        {
            Destroy(temp.gameObject);

            UpdatePatternDisplayButtons();
        });
    }

    private void UpdatePatternDisplayButtons()
    {
        // Set all button colors to white
        for (int i = 0; i < displayPatternButtons.Length; i++)
        {
            displayPatternButtons[i].image.color = Color.white;
        }

        for (int i = 0; i < patternIndices.Count; i++)
        {
            int patternIndex = patternIndices[i];
            switch (patternIndex)
            {
                case 0:
                    displayPatternButtons[i].image.color = Color.red;
                    break;
                case 1:
                    displayPatternButtons[i].image.color = Color.green;
                    break;
                case 2:
                    displayPatternButtons[i].image.color = Color.blue;
                    break;
            }
        }
    }


    public void EnqueueRedMoves()
    {
        if (isMoving) return;

        Vector3[] patternDirections =
        {
            new(-1, 1, 0), // Left-Up
            new(1, 1, 0), // Right-Up
            new(1, -1, 0), // Right-Down
            new(0, 1, 0), // Up
            new(-1, 1, 0) // Left-Up
        };

        EnqueueDirections(patternDirections, 0, enqueueRedButton);
    }


    public void EnqueueGreenMoves()
    {
        if (isMoving) return;

        Vector3[] secondPatternDirections =
        {
            new(1, -1, 0), // Right-Down
            new(1, 1, 0), // Right-Up
            new(0, -1, 0) // Down
        };

        EnqueueDirections(secondPatternDirections, 1, enqueueGreenPatternButton);
    }

    public void EnqueueBlueMoves()
    {
        if (isMoving) return;

        Vector3[] thirdPatternDirections =
        {
            new(0, -1, 0), // Down
            new(0, -1, 0), // Down
            new(-1, 1, 0), // Left-Up
            new(-1, 1, 0) // Left-Up
        };

        EnqueueDirections(thirdPatternDirections, 2, enqueueBluePatternButton);
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