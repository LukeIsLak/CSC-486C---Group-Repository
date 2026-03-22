using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterDoorCreator : MonoBehaviour
{
    [Header("References")]
    public GameObject traversableLayoutPrefab;
    public GameObject encounterDoorPrefab;
    public LayoutData layoutData;

    [Header("Door Placement")]
    public List<GameObject> encounterDoors;
    public float centerToCenter;
    private TraversableLayout traversableLayout;
    void Start()
    {
        traversableLayout = Instantiate(traversableLayoutPrefab).GetComponent<TraversableLayout>();
        traversableLayout.Initialize();
        List<(EncounterInfo encounter, int index)> encIndxList = traversableLayout.GetNextEncounters(layoutData.completedIndices);

        foreach (var pair in encIndxList)
        {
            GameObject curDoor = Instantiate(encounterDoorPrefab);
            Debug.Log(pair.index);
            curDoor.GetComponent<EncounterDoor>().Initialize(pair.encounter, pair.index, layoutData);
            encounterDoors.Add(curDoor);
        }
        traversableLayout.DestroyEverything();
        DoDoorPlacement(encounterDoors);
    }

    void DoDoorPlacement(List<GameObject> encounterDoors)
    {
        
    }
}
