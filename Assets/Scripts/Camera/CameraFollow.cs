using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector2 followOffset = new Vector2(0, 0);
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float cameraOrthoSize = 4.5f;

    [Header("Camera Bounds")]
    [SerializeField] float minXPos = 0f, maxXPos = 0f, minYPos = 0f, maxYPos = 0f;

    // Cached Components
    GameObject followObject = null;
    Vector2 threshold = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        threshold = CalculateThreshold();
        if(followObject) transform.position = followObject.transform.position;
    }

    public void SetFollowObject(GameObject newfollowObject) => followObject = newfollowObject;
    public void SetMoveSpeed(float newSpeed) => moveSpeed = newSpeed;

    void FixedUpdate()
    {
        if (!followObject) return;

        Vector2 followPos = followObject.transform.position;
        float xDiff = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * followPos.x);
        float yDiff = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * followPos.y);

        Vector3 newCameraPosition = transform.position;

        if (Mathf.Abs(xDiff) >= threshold.x) newCameraPosition.x = followPos.x;
        if (Mathf.Abs(yDiff) >= threshold.y) newCameraPosition.y = followPos.y;

        // Apply Camera Bounds
        newCameraPosition = new Vector3(
            Mathf.Clamp(newCameraPosition.x, minXPos, maxXPos),
            Mathf.Clamp(newCameraPosition.y, minYPos, maxYPos),
            -10
        );

        transform.position = Vector3.MoveTowards(transform.position, newCameraPosition, moveSpeed * Time.deltaTime);
    }

    public IEnumerator LerpFromTo(Vector2 pos1, Vector2 pos2, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        transform.position = pos2;
    }

    private Vector3 CalculateThreshold()
    {
        Rect cameraAspectRatio = Camera.main.pixelRect;
        Vector2 dimensions = new Vector2(cameraOrthoSize * cameraAspectRatio.width / cameraAspectRatio.height, cameraOrthoSize);
        dimensions.x -= followOffset.x;
        dimensions.y -= followOffset.y;
        return dimensions;
    }

    // See Threshold in Editor Mode
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 border = CalculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
        Gizmos.DrawLine(new Vector3(minXPos, minYPos, 0), new Vector3(minXPos, maxYPos, 0));
        Gizmos.DrawLine(new Vector3(maxXPos, minYPos, 0), new Vector3(maxXPos, maxYPos, 0));
        Gizmos.DrawLine(new Vector3(maxXPos, minYPos, 0), new Vector3(minXPos, minYPos, 0));
        Gizmos.DrawLine(new Vector3(maxXPos, maxYPos, 0), new Vector3(minXPos, maxYPos, 0));
    }
}
