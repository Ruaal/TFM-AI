using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int actualHP = 100;
    private int maxHP = 100;

    public void Heal(int amount)
    {
        actualHP = Mathf.Min(actualHP + amount, maxHP);
    }
}
