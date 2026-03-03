using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/LightningBolt")]
public class LightningBolt : Cards
{
    private float range = 20f;
    private float damage = 20f;


    [SerializeField] private LayerMask enemylayer;
    private GameObject player;
    private GameObject camera;

    public override IEnumerator Play(Cards card)
    {
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        //If the card is played, shoot the lighting from the camera position.
        
        shootLightning(camera);

        yield break;
    }

    private void shootLightning(GameObject camera)
    {
        //Make raycast from the camera position and shoot it forward based on the range
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        Vector3 endPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemylayer))
        {
            //Find the point that the raycast collided with an object
            //If an enemy, deal damage

            endPoint = hit.point;

            EnemyInterface enemy = hit.collider.GetComponent<EnemyInterface>();
            if (enemy != null)
            {
                enemy.Hit(damage);
            }
            else 
            {
                enemy = enemy.GetComponentInParent<EnemyInterface>();
                enemy.Hit(damage);
            }
        }
    }

    //To add: Visual Effect
}
