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
    public Transform doorParent;
    public List<GameObject> encounterDoors;
    public float centerToCenter;
    private TraversableLayout traversableLayout;

    [Header("Layout Map")]
    public Transform mapParent;
    void Start()
    {
        traversableLayout = Instantiate(traversableLayoutPrefab).GetComponent<TraversableLayout>();
        traversableLayout.InitializeUninteractable(true);
        traversableLayout.DoProgress(layoutData.completedIndices);
        traversableLayout.transform.SetParent(mapParent, false);
        AdjustLayout();
        traversableLayout.UpdateAppearance();

        List<(EncounterInfo encounter, int index)> encIndxList = traversableLayout.GetNextEncounters(layoutData.completedIndices);

        foreach (var pair in encIndxList)
        {
            GameObject curDoor = Instantiate(encounterDoorPrefab, doorParent);
            curDoor.GetComponent<EncounterDoor>().Initialize(pair.encounter, pair.index, layoutData);
            encounterDoors.Add(curDoor);
        }
        // traversableLayout.DestroyEverything();
        DoDoorPlacement(encounterDoors);
    }

    void AdjustLayout()
    {
        List<Transform> layers = traversableLayout.layerContainers;
        int numCompleted = layoutData.completedIndices.Count;
        for (int i = 0; i < layers.Count; i++ )
        {
            if (numCompleted - 1 <= i && i < numCompleted + layoutData.lookAhead)
            {
                layers[i].gameObject.SetActive(true);
                continue;
            }
            layers[i].gameObject.SetActive(false);
        }
        traversableLayout.transform.localPosition += 3 * (numCompleted - 1) * Vector3.forward;
        traversableLayout.playerOnMap.SetActive(false);
    }

    void DoDoorPlacement(List<GameObject> encounterDoors)
    {
        int count = encounterDoors.Count;
        if (count == 0) return;

        float start = (count - 1) * centerToCenter / 2;
        int i = 0;
        foreach (GameObject curDoor in encounterDoors)
        {
            curDoor.transform.localPosition = (start - centerToCenter * i++) * Vector3.left;
        }
    }
}
