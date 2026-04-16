using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/LightningBolt")]
public class LightningBolt : Cards
{
    [SerializeField] private float range = 20f;


    [SerializeField] private LightningBoltObject lightningPrefab;

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
        Ray ray = new Ray(camera.transform.position + camera.transform.forward * 2f, camera.transform.forward);

        Vector3 endPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, enemylayer))
        {
            //Find the point that the raycast collided with an object
            //If an enemy, deal damage

            endPoint = hit.point;

            EnemyInterface enemy = hit.collider.GetComponent<EnemyInterface>();
            if (enemy != null)
            {
                enemy.Hit(effectValue);
            }
            else 
            {
                enemy = hit.collider.GetComponentInParent<EnemyInterface>();
                enemy.Hit(effectValue);
            }
        }
        Debug.Log("Trying Lightning Bolt");
        LightningBoltObject lightning = Instantiate(lightningPrefab);
        LineRenderer line = lightning.GetComponent<LineRenderer>();
        int points = 6;
        line.positionCount = points;
        Vector3 start = camera.transform.position + camera.transform.forward * 0.5f;
        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);
            Vector3 pos = Vector3.Lerp(start, endPoint, t);

            pos += new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            );

            line.SetPosition(i, pos);
        }
    }

    //To add: Visual Effect
}
