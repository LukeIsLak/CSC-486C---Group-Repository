using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/BrambleSnare")]
public class BrambleSnare : Cards
{
    // Definition from Design Team:
    // Creates a thorny vine trap area that slows down enemies as they move through it. Enemies take DOT as they move through the area.

    public BrambleGrenade grenadePrefab;

    public BrambleTrap bramblePrefab;
    [SerializeField] private float spawnDistance = 5f;

    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        Debug.Log("Used card");

        BrambleGrenade projectile = Instantiate(grenadePrefab, camera.transform.position + camera.transform.forward, player.transform.rotation);
        projectile.Throw(camera.transform.forward);
        
        yield break;

    }
}
