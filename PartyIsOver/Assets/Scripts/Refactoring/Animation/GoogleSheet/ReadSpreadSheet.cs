using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Threading.Tasks;
using System.Drawing;

public class ReadSpreadSheet : MonoBehaviour
{
    public string ADDRESS  ;
    public string RANGE    ;
    public long SHEET_ID   ;

    public string[,] sheetData;
    int size;

    MovementSM sm;

    private void Awake()
    {
        sm = GetComponent<MovementSM>();
    }

    //��ȯ ���� �ʿ�� �ϸ� <string>�� �߰��ؼ� �����ϸ� ��
    public async Task LoadDataAsync(string dataName, string motion)
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
        }
        else
        {
            // ��Ʈ�� �ִ� ���� �о��
            string rawSheetData = www.downloadHandler.text;
            // �� ���� ������ ����
            string[] rows = rawSheetData.Split('\n');
            size = rows.Length;
            ArraySize(dataName);

            sheetData = new string[rows.Length, rows[0].Split('\t').Length];
            for (int i = 0; i < rows.Length; i++)
            {
                string[] values = rows[i].Split('\t');
                for (int j = 0; j < values.Length; j++)
                {
                    sheetData[i, j] = values[j];
                    // TODO : ��� �ִ� ������ ũ�������ֱ�
                    if (motion == "Animation")
                        sm.DataSave(i, j, sheetData[i, j]);
                    else if (motion == "Angle")
                        sm.DataAngle(i, j, sheetData[i, j]);
                    else
                        Debug.Log("motion Error");
                }
            }
        }
    }

    public void ArraySize(string dataName)
    {
        // TODO : ���߿� string �� ������ �ͼ� �̸��� ���� switch case ������ ���� �޾� ���� �ɵ�
        switch (dataName)
        {
            //dataName Ȯ�� ���ϱ�
            case "JumpAnimation":
                {
                    sm.JumpAnimation.ReferenceRigidbodies = new Rigidbody[size];
                    sm.JumpAnimation.ActionRigidbodies = new Rigidbody[size];
                    sm.JumpAnimation.ActionForceValues = new float[size];
                    sm.JumpAnimation.ActionForceDirections = new Define.ForceDirection[size];
                }
                break;
            case "JumpAngle":
                {
                    sm.JumpAngle.StandardRigidbodies = new Rigidbody[size];
                    sm.JumpAngle.ActionDirections = new Transform[size];
                    sm.JumpAngle.TargetDirections = new Transform[size];
                    sm.JumpAngle.ActionRotationDirections = new Define.ForceDirection[size];
                    sm.JumpAngle.TargetRotationDirections = new Define.ForceDirection[size];
                    sm.JumpAngle.AngleStabilities = new float[size];
                    sm.JumpAngle.AnglePowerValues = new float[size];
                }
                break;
        }
    }

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }
}
