using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
	[SerializeField] GameObject graphics;

	[SerializeField]
	private Camera subCamera;

	[SerializeField]
	private Transform grassPosition;

	[SerializeField]
	private Transform homePosition;

	[SerializeField]
	private Transform shopPosition;

	void Awake()
	{
		graphics.SetActive(false);
	}

	public Camera GetSubCamera()
	{
		return subCamera;
	}

	public Vector3 GetGrassPosition()
	{
		return grassPosition.position;
	}

	public Vector3 GetHomePosition()
    {
		return homePosition.position;
	}

	public Vector3 GetShopPosition()
	{
		return shopPosition.position;
	}
}
