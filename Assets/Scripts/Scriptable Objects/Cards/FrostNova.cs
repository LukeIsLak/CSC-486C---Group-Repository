using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/FrostNova")]
public class FrostNova : Cards
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 1f;

    [SerializeField] private ShootFrostNova aoeprojprefab;
    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;


    [SerializeField]public float dmg = 20f;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        useSparkCard(card);
        Debug.Log("Used card");
        yield break;

    }

    public void useSparkCard(Cards card)
    {
        //Switch case based on attacktype, will add more cases as more cards get developed
        handleAoE(card);
            
        
    }

    public void handleAoE(Cards card)
    {
        //For AoE attacks, create the projectile and fire it forward based on the player position
        ShootFrostNova proj = Instantiate(aoeprojprefab, camera.transform.position + camera.transform.forward * 0.5f, 
        Quaternion.identity);

        if (proj == null)
{
    Debug.LogError("Projectile prefab not assigned!");
}

        proj.Init(camera.transform.forward, card);
    }
}
