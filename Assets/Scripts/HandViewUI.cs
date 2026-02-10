using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class HandViewUI : MonoBehaviour
{
    [SerializeField] private DeckSystems deckSystems;
    [SerializeField] private SplineContainer splineContainer;
    private void UpdateCardPosition()
    {
        if(deckSystems.hand.Count == 0) return;


    }
}
