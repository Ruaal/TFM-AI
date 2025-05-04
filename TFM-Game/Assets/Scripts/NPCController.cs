using UnityEngine;
using UnityEngine.InputSystem;

public class NPCController : MonoBehaviour
{
    public string npcId = "Aria";
    public UIChatHandler chatUI;

    public void interact()
    {
        chatUI.Open(npcId);
    }
}
