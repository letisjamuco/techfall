using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechfallConveyorController : MonoBehaviour
{
    [Header("Spawn Prefabs (put your 20 items here)")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public enum SpawnMode { InOrder, ShuffleNoRepeat, Random }
    public SpawnMode spawnMode = SpawnMode.ShuffleNoRepeat;

    [Header("Spawn Timing")]
    public float spawnIntervalSeconds = 20f; // time between spawns
    public int maxActiveOnBelt = 1;          // how many items can be visible on belt at once
    public int maxTotalSpawns = 20;          // total items for MVP (0 = infinite)

    [Header("Path")]
    public Transform generatedPathParent; // GeneratedPath (parent of WP_00..)
    public float travelTimeSeconds = 20f; // time to travel from WP_00 to last WP

    [Header("Boss Strikes")]
    public int strikes = 0;
    public int strikesToFail = 4;             // on this strike -> fail sequence
    public AudioClip[] bossReprimandClips;    // size 3: calm, harsh, toxic (strikes 1..3)
    public AudioClip failFinalClip;           // played ONLY on strike 4

    [Header("Audio Sources")]
    public AudioSource machineAudio; // looping conveyor sound
    public AudioSource bossAudio;    // voice over (2D recommended)

    [Header("Fail Transition")]
    public string failSceneName = "FarmScene"; // Will later change on PreTechnologyWorldScene
    public ScreenFader fader;
    public float fadeOutSeconds = 1.5f; // fade duration AFTER final voice ends

    [Header("Option")]
    public bool pauseBeltOnGrab = false;

    // Runtime state
    float _timer;
    int _orderIndex;
    int _totalSpawned;
    readonly HashSet<GameObject> _active = new();
    readonly List<int> _bag = new();
    bool _paused = false;
    bool _running = false; // belt starts OFF; StartBelt() turns it ON
    bool _failing = false; // prevents double fail sequence
    Transform[] _wps;

    void Start()
    {
        CacheWaypoints();

        // IMPORTANT: do NOT auto-start.
        // Belt starts only when StartBelt() is called by the StartButton trigger.
        StopMachineAudio();
    }

    void CacheWaypoints()
    {
        if (!generatedPathParent) return;

        int n = generatedPathParent.childCount;
        _wps = new Transform[n];
        for (int i = 0; i < n; i++) _wps[i] = generatedPathParent.GetChild(i);
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

    // Called by StartButton trigger
    public void StartBelt()
    {
        if (_failing) return;

        _running = true;
        _paused = false;

        // Spawn first item immediately
        _timer = 0f;
        TrySpawn();

        StartMachineAudio();
    }

    // Optional: if you want a Stop button later
    public void StopBelt()
    {
        _running = false;
        PauseBelt(true);
        StopMachineAudio();
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

        if (_wps == null || _wps.Length < 2) CacheWaypoints();
        if (_wps == null || _wps.Length < 2) return;
        if (spawnPrefabs == null || spawnPrefabs.Count == 0) return;

        if (maxTotalSpawns > 0 && _totalSpawned >= maxTotalSpawns) return;
        if (_active.Count >= maxActiveOnBelt) return;

        GameObject prefab = PickPrefab();
        if (!prefab) return;

        GameObject go = Instantiate(prefab, _wps[0].position, _wps[0].rotation);
        _totalSpawned++;

        var mover = go.GetComponent<BeltPathMover>();
        if (!mover) mover = go.AddComponent<BeltPathMover>();
        mover.Init(_wps, travelTimeSeconds, this);

        _active.Add(go);
    }

    GameObject PickPrefab()
    {
        switch (spawnMode)
        {
            case SpawnMode.InOrder:
                var p = spawnPrefabs[_orderIndex % spawnPrefabs.Count];
                _orderIndex++;
                return p;

            case SpawnMode.Random:
                return spawnPrefabs[UnityEngine.Random.Range(0, spawnPrefabs.Count)];

            case SpawnMode.ShuffleNoRepeat:
            default:
                if (_bag.Count == 0)
                {
                    for (int i = 0; i < spawnPrefabs.Count; i++) _bag.Add(i);

                    // Fisher-Yates shuffle
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

    // Called by WallEaterTrigger when an ungrabbed object "disappears" behind the wall
    public void RegisterMiss(GameObject obj)
    {
        if (_failing) return;

        if (obj != null) _active.Remove(obj);
        strikes++;

        // Play reprimands only for strikes 1..(strikesToFail-1)
        if (strikes < strikesToFail)
        {
            if (bossAudio && bossReprimandClips != null && bossReprimandClips.Length > 0)
            {
                int clipIndex = Mathf.Clamp(strikes - 1, 0, bossReprimandClips.Length - 1);
                var clip = bossReprimandClips[clipIndex];
                if (clip) bossAudio.PlayOneShot(clip);
            }
        }

        // On strike 4 -> fail sequence
        if (strikes >= strikesToFail)
            StartCoroutine(FailSequence());
    }

    // Called by grab system
    public void RegisterGrabbed(GameObject obj)
    {
        if (obj != null) _active.Remove(obj);
        if (pauseBeltOnGrab) PauseBelt(true);
    }

    public void PauseBelt(bool pause)
    {
        _paused = pause;

        foreach (var go in _active)
        {
            if (!go) continue;
            var mover = go.GetComponent<BeltPathMover>();
            if (mover) mover.paused = pause;
        }

        if (machineAudio)
        {
            if (pause) machineAudio.Pause();
            else machineAudio.UnPause();
        }
    }

    IEnumerator FailSequence()
    {
        _failing = true;

        // Stop belt & machine sound
        PauseBelt(true);

        // Stop any current boss audio so it won't overlap with final clip
        if (bossAudio) bossAudio.Stop();

        // Play final clip (full length), then fade, then load scene
        if (failFinalClip && bossAudio)
        {
            bossAudio.PlayOneShot(failFinalClip);
            yield return new WaitForSeconds(failFinalClip.length);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (fader) yield return fader.FadeOut(fadeOutSeconds);

        // IMPORTANT: make sure the target scene is added to Build Settings
        SceneManager.LoadScene(failSceneName);
    }
}
