using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[DisallowMultipleComponent]
public class TechfallConveyorController : MonoBehaviour
{
    [Header("Spawn Prefabs")]
    [Tooltip("Prefabs spawned onto the conveyor belt.")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public enum SpawnMode { InOrder, ShuffleNoRepeat, Random }
    [Tooltip("ShuffleNoRepeat: each prefab appears once per cycle, then the cycle repeats.")]
    public SpawnMode spawnMode = SpawnMode.ShuffleNoRepeat;

    [Header("Spawn Timing")]
    [Tooltip("Time between spawns in seconds.")]
    [Min(0.01f)]
    public float spawnIntervalSeconds = 20f;

    [Tooltip("Maximum number of items allowed on the belt at once. Set to 1 for strict one-at-a-time behavior.")]
    [Min(1)]
    public int maxActiveOnBelt = 1;

    [Tooltip("Total number of items to spawn. Set to 0 for infinite.")]
    [Min(0)]
    public int maxTotalSpawns = 0;

    [Header("Cinemachine Path (required)")]
    [Tooltip("Cinemachine path followed by spawned items.")]
    public CinemachinePathBase cinemachinePath;

    [Tooltip("Time for an item to travel from start (t=0) to end (t=1).")]
    [Min(0.1f)]
    public float travelTimeSeconds = 20f;

    [Header("Boss Strikes")]
    [Tooltip("Current strike count (runtime).")]
    public int strikes = 0;

    [Tooltip("On this strike count, the fail sequence is executed.")]
    [Min(1)]
    public int strikesToFail = 4;

    [Tooltip("Reprimand clips for strikes 1..(strikesToFail-1).")]
    public AudioClip[] bossReprimandClips;

    [Tooltip("Final boss clip played on strike strikesToFail.")]
    public AudioClip failFinalClip;

    [Header("Audio Sources")]
    [Tooltip("Looping conveyor/machine audio.")]
    public AudioSource machineAudio;

    [Tooltip("Boss voice audio source (2D recommended).")]
    public AudioSource bossAudio;

    [Header("Final Alarm SFX (optional)")]
    public AudioSource sfxAudio;
    public AudioClip finalAlarmClip;

    [Header("Camera Shake (optional)")]
    public Transform shakeTarget;
    public float shakeDuration = 0.12f;
    public float shakeMagnitude = 0.02f;

    [Header("Fail Transition")]
    [Tooltip("Scene loaded on failure (strike strikesToFail). Must be in Build Settings.")]
    public string failSceneName = "PreTechnologyWorldScene";

    public ScreenFader fader;
    public float fadeOutSeconds = 1.5f;

    [Header("Pause On Grab")]
    [Tooltip("If true, spawning and machine audio pause briefly when an item is grabbed (movement is handled by the grabbed item).")]
    public bool pauseBeltOnGrab = false;

    [Min(0f)]
    public float pauseOnGrabSeconds = 0.6f;

    [Header("Debug / Testing")]
    [Tooltip("If true, starts automatically without pressing the start button.")]
    public bool autoStartWithoutButton = false;

    [Min(0f)]
    public float autoStartDelaySeconds = 0f;

    Coroutine _resumeCo;
    Coroutine _shakeCo;

    float _timer;
    int _orderIndex;
    int _totalSpawned;

    readonly HashSet<GameObject> _active = new HashSet<GameObject>();
    readonly List<int> _bag = new List<int>();

    bool _paused;
    bool _running;
    bool _failing;

    void Start()
    {
        StopMachineAudio();

        if (autoStartWithoutButton)
            StartCoroutine(AutoStartRoutine());
    }

    IEnumerator AutoStartRoutine()
    {
        yield return null;

        if (autoStartDelaySeconds > 0f)
            yield return new WaitForSeconds(autoStartDelaySeconds);

        StartBelt();
    }

    void Update()
    {
        if (!_running || _paused || _failing) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnIntervalSeconds)
        {
            _timer = 0f;
            TrySpawn();
        }
    }

    public void StartBelt()
    {
        if (_failing) return;
        if (!cinemachinePath) return;

        _running = true;
        _paused = false;

        _timer = 0f;
        TrySpawn();

        StartMachineAudio();
    }

    public void StopBelt()
    {
        _running = false;
        PauseBelt(true);
        StopMachineAudio();
    }

    void PauseBelt(bool pause)
    {
        _paused = pause;

        if (!machineAudio) return;

        if (pause)
        {
            if (machineAudio.isPlaying) machineAudio.Pause();
        }
        else
        {
            machineAudio.UnPause();
        }
    }

    void StartMachineAudio()
    {
        if (!machineAudio) return;

        machineAudio.loop = true;
        if (!machineAudio.isPlaying) machineAudio.Play();
        else machineAudio.UnPause();
    }

    void StopMachineAudio()
    {
        if (!machineAudio) return;
        if (machineAudio.isPlaying) machineAudio.Stop();
    }

    void CleanupNulls() => _active.RemoveWhere(x => x == null);

