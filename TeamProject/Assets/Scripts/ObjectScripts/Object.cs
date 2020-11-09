using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    // enum type별로 switch문에서 기능을 처리하기 때문에 Object를 상속받는 모든 객체는 별도로 enum type 정의 요망.
    // 하지만 Robot이 상호작용하는 Object는 Robot 종류별로 상호작용 가능하도록 정해둔 name을 통해 처리.
    protected enum EType { None, Robot, Mirror, Antenna }
    protected EType type = EType.None;

    private RayManager rayShooter = null;


    public void ObjActive(RayManager gameObject) 
    {
        rayShooter = gameObject;
        RaycastHit hit;
        Vector3 hitRayPos = transform.position; 
        hitRayPos.y = rayShooter._nowRay.hitPoint.y;
        Vector3 hitRayDir = rayShooter._nowRay.hitPoint - hitRayPos;

        switch (type)
        {
            case EType.None:
                Debug.Log(name + " no type");
                break;

            case EType.Robot:
                if (!GetComponent<RobotMotion>().enabled)
                {
                    // (임시) LayerMask 1.
                    // Robot에서 Ray를 쏜 주체로 다시 한번 쏴주는 역벡터 Ray.
                    Physics.Raycast(hitRayPos, hitRayDir, out hit, Vector3.Distance(rayShooter._nowRay.hitPoint, hitRayPos), 1);

                    // 역벡터Ray의 충돌 포인트가 Ray가 쏘아진 시작점과 일치하거나 역벡터Ray의 충돌체가 Ray를 쏴주는 Object와 같으면 Robot을 기동.
                    if (hit.point == rayShooter._nowRay.hitPoint)
                        ActiveRobot();
                    else if (hit.collider.GetComponent<RayManager>() == rayShooter)
                        ActiveRobot();
                }
                break;

            case EType.Mirror:
                Vector3 reflDir = Vector3.Reflect(rayShooter._nowRay.hitDir, rayShooter._nowRay.hitRay.normal);
                rayShooter.CreateRay(rayShooter._nowRay.hitRay.point, reflDir);
                break;

            case EType.Antenna:
                Physics.Raycast(hitRayPos, hitRayDir, out hit, Vector3.Distance(rayShooter._nowRay.hitPoint, hitRayPos), 1);

                if (rayShooter._nowRay.hitRay.collider.GetComponent<RayManager>() && 
                    !rayShooter._nowRay.hitRay.collider.GetComponent<RayManager>().enabled)
                {
                    // Robot과 같음.
                    if (hit.point == rayShooter._nowRay.hitPoint)
                        ActiveAntenna();
                    else if (hit.collider.GetComponent<RayManager>() == rayShooter)
                        ActiveAntenna();
                }
                break;
        }
    }

    public virtual void ObjActive() {}

    private void ActiveRobot()
    {
        GetComponent<RobotMotion>().enabled = true;
        GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
        GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
        GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationY;
        rayShooter._lastObject = transform;
        rayShooter._isRock = true;
        rayShooter._lastRay.PushRayData(rayShooter._nowRay);
    }

    private void ActiveAntenna()
    {
        rayShooter._lastObject = transform;
        rayShooter._isRock = true;
        rayShooter._lastRay.PushRayData(rayShooter._nowRay);
        GetComponent<RayManager>().enabled = true;
    }
}
