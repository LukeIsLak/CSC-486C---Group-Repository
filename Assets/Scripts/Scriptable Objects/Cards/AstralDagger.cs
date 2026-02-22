using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/AstralDagger")]
public class AstralDagger : Cards
{
    //Play the Dagger Object
    [SerializeField] private DaggerObject daggerPrefab;

    private GameObject player;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");

        //Instantiate the object and start the homing routine

        DaggerObject dagger = Instantiate(daggerPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation);

        HomingSystem homing = dagger.GetComponent<HomingSystem>();
        homing.Initialize();

        yield break;
    }
}
