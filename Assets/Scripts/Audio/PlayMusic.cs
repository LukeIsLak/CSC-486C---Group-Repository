using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField]
    private AudioManagerScriptableObject manager;
   
    // Start is called before the first frame update
    void Start()
    {
          FMODUnity.RuntimeManager.PlayOneShot(manager.music);
    }
}
