using TMPro;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Transform objectiveListContainer;
    public GameObject objectiveItemPrefab;

    private MissionData CurrentMission => MissionManager.Instance.GetActiveMission();

    private void Start()
    {
        Hide();
    }

    private void OnEnable()
    {
        PlayerController.OnObjectivesUpdated += RefreshProgress;
    }

    public void ShowMission(MissionData mission)
    {
        UpdateObjectivesDisplay();
        panel.SetActive(true);
        titleText.text = "Mission";
        descriptionText.text = mission.description;
    }

    private void UpdateObjectivesDisplay()
    {
        ClearObjectives();
        if (
            CurrentMission == null
            || CurrentMission.objectives == null
            || CurrentMission.objectives.Length == 0
        )
        {
            Debug.Log("Mission data is null.");
            return;
        }

        foreach (var obj in CurrentMission.objectives)
        {
            GameObject item = Instantiate(objectiveItemPrefab, objectiveListContainer);
            var text = item.GetComponentInChildren<TextMeshProUGUI>();
            int progress = 0;
            if (obj.type == "collect")
                progress = PlayerController.Instance.GetItemCount(obj.target);
            else if (obj.type == "defeat")
                progress = PlayerController.Instance.GetEnemyCount(obj.target);

            text.text =
                $"- {obj.type.ToUpper()}: {progress}/{obj.quantity} × {obj.target}\n  Reward: {obj.quantity_reward} × {obj.reward}";
        }
    }

    private void ClearObjectives()
    {
        foreach (Transform child in objectiveListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    // Llama a este método cuando quieras refrescar el progreso (por ejemplo, desde PlayerProgressManager tras un evento)
    public void RefreshProgress()
    {
        if (panel.activeSelf && CurrentMission != null)
            UpdateObjectivesDisplay();
    }
}
