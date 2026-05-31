using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PosterPeelTunnelAnimated : MonoBehaviour
{
    [Header("Poster Size")]
    public float width = 1.5f;
    public float height = 2.0f;
    public int xSegments = 16;
    public int ySegments = 20;

    [Header("Hidden tunnel parts")]
    public GameObject tunnelRim;
    public GameObject tunnelHole;
    public GameObject tunnelTrigger;

    [Header("Player")]
    public Transform player;
    public float interactDistance = 3.0f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Peel Animation")]
    public float peelDuration = 1.2f;
    public float resetDuration = 0.8f;
    public float liftAmount = 1.05f;
    public float sidePullAmount = 0.35f;
    public float forwardCurlAmount = 0.35f;
    public float curlStrength = 0.25f;

    [Header("Prompt")]
    public string promptText = "Press E to lift poster";

    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] animatedVertices;

    private bool playerNear = false;
    private bool revealed = false;
    private bool opening = false;
    private bool closing = false;

    private void Start()
    {
        BuildPosterMesh();

        if (tunnelRim != null)
            tunnelRim.SetActive(false);

        if (tunnelHole != null)
            tunnelHole.SetActive(false);

        if (tunnelTrigger != null)
            tunnelTrigger.SetActive(false);

        ApplyPeel(0f);
    }

    private void Update()
    {
        // 海报已经掀开、正在掀开、正在复原时，绝对不显示 lift poster 提示
        if (revealed || opening || closing)
        {
            playerNear = false;
            return;
        }

        if (player == null)
        {
            playerNear = false;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);
        playerNear = distance <= interactDistance;

        if (playerNear && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(PeelPoster());
        }
    }

    private void BuildPosterMesh()
    {
        mesh = new Mesh();
        mesh.name = "Peelable Poster Mesh";

        int vertexCountX = xSegments + 1;
        int vertexCountY = ySegments + 1;

        baseVertices = new Vector3[vertexCountX * vertexCountY];
        animatedVertices = new Vector3[baseVertices.Length];

        Vector2[] uvs = new Vector2[baseVertices.Length];
        int[] triangles = new int[xSegments * ySegments * 6];

        int index = 0;

        for (int y = 0; y < vertexCountY; y++)
        {
            float v = (float)y / ySegments;

            for (int x = 0; x < vertexCountX; x++)
            {
                float u = (float)x / xSegments;

                float localX = (u - 0.5f) * width;
                float localY = (v - 0.5f) * height;

                baseVertices[index] = new Vector3(localX, localY, 0f);
                animatedVertices[index] = baseVertices[index];
                uvs[index] = new Vector2(u, v);

                index++;
            }
        }

        int tri = 0;

        for (int y = 0; y < ySegments; y++)
        {
            for (int x = 0; x < xSegments; x++)
            {
                int bottomLeft = y * vertexCountX + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + vertexCountX;
                int topRight = topLeft + 1;

                triangles[tri++] = bottomLeft;
                triangles[tri++] = topLeft;
                triangles[tri++] = bottomRight;

                triangles[tri++] = bottomRight;
                triangles[tri++] = topLeft;
                triangles[tri++] = topRight;
            }
        }

        mesh.vertices = animatedVertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private IEnumerator PeelPoster()
    {
        // 按 E 的瞬间，立刻标记为已掀开
        // 这样 lift poster 提示马上消失
        revealed = true;
        opening = true;
        playerNear = false;

        if (tunnelRim != null)
            tunnelRim.SetActive(true);

        if (tunnelHole != null)
            tunnelHole.SetActive(true);

        float timer = 0f;

        while (timer < peelDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / peelDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            ApplyPeel(t);

            yield return null;
        }

        ApplyPeel(1f);

        if (tunnelTrigger != null)
            tunnelTrigger.SetActive(true);

        opening = false;
    }

    public void ResetPoster()
    {
        if (!revealed || closing) return;

        StartCoroutine(ResetPosterRoutine());
    }

    private IEnumerator ResetPosterRoutine()
    {
        closing = true;

        if (tunnelTrigger != null)
            tunnelTrigger.SetActive(false);

        float timer = 0f;

        while (timer < resetDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / resetDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            ApplyPeel(1f - t);

            yield return null;
        }

        ApplyPeel(0f);

        if (tunnelRim != null)
            tunnelRim.SetActive(false);

        if (tunnelHole != null)
            tunnelHole.SetActive(false);

        revealed = false;
        opening = false;
        closing = false;
        playerNear = false;
    }

    private void ApplyPeel(float t)
    {
        int vertexCountX = xSegments + 1;
        int vertexCountY = ySegments + 1;

        float finalT = t * 0.9f;

        for (int y = 0; y < vertexCountY; y++)
        {
            float v = (float)y / ySegments;

            for (int x = 0; x < vertexCountX; x++)
            {
                float u = (float)x / xSegments;
                int index = y * vertexCountX + x;

                Vector3 p = baseVertices[index];

                // 右下角翻起：u 越接近 1，v 越接近 0，影响越大
                float cornerInfluence = u * (1f - v);
                cornerInfluence = Mathf.Pow(cornerInfluence, 1.7f);

                float amount = cornerInfluence * finalT;

                // 右下角向上翻
                p.y += amount * liftAmount;

                // 向左侧略微收，形成翻书角
                p.x -= amount * sidePullAmount * 0.6f;

                // 从墙面向外翘起
                p.z -= amount * forwardCurlAmount;

                // 卷曲弧度
                float curl = Mathf.Sin(amount * Mathf.PI);
                p.z -= curl * curlStrength;

                // 右下角边缘额外翘起
                if (u > 0.75f && v < 0.35f)
                {
                    float edgeBoost =
                        Mathf.InverseLerp(0.75f, 1f, u) *
                        Mathf.InverseLerp(0.35f, 0f, v);

                    p.y += edgeBoost * finalT * 0.35f;
                    p.z -= edgeBoost * finalT * 0.25f;
                }

                animatedVertices[index] = p;
            }
        }

        mesh.vertices = animatedVertices;
        mesh.RecalculateNormals();
    }

    private void OnGUI()
    {
        // 只在“海报关闭 + 玩家靠近”时显示
        if (revealed) return;
        if (opening) return;
        if (closing) return;
        if (!playerNear) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        Rect rect = new Rect(0, Screen.height - 120, Screen.width, 50);
        GUI.Label(rect, promptText, style);
    }
}