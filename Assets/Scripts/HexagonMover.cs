using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HexagonMover : MonoBehaviour
{
    public float moveDuration = 0.25f;
    public ColorWheelController colorWheelController;

    private bool isMoving = false;
    
    private Queue<Vector3> moveCommands = new Queue<Vector3>();
    
    public Button enqueuePatternButton;
    public Button enqueueSecondPatternButton;
    public GameObject startPatternSprite;
    private bool isMouseDownOnStartPattern = false;

    private Queue<Vector3> moveQueue = new Queue<Vector3>();
    
    private void Start()
    {
        enqueuePatternButton.onClick.AddListener(EnqueueMoves);
        enqueueSecondPatternButton.onClick.AddListener(EnqueueSecondPatternMoves);
    }

    private void Update()
    {
        if (isMoving)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * 60 * Time.deltaTime);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null && hitCollider.gameObject == startPatternSprite.gameObject)
            {
                isMouseDownOnStartPattern = true;
                StartPatternMoves();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isMouseDownOnStartPattern)
            {
                isMouseDownOnStartPattern = false;
                StopPatternMoves();
            }
        }

        if (moveCommands.Count > 0)
        {
            Vector3 direction = moveCommands.Dequeue();
            StartCoroutine(MoveToNextHexagon(direction));
        }
        else
        {
            int horizontal = (int)Input.GetAxisRaw("Horizontal");
            int vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;
                StartCoroutine(MoveToNextHexagon(direction));
                PrintCurrentHexagonColor();
            }
        }
    }


    private IEnumerator MoveToNextHexagon(Vector3 direction)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + GetHexagonOffset(direction);

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;

        PrintCurrentHexagonColor(); // Hareket işlemi tamamlandıktan sonra rengi yazdır

        isMoving = false;
    }


    private Vector3 GetHexagonOffset(Vector3 direction)
    {
        Vector3 offset = Vector3.zero;

        if (direction == Vector3.up) // Up
        {
            offset = new Vector3(0f, 0.55f, 0f);
        }
        else if (direction == Vector3.down) // Down
        {
            offset = new Vector3(0f, -0.55f, 0f);
        }
        else if (direction.x > 0) // Right
        {
            if (direction.y > 0) // Right-Up
            {
                offset = new Vector3(0.475f, 0.275f, 0f);
            }
            else // Right-Down
            {
                offset = new Vector3(0.475f, -0.275f, 0f);
            }
        }
        else if (direction.x < 0) // Left
        {
            if (direction.y > 0) // Left-Up
            {
                offset = new Vector3(-0.475f, 0.275f, 0f);
            }
            else // Left-Down
            {
                offset = new Vector3(-0.475f, -0.275f, 0f);
            }
        }

        return offset;
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
    
    /////////////////////////Patterns/////////////////////////////////
    ///
    private IEnumerator ExecutePatternMoves(Vector3[] directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 direction = directions[i];
            StartCoroutine(MoveToNextHexagon(direction));
            yield return new WaitForSeconds(moveDuration);
        }
    }
    
    public void EnqueueMoves()
    {
        if (!isMoving)
        {
            Vector3[] patternDirections = {
                new Vector3(-1, 1, 0),  // Left-Up
                new Vector3(1, 1, 0),   // Right-Up
                new Vector3(1, -1, 0),  // Right-Down
                new Vector3(0, 1, 0),   // Up
                new Vector3(-1, 1, 0)   // Left-Up
            };

            EnqueueDirections(patternDirections);
        }
    }
    
    public void EnqueueSecondPatternMoves()
    {
        if (!isMoving)
        {
            Vector3[] secondPatternDirections = {
                new Vector3(1, -1, 0),  // Right-Down
                new Vector3(1, 1, 0),   // Right-Up
                new Vector3(0, -1, 0)   // Down
            };

            EnqueueDirections(secondPatternDirections);
        }
    }
    
    private void EnqueueDirections(Vector3[] directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            moveQueue.Enqueue(directions[i]);
        }
        PrintMoveQueue();
    }

    public void StartPatternMoves()
    {
        if (!isMoving && moveQueue.Count > 0)
        {
            StartCoroutine(ExecuteMoveQueue());
        }
    }

    public void StopPatternMoves()
    {
        StopCoroutine(ExecuteMoveQueue());
    }

    private IEnumerator ExecuteMoveQueue()
    {
        isMoving = true;

        while (moveQueue.Count > 0)
        {
            Vector3 direction = moveQueue.Dequeue();
            StartCoroutine(MoveToNextHexagon(direction));
            yield return new WaitForSeconds(moveDuration);
        }

        isMoving = false;
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
