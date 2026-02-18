using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/AcquirableList")]
public class AcquirableList : ScriptableObject
{
    public List<Acquirable> items;
}
