using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/ShopItemList")]
public class ShopItemList : ScriptableObject
{
    public List<ShopItem> items;
}
