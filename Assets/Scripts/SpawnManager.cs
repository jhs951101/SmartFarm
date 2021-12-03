using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;

	Spawnpoint[] spawnpoints;

	void Awake()
	{
		Instance = this;
		spawnpoints = GetComponentsInChildren<Spawnpoint>();
	}

	public Spawnpoint GetSpawnpoint()
	{
		return spawnpoints[0];
	}
}
