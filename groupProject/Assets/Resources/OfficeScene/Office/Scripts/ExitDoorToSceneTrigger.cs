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

    [Header("Who can trigger")]
    public string playerTag = "Player";
    public LayerMask playerLayers = ~0;
    public bool useTagCheck = true;

    [Header("Safety")]
    public bool oneShot = true;

    bool _triggered;

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && _triggered) return;

        if (useTagCheck)
        {
            if (!other.CompareTag(playerTag)) return;
        }
        else
        {
            if (((1 << other.gameObject.layer) & playerLayers) == 0) return;
        }

        _triggered = true;
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (fader != null)
            yield return fader.FadeOut(fadeOutSeconds);

        SceneManager.LoadScene(targetSceneName);
    }
}
