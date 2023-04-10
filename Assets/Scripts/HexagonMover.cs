using System.Collections;
using UnityEngine;

public class HexagonMover : MonoBehaviour
{
    public float moveDuration = 0.25f;
    public ColorWheelController colorWheelController;

    private bool isMoving = false;

    private void Update()
    {
        if (isMoving)
        {
            CaldronManager.Instance.spiral.transform.Rotate(Vector3.forward * 60 * Time.deltaTime);
            return;
        }

        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;
            StartCoroutine(MoveToNextHexagon(direction));
            PrintCurrentHexagonColor();
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
        
        isMoving = false;
    }

    private Vector3 GetHexagonOffset(Vector3 direction)
    {
        Vector3 offset = Vector3.zero;

        if (direction.y > 0) // Up
        {
            offset = new Vector3(0f, 0.55f, 0f);
        }
        else if (direction.y < 0) // Down
        {
            offset = new Vector3(0f, -0.55f, 0f);
        }
        else if (direction.x > 0) // Right
        {
            if (direction.y > -0.5f) // Right-Up
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
            if (direction.y > -0.5f) // Left-Up
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
                Debug.Log("Current hexagon color: " + hexagon.color);
            }
        }
    }
}
