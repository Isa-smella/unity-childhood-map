using UnityEngine;

public class WindSwing : MonoBehaviour
{
    public float swingAngle = 6f;
    public float swingSpeed = 1.5f;
    public float randomOffset = 0f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float angle = Mathf.Sin((Time.time + randomOffset) * swingSpeed) * swingAngle;

        transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }
}