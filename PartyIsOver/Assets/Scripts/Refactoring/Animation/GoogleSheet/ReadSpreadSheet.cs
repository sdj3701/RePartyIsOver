using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Threading.Tasks;

public class ReadSpreadSheet : MonoBehaviour
{
    public string ADDRESS  = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
    public string RANGE    = "B2:E";
    public long SHEET_ID   = 0;

    public string[,] sheetData;

    public async Task<string> LoadDataAsync(int row, int col)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));

        // 비동기적으로 SendWebRequst 호출
        var operation = www.SendWebRequest();

        // 비동기 작업이 완료될 때까지 대기
        while (!operation.isDone)
        {
            // 대기
            await Task.Delay(10);
        }

        if (www.isNetworkError || www.isHttpError)
        {
            // 문제가 있으면 디버그로 알려주고 null을 반환
            Debug.Log("Error loading data : " + www.error);
            return null;
        }
        else
        {
            // 시트에 있는 것을 읽어옴
            string rawSheetData = www.downloadHandler.text;
            // 행 별로 데이터 분할
            string[] rows = rawSheetData.Split('\n');

            sheetData = new string[rows.Length - 1, rows[0].Split('\t').Length];
            for (int i = 0; i < rows.Length - 1; i++)
            {
                string[] values = rows[i].Split('\t');
                for (int j = 0; j < values.Length; j++)
                {
                    sheetData[i, j] = values[j];
                }
            }
            Debug.Log(sheetData[row, col]);

        }
        return sheetData[row, col];
    }

    public IEnumerator LoadData(int row =0, int col=0)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error loading data : " + www.error);
        }
        else
        {
            string rawsheetData = www.downloadHandler.text;
            //행 별로 데이터 분할
            string[] rows = rawsheetData.Split('\n');

            // 이차원 배열 초기화
            sheetData = new string[rows.Length - 1, rows[0].Split('\t').Length];

            //데이터 채우기
            for (int i = 0; i < rows.Length - 1; i++)
            {
                string[] values = rows[i].Split('\t');
                for (int j = 0; j < values.Length; j++)
                {
                    sheetData[i, j] = values[j];
                }
            }
            Debug.Log($"Value at (row, col): {sheetData[row, col]}");
        }
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
