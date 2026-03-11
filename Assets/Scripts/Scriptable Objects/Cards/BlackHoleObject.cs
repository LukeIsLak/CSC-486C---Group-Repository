using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleObject : MonoBehaviour
{

    private Vector3 direction;
    [SerializeField] private float speed = 2f;

    [SerializeField]private float radius = 5f;
    [SerializeField] private float ttl = 5f;

    void Start(){
        StartCoroutine(timeToLive(ttl));
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
        EnemyInterface enem = other.GetComponent<EnemyInterface>();
            if (enem != null){
                //Pull towards
            }
            else {
                enem = other.GetComponentInParent<EnemyInterface>();
                if (enem != null){
                }
            }
        Debug.Log("In hitbox");

    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }
    
}
