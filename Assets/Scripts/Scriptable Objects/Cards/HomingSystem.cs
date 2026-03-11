using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingSystem : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 20f;
    public float radius = 50f;


    private Transform target;
    private bool isActive = false;

    public HashSet<EnemyInterface> ignoreEnemies;
    // Initialize the homing projectile
    public void Initialize()
    {
        target = findNearestEnemy();
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        goToEnemy();
    }


    void goToEnemy(){
        //Find the direction vector from the target and current position
        Vector3 direction = (target.position - transform.position).normalized;

        //Make a quaternion in the desired direction and send the object that way.
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

    transform.position += transform.forward * speed * Time.deltaTime;
    }

    Transform findNearestEnemy()
    //Find the nearest enemy to the object
    {
        GameObject[] listOfEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in listOfEnemies)
        //Loop through each enemy, find the distance from player to enemy
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance && distance <= radius){
                EnemyInterface enem = enemy.GetComponent<EnemyInterface>();

                if (ignoreEnemies != null && ignoreEnemies.Contains(enem))
                    continue;
                closest = enemy.transform;
                closestDistance = distance;
            }
        }

        return closest;


    }
}
