using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ModifierCards")]
public class ModifierCards : Cards
{
    public string effect;
    public int value;

    public override IEnumerator Play(Cards card){
        yield break;
    }
}