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



    public void Init(Vector3 direct, Cards card)
    {
        dir = direct;
    }

    void Start(){
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
        //Handle knockback and damage for enemies in the collider
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy != null){
                enemy.Hit(dmg, effect.type, effect, transform.position);
            }
            else {
                enemy = other.GetComponentInParent<EnemyInterface>();
                if (enemy != null){
                    enemy.Hit(dmg, effect.type, effect, transform.position);
                }
            }
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }
}
