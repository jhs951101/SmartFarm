using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class TransferOther : MonoBehaviour
{
    //NH API 헤더 공통부에 들어가는 정보
    struct NhParameter
    {
        public string Iscd; //기관코드
        public string AccessToken;  //인증키
        public string FinAcno;  //핀어카운트 발급번호(농협 오픈플랫폼에서 발급명령을 실행하여 발급 후 사용가능
        public string Acno; //모계좌번호
    }

    delegate void WebserverCallback(bool isSuccess, object data);

    Dictionary<string, NhParameter> dicAccountInfo;

    public void Awake()
    {
        dicAccountInfo = new Dictionary<string, NhParameter>();
        SetUsers();
    }

    void SetUsers()
    {
        NhParameter nhParameter;

        nhParameter = new NhParameter
        {
            Iscd = "001129",
            AccessToken = "5de7cce764b1077d1d8ce23d13aea6e9b2edfe022a493b9dffabd25913c81ad7",
            FinAcno = "00820100011290000000000011798",
            Acno = "3020000004983"
        };

        dicAccountInfo.Add("농부", nhParameter);

        //

        nhParameter = new NhParameter
        {
            Iscd = "001283",
            AccessToken = "854d5892501d480dccd7380e88fa3f32de0549694274671546bf7e378e4e7eef",
            FinAcno = "00820100012830000000000012976",
            Acno = "3020000005605"
        };

        dicAccountInfo.Add("심지훈", nhParameter);

        //
    }

    #region 농협API 연동
    /// <summary>
    /// 모계좌에서 자계좌로 출금
    /// </summary>
    /// <returns></returns>
    IEnumerator Co_DrawRealCash(string pNickname, string pCashAmount, WebserverCallback pCallback = null)
    {
        if (dicAccountInfo.ContainsKey(pNickname) == false)
        {
            pCallback?.Invoke(false, "출금 에러 : 계좌정보가 없습니다.");
            yield break;
        }

        NhParameter accountInfo = dicAccountInfo[pNickname];
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("ApiNm", "DrawingTransfer");
        header.Add("Tsymd", DateTime.Now.ToString("yyyyMMdd"));
        header.Add("Trtm", DateTime.Now.ToString("HHmmss"));
        header.Add("Iscd", accountInfo.Iscd);
        header.Add("FintechApsno", "001");
        header.Add("ApiSvcCd", "DrawingTransferA");
        header.Add("IsTuno", DateTime.Now.Ticks.ToString());
        header.Add("AccessToken", accountInfo.AccessToken);
        string jsonHeader = ObjectToJson(header);
        jsonHeader = @"""Header"":" + jsonHeader + ",";
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("FinAcno", accountInfo.FinAcno);
        body.Add("Tram", pCashAmount);
        body.Add("DractOtlt", "test");

        string temp = ObjectToJson(body).Replace("\\", "");
        temp = "{" + jsonHeader + temp.Substring(1, temp.Length - 1);
        byte[] myData = Encoding.UTF8.GetBytes(temp);
        string uri = "https://developers.nonghyup.com/DrawingTransfer.nh";
        var www = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(myData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            pCallback?.Invoke(true, Encoding.UTF8.GetString(www.downloadHandler.data));
        }
        else
        {
            pCallback?.Invoke(false, Encoding.UTF8.GetString(www.downloadHandler.data));
        }
    }

    /// <summary>
    /// 자계좌에서 모계좌로 입금
    /// </summary>
    /// <param name="pNickname">계정명</param>
    /// <param name="pCashAmount">입금할 금액</param>
    /// <param name="pCallback">통신 완료시 호출할 함수</param>
    /// <returns></returns>
    IEnumerator Co_DepositRealCash(string pNickname, string pCashAmount, WebserverCallback pCallback = null)
    {
        if (dicAccountInfo.ContainsKey(pNickname) == false)
        {
            pCallback?.Invoke(false, "입금 에러 : 계좌 정보가 없습니다.");
            yield break;
        }
        NhParameter accountInfo = dicAccountInfo[pNickname];
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("ApiNm", "ReceivedTransferAccountNumber");
        header.Add("Tsymd", DateTime.Now.ToString("yyyyMMdd"));
        header.Add("Trtm", DateTime.Now.ToString("HHmmss"));
        header.Add("Iscd", accountInfo.Iscd);
        header.Add("FintechApsno", "001");
        header.Add("ApiSvcCd", "DrawingTransferA");
        header.Add("IsTuno", DateTime.Now.Ticks.ToString());
        header.Add("AccessToken", accountInfo.AccessToken);
        string jsonHeader = ObjectToJson(header);
        jsonHeader = @"""Header"":" + jsonHeader + ",";
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("Bncd", "011");
        body.Add("Acno", accountInfo.Acno);
        body.Add("Tram", pCashAmount);
        body.Add("DractOtlt", "test");
        body.Add("MractOtlt", "test");

        string temp = ObjectToJson(body).Replace("\\", "");
        temp = "{" + jsonHeader + temp.Substring(1, temp.Length - 1);
        byte[] myData = Encoding.UTF8.GetBytes(temp);
        string uri = "https://developers.nonghyup.com/ReceivedTransferAccountNumber.nh";
        var www = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(myData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            pCallback?.Invoke(true, Encoding.UTF8.GetString(www.downloadHandler.data));
        }
        else
        {
            pCallback?.Invoke(false, Encoding.UTF8.GetString(www.downloadHandler.data));
        }
    }

    /// <summary>
    /// 잔액조회
    /// </summary>
    /// <param name="pNickname">계정명</param>
    /// <param name="pCallback">통신 완료시 콜백함수</param>
    /// <returns></returns>
    IEnumerator Co_InquireBalance(string pNickname, WebserverCallback pCallback)
    {
        if (dicAccountInfo.ContainsKey(pNickname) == false)
        {
            pCallback?.Invoke(false, "잔액조회 에러 : 계좌 정보가 없습니다.");
            yield break;
        }
        NhParameter accountInfo = dicAccountInfo[pNickname];
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("ApiNm", "InquireBalance");
        header.Add("Tsymd", DateTime.Now.ToString("yyyyMMdd"));
        header.Add("Trtm", DateTime.Now.ToString("HHmmss"));
        header.Add("Iscd", accountInfo.Iscd);
        header.Add("FintechApsno", "001");
        header.Add("ApiSvcCd", "DrawingTransferA");
        header.Add("IsTuno", DateTime.Now.Ticks.ToString());
        header.Add("AccessToken", accountInfo.AccessToken);
        string jsonHeader = ObjectToJson(header);
        jsonHeader = @"""Header"":" + jsonHeader + ",";

        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("FinAcno", accountInfo.FinAcno);

        string temp = ObjectToJson(body).Replace("\\", "");
        temp = "{" + jsonHeader + temp.Substring(1, temp.Length - 1);
        byte[] myData = Encoding.UTF8.GetBytes(temp);
        string uri = "https://developers.nonghyup.com/InquireBalance.nh";
        var www = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
        www.uploadHandler = new UploadHandlerRaw(myData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string resultData = Encoding.UTF8.GetString(www.downloadHandler.data);
            string[] result = resultData.Split(',');
            resultData = result[11].Replace(@"""", "").Split(':')[1];
            resultData = resultData.Substring(0, resultData.Length - 1);
            pCallback?.Invoke(true, resultData);
        }
        else
        {
            pCallback?.Invoke(false, Encoding.UTF8.GetString(www.downloadHandler.data));
        }
    }

    #endregion

    /// <summary>
    /// 출금
    /// </summary>
    /// <param name="pNickname"></param>
    /// <param name="pCashAmount"></param>
    /// <param name="pCallback"></param>
    void DrawCash(string pNickname, string pCashAmount, WebserverCallback pCallback = null)
    {
        StartCoroutine(Co_DrawRealCash(pNickname, pCashAmount, (result, data) =>
        {
            //응답이 왔을 경우 실행할 내용
            //string temp = Encoding.Default.GetString((byte[])data);
            pCallback?.Invoke(result, data);
        }));
    }

    /// <summary>
    /// 입금
    /// </summary>
    /// <param name="pNickname"></param>
    /// <param name="pCashAmount"></param>
    /// <param name="pCallback"></param>
    void Deposit(string pNickname, string pCashAmount, WebserverCallback pCallback = null)
    {
        StartCoroutine(Co_DepositRealCash(pNickname, pCashAmount, (result, data) =>
        {
            pCallback?.Invoke(result, data);
        }));
    }

    /// <summary>
    /// 잔액조회
    /// </summary>
    /// <param name="pNickname"></param>
    /// <param name="pCallback"></param>
    void InquireBalance(string pNickname, WebserverCallback pCallback = null)
    {
        StartCoroutine(Co_InquireBalance(pNickname, (result, data) =>
        {
            pCallback?.Invoke(result, data);
        }));
    }

    /// <summary>
    /// 이체
    /// </summary>
    /// <param name="pFromUser">출금 될 유저</param>
    /// <param name="pDestAcNo"></param>
    /// <param name="pCashAmount">이체 할 금액</param>
    void Transfer(string pFromUser, string pDestAcNo, string pCashAmount, WebserverCallback pCallback = null)
    {
        if (dicAccountInfo.ContainsKey(pFromUser) == false)
        {
            Debug.Log("계좌이체 : 보내는 사람의 계좌정보가 없습니다.");
            return;
        }
        string destUser = FindNameWithAcno(pDestAcNo);
        if (dicAccountInfo.ContainsKey(destUser) == false)
        {
            Debug.Log("계좌이체 : 받는 사람의 계좌정보가 없습니다.");
            return;
        }

        DrawCash(pFromUser, pCashAmount, (result, data) =>
        {
            if (result == true)
            {
                Deposit(destUser, pCashAmount, (result2, data2) =>
                {
                    if (result2 == true)
                        pCallback?.Invoke(true, data2);
                    else
                    {
                        Debug.Log("계좌이체 : 받는 계좌에 입금 실패. 다시 시도해주십시오");
                        return;
                    }
                });
            }
            else
            {
                Debug.Log("계좌이체 : 보내는 계좌에서 출금 실패. 다시 시도해주십시오");
                return;
            }
        });

    }

    string ObjectToJson(object pObj)
    {
        string temp = Newtonsoft.Json.JsonConvert.SerializeObject(pObj);
        //temp = temp.Substring(1, temp.Length - 1);
        return temp;
    }

    /// <summary>
    /// 계좌번호로 이름찾기
    /// </summary>
    /// <param name="pAcno"></param>
    /// <returns></returns>
    string FindNameWithAcno(string pAcno)
    {
        foreach (KeyValuePair<string, NhParameter> info in dicAccountInfo)
        {
            if (info.Value.Acno == pAcno)
            {
                return info.Key;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 현금을 가상화폐로 전환
    /// </summary>
    /// <param name="pName">가상화폐를 충전할 계정명</param>
    /// <param name="pCashAmount">액수</param>
    /// <param name="pCallback">통신이 완료되었을 때 실행할 콜백함수</param>
    void RealCashToVirtualCash(string fromName, string toName, string pCashAmount, WebserverCallback pCallback)
    {
        //운영자 계좌에 이체
        Transfer(fromName, dicAccountInfo[toName].Acno, pCashAmount, (result, data) =>
        {
            if (result == true)
            {
                StartCoroutine(Co_SendCashRequest(fromName, pCashAmount, pCallback));
            }
            else
            {
                Debug.Log("가상화폐 환전 : 통신에 실패하였습니다.");
            }
        });

    }

    /// <summary>
    /// 현금을 가상화폐로 환전할 때 운영자 계좌에 이체가 성공했을 경우 웹서버에게 알려서 가상화폐 충전을 요청
    /// </summary>
    /// <param name="pName"></param>
    /// <param name="pCashAmount"></param>
    /// <param name="pCallback"></param>
    /// <returns></returns>
    IEnumerator Co_SendCashRequest(string pName, string pCashAmount, WebserverCallback pCallback)
    {
        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("Name", pName);
        body.Add("Amount", pCashAmount);
        string temp = ObjectToJson(body);
        string uri = $"http://3.36.116.224:8080/push?data={temp}";
        var www = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            pCallback?.Invoke(true, Encoding.UTF8.GetString(www.downloadHandler.data));
            Debug.Log("타인 계좌에 입금 성공하였습니다!");
        }
        else
        {
            pCallback?.Invoke(false, Encoding.UTF8.GetString(www.downloadHandler.data));
            Debug.Log("타인 계좌에 입금 실패하였습니다...");
        }
    }

    void OnApplicationQuit()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject pm = jo.Call<AndroidJavaObject>("getPackageManager");
        AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.FakeEyes.Hackathon");
        jo.Call("startActivity", intent);
    }

    public void Do(string fromName, string toName, string money)
    {
        RealCashToVirtualCash(fromName, toName, money, null);
    }
}