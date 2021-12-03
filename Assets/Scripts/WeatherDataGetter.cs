using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherDataGetter : MonoBehaviour
{
    [System.Serializable]
    class Response
    {
        public int count;
        public Result[] list;
    }

    [System.Serializable]
    class Result
    {
        public string sdate;
        public string stdHour;
        public string unitCode;
        public string unitName;
        public string routeNo;
        public string routeName;
        public string updownTypeCode;
        public string xValue;
        public string yValue;
        public string tmxValue;
        public string tmyValue;
        public string measurement;
        public string addr;
        public string addrCode;
        public string addrName;
        public string weatherContents;
        public string statusNo;
        public string correctNo;
        public string cloudValue;
        public string addcloudValue;
        public string cloudformValue;
        public string tempValue;
        public string dewValue;
        public string discomforeValue;
        public string sensoryTemp;
        public string highestTemp;
        public string highestyearTemp;
        public string highestcompTemp;
        public string lowestTemp;
        public string lowestyearTemp;
        public string lowestcompTemp;
        public string rainfallValue;
        public string rainfallstrengthValue;
        public string newsnowValue;
        public string snowValue;
        public string humidityValue;
        public string windContents;
        public string windValue;
    }

    private Result[] results;

    private string tempValue;
    private string humidityValue;

    void Awake()
    {
        tempValue = "0";
        humidityValue = "0";

        StartCoroutine(GetWeatherData());
    }

    private IEnumerator GetWeatherData()
    {
        string date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
        string hour = DateTime.Now.AddDays(-1).ToString("hh");

        string GetPath = $"http://data.ex.co.kr/openapi/restinfo/restWeatherList?" +
            $"key=6963127770&type=json&sdate={date}&stdHour={hour}";

        using (UnityWebRequest www = UnityWebRequest.Get(GetPath))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error: 정상적이지 않은 값이 들어왔습니다.");
            }
            else
            {
                if(www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Response response = JsonUtility.FromJson<Response>(jsonResult);
                    results = null;
                    results = response.list;
                    /*
                    for (int i = 0; i < response.count; i++)
                    {
                        Debug.Log(response.list[i].addrName);
                        Debug.Log(response.list[i].tempValue);
                        Debug.Log(response.list[i].humidityValue);
                        Debug.Log(response.list[i].rainfallValue);
                        Debug.Log(response.list[i].windValue);
                    }
                    */
                }
            }
        }
    }

    public void InitializeValues()
    {
        tempValue = "";
        humidityValue = "";
    }

    public void Do()
    {
        if (results != null)
        {
            if (results.Length > 0)
            {
                Result result = results[UnityEngine.Random.Range(0, results.Length)];

                tempValue = result.tempValue.Substring(0, result.tempValue.IndexOf(".") + 2);
                humidityValue = result.humidityValue.Substring(0, result.humidityValue.IndexOf(".") + 2);
            }
        }
    }

    public string GetTemp()
    {
        return tempValue;
    }

    public string GetHumidity()
    {
        return humidityValue;
    }
}