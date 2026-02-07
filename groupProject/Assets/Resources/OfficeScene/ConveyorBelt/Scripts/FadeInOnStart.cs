using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class FadeInOnStart : MonoBehaviour
{
    [Tooltip("Screen fader used to fade from black at scene start.")]
    public ScreenFader fader;

    [Tooltip("Fade-in duration in seconds.")]
    public float fadeInSeconds = 1.5f;

    IEnumerator Start()
    {
        if (!fader) fader = GetComponent<ScreenFader>();
        if (!fader || !fader.canvasGroup) yield break;

        // Start fully black, then fade to transparent.
        fader.canvasGroup.alpha = 1f;
        yield return fader.FadeIn(fadeInSeconds);
    }
}
