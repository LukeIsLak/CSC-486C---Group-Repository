using UnityEngine;
public class GoldDropSpawner : MonoBehaviour
{
    public GameObject goldDropPrefab;
    public int amount = 1;

    private bool isQuitting = false;
    void OnDestroy()
    {
        if (!Application.isPlaying || isQuitting) return;
        GameObject goldDrop = Instantiate(goldDropPrefab);
        goldDrop.transform.position = transform.position;
        goldDrop.GetComponent<GoldDrop>().DoGoldAcquire(amount);
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public void DisableSpawn()
    {
        isQuitting = true;
    }

}
