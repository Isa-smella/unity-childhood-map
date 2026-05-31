using UnityEngine;

public class TunnelTeleport : MonoBehaviour
{
    [Header("Teleport")]
    public Transform player;
    public Transform exitPoint;
    public KeyCode interactKey = KeyCode.E;

    [Header("Poster Reset")]
    public PosterPeelTunnelAnimated posterSystem;

    [Header("Prompt")]
    public string promptText = "按 E 进入通道";

    private bool playerInside = false;
    private float enableTime;

    private void OnEnable()
    {
        enableTime = Time.time;
        playerInside = false;
    }

    private void Update()
    {
        if (!playerInside) return;
        if (player == null || exitPoint == null) return;

        // 防止掀海报那一次 E 直接连带触发传送
        if (Time.time - enableTime < 0.5f) return;

        if (Input.GetKeyDown(interactKey))
        {
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player == null) return;

        if (other.transform == player)
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player == null) return;

        if (other.transform == player)
        {
            playerInside = false;
        }
    }

    private void TeleportPlayer()
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        playerInside = false;

        if (posterSystem != null)
            posterSystem.ResetPoster();
    }

    private void OnGUI()
    {
        if (!playerInside) return;
        if (Time.time - enableTime < 0.5f) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        Rect rect = new Rect(0, Screen.height - 180, Screen.width, 50);
        GUI.Label(rect, promptText, style);
    }
}