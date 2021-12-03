using System;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] GameObject ui;

	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	//

	[SerializeField]
	private GameObject plantObject;

	//

	[SerializeField]
	private Camera mainCamera;

	private Camera farmerCamera;

	[SerializeField]
	private GameObject confirmPanel;

	[SerializeField]
	private Text confirmPanelText;

	[SerializeField]
	private GameObject alertPanel;

	[SerializeField]
	private Text alertPanelText;

	[SerializeField]
	private GameObject profileImage;

	[SerializeField]
	private GameObject basicButtonList;

	//

	[SerializeField]
	private GameObject chargePanel;

	[SerializeField]
	private Text currentMoney1;

	[SerializeField]
	private InputField moneyInputField;

	[SerializeField]
	private GameObject bestFarmListPanel;

	[SerializeField]
	private GameObject liveStreamingPanel;

	[SerializeField]
	private GameObject particularFarmInfo;

	[SerializeField]
	private GameObject buttonToShowPlant;

	[SerializeField]
	private GameObject plantInfoPanel;

	[SerializeField]
	private Text tempText;

	[SerializeField]
	private Text humiText;

	[SerializeField]
	private GameObject streamingPanel;

	[SerializeField]
	private GameObject streamingController;

	[SerializeField]
	private GameObject contractPaper;

	[SerializeField]
	private GameObject blurScreen;

	[SerializeField]
	private Toggle agreeCheckbox;

	[SerializeField]
	private GameObject payingPanel;

	[SerializeField]
	private Text currentMoney2;

	[SerializeField]
	private Text moneyAfterPaying;

	//

	[SerializeField]
	private GameObject NHBankController;

	[SerializeField]
	private WeatherDataGetter weatherDataGetter;

	//

	private Vector3 shopPosition;
	private Vector3 homePosition;
	private Vector3 grassPosition;
	private OperationType operationType;

	private string username;
	private float plantY = -1.85f;
	private bool showingMouse;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();

		showingMouse = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}

	void Start()
	{
		if(PV.IsMine)
		{
			//EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(ui);
		}
	}

	void Update()
	{
		if(!PV.IsMine)
			return;

		Look();
		Move();
		Jump();
		Mouse();
		SetCurrentBalance();
	}

	void FixedUpdate()
	{
		if (!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	private void SetCurrentBalance()
    {
		if (chargePanel.activeSelf)
		{
			currentMoney1.text = ToMoneyFormat(NHBankController.GetComponent<InquireBalance>().GetBalance());
		}
		else if (payingPanel.activeSelf)
		{
			string balance = NHBankController.GetComponent<InquireBalance>().GetBalance();
			string moneyAfterPay = (Int32.Parse(balance) - 100000).ToString();

			currentMoney2.text = ToMoneyFormat(balance);
			moneyAfterPaying.text = ToMoneyFormat(moneyAfterPay);

			if (moneyAfterPaying.text.Substring(0, 1) == "-")
				moneyAfterPaying.color = HexToColor("F96A6AFF");
			else
				moneyAfterPaying.color = Color.white;
		}
	}

	private string ToMoneyFormat(string number)
    {
		string result = "";
		int count = 0;

		for (int i = number.Length - 1; i >= 0; i--)
		{
			result = number.Substring(i, 1) + result;
			count++;

			if (i > 0 && count >= 3)
			{
				result = "," + result;
				count = 0;
			}
		}

		return result;
	}

	public Color HexToColor(string hex)
	{

		hex = hex.Replace("0x", "");
		hex = hex.Replace("#", "");

		byte a = 255;
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

		if (hex.Length == 8)
		{
			a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		}

		return new Color32(r, g, b, a);
	}

	private void Mouse()
    {
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (showingMouse)
			{
				showingMouse = false;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				showingMouse = true;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}

	void Look()
	{
		if (!showingMouse && mainCamera.enabled)
		{
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

			cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}
	}

	void Move()
	{
        if (!showingMouse && mainCamera.enabled)
        {
			Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
			moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
		}
		else
        {
			moveAmount = new Vector3(0, 0, 0);
		}
	}

	void Jump()
	{
		if(!showingMouse && mainCamera.enabled && Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	public void TakeDamage(float damage)
	{
		//PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
	}

	void OpenAlertPanel(string message)
	{
		alertPanelText.text = message;
		alertPanel.SetActive(true);
	}

	public void CloseAlertPanel()
    {
		alertPanel.SetActive(false);

		if (operationType == OperationType.BuyPlant)
		{
			blurScreen.SetActive(false);
			particularFarmInfo.SetActive(true);
		}
	}

	public void CloseConfirmPanel()
	{
		confirmPanel.SetActive(false);

		if(operationType == OperationType.Exit)
			blurScreen.SetActive(false);
	}

	void OpenConfirmPanel(string message, OperationType opType)
	{
		operationType = opType;
		confirmPanelText.text = message;
		blurScreen.SetActive(true);
		confirmPanel.SetActive(true);
	}

	public void ExecuteConfirmPanel()
    {
		CloseConfirmPanel();

		string message = "";

		if (operationType == OperationType.Exit)
        {
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}
		else if(operationType == OperationType.ChargeMoney)
        {
			NHBankController.GetComponent<ChargeMoney>().Do(moneyInputField.text);
			moneyInputField.text = "0";
			message = "정상적으로 충전되었습니다!";
		}
		else if (operationType == OperationType.BuyPlant)
		{
			NHBankController.GetComponent<TransferOther>().Do("심지훈", "농부", "100000");
			PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", plantObject.name), new Vector3(grassPosition.x, plantY, grassPosition.z), plantObject.transform.rotation, 0, new object[] { PV.ViewID });
			payingPanel.SetActive(false);
			message = "정상적으로 구매하였습니다!";
		}
		else if (operationType == OperationType.GiveWater)
		{
			message = "정상적으로 주었습니다!";
		}

		if (operationType != OperationType.Exit)
			OpenAlertPanel(message);
	}

	public void ShowPlantInfo()
	{
		buttonToShowPlant.SetActive(false);
		StartStreaming();

		weatherDataGetter.InitializeValues();
		weatherDataGetter.Do();

		int index;

		index = weatherDataGetter.GetTemp().IndexOf(".");
		tempText.text = weatherDataGetter.GetTemp().Substring(0,index) + "°";

		index = weatherDataGetter.GetHumidity().IndexOf(".");
		humiText.text = weatherDataGetter.GetHumidity().Substring(0, index) + "%";

		plantInfoPanel.SetActive(true);
	}

	public void StartStreaming()
    {
		streamingController.SetActive(true);
		streamingController.GetComponent<StreamingController>().Reset();
		streamingPanel.SetActive(true);
	}

	public void StopStreaming()
	{
		streamingPanel.SetActive(false);
		streamingController.SetActive(false);
	}

	public void ShowContractPaper()
	{
		particularFarmInfo.SetActive(false);
		agreeCheckbox.isOn = false;

		blurScreen.SetActive(true);
		contractPaper.SetActive(true);
	}

	public void CloseContractPaper()
	{
		contractPaper.SetActive(false);
		blurScreen.SetActive(false);
	}

	public void ClosePayingPanel()
	{
		payingPanel.SetActive(false);
		blurScreen.SetActive(false);
	}

	public void AgreeToBuyPlant()
    {
		if (agreeCheckbox.isOn)
		{
			NHBankController.GetComponent<InquireBalance>().Do();
			contractPaper.SetActive(false);
			payingPanel.SetActive(true);
		}
        else
        {
			OpenAlertPanel("동의란에 체크해 주세요.");
        }
	}

	public void PayingNow()
	{
		if (Int32.Parse(NHBankController.GetComponent<InquireBalance>().GetBalance()) < 100000)
			OpenAlertPanel("가상 머니가 부족합니다.");
		else
			OpenConfirmPanel("구매하시겠습니까?", OperationType.BuyPlant);
	}

	public void ClosePlantInfoPanel()
	{
		plantInfoPanel.SetActive(false);
	}

	public void AddMoney(int money)
    {
		moneyInputField.text = (Int32.Parse(moneyInputField.text) + money).ToString();
	}

	public void ChargeMoney()
	{
		if (!string.IsNullOrEmpty(moneyInputField.text))
		{
			OpenConfirmPanel("충전하시겠습니까?", OperationType.ChargeMoney);
		}
	}

	public void GiveWater()
	{
		OpenConfirmPanel("물을 주시겠습니까?", OperationType.GiveWater);
	}

	public void ShowFarmList()
	{
		bestFarmListPanel.SetActive(true);
	}

	public void CloseBestFarmList()
	{
		bestFarmListPanel.SetActive(false);
	}

	public void CloseChargePanel()
	{
		chargePanel.SetActive(false);

		if(!payingPanel.activeSelf)
			blurScreen.SetActive(false);
	}

	public void ChargeButtonClicked()
	{
		NHBankController.GetComponent<InquireBalance>().Do();
		chargePanel.SetActive(true);
		blurScreen.SetActive(true);
		moneyInputField.text = "0";
	}

	public void ShowLiveWindowButtonClicked()
	{
		bestFarmListPanel.SetActive(false);
		liveStreamingPanel.SetActive(true);
	}

	public void CloseLiveWindow()
	{
		liveStreamingPanel.SetActive(false);
	}

	public void ParticularShopButtonClicked()
	{
		bestFarmListPanel.SetActive(false);

		showingMouse = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		transform.position = new Vector3(shopPosition.x, transform.position.y, shopPosition.z);
	}

	public void GoHomeButtonClicked()
	{
		transform.position = new Vector3(homePosition.x, transform.position.y, homePosition.z);
	}

	public void ParticularProductClicked()
	{
		basicButtonList.SetActive(false);
		profileImage.SetActive(false);
		liveStreamingPanel.SetActive(false);
		particularFarmInfo.SetActive(true);

		ToSubCamera();
	}

	public void CloseParticularProduct()
	{
		particularFarmInfo.SetActive(false);
		basicButtonList.SetActive(true);
		profileImage.SetActive(true);
		
		ToMainCamera();
	}

	private void ToSubCamera()
    {
		farmerCamera.enabled = true;
		mainCamera.enabled = false;
	}

	private void ToMainCamera()
	{
		mainCamera.enabled = true;
		farmerCamera.enabled = false;
	}

	public void ExitButtonClicked()
	{
		OpenConfirmPanel("종료하시겠습니까?", OperationType.Exit);
	}

	//

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Event")
		{
			if(other.gameObject.GetComponent<EventController>().GetEventType() == EventType.Shop)
            {
				buttonToShowPlant.SetActive(true);
            }
			else if (other.gameObject.GetComponent<EventController>().GetEventType() == EventType.Plant)
			{
				plantInfoPanel.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Event")
		{
			if (other.gameObject.GetComponent<EventController>().GetEventType() == EventType.Shop)
			{
				buttonToShowPlant.SetActive(false);
				contractPaper.SetActive(false);
			}

			plantInfoPanel.SetActive(false);
			confirmPanel.SetActive(false);
			alertPanel.SetActive(false);
		}
	}

	//

	public void SetUsername(string value)
    {
		username = value;
    }

	public void SetSubCamera(Camera value)
    {
		farmerCamera = value;
	}

	public void SetHomePosition(Vector3 value)
    {
		homePosition = value;
	}

	public void SetGrassPosition(Vector3 value)
	{
		grassPosition = value;
	}

	public void SetShopPosition(Vector3 value)
	{
		shopPosition = value;
	}
}