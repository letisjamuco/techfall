using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRSimpleInteractable))]
public class DoorHandleXRI : MonoBehaviour
{
    [Header("Scene")]
    public string targetSceneName = "FarmScene";

    [Header("Optional Fade")]
    public ScreenFader fader;
    public float fadeOutSeconds = 1.0f;

    [Header("Behavior")]
    public bool oneShot = true;
    public bool enabledInteraction = true;

    [Header("Optional FX")]
    public GameObject sparkleToEnable; // optional: enable on hover/select

    private bool _used;
    private XRSimpleInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        ApplyEnabledState();
    }

    void OnEnable()
    {
        if (!_interactable) _interactable = GetComponent<XRSimpleInteractable>();
        ApplyEnabledState();

        if (_interactable)
        {
            _interactable.selectEntered.AddListener(OnSelectEntered);
            _interactable.hoverEntered.AddListener(OnHoverEntered);
            _interactable.hoverExited.AddListener(OnHoverExited);
        }
    }

    void OnDisable()
    {
        if (_interactable)
        {
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
            _interactable.hoverEntered.RemoveListener(OnHoverEntered);
            _interactable.hoverExited.RemoveListener(OnHoverExited);
        }
    }

    void ApplyEnabledState()
    {
        if (_interactable) _interactable.enabled = enabledInteraction;
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (sparkleToEnable) sparkleToEnable.SetActive(true);
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (sparkleToEnable) sparkleToEnable.SetActive(false);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!enabledInteraction) return;
        if (oneShot && _used) return;

        _used = true;
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (fader != null)
            yield return fader.FadeOut(fadeOutSeconds);

        SceneManager.LoadScene(targetSceneName);
    }
}
