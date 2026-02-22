using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform hinge;
    [SerializeField] private float openAngle = 80f;
    [SerializeField] private float duration = 0.6f;

    private bool isOpen;

    private Quaternion closedLocalRot;
    private Quaternion openLocalRot;

    private void Awake()
    {
        if (hinge == null) hinge = transform;

        closedLocalRot = hinge.localRotation;
        openLocalRot = closedLocalRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StopAllCoroutines();
        StartCoroutine(RotateTo(openLocalRot));
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        StopAllCoroutines();
        StartCoroutine(RotateTo(closedLocalRot));
    }

    private IEnumerator RotateTo(Quaternion target)
    {
        Quaternion start = hinge.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            hinge.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        hinge.localRotation = target;
    }
}