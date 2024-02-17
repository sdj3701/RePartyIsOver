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

    //반환 값이 필요로 하면 <string>을 추가해서 적용하면 됨
    public async Task LoadDataAsync(string dataName, string motion)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        size = 0;
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
        }
        else
        {
            // 시트에 있는 것을 읽어옴
            string rawSheetData = www.downloadHandler.text;
            // 행 별로 데이터 분할
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
                    // TODO : 들어 있는 데이터 크기정해주기
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
        // TODO : 나중에 string 을 가지고 와서 이름에 따라서 switch case 문으로 같은 받아 오면 될듯
        switch (dataName)
        {
            //dataName 확인 잘하기
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
