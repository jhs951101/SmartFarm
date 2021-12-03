using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InquireBalance : MonoBehaviour
{
    class InquireBalanceClass
    {
        public HEADER Header;
        public string FinAcno;
        public string Ldbl;
        public string RlpmAbamt;
    }

    private string balance;

    void Start()
    {
        balance = "0";
    }

    public void Do()
    {
        HEADER header = new HEADER
        {
            ApiNm = "InquireBalance",
            Tsymd = DateTime.Now.ToString("yyyyMMdd"),
            Trtm = "090000",
            Iscd = "001283",
            FintechApsno = "001",
            ApiSvcCd = "ReceivedTransferA",
            IsTuno = HEADER.GetRandom6Digits(),
            AccessToken = "854d5892501d480dccd7380e88fa3f32de0549694274671546bf7e378e4e7eef",
        };

        InquireBalanceClass inquireBalanceRequest = new InquireBalanceClass
        {
            Header = header,
            FinAcno = "00820100012830000000000012976",
        };

        string json = JsonUtility.ToJson(inquireBalanceRequest);
        StartCoroutine(Upload("https://developers.nonghyup.com/InquireBalance.nh", json));
    }

    IEnumerator Upload(string URL, string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                InquireBalanceClass response = JsonUtility.FromJson<InquireBalanceClass>(request.downloadHandler.text);
                balance = response.RlpmAbamt;
            }
        }
    }

    public string GetBalance()
    {
        return balance;
    }
}
