using UnityEngine;
using UnityEngine.UI;

public class TutorialChecklistDoorUnlock : MonoBehaviour
{
    [SerializeField] private Toggle[] tasks;
    [SerializeField] private DoorController door;
    [SerializeField] private bool openOnlyOnce = true;
    [SerializeField] private bool resetTogglesOnStart = true;

    private bool hasOpened;

    private void Start()
    {
        if (resetTogglesOnStart && tasks != null)
        {
            foreach (var t in tasks)
                if (t != null) t.SetIsOnWithoutNotify(false);
        }

        hasOpened = false;
        Evaluate();
    }

    private void OnEnable()
    {
        if (tasks == null) return;
        foreach (var t in tasks)
            if (t != null) t.onValueChanged.AddListener(OnAnyTaskToggled);
    }

    private void OnDisable()
    {
        if (tasks == null) return;
        foreach (var t in tasks)
            if (t != null) t.onValueChanged.RemoveListener(OnAnyTaskToggled);
    }

    private void OnAnyTaskToggled(bool _)
    {
        Evaluate();
    }

    private void Evaluate()
    {
        if (door == null || tasks == null || tasks.Length == 0) return;
        if (openOnlyOnce && hasOpened) return;

        foreach (var t in tasks)
        {
            if (t == null) continue;
            if (!t.isOn) return;
        }

        hasOpened = true;
        door.Open();
    }
}