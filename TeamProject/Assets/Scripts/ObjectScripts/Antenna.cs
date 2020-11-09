using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenna : RayManager
{
    void Update()
    {
        // Input.left ctrl를 제외하면 Player와 동일한 내용.
        // RayManager안의 virtual Update()로 병합 후 override해도 될듯? 
        if (!isRock)
        {
            SetForwardToMousePos(headTr);

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
            Vector3 targetDir = new Vector3(lastObject.position.x - lastRay.hitPoint.x,
                lastObject.position.y, lastObject.position.z - lastRay.hitPoint.z);

            CreateRay(lastRay.hitPoint, targetDir);
            lr.SetPosition(RayDataList.Count, nowRay.hitRay.point);
        }
    }

    public void AllReset()
    {
        headTr.rotation = Quaternion.Euler(0f, 0f, 0f);
        GetComponent<Antenna>().enabled = false;
        lr.enabled = false;
    }
}
