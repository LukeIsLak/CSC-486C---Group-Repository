using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockedChest : MonoBehaviour
{
    public Transform enemyChestParent;
    public float enemyCheckInterval = 5f;
    public string enemyTag;
    public GameObject lockedEffect;
    public int enemiesLeft;
    
    private List<GameObject> chests = new();
    private List<GameObject> lockedEffects = new();
    void Start()
    {
        foreach (Transform child in enemyChestParent)
        {
            Chest chest = child.GetComponent<Chest>();
            if (chest == null) continue;
            chests.Add(chest.gameObject);
            chest.canBeOpened = false;
            GameObject effect = Instantiate(lockedEffect);
            effect.transform.position = chest.transform.position;
            lockedEffects.Add(effect);
        }
        StartCoroutine(CheckForEnemies());
    }
    private void UpdateEnemiesLeft()
    {
        int enemiesFound = 0;
        foreach (Transform child in enemyChestParent)
        {
            if (child.CompareTag(enemyTag))
            enemiesFound++;
        }
        enemiesLeft = enemiesFound;
    }

    private IEnumerator CheckForEnemies()
    {
        UpdateEnemiesLeft();

        while (enemiesLeft > 0)
        {
            yield return new WaitForSeconds(enemyCheckInterval);
            UpdateEnemiesLeft();
        }
        foreach (GameObject chest in chests)
            chest.GetComponent<Chest>().canBeOpened = true;


        foreach (GameObject effect in lockedEffects)
            Destroy(effect);
    }
}
