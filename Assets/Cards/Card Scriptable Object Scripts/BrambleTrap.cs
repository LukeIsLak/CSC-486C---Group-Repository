using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleTrap : MonoBehaviour
{
    [SerializeField] private float vineLength = 1f;
    [SerializeField] private GameObject vine;
    [SerializeField] private float slowAmount = 5f;
    [SerializeField] private float damage = 2f;

    [SerializeField] private float ttl = 5f;

    [SerializeField] private DamageOverTime DoT;

    [SerializeField] private Stop stopEffect;

    private List<EnemyInterface> seenEnemies = new List<EnemyInterface>();
    private List<GameObject> activeVines = new List<GameObject>();

    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    //Change this function later to prevent double hits
    void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyInterface>();

        if (enemy != null && !seenEnemies.Contains(enemy)) {
            enemy.Hit(damage, DoT.type, DoT);
            enemy.Hit(0, stopEffect.type, stopEffect);
            seenEnemies.Add(enemy);

            if (vine != null) {
                GameObject vineRoot = new GameObject("VineRoot");
                vineRoot.transform.position = transform.position;
                vineRoot.AddComponent<VineSegmentFollow>().Init(this.transform, enemy.transform, vine, vineLength);
                activeVines.Add(vineRoot);
            }
        }
        //Implement slow
    }

    void OnTriggerExit(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyInterface>();
        if (enemy != null && seenEnemies.Contains(enemy)) {
            seenEnemies.Remove(enemy);
            for (int i = activeVines.Count - 1; i >= 0; i--) {
                VineSegmentFollow vineFollow = activeVines[i].GetComponent<VineSegmentFollow>();
                if (vineFollow != null && vineFollow.EnemyTransform == enemy.transform) {
                    Destroy(activeVines[i]);
                    activeVines.RemoveAt(i);
                }
            }
        }
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        foreach (var v in activeVines) if (v != null) Destroy(v);
        Destroy(this.gameObject);
    }
}



public class VineSegmentFollow : MonoBehaviour
{
    private Transform trap;
    private Transform enemy;
    private GameObject vinePrefab;
    private float vineLength;
    private List<GameObject> segments = new List<GameObject>();
    public Transform EnemyTransform => enemy;
    private FMOD.Studio.EventInstance brambleGrab;


    public void Init(Transform trap, Transform enemy, GameObject vinePrefab, float vineLength)
    {
        this.trap = trap;
        this.enemy = enemy;
        this.vinePrefab = vinePrefab;
        this.vineLength = vineLength;
        PlayBrambleGrab();
    }

    void Update()
    {
        if (trap == null || enemy == null) {
            foreach (var seg in segments) if (seg != null) Destroy(seg);
            Destroy(gameObject);
            return;
        }

        foreach (var seg in segments) if (seg != null) Destroy(seg);
        segments.Clear();

        Vector3 start = trap.position;
        Vector3 end = enemy.position;
        float endYOffset = 0.5f;
        end += Vector3.up * endYOffset;

        Vector3 dir = (end - start).normalized;
        float dist = Vector3.Distance(start, end);

        int count = Mathf.CeilToInt(dist / vineLength);
        for (int i = 0; i < count; i++) {
            float t = Mathf.Clamp01((i * vineLength) / dist);
            Vector3 pos = Vector3.Lerp(start, end, t);
            GameObject seg = Instantiate(vinePrefab, pos, Quaternion.identity, this.transform);
            seg.transform.LookAt(i == count - 1 ? end : Vector3.Lerp(start, end, Mathf.Clamp01(((i + 1) * vineLength) / dist)));

            Vector3 scale = seg.transform.localScale;
            scale.y = vineLength * 0.5f;
            seg.transform.localScale = scale;

            segments.Add(seg);
        }
    }

    private void PlayBrambleGrab() 
    {
        brambleGrab = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/brambleSnareGrab");
        brambleGrab.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        brambleGrab.start();
        brambleGrab.release();
    }
}
