using System.Collections;
using UnityEngine;

public class PosterSecretTunnelAnimated : MonoBehaviour
{
    [Header("Poster")]
    public Transform posterPivot;
    public GameObject poster;

    [Header("Hidden tunnel parts")]
    public GameObject tunnelRim;
    public GameObject tunnelHole;
    public GameObject tunnelTrigger;

    [Header("Player")]
    public Transform player;
    public float interactDistance = 2.0f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    public float openAngle = 85f;
    public float openDuration = 0.8f;

    [Header("Prompt")]
    public string promptText = "Press E to lift poster";

    private bool revealed = false;
    private bool playerNear = false;
    private bool opening = false;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Start()
    {
        if (posterPivot == null)
        {
            posterPivot = transform;
        }

        closedRotation = posterPivot.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);

        if (tunnelRim != null)
        {
            tunnelRim.SetActive(false);
        }

        if (tunnelHole != null)
        {
            tunnelHole.SetActive(false);
        }

        if (tunnelTrigger != null)
        {
            tunnelTrigger.SetActive(false);
        }
    }

    private void Update()
    {
        if (revealed || opening) return;
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        playerNear = distance <= interactDistance;

        if (playerNear && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(OpenPoster());
        }
    }

    private IEnumerator OpenPoster()
    {
        opening = true;
        playerNear = false;

        if (tunnelRim != null)
        {
            tunnelRim.SetActive(true);
        }

        if (tunnelHole != null)
        {
            tunnelHole.SetActive(true);
        }

        float timer = 0f;

        while (timer < openDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / openDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            posterPivot.localRotation = Quaternion.Slerp(
                closedRotation,
                openedRotation,
                t
            );

            yield return null;
        }

        posterPivot.localRotation = openedRotation;

        if (tunnelTrigger != null)
        {
            tunnelTrigger.SetActive(true);
        }

        revealed = true;
        opening = false;
    }

    private void OnGUI()
    {
        if (!playerNear || revealed || opening) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        Rect rect = new Rect(0, Screen.height - 120, Screen.width, 50);
        GUI.Label(rect, promptText, style);
    }
}