    void TrySpawn()
    {
        CleanupNulls();

        if (!cinemachinePath) return;
        if (spawnPrefabs == null || spawnPrefabs.Count == 0) return;

        // If maxTotalSpawns == 0 -> infinite.
        if (maxTotalSpawns > 0 && _totalSpawned >= maxTotalSpawns) return;

        if (_active.Count >= maxActiveOnBelt) return;

        GameObject prefab = PickPrefab();
        if (!prefab) return;

        Vector3 startPos = cinemachinePath.EvaluatePositionAtUnit(
            0f, CinemachinePathBase.PositionUnits.Normalized);

        Vector3 startDir = cinemachinePath.EvaluateTangentAtUnit(
            0f, CinemachinePathBase.PositionUnits.Normalized);

        Quaternion startRot =
            (startDir.sqrMagnitude > 0.0001f)
                ? Quaternion.LookRotation(startDir.normalized, Vector3.up)
                : Quaternion.identity;

        GameObject go = Instantiate(prefab, startPos, startRot);
        _totalSpawned++;

        var mover = go.GetComponent<BeltPathMover>();
        if (!mover) mover = go.AddComponent<BeltPathMover>();
        mover.Init(cinemachinePath, travelTimeSeconds, this);

        _active.Add(go);
    }

    GameObject PickPrefab()
    {
        switch (spawnMode)
        {
            case SpawnMode.InOrder:
                {
                    var p = spawnPrefabs[_orderIndex % spawnPrefabs.Count];
                    _orderIndex++;
                    return p;
                }

            case SpawnMode.Random:
                return spawnPrefabs[UnityEngine.Random.Range(0, spawnPrefabs.Count)];

            case SpawnMode.ShuffleNoRepeat:
            default:
                {
                    // Refill and reshuffle when the bag is empty.
                    // This is what makes the system repeat after one full cycle (expected behavior).
                    if (_bag.Count == 0)
                    {
                        _bag.Clear();
                        for (int i = 0; i < spawnPrefabs.Count; i++)
                            _bag.Add(i);

                        for (int i = 0; i < _bag.Count; i++)
                        {
                            int j = UnityEngine.Random.Range(i, _bag.Count);
                            (_bag[i], _bag[j]) = (_bag[j], _bag[i]);
                        }
                    }

                    int idx = _bag[0];
                    _bag.RemoveAt(0);
                    return spawnPrefabs[idx];
                }
        }
    }

    public void RegisterMiss(GameObject obj)
    {
        if (_failing) return;

        if (obj != null) _active.Remove(obj);
        strikes++;

        TriggerShake();

        // Strikes 1..(strikesToFail-1): reprimands
        if (strikes < strikesToFail)
        {
            if (bossAudio && bossReprimandClips != null && bossReprimandClips.Length > 0)
            {
                int clipIndex = Mathf.Clamp(strikes - 1, 0, bossReprimandClips.Length - 1);
                var clip = bossReprimandClips[clipIndex];
                if (clip) bossAudio.PlayOneShot(clip);
            }
        }

        // Strike strikesToFail: fail sequence
        if (strikes >= strikesToFail)
            StartCoroutine(FailSequence());
    }

    public void RegisterGrabbed(GameObject obj)
    {
        if (obj != null) _active.Remove(obj);

        if (pauseBeltOnGrab)
        {
            PauseBelt(true);

            if (_resumeCo != null) StopCoroutine(_resumeCo);
            _resumeCo = StartCoroutine(ResumeAfterDelay());
        }
    }

    IEnumerator ResumeAfterDelay()
    {
        yield return new WaitForSeconds(pauseOnGrabSeconds);
        PauseBelt(false);
    }

    void TriggerShake()
    {
        if (!shakeTarget) return;
        if (shakeDuration <= 0f || shakeMagnitude <= 0f) return;

        if (_shakeCo != null) StopCoroutine(_shakeCo);
        _shakeCo = StartCoroutine(ShakeOnce());
    }

    IEnumerator ShakeOnce()
    {
        Vector3 original = shakeTarget.localPosition;
        float t = 0f;

        while (t < shakeDuration)
        {
            float x = (UnityEngine.Random.value * 2f - 1f) * shakeMagnitude;
            float y = (UnityEngine.Random.value * 2f - 1f) * shakeMagnitude;

            shakeTarget.localPosition = original + new Vector3(x, y, 0f);

            t += Time.deltaTime;
            yield return null;
        }

        shakeTarget.localPosition = original;
        _shakeCo = null;
    }

    IEnumerator FailSequence()
    {
        _failing = true;

        PauseBelt(true);

        if (bossAudio) bossAudio.Stop();

        if (failFinalClip && bossAudio)
        {
            bossAudio.PlayOneShot(failFinalClip);
            yield return new WaitForSeconds(failFinalClip.length);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (sfxAudio && finalAlarmClip)
            sfxAudio.PlayOneShot(finalAlarmClip);

        if (fader) yield return fader.FadeOut(fadeOutSeconds);

        SceneManager.LoadScene(failSceneName);
    }
}
