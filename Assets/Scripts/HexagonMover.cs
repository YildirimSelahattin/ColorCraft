using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HexagonMover : MonoBehaviour
{
    bool isPathDrawing = false;
    [SerializeField] private GameObject startPatternSprite;
    public GameObject[] movingSprites;
    public bool isMoving = false;
    public bool isMouseDownOnStartPattern = false;
    public Queue<Vector3> moveQueue = new Queue<Vector3>();
    public LineRenderer lineRenderer;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private Button[] displayPatternButtons = new Button[3];
    public List<int> patternIndices = new List<int>();
    public List<int> patternLengths = new List<int>();
    private int currentPatternMoveCount = 0;
    public static HexagonMover Instance;
    
    private bool isButtonPressed = false;
    public Button patternButton;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void StartFunctions()
    {
        for (int colorCounter = 0; colorCounter < ColorManager.Instance.sideButtonArray.Length; colorCounter++)
        {
            if (ColorManager.Instance.sideButtonArray[colorCounter] != null)
            {
                switch (ColorManager.Instance.sideButtonArray[colorCounter].GetComponent<ColorButtonData>().colorName)
                {
                    case "red":
                        ColorManager.Instance.sideButtonArray[colorCounter].GetComponent<Button>().onClick.AddListener(EnqueueRedMoves);
                        break;
                    case "green":
                        ColorManager.Instance.sideButtonArray[colorCounter].GetComponent<Button>().onClick.AddListener(EnqueueGreenMoves);
                        break;
                    case "blue":
                        ColorManager.Instance.sideButtonArray[colorCounter].GetComponent<Button>().onClick.AddListener(EnqueueBlueMoves);
                        break;
                }
            }
        }
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
    private void Update()
    {
        if (isPathDrawing && Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosition);
        }
        else if (isPathDrawing && Input.GetMouseButtonUp(0))
        {
            isPathDrawing = false;
            lineRenderer.positionCount = 0;
        }
        
        if (isMouseDownOnStartPattern)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        if (moveQueue.Count == 0)
        {
            isMouseDownOnStartPattern = false;
            isMoving = false;
        }
    
        if (EventSystem.current.currentSelectedGameObject == patternButton.gameObject && Input.GetMouseButton(0))
        {
            if (!isButtonPressed)
            {
                isButtonPressed = true;
                OnStartPatternButtonPress();
            }
        }
        else if (isButtonPressed && Input.GetMouseButtonUp(0))
        {
            isButtonPressed = false;
        }
    }
    public void OnStartPatternButtonPress()
    {
        isMouseDownOnStartPattern = !isMouseDownOnStartPattern;
        
        if (isMouseDownOnStartPattern == true)
        {
            isMoving = true;
            MoveToNextHexagon();
        }
        else
        {
            if(patternIndices.Count > 0)
            patternIndices.RemoveAt(0);
            //patternLengths.RemoveAt(0);
            UpdatePatternDisplayButtons();
            //UpdateLineRendererPreview();
            if (patternLengths.Count > 0)
            {
                currentPatternMoveCount = patternLengths[0];
            }
            isMoving = false;
        }
    }
    private void UpdateLineRendererPreview()
    {
        Vector3 currentPosition = transform.position;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);
        int remainingPatternMoveCount = currentPatternMoveCount;
        int currentPatternIndex = 0;
        foreach (Vector3 direction in moveQueue)
        {
            if (remainingPatternMoveCount <= 0)
            {
                if (patternLengths.Count > currentPatternIndex + 1)
                {
                    currentPatternIndex++;
                    remainingPatternMoveCount = patternLengths[currentPatternIndex];
                }
                else
                {
                    break;
                }
            }
            Vector3 hexagonOffset = GetHexagonOffset(direction);
            currentPosition += hexagonOffset;
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.zero);
            if (hit.collider != null)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);
            }
            else
            {
                break;
            }
            remainingPatternMoveCount--;
        }
    }
    private void MoveToNextHexagon()
    {
        if(moveQueue.Count < 1)
        {
            return;
        }
        Vector3 direction = moveQueue.Dequeue();
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + GetHexagonOffset(direction);
        transform.DOMove(targetPosition, 0.5f).OnComplete(() =>
        {
            PrintCurrentHexagonColor();
            currentPatternMoveCount--;
            if (currentPatternMoveCount <= 0 && patternIndices.Count > 0)
            {
                patternIndices.RemoveAt(0);
                patternLengths.RemoveAt(0);
                UpdatePatternDisplayButtons();
                if (patternLengths.Count > 0)
                {
                    currentPatternMoveCount = patternLengths[0];
                }
            }
            if (isMouseDownOnStartPattern == true && moveQueue.Count > 0)
            {
                MoveToNextHexagon();
            }
            else
            {
                isMoving = false;
            }
        }).SetEase(Ease.Linear);
    }
    private void EnqueueDirections(Vector3[] directions, int patternIndex, Button pressedButton)
    {
        
        if (patternIndices.Count < 3)
        {
            patternIndices.Add(patternIndex);
            patternLengths.Add(directions.Length);
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
        if (index >= patternIndices.Count || isMoving)
        {
            return;
        }
        int patternIndexToRemove = patternIndices[index];
        Vector3[] patternToRemove = null;
        switch (patternIndexToRemove)
        {
            case 0:
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
        displayPatternButtons[index].image.color = Color.white;
        GameObject temp = Instantiate(movingSprites[patternIndices[index]], displayPatternButtons[index].transform);
        Vector3 aimedPos = ColorManager.Instance.sideButtonArray[patternIndices[index]].transform.position;
        
        temp.transform.localRotation = new Quaternion(0, 0, 90, 0);
        temp.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);

        temp.transform.DOMove(aimedPos, 0.5f).OnComplete(() =>
        {
            ColorManager.Instance.sideButtonArray[patternIndices[index]].GetComponent<ColorButtonData>().colorAmount++;
            ColorManager.Instance.sideButtonArray[patternIndices[index]].GetComponent<ColorButtonData>().button.interactable = true;
            ColorManager.Instance.sideButtonArray[patternIndices[index]].GetComponent<ColorButtonData>().numberText.text = ColorManager.Instance.sideButtonArray[patternIndices[index]].GetComponent<ColorButtonData>().colorAmount.ToString();
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
                    displayPatternButtons[index - 1].image.color = new Color(256/256f, 131/256f,131/256f);
                    break;
                case 1:
                    displayPatternButtons[index - 1].image.color = new Color(110/256f, 218/256f,140/256f); 
                    break;
                case 2:
                    displayPatternButtons[index - 1].image.color = new Color(131/256f, 131/256f,256/256f);
                    break;
            }
        });
    }
    private Vector3 GetHexagonOffset(Vector3 direction)
    {
        Vector3 potentialOffset = Vector3.zero;

        if (direction == Vector3.up) potentialOffset = new Vector3(0f, 0.55f, 0f);
        if (direction == Vector3.down) potentialOffset = new Vector3(0f, -0.55f, 0f);

        float x = direction.x > 0 ? 0.475f : -0.475f;
        float y = direction.y > 0 ? 0.275f : -0.275f;
        if (direction != Vector3.up && direction != Vector3.down) potentialOffset = new Vector3(x, y, 0f);

        Vector3 potentialNewPosition = transform.position + potentialOffset;
        RaycastHit2D hit = Physics2D.Raycast(potentialNewPosition, Vector2.zero);
        if (hit.collider != null)
        {
            Hexagon hexagon = hit.collider.GetComponent<Hexagon>();
            if (hexagon != null)
            {
                return potentialOffset;
            }
        }

        return Vector3.zero;
    }
    private void PrintCurrentHexagonColor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            Hexagon hexagon = hit.collider.GetComponent<Hexagon>();
            if (hexagon != null)
            {
                CaldronManager.Instance.water.DOColor(hexagon.color,0.5f);
                CaldronManager.Instance.spiral.DOColor(hexagon.color,0.5f);

                Debug.Log("Current hexagon color: " + hexagon.color);

                //EndGame
                if (IsOnRightSpot() == true)
                {
                    
                    UIManager.Instance.winScreen.SetActive(true);
                    ColorWheelController.Instance.isFinished = true;
                    return;
                }
            }
        }
    }
    private bool IsOnRightSpot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            Hexagon hexagon = hit.collider.GetComponent<Hexagon>();
            if (hexagon != null)
            {
                if (GameDataManager.Instance.rawData.levelsArray[GameDataManager.Instance.currentLevel].winIndex == hexagon.index)
                return true;
            }
        }
        return false;
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
                    displayPatternButtons[i].image.color = new Color(256/256f, 131/256f,131/256f);
                    break;
                case 1:
                    displayPatternButtons[i].image.color = new Color(110 / 256f, 218 / 256f, 140 / 256f);
                    break;
                case 2:
                    displayPatternButtons[i].image.color = new Color(131/256f, 131/256f,256/256f);
                    break;
            }
        }
    }
    public void EnqueueRedMoves()
    {
        if (isMoving || ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().colorAmount < 1 || patternIndices.Count >= 3)
        {
            return;
        }
        else
        {
            if (ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().colorAmount == 1)
            {
                ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().button.interactable = false;
            }
            Vector3[] patternDirections =
            {
            new(-1, 1, 0), // Left-Up
            new(1, 1, 0), // Right-Up
            new(1, -1, 0), // Right-Down
            new(0, 1, 0), // Up
            new(-1, 1, 0) // Left-Up
        };
            ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().colorAmount--;
            ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().numberText.text = ColorManager.Instance.sideButtonArray[0].GetComponent<ColorButtonData>().colorAmount.ToString();
            EnqueueDirections(patternDirections, 0, ColorManager.Instance.sideButtonArray[0].GetComponent<Button>());
        }
    }
    public void EnqueueGreenMoves()
    {
        if (isMoving || ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().colorAmount < 0 || patternIndices.Count >= 3)
        {
            return;
        }
        else
        {
            if (ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().colorAmount == 1)
            {
                ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().button.interactable = false;
            }
            Vector3[] patternDirections =
            {
            new(1, -1, 0), // Right-Down
            new(1, 1, 0), // Right-Up
            new(0, -1, 0) // Down
        };
            ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().colorAmount--;
            ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().numberText.text = ColorManager.Instance.sideButtonArray[1].GetComponent<ColorButtonData>().colorAmount.ToString();
            EnqueueDirections(patternDirections, 1, ColorManager.Instance.sideButtonArray[1].GetComponent<Button>());
        }
    }
    public void EnqueueBlueMoves()
    {
        if (isMoving || ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().colorAmount < 0 || patternIndices.Count >= 3)
        {
            return;
        }
        else
        {
            if (ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().colorAmount == 1)
            {
                ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().button.interactable = false;
            }
            Vector3[] patternDirections =
            {
            new(0, -1, 0), // Down
            new(0, -1, 0), // Down
            new(-1, 1, 0), // Left-Up
            new(-1, 1, 0) // Left-Up
        };
            ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().colorAmount--;
            ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().numberText.text = ColorManager.Instance.sideButtonArray[2].GetComponent<ColorButtonData>().colorAmount.ToString();
            EnqueueDirections(patternDirections, 2, ColorManager.Instance.sideButtonArray[2].GetComponent<Button>());
        }
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