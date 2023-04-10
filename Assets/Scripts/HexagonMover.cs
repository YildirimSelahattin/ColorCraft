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
        float q = Mathf.RoundToInt((2 * direction.x - direction.y) / 3);
        float r = Mathf.RoundToInt((2 * direction.y - direction.x) / 3);

        float x = colorWheelController.hexagonSize * (1.5f * q);
        float y = colorWheelController.hexagonSize * (Mathf.Sqrt(3) * (r + q / 2));

        Vector3 offset = new Vector3(x, y, 0);

        // Adjust the offset to align with the hexagon center
        if (Mathf.Abs(q) % 2 != 0 && Mathf.Abs(r) % 2 != 0)
        {
            offset += (direction.y > 0 ? -1 : 1) * colorWheelController.hexagonSize * new Vector3(0.5f, Mathf.Sqrt(3) / 2, 0);
        }

        return offset;
    }
}
