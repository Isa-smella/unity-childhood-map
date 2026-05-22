using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 2.2f, -5f);
    public float followSpeed = 8f;
    public float rotationSpeed = 8f;

    private bool usingOverrideView = false;
    private Transform overrideViewPoint;

    void LateUpdate()
    {
        if (usingOverrideView && overrideViewPoint != null)
        {
            transform.position = overrideViewPoint.position;
            transform.rotation = overrideViewPoint.rotation;
            return;
        }

        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * 1.2f);
    }

    public void StartOverrideView(Transform viewPoint)
    {
        overrideViewPoint = viewPoint;
        usingOverrideView = true;

        if (overrideViewPoint != null)
        {
            transform.position = overrideViewPoint.position;
            transform.rotation = overrideViewPoint.rotation;
        }
    }

    public void StopOverrideView()
    {
        usingOverrideView = false;
        overrideViewPoint = null;
    }
}