using UnityEngine;
// Use the CreateAssetMenu attribute to allow creating instances of this ScriptableObject from the Unity Editor.
[CreateAssetMenu(menuName = "AudioManager")]
public class AudioManagerScriptableObject : ScriptableObject
{
 public FMODUnity.EventReference music;
}