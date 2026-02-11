using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="EnemyEffects")]
public class EnemyEffects : ScriptableObject
{
    //constants for resetting
    const float ENEMYSPEEDNORMAL = 1f;
    
    //vars important to enemys
    public float EnemySpeedModifier = ENEMYSPEEDNORMAL;

    //called by decksystem to modify all enemys speed
    public void changeEnemySpeed(float value) {
        EnemySpeedModifier = value;
    }

    //called by decksystem to signal the end of enemy speed modifcation
    public void resetEnemySpeed(){
        EnemySpeedModifier = ENEMYSPEEDNORMAL;
    }

    public float getEnemySpeedModifier(){
        return EnemySpeedModifier;
    }
}
