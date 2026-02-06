using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    // CanvasGroup controls UI alpha (0 = visible scene, 1 = black)
    public CanvasGroup canvasGroup;

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (canvasGroup) canvasGroup.alpha = 0f; // start transparent
    }

    public IEnumerator FadeOut(float seconds)
    {
        if (!canvasGroup) yield break;

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
