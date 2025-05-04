using TMPro;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Transform objectiveListContainer;
    public GameObject objectiveItemPrefab;

    private void Start()
    {
        Hide();
    }

    public void ShowMission(MissionData mission)
    {
        ClearObjectives();
        if (mission == null || mission.objectives == null || mission.objectives.Length == 0)
        {
            Debug.Log("Mission data is null.");
            return;
        }
        panel.SetActive(true);
        titleText.text = "Mission";
        descriptionText.text = mission.description;

        foreach (var obj in mission.objectives)
        {
            GameObject item = Instantiate(objectiveItemPrefab, objectiveListContainer);
            var text = item.GetComponentInChildren<TextMeshProUGUI>();
            text.text = text.text =
                $"- {obj.type.ToUpper()}: {obj.quantity} × {obj.target}\n  Reward: {obj.quantity_reward} × {obj.reward}";
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
}
