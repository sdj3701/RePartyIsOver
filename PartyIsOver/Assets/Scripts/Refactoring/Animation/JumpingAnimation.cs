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
        // << : Load the player data when creating/ Move the code        
        if (sm.JumpAnimation.ReferenceRigidbodies == null)
        {
            sm.ReadSpreadSheet.ADDRESS = "https://docs.google.com/spreadsheets/d/16slVFqeg2egBHNcS-NPDRZzizFQwPH1oyr9AVtt9U2k";
            sm.ReadSpreadSheet.RANGE = "B2:E";
            sm.ReadSpreadSheet.SHEET_ID = 0;

            Debug.Log("데이터 없으니까 생성");
            await sm.ReadSpreadSheet.LoadDataAsync("JumpAnimation", "Animation");
            Debug.Log("생성 완료");
        }
        // >> : 
        
        for(int i =0; i< sm.JumpAnimation.ReferenceRigidbodies.Length; i++)
        {
            AnimateWithDirectedForce(sm.JumpAnimation,Vector3.up * 1.2f);
            if (i == 2)
                AnimateWithDirectedForce(sm.JumpAnimation, Vector3.down);
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