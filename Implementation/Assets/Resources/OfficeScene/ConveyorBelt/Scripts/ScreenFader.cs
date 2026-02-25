using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class ScreenFader : MonoBehaviour
{
    [Tooltip("CanvasGroup controlling overlay alpha (0 = transparent, 1 = black).")]
    public CanvasGroup canvasGroup;

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (canvasGroup)
            canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut(float seconds)
    {
        if (!canvasGroup) yield break;

        seconds = Mathf.Max(0.01f, seconds);

        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / seconds);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn(float seconds)
    {
        if (!canvasGroup) yield break;

        seconds = Mathf.Max(0.01f, seconds);

        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / seconds);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
