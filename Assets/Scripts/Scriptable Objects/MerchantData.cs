using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/MerchantData")]

public class MerchantData : ScriptableObject
{
    public List<string> openerOptions;
    public List<string> successOptions;
    public List<string> failOptions;
    public List<ShopItem> wares;

    public string GetOpener()
    {
        if (openerOptions.Count == 0) return "";
        int rnd = Random.Range(0, openerOptions.Count);
        return openerOptions[rnd];
    }

    public string GetResponse(bool success)
    {
        List<string> options = success ? successOptions : failOptions;
        if (options.Count == 0) return "";
        int rnd = Random.Range(0, options.Count);
        return options[rnd];
    }
}
