using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : RayManager
{
    void Update()
    {
        if (!isRock)
        {
            SetForwardToMousePos(transform);

            if (Input.GetMouseButton(0))
            {
                ResetRayDataList();
                CreateRay(headTr.position, headTr.forward);
                CreateLine();
            }
            else if (Input.GetMouseButtonUp(0))
                lr.enabled = false;
        }
        else
        {
            // 연결된 Object와 lastRay 발사점의 좌표를 이용해서 계속해서 갱신하고 쏴줄 Ray의 방향을 보정,
            // 보정된 방향으로 연결된 Object에 Ray를 계속해서 쏴주고
            // 그렇게 쏴주고있는 Ray의 충돌점과 LineRenderer의 마지막 Index에 해당하는 Point를 이어줌.

            Vector3 targetDir = new Vector3(lastObject.position.x - lastRay.hitPoint.x,
                lastObject.position.y, lastObject.position.z - lastRay.hitPoint.z);

            
            CreateRay(lastRay.hitPoint, targetDir);
            lr.SetPosition(RayDataList.Count, nowRay.hitRay.point);
            
            if (Input.GetKeyDown(KeyCode.LeftControl))
                Unrock();
        }
    }
}
