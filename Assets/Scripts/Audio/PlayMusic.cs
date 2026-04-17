using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
   
static FMOD.Studio.EventInstance instance;

//public FMODUnity.EventReference fmodEvent;
[SerializeField] 
float inCombat = 0;
void OnEnable()
{
    instance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/DynamicMusic");
    instance.start();
}
void Update()
{
    instance.setParameterByName("Combat", inCombat);
}

private void OnTriggerEnter(Collider other)
{
    if(other.tag == "Enemy")
    {
        inCombat = 1;    
    }
}

private void OnTriggerExit(Collider other)
{
    if(other.tag == "Enemy")
    {
        inCombat = 0;    
    } 
}

public static void stopMusic()
{
    instance.release();
}


}
