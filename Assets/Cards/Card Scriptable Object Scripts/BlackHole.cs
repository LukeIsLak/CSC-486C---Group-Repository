using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Cards/BlackHole")]
public class BlackHole : Cards
{
    //Design Description
    //Moving projectile that vacuums in nearby enemies.

    [SerializeField] private BlackHoleObject blackHolePrefab;

    private GameObject camera;
    private GameObject player;

    public override IEnumerator Play(Cards card){

        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        Debug.Log("Used card");

        // Fire the object from the camera position and go forward
        Vector3 startPos = camera.transform.position + camera.transform.forward * 2f;

        BlackHoleObject blackhole = Instantiate(blackHolePrefab, startPos, Quaternion.identity);
        blackhole.Init(camera.transform.forward, card);

        yield break;

    }



}
