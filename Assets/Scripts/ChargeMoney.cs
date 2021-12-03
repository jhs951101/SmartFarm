using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ChargeMoney : MonoBehaviour
{
    class ChargeMoneyClass
    {
        public HEADER Header;
        public string Bncd;
        public string Acno;
        public string Tram;
        public string DractOtlt;
        public string MractOtlt;
    }

    public void Do(string money)
    {
        HEADER header = new HEADER
        {
            ApiNm = "ReceivedTransferAccountNumber",
            Tsymd = DateTime.Now.ToString("yyyyMMdd"),
            Trtm = "090000",
            Iscd = "001283",
            FintechApsno = "001",
            ApiSvcCd = "ReceivedTransferA",
            IsTuno = HEADER.GetRandom6Digits(),
            AccessToken = "854d5892501d480dccd7380e88fa3f32de0549694274671546bf7e378e4e7eef",
        };

        ChargeMoneyClass chargeMoneyRequest = new ChargeMoneyClass
        {
            Header = header,
            Bncd = "011",
            Acno = "3020000005605",
            Tram = money,
            DractOtlt = "충전",
            MractOtlt = "충전",
        };

        string json = JsonUtility.ToJson(chargeMoneyRequest);
        StartCoroutine(Upload("https://developers.nonghyup.com/ReceivedTransferAccountNumber.nh", json));
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
                GetComponent<InquireBalance>().Do();

                ChargeMoneyClass response = JsonUtility.FromJson<ChargeMoneyClass>(request.downloadHandler.text);
                Debug.Log(response.Header.Rsms);
            }
        }
    }
}
