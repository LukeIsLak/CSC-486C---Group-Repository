using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/AstralDagger")]
public class AstralDagger : Cards
{
    //Play the Dagger Object
    [SerializeField] private DaggerObject daggerPrefab;

    private GameObject player;
    private GameObject camera;


    

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");


        //Instantiate the object and start the homing routine

        DaggerObject dagger = Instantiate(daggerPrefab, camera.transform.position + camera.transform.forward * 2f, camera.transform.rotation);

        HomingSystem homing = dagger.GetComponent<HomingSystem>();
        homing.Initialize();

        yield break;
    }
}
