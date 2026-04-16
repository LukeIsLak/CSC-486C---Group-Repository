using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    private Vector3 dir;
    [SerializeField] private float dmg = 4f;
    [SerializeField] private float speed = 3f;

    [SerializeField] private float ttl = 3f;
    [SerializeField] private Knockback effect;
    private FMOD.Studio.EventInstance waveActive;
    private FMOD.Studio.EventInstance waveHit;





    public void Init(Vector3 direct, Cards card)
    {
        dir = direct;
    }

    void Start(){
        waveActive = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/tidalWaveActive");
        waveActive.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        waveActive.start();
        StartCoroutine(timeToLive(ttl));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TEST!");
        waveHit = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/tidalWaveHit");
        waveHit.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        waveHit.start();
        waveHit.release();
        //Handle knockback and damage for enemies in the collider
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy != null){
                GameObject player = GameObject.FindWithTag("Player");
                enemy.Hit(dmg, effect.type, effect, player.transform.position);
            }
            else {
                enemy = other.GetComponentInParent<EnemyInterface>();
                if (enemy != null){
                    GameObject player = GameObject.FindWithTag("Player");
                    enemy.Hit(dmg, effect.type, effect, player.transform.position);
                }
            }
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        waveActive.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        waveActive.release();
        Destroy(this.gameObject);
    }
}
