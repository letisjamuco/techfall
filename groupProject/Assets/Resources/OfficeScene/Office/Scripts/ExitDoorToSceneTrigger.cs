using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorToSceneTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string targetSceneName = "FarmScene";

    [Header("Optional Fade")]
    public ScreenFader fader;
    public float fadeOutSeconds = 1.0f;


    public void OnInteractExit()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (fader != null)
            yield return fader.FadeOut(fadeOutSeconds);

        SceneManager.LoadScene(targetSceneName);
    }
}
