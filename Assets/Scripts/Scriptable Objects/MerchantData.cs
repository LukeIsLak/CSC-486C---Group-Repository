using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/MerchantData")]

public class MerchantData : ScriptableObject
{
    public string dialogue;
    public List<ShopItem> wares;
}
