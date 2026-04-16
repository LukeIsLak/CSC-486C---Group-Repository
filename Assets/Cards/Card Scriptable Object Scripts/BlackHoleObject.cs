using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleObject : MonoBehaviour
{

    private Vector3 direction;
    [SerializeField] private float speed = 2f;

    [SerializeField]private float radius = 5f;
    [SerializeField] private float ttl = 5f;
    [SerializeField] private Knockback effect;
    private FMOD.Studio.EventInstance blackHoleActive;


    void Start(){
        StartCoroutine(timeToLive(ttl));
        blackHoleActive = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/blackholeActive");
        blackHoleActive.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        blackHoleActive.start();
    }

    public void Init(Vector3 dir, Cards card)
    {
        direction = dir;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        //For enemies in the hitbox, pull them towards
        // XXX fix the dup call
        // XXX fix the reference of origin (in enemy interface)
        EnemyInterface enem = other.GetComponent<EnemyInterface>();
            if (enem != null){
                enem.Hit(0, effect.type, effect, transform.position);
            }
            else {
                enem = other.GetComponentInParent<EnemyInterface>();
                if (enem != null){
                    enem.Hit(0, effect.type, effect, transform.position);
                }
            }
        Debug.Log("In hitbox");

    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        blackHoleActive.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        blackHoleActive.release();
        Destroy(this.gameObject);
    }
    
}
