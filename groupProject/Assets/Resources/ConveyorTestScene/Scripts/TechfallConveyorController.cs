using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechfallConveyorController : MonoBehaviour
{
    [Header("Prefabs to spawn (drag up to 20 here)")]
    public List<GameObject> spawnPrefabs = new List<GameObject>();

    public enum SpawnMode { InOrder, ShuffleNoRepeat, Random }
    public SpawnMode spawnMode = SpawnMode.ShuffleNoRepeat;

    [Header("Spawn timing")]
    [Tooltip("How often a new item appears, regardless of what happened to the previous one.")]
    public float spawnIntervalSeconds = 20f;

    [Tooltip("Maximum items allowed on the belt at the same time.")]
    public int maxActiveOnBelt = 1;

    [Tooltip("Stop spawning after this many total spawns. Set 0 to mean 'no limit'.")]
    public int maxTotalSpawns = 20;

    [Header("Path (visible belt path)")]
    [Tooltip("Waypoints the item will follow (WP_0 ... WP_last). Use 8-15 for smooth curve.")]
    public Transform[] pathWaypoints;

    [Tooltip("Time to travel from first to last waypoint.")]
    public float travelTimeSeconds = 20f;

    [Header("Boss reprimands (misses)")]
    public int strikes = 0;

    [Tooltip("On this strike, the game loads the fail scene. Example: 4 means fail on the 4th miss.")]
    public int strikesToFail = 4;

    [Tooltip("Boss voice clips for 1st, 2nd, 3rd miss (calm -> harsh -> toxic).")]
    public AudioClip[] bossReprimandClips;

    [Tooltip("Scene name to load on failure.")]
    public string failSceneName = "PreTechnologyWorldScene";

    [Header("Audio Sources")]
    [Tooltip("Looping conveyor machine sound.")]
    public AudioSource machineLoopAudio;

    [Tooltip("Boss voice-over audio source.")]
    public AudioSource bossVoiceAudio;

    [Header("Optional: Pause belt when an item is grabbed")]
    public bool pauseBeltOnGrab = false;

    float _timer;
    int _orderIndex;
    int _totalSpawned;
    readonly HashSet<GameObject> _active = new();
    readonly List<int> _bag = new();

    bool _beltPaused = false;

    void Update()
    {
        if (_beltPaused) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnIntervalSeconds)
        {
            _timer = 0f;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        CleanupNulls();

        if (spawnPrefabs == null || spawnPrefabs.Count == 0) return;
        if (pathWaypoints == null || pathWaypoints.Length < 2) return;

        if (maxTotalSpawns > 0 && _totalSpawned >= maxTotalSpawns) return;
        if (_active.Count >= maxActiveOnBelt) return;

        GameObject prefab = PickPrefab();
        if (!prefab) return;

        var go = Instantiate(prefab, pathWaypoints[0].position, pathWaypoints[0].rotation);
        _totalSpawned++;

        var mover = go.GetComponent<BeltPathMover>();
        if (!mover) mover = go.AddComponent<BeltPathMover>();
        mover.Init(pathWaypoints, travelTimeSeconds, this);

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
                return spawnPrefabs[Random.Range(0, spawnPrefabs.Count)];

            case SpawnMode.ShuffleNoRepeat:
            default:
                if (_bag.Count == 0)
                {
                    for (int i = 0; i < spawnPrefabs.Count; i++) _bag.Add(i);
                    for (int i = 0; i < _bag.Count; i++)
                    {
                        int j = Random.Range(i, _bag.Count);
                        (_bag[i], _bag[j]) = (_bag[j], _bag[i]);
                    }
                }
                int idx = _bag[0];
                _bag.RemoveAt(0);
                return spawnPrefabs[idx];
        }
    }

    void CleanupNulls()
    {
        _active.RemoveWhere(x => x == null);
    }

    // Called when an item reaches the "wall eater" trigger without being grabbed
    public void RegisterMiss(GameObject obj)
    {
        if (obj != null) _active.Remove(obj);

        strikes++;

        // Play boss clip for strike 1..3 (if provided)
        if (bossVoiceAudio && bossReprimandClips != null && bossReprimandClips.Length > 0)
        {
            int clipIndex = Mathf.Clamp(strikes - 1, 0, bossReprimandClips.Length - 1);
            if (clipIndex < bossReprimandClips.Length && bossReprimandClips[clipIndex])
                bossVoiceAudio.PlayOneShot(bossReprimandClips[clipIndex]);
        }

        Debug.Log($"MISS -> strikes {strikes}/{strikesToFail}");

        if (strikes >= strikesToFail)
        {
            SceneManager.LoadScene(failSceneName);
        }
    }

    // Called by grab system when the player grabs the item
    public void RegisterGrabbed(GameObject obj)
    {
        if (obj != null) _active.Remove(obj);
        if (pauseBeltOnGrab) _beltPaused = true;
    }

    public void SetBeltPaused(bool paused) => _beltPaused = paused;
}
