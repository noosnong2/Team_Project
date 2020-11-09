using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RobotMotion))]

public abstract class Robot : Object 
{
    protected Object object_ = null;
    protected Vector3 robotRayDir;
    protected RaycastHit hitRay;


    void OnEnable() 
    {
        type = EType.Robot;
        GetComponent<RobotMotion>().enabled = false;
    }

    protected virtual void Update() 
    {
        if (GetComponent<RobotMotion>().enabled) 
        {
            // (임시) 오브젝트가 null이 아니라면 오브젝트에 rim light를 뿌려주는 걸로 상호작용 가능여부 시각화 예정.
            Debug.DrawRay(transform.position, robotRayDir * 10f);
            
            Physics.Raycast(transform.position, robotRayDir, out hitRay, 10f);
        }

        // 특정 name의 Object와 Ray가 닿았을 때 그것을 object_로 참조하고,
        // object_가 null이 아닐 때 F키를 누르면 현재 로봇 종류에 따른 함수를 호출.
        if (Input.GetKeyDown(KeyCode.F) && object_)
        {
            if (GetComponent<Robot_transport>() && !GetComponent<Robot_transport>()._isLifting)
            {
                StartCoroutine(GetComponent<Robot_transport>().ClampTargetPos());
                GetComponent<RobotMotion>().LiftORPut();
            }
            else
                object_.ObjActive();
        }
    }
}
