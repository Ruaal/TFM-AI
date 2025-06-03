using System;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private MissionData activeMission;
    public event Action<MissionData> OnMissionAssigned;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool HasActiveMission() => activeMission != null;

    public MissionData GetActiveMission() => activeMission;

    public bool AssignMission(MissionData mission)
    {
        if (activeMission != null)
        {
            return false;
        }

        activeMission = mission;
        OnMissionAssigned?.Invoke(mission);
        return true;
    }

    public bool IsMissionComplete()
    {
        if (activeMission.description == null)
            return false;

        foreach (var obj in activeMission.objectives)
        {
            if (
                obj.type == "collect"
                && PlayerController.Instance.GetItemCount(obj.target) < obj.quantity
            )
                return false;
            if (
                obj.type == "defeat"
                && PlayerController.Instance.GetEnemyCount(obj.target) < obj.quantity
            )
                return false;
        }

        return true;
    }

    public void CompleteMission()
    {
        if (activeMission == null)
            return;

        var inventory = PlayerController.Instance.GetComponent<Inventory>();
        if (inventory == null)
        {
            return;
        }

        foreach (var obj in activeMission.objectives)
        {
            var rewardItem = Resources.Load<ItemData>($"Items/{obj.reward}");
            if (rewardItem == null)
            {
                continue;
            }

            inventory.AddItem(rewardItem, obj.quantity_reward);
        }

        activeMission = null;
    }
}
