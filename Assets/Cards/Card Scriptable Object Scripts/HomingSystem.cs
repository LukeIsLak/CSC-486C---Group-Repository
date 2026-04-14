using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingSystem : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 20f;
    public float radius = 50f;

    [SerializeField] private float avoidScalar = 2f;


    private Transform target;
    private bool isActive = false;

    [SerializeField] LayerMask wallLayer;

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
        Vector3 direction = transform.forward;
        if (target != null)
        direction = (target.position - transform.position).normalized;


        //Cast Ray to avoid walls
        RaycastHit hit;
        Vector3 avoidance = Vector3.zero;

        Vector3 rayOrigin = transform.position + transform.position * 0.01f;
        Vector3 lookRight = (transform.forward + transform.right).normalized;
        Vector3 lookLeft = (transform.forward - transform.right).normalized;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, avoidScalar, wallLayer))
            {
                avoidance += hit.normal;
            }
        
        if (Physics.Raycast(rayOrigin, lookRight, out hit, avoidScalar, wallLayer))
            {
                avoidance += hit.normal;
            }

        if (Physics.Raycast(rayOrigin, lookLeft, out hit, avoidScalar, wallLayer))
            {
                avoidance += hit.normal;
            }

        direction = (direction + avoidance * 2f).normalized;
        float turnMultiplier = (avoidance.magnitude > 0) ? 3f : 1f;

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
        Debug.Log("in find nearest enemy");
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
