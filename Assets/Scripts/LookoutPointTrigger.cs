using UnityEngine;

public class LookoutPointTrigger : MonoBehaviour
{
    public GameObject useTelescopeText;
    public GameObject stopLookingText;

    public GameObject fogWall;
    public GameObject telescopeMask;

    public CameraFollow cameraFollow;
    public Transform telescopeViewPoint;
    public Transform telescopePivot;

    public PlayerController playerController;

    [Range(0f, 1f)]
    public float normalFogAlpha = 1f;

    [Range(0f, 1f)]
    public float lookingFogAlpha = 0.35f;

    public float maxTelescopeYaw = 30f;
    public float telescopeTurnSpeed = 60f;

    private bool playerInside = false;
    private bool isLooking = false;

    private Renderer fogRenderer;

    private Quaternion originalPivotRotation;
    private float currentYaw = 0f;

    private void Start()
    {
        HideAllPrompts();

        if (telescopeMask != null)
        {
            telescopeMask.SetActive(false);
        }

        if (fogWall != null)
        {
            fogRenderer = fogWall.GetComponent<Renderer>();
            SetFogAlpha(normalFogAlpha);
        }

        if (telescopePivot != null)
        {
            originalPivotRotation = telescopePivot.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (!isLooking)
        {
            ShowUsePrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        StopLooking();
        HideAllPrompts();
    }

    private void Update()
    {
        if (!playerInside) return;

        if (!isLooking && Input.GetKeyDown(KeyCode.E))
        {
            StartLooking();
        }

        if (isLooking)
        {
            HandleTelescopeRotation();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                StopLooking();
                ShowUsePrompt();
            }
        }
    }

    private void StartLooking()
    {
        isLooking = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        ApplyTelescopeRotation();

        SetFogAlpha(lookingFogAlpha);

        if (telescopeMask != null)
        {
            telescopeMask.SetActive(true);
        }

        if (cameraFollow != null && telescopeViewPoint != null)
        {
            cameraFollow.StartOverrideView(telescopeViewPoint);
        }

        ShowStopPrompt();
    }

    private void StopLooking()
    {
        isLooking = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        SetFogAlpha(normalFogAlpha);

        if (telescopeMask != null)
        {
            telescopeMask.SetActive(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.StopOverrideView();
        }

        // 不重置 currentYaw，保留望远镜偏移角度
    }

    private void HandleTelescopeRotation()
    {
        if (telescopePivot == null) return;

        float horizontal = Input.GetAxisRaw("Horizontal");

        currentYaw += horizontal * telescopeTurnSpeed * Time.deltaTime;
        currentYaw = Mathf.Clamp(currentYaw, -maxTelescopeYaw, maxTelescopeYaw);

        ApplyTelescopeRotation();
    }

    private void ApplyTelescopeRotation()
    {
        if (telescopePivot == null) return;

        telescopePivot.rotation =
            originalPivotRotation * Quaternion.Euler(0f, currentYaw, 0f);
    }

    private void ShowUsePrompt()
    {
        if (useTelescopeText != null)
        {
            useTelescopeText.SetActive(true);
        }

        if (stopLookingText != null)
        {
            stopLookingText.SetActive(false);
        }
    }

    private void ShowStopPrompt()
    {
        if (useTelescopeText != null)
        {
            useTelescopeText.SetActive(false);
        }

        if (stopLookingText != null)
        {
            stopLookingText.SetActive(true);
        }
    }

    private void HideAllPrompts()
    {
        if (useTelescopeText != null)
        {
            useTelescopeText.SetActive(false);
        }

        if (stopLookingText != null)
        {
            stopLookingText.SetActive(false);
        }
    }

    private void SetFogAlpha(float alpha)
    {
        if (fogRenderer == null) return;

        Color color = fogRenderer.material.color;
        color.a = alpha;
        fogRenderer.material.color = color;
    }
}