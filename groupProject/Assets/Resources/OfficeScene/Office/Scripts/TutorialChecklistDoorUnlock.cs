using UnityEngine;
using UnityEngine.UI;

public class TutorialChecklistDoorUnlock : MonoBehaviour
{
    [SerializeField] private Toggle[] tasks;
    [SerializeField] private DoorController door;
    [SerializeField] private bool openOnlyOnce = true;

    private bool hasOpened = false;

    private void OnEnable()
    {
        if (tasks == null) return;
        foreach (var t in tasks)
        {
            if (t == null) continue;
            t.onValueChanged.AddListener(OnAnyTaskToggled);
        }
        Evaluate();
    }

    private void OnDisable()
    {
        if (tasks == null) return;
        foreach (var t in tasks)
        {
            if (t == null) continue;
            t.onValueChanged.RemoveListener(OnAnyTaskToggled);
        }
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