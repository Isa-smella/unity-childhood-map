using UnityEngine;

public class HillViewTrigger : MonoBehaviour
{
    public GameObject secretEntrance;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (secretEntrance != null)
            {
                secretEntrance.SetActive(true);
            }

            Debug.Log("你站上小山，看见了远处的秘密入口。");
        }
    }
}