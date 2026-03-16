using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHandler : MonoBehaviour
{
    public LayoutData layoutData;
    public GameObject fogPrefab;
    public Transform fogContainer;
    
    public List<List<GameObject>> fogLayerList;    // Contains only active fog
    private List<GameObject> fogLayer;

    // Start is called before the first frame update

    void Awake()
    {
        fogLayerList = new List<List<GameObject>>();
    }
    void Start()
    {
        InitialFogPlacement();
    }

    public void InitialFogPlacement()
    {
        float startX = -layoutData.maxWidth * layoutData.encounterSep / 2 + 0.5f * layoutData.encounterSep;
        
        // Place fog at unrevealed layers
        // XXX Eventually remove magic 4 here 
        for (int i = 0; i <= layoutData.depth + 4; i++)
        {
            if (i < layoutData.layersRevealed) continue;
            fogLayer = new List<GameObject>();
            
            float offsetZ = -i * layoutData.layerDistance;
            
            // Place object according to layer width, encounter separation
            for (int j = 0; j < layoutData.maxWidth; j++)
            {
                GameObject fogObj = Instantiate(fogPrefab, fogContainer);
                fogObj.transform.localPosition = new Vector3(startX + j * layoutData.encounterSep, 0f, offsetZ);
                fogLayer.Add(fogObj);
            }
            fogLayerList.Add(fogLayer);
        }
    }

    public void FogUpdated()
    {
        if (fogLayerList.Count == 0) return;

        foreach (GameObject fog in fogLayerList[0])
        {
            fog.GetComponent<FogObject>().DoFadeAndDelete();
        }
        fogLayerList[0].Clear();
        fogLayerList.RemoveAt(0);
    }
}
