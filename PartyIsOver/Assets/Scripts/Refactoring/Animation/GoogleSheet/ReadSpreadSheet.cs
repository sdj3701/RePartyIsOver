using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadSpreadSheet : MonoBehaviour
{
    public readonly string ADDRESS  = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
    public readonly string RANGE    = "B2:E";
    public readonly long SHEET_ID   = 0;
    public int rowCount;

    public string[,] sheetData;

    private void Start()
    {
        //StartCoroutine(LoadData());
        LoadData();
    }

    public string LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        //return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error loading data : " + www.error);
            return null;
        }
        else
        {
            string rawsheetData = www.downloadHandler.text;
            //행 별로 데이터 분할
            string[] rows = rawsheetData.Split('\n');

            // 이차원 배열 초기화
            sheetData = new string[rows.Length - 1, rows[0].Split('\t').Length];

            //데이터 채우기
            for(int i = 0; i < rows.Length -1; i++)
            {
                string[] values = rows[i].Split('\t');
                for(int j = 0; j < values.Length; j++)
                {
                    sheetData[i, j] = values[j];
                }
            }
            Debug.Log($"Value at (2, 2): {sheetData[1, 1]}");
            //rowCount  = rows.Length -1;
            return sheetData[1, 1];
    }

        //Debug.Log(www.downloadHandler.text);
        //Debug.Log(rowCount);
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
