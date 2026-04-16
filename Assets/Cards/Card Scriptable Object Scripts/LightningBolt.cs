using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/LightningBolt")]
public class LightningBolt : Cards
{
    [SerializeField] private float range = 20f;


    [SerializeField] private LightningBoltObject lightningPrefab;

    [SerializeField] private LayerMask enemylayer;
    [SerializeField] private LayerMask surfaceLayer;
    private GameObject player;
    private GameObject camera;
    private FMOD.Studio.EventInstance lightningSound;


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
        PlayLightningSound();
        //Make raycast from the camera position and shoot it forward based on the range

        // Visual effect

        Ray ray = new Ray(camera.transform.position + camera.transform.forward * 2f, camera.transform.forward);
        Vector3 endPoint = ray.origin + ray.direction * range;

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

        //Make raycast from the camera position and shoot it forward based on the range


        RaycastHit[] hits = Physics.RaycastAll(ray, range);
        hits = hits.OrderBy(c => (camera.transform.position - c.transform.position).sqrMagnitude).ToArray();


        foreach (RaycastHit hit in hits)
        {
            // Check if we found a surface
            if ((surfaceLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                return;

            EnemyInterface ei = hit.collider.GetComponentInParent<EnemyInterface>();
            if (!ei) return;
            ei.Hit(effectValue);
        }
        /*
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
        */
    }

    private void PlayLightningSound() 
    {
        lightningSound = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/lightningHit");
        lightningSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(player));
        lightningSound.start();
        lightningSound.release();
    }

    //To add: Visual Effect
}
