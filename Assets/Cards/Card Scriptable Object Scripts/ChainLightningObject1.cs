using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class ChainLightningObject1 : MonoBehaviour
{

    [SerializeField] private float damage = 10f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private int chainNumber = 0;
    [SerializeField] private int maxChain = 5;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private LayerMask surfaceLayers;
    

    public List<EnemyInterface> alreadyHit = new List<EnemyInterface>();

    [SerializeField] private GameObject ChainLightningPrefab;
    [SerializeField] private GameObject lightningPrefab;
    private FMOD.Studio.EventInstance lightningSound;



    public void Init(float dmg, float maxRange, int chainNum, int chainMax, List<EnemyInterface> pastHits, LayerMask surfaces)
    {
        // Set values
        damage = dmg;
        radius = maxRange;
        chainNumber = chainNum;
        maxChain = chainMax;
        alreadyHit = pastHits;
        surfaceLayers = surfaces;
        // Do lightning logic
        Fire();
        StartCoroutine(HandleNext());
    }

    private void Fire()
    {
        List<EnemyInterface> nearbyEnemies = new();

        // Check for enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            EnemyInterface ei = hitCollider.GetComponentInParent<EnemyInterface>();
            if (!ei) continue;

            if (alreadyHit.Contains(ei)) continue;
            // Check if visible 
            Vector3 targetPoint = hitCollider.ClosestPoint(transform.position);
            Vector3 direction = targetPoint - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction.normalized, out hit, direction.magnitude, surfaceLayers)) 
            {
                continue;
            }

            nearbyEnemies.Add(ei);
        }
        if (nearbyEnemies.Count == 0) return;

        // Order list by proximity, get earliest
        EnemyInterface target = nearbyEnemies.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray()[0];
        if(chainNumber != 1)
        {
            PlayLightningSound();
        }
        target.Hit(damage);
        alreadyHit.Add(target);
        targetPos = target.transform.position;

        // Draw graphics
        LightningBoltObject lightning = Instantiate(lightningPrefab).GetComponent<LightningBoltObject>();
        LineRenderer line = lightning.GetComponent<LineRenderer>();
        int points = 6;
        line.positionCount = points;
        Vector3 start = transform.position;
        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);
            Vector3 pos = Vector3.Lerp(start, targetPos, t);

            pos += new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            );
            line.SetPosition(i, pos);
        }
    }

    private IEnumerator HandleNext()
    {
        if (chainNumber >= maxChain) yield break;
        yield return new WaitForSeconds(0.1f);
        GameObject nextLightning = Instantiate(ChainLightningPrefab);
        nextLightning.transform.position = targetPos;
        nextLightning.GetComponent<ChainLightningObject1>().Init(damage, radius, chainNumber + 1, maxChain, alreadyHit, surfaceLayers);
        Destroy(this);
    }

    private void PlayLightningSound() 
    {
        lightningSound = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/chainlightningHit");
        lightningSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        lightningSound.start();
        lightningSound.release();
    }
}
