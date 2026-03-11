using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/BrambleSnare")]
public class BrambleSnare : Cards
{
    // Definition from Design Team:
    // Creates a thorny vine trap area that slows down enemies as they move through it. Enemies take DOT as they move through the area.

    public BrambleTrap bramblePrefab;
    private float spawnDistance = 5f;

    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        Debug.Log("Used card");

        Vector3 startPos = player.transform.position + player.transform.forward * spawnDistance;
        

        BrambleTrap trap = Instantiate(bramblePrefab, startPos, Quaternion.identity);

        yield break;

    }
}
