using UnityEngine;
using UnityEngine.SceneManagement;
public class GoldDropSpawner : MonoBehaviour
{
    public GameObject goldDropPrefab;
    public int amount = 1;

    public void DoGoldDrop()
    {
        GameObject goldDrop = Instantiate(goldDropPrefab);
        goldDrop.transform.position = transform.position;
        goldDrop.GetComponent<GoldDrop>().DoGoldAcquire(amount);
    }
}
