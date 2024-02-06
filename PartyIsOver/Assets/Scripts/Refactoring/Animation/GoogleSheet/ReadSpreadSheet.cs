using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Threading.Tasks;
using System.Drawing;

public class ReadSpreadSheet : MonoBehaviour
{
    public string ADDRESS  = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
    public string RANGE    = "B2:E";
    public long SHEET_ID   = 0;

    public string[,] sheetData;
    int size;

    MovementSM sm;

    private void Awake()
    {
        sm = GetComponent<MovementSM>();
    }

    public async Task<string> LoadDataAsync()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        size = 0;
        // �񵿱������� SendWebRequst ȣ��
        var operation = www.SendWebRequest();

        // �񵿱� �۾��� �Ϸ�� ������ ���
        while (!operation.isDone)
        {
            // ���
            await Task.Delay(10);
        }

        if (www.isNetworkError || www.isHttpError)
        {
            // ������ ������ ����׷� �˷��ְ� null�� ��ȯ
            Debug.Log("Error loading data : " + www.error);
            return null;
        }
        else
        {
            // ��Ʈ�� �ִ� ���� �о��
            string rawSheetData = www.downloadHandler.text;
            // �� ���� ������ ����
            string[] rows = rawSheetData.Split('\n');

            sheetData = new string[rows.Length - 1, rows[0].Split('\t').Length];
            for (int i = 0; i < rows.Length - 1; i++)
            {
                string[] values = rows[i].Split('\t');
                for (int j = 0; j < values.Length; j++)
                {
                    sheetData[i, j] = values[j];
                    // TODO : ���⼭ �����͸� ������ �Լ��� ȣ���ؼ� ���
                    Debug.Log("1");
                    sm.DataSave(i, j, sheetData[i, j]);
                }
            }
            size = rows.Length - 1;
            return null;
        }
    }

    public int Size()
    {
        return size;
    }

    // ��ȯ���� �޾ƾ���
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
            //�� ���� ������ ����
            string[] rows = rawsheetData.Split('\n');

            // ������ �迭 �ʱ�ȭ
            sheetData = new string[rows.Length - 1, rows[0].Split('\t').Length];

            //������ ä���
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
