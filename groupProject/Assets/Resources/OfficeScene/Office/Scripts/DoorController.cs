using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.8f;

    private bool isOpen = false;

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        if (doorPivot == null) doorPivot = transform;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        Quaternion start = doorPivot.localRotation;
        Quaternion target = start * Quaternion.Euler(0f, openAngle, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            doorPivot.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        doorPivot.localRotation = target;
    }
}