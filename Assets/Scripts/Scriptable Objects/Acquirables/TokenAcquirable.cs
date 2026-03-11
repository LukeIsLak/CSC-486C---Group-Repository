using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/TokenAcquirable")]
public class TokenAcquirable : Acquirable
{
    public PlayerInventory playerInventory;
    public override void Acquire()
    {
        playerInventory.AddRemovalToken();
    }
}
