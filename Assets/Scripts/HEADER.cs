using UnityEngine;

[System.Serializable]
public class HEADER
{
    public string ApiNm;
    public string Tsymd;
    public string Trtm;
    public string Iscd;
    public string FintechApsno;
    public string ApiSvcCd;
    public string IsTuno;
    public string AccessToken;
    public string Rsms;
    public string Rpcd;

    public static string GetRandom6Digits()
    {
        string result = "";

        for (int i = 0; i < 6; i++)
            result += (Random.Range(0, 10) + "");

        return result;
    }
}