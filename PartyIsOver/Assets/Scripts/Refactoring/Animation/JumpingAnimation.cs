using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using static AniFrameData;
using static CharacterPhysicsMotion;
using System.Threading.Tasks;
using UnityEngine.Scripting;

public class JumpingAnimation : Jumping
{
    public JumpingAnimation(MovementSM stateMachine) : base(stateMachine) { }

    public override async void Enter()
    {
        base.Enter();
        sm.ReadSpreadSheet.ADDRESS  = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
        sm.ReadSpreadSheet.RANGE    = "B2:E";
        sm.ReadSpreadSheet.SHEET_ID = 0;

        if (sm.JumpAnimation.ReferenceRigidbodies == null)
        {
            // 데이터 로드
            await sm.ReadSpreadSheet.LoadDataAsync("JumpAnimation");
        }
        else
        {
            Debug.Log(sm.JumpAnimation.ReferenceRigidbodies[0]);
            Debug.Log(sm.JumpAnimation.ActionRigidbodies[0]);
            Debug.Log(sm.JumpAnimation.ActionForceDirections[0]);
            Debug.Log(sm.JumpAnimation.ActionForceValues[0]);
            Debug.Log("데이터가 있습니다.");
        }

    }

    public override async void UpdatePhysics()
    {
        base.UpdatePhysics();
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, -sm.BodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, -sm.BodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, -sm.BodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, sm.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);


    }
}

        //animationDictionary.Add(sm.aaa.ReferenceRigidbodies[0], Part(await sm.ReadSpreadSheet.LoadDataAsync(0, 0)));