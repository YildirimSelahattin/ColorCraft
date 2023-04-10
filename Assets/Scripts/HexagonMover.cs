using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexagonMover : MonoBehaviour
{
    public float moveDuration = 0.25f;
    public ColorWheelController colorWheelController;

    private bool isMoving = false;
    
    private Queue<Vector3> moveCommands = new Queue<Vector3>();
    public Button redPatternMoveButton;
    public Button greenPatternMoveButton;
    public Button bluePatternMoveButton;
    
    private void Start()
    {
        redPatternMoveButton.onClick.AddListener(EnqueueRedMoves);
        greenPatternMoveButton.onClick.AddListener(EnqueueGreenPatternMoves); 
        bluePatternMoveButton.onClick.AddListener(EnqueueBluePatternMoves); 
    }
    
    private void Update()
    {
        if (isMoving)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * 60 * Time.deltaTime);
            return;
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
    
    public void EnqueueRedMoves()
    {
        if (!isMoving)
        {
            Vector3[] firstPatternDirections = {
                new Vector3(-1, 1, 0), // Left-Up
                new Vector3(1, 1, 0),  // Right-Up
                new Vector3(1, -1, 0),  // Right-Down
                new Vector3(0, 1, 0),  // Up
                new Vector3(-1, 1, 0)  // Left-Up
            };

            StartCoroutine(ExecutePatternMoves(firstPatternDirections));
        }
    }
    
    public void EnqueueGreenPatternMoves()
    {
        if (!isMoving)
        {
            Vector3[] secondPatternDirections = {
                new Vector3(1, -1, 0), // Right-Down
                new Vector3(1, 1, 0),  // Right-Up
                new Vector3(0, -1, 0)  // Down
            };

            StartCoroutine(ExecutePatternMoves(secondPatternDirections));
        }
    }
    
    public void EnqueueBluePatternMoves()
    {
        if (!isMoving)
        {
            Vector3[] thirdPatternDirections = {
                new Vector3(0, -1, 0), // Down
                new Vector3(0, -1, 0),  // Down
                new Vector3(-1, 1, 0),  // Left-Up
                new Vector3(-1, 1, 0)  // Left-Up
            };

            StartCoroutine(ExecutePatternMoves(thirdPatternDirections));
        }
    }
}
