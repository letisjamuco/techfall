using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechfallConveyorController : MonoBehaviour
{
	[Header("Spawn Prefabs (set from Inspector)")]
	public List<GameObject> prefabs = new List<GameObject>();

	public enum SpawnMode { InOrder, Random, ShuffleBag }
	public SpawnMode spawnMode = SpawnMode.ShuffleBag;

	[Header("Spawn")]
	public Transform spawnPoint;
	public float spawnIntervalSeconds = 20f;
	public int maxActiveOnBelt = 1;

	[Header("Path")]
	public Transform[] pathWaypoints; // set in inspector
	public float travelTimeSeconds = 20f; // time to go from first to last waypoint

	[Header("Miss / Employer")]
	public int strikes = 0;
	public int strikesToFail = 3;
	public AudioSource audioSource;
	public AudioClip[] reprimands; // size 3: 1st,2nd,3rd (or more)
	public string failSceneName = "PreTechnologyWorldScene";

	// internal
	float _t;
	int _orderIndex;
	readonly List<int> _bag = new();
	readonly HashSet<GameObject> _active = new();

	void Awake()
	{
		if (!audioSource) audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		_t += Time.deltaTime;
		if (_t >= spawnIntervalSeconds)
		{
			_t = 0f;
			TrySpawn();
		}
	}

	void TrySpawn()
	{
		if (prefabs == null || prefabs.Count == 0 || spawnPoint == null) return;
		if (pathWaypoints == null || pathWaypoints.Length < 2) return;

		// limit active
		CleanupNulls();
		if (_active.Count >= maxActiveOnBelt) return;

		GameObject prefab = PickPrefab();
		if (!prefab) return;

		GameObject go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

		// add belt mover
		var mover = go.GetComponent<BeltPathMover>();
		if (!mover) mover = go.AddComponent<BeltPathMover>();

		mover.Init(pathWaypoints, travelTimeSeconds, this);

		_active.Add(go);
	}

	void CleanupNulls()
	{
		_active.RemoveWhere(x => x == null);
	}

	GameObject PickPrefab()
	{
		switch (spawnMode)
		{
			case SpawnMode.InOrder:
				var p = prefabs[_orderIndex % prefabs.Count];
				_orderIndex++;
				return p;

			case SpawnMode.Random:
				return prefabs[Random.Range(0, prefabs.Count)];

			case SpawnMode.ShuffleBag:
			default:
				if (_bag.Count == 0)
				{
					for (int i = 0; i < prefabs.Count; i++) _bag.Add(i);
					// shuffle
					for (int i = 0; i < _bag.Count; i++)
					{
						int j = Random.Range(i, _bag.Count);
						(_bag[i], _bag[j]) = (_bag[j], _bag[i]);
					}
				}
				int idx = _bag[0];
				_bag.RemoveAt(0);
				return prefabs[idx];
		}
	}

	public void RegisterMiss(GameObject obj)
	{
		if (obj != null) _active.Remove(obj);

		strikes++;

		// play reprimand
		if (reprimands != null && reprimands.Length > 0 && audioSource)
		{
			int clipIndex = Mathf.Clamp(strikes - 1, 0, reprimands.Length - 1);
			if (reprimands[clipIndex])
				audioSource.PlayOneShot(reprimands[clipIndex]);
		}

		Debug.Log($"MISS: strikes {strikes}/{strikesToFail}");

		if (strikes >= strikesToFail)
		{
			SceneManager.LoadScene(failSceneName);
		}
	}

	public void RegisterGrabbed(GameObject obj)
	{
		// object removed from belt system (player took it)
		if (obj != null) _active.Remove(obj);
	}
}
