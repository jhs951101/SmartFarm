using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
	PhotonView PV;

	GameObject controller;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if(PV.IsMine)
		{
			CreateController();
		}
	}

	void CreateController()
	{
		Spawnpoint spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.transform.position, spawnpoint.transform.rotation, 0, new object[] { PV.ViewID });
		controller.GetComponent<PlayerController>().SetUsername(Launcher.username);
		controller.GetComponent<PlayerController>().SetSubCamera(spawnpoint.GetSubCamera());
		controller.GetComponent<PlayerController>().SetGrassPosition(spawnpoint.GetGrassPosition());
		controller.GetComponent<PlayerController>().SetHomePosition(spawnpoint.GetHomePosition());
		controller.GetComponent<PlayerController>().SetShopPosition(spawnpoint.GetShopPosition());
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}
}