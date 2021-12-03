using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;
	public static string username;

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] InputField usernameInputField;
	[SerializeField] InputField passwordInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;

	[SerializeField]
	private GameObject alertPanel;

	[SerializeField]
	private Text alertPanelText;

	[SerializeField]
	private GameObject confirmPanel;

	[SerializeField]
	private Text confirmPanelText;

	private int roomCount;

	/*
	public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static FirebaseUser User;
	*/

	void Awake()
	{
		Instance = this;
		roomCount = 0;
	}

	void Start()
	{
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();

		/*
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            {
                var result = task.Result;

                if (result != DependencyStatus.Available)
                {
                    Debug.LogError(result.ToString());
                    IsFirebaseReady = false;
                }
                else
                {
                    IsFirebaseReady = true;

                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;
                }

                signInButton.interactable = IsFirebaseReady;
            }
        }
        );
		*/
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
	}

	public void CreateRoom()
	{
		PhotonNetwork.CreateRoom("basic");
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRandom()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinedRoom()
	{
		Launcher.username = usernameInputField.text;
		StartGame();
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}

	public void StartGame()
	{
		PhotonNetwork.LoadLevel(1);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRoom(string roomname)
	{
		PhotonNetwork.JoinRoom(roomname);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("title");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		roomCount = roomList.Count;

		foreach(Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}

		for(int i = 0; i < roomList.Count; i++)
		{
			if(roomList[i].RemovedFromList)
				continue;
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}

    public void LoginButtonClicked()
    {
		if (string.IsNullOrEmpty(usernameInputField.text))
		{
			alertPanelText.text = "아이디를 입력해주세요.";
			alertPanel.SetActive(true);
		}
		else if (string.IsNullOrEmpty(passwordInputField.text))
		{
			alertPanelText.text = "비밀번호를 입력해주세요.";
			alertPanel.SetActive(true);
		}
        else
        {
			/*
			if (IsFirebaseReady && !IsSignInOnProgress && User == null)
			{
				IsSignInOnProgress = true;
				signInButton.interactable = false;

				firebaseAuth.SignInWithEmailAndPasswordAsync(usernameField.text, passwordField.text).ContinueWithOnMainThread(task =>
				{
					//Debug.Log($"Sign in status : {task.Status}");

					IsSignInOnProgress = false;
					signInButton.interactable = true;

					if (task.IsFaulted)
					{
						Debug.LogError(task.Exception);
					}
					else if (task.IsCanceled)
					{
						Debug.LogError("Sign-in canceled");
					}
					else
					{
						User = task.Result;
					}
				}
				);
			}
			*/

			if (roomCount <= 0)
			{
				CreateRoom();
			}
			else
			{
				JoinRoom("basic");
			}
		}
    }

	public void SignUpButtonClicked()
	{
	}

	public void CloseAlertPanel()
    {
		alertPanel.SetActive(false);
	}
}