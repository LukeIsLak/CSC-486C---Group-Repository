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

    List<GameObject> enemiesInRange = new();

    void OnEnable()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/DynamicMusic");
        instance.start();
    }
    void Update()
    {
        enemiesInRange.RemoveAll(item => item == null);
        inCombat = enemiesInRange.Count > 0? 1 : 0;
        instance.setParameterByName("Combat", inCombat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemiesInRange.Remove(other.gameObject);
        } 
    }

    public static void stopMusic()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }

    void OnDestroy()
    {
        //stopMusic();
    }
}
