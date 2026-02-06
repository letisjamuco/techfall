using System.Collections;
using UnityEngine;

public class FadeInOnStart : MonoBehaviour
{
    public ScreenFader fader;
    public float fadeInSeconds = 1.5f;

    IEnumerator Start()
    {
        if (!fader) fader = GetComponent<ScreenFader>();
        if (!fader || !fader.canvasGroup) yield break;

        // start black
        fader.canvasGroup.alpha = 1f;

        // fade to clear
        yield return fader.FadeIn(fadeInSeconds);
    }
}
