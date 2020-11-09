using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    public class RayData
    {
        // Ray 생성위치.
        public Vector3 hitPoint;
        // Ray 방향.
        public Vector3 hitDir;
        // Ray의 충돌처리 데이터.
        public RaycastHit hitRay;
        // Ray 충돌 시 거리 갱신용.
        public float rayDistance = 1000f;

        // nowRay 리셋
        public void nowRayReset(Vector3 _hitPoint, Vector3 _hitDir) 
        {
            hitPoint = _hitPoint;
            hitDir = _hitDir;
            rayDistance = 1000f;
        }

        // RayData 삽입
        public void PushRayData(RayData _RayData)
        {
            hitPoint = _RayData.hitPoint;
            hitDir = _RayData.hitDir;
            rayDistance = _RayData.rayDistance;
            hitRay = _RayData.hitRay;
        }
    }

    public RayData _nowRay { get => nowRay; }
    public RayData _lastRay { get => lastRay; }
    public Transform _lastObject { set { lastObject = value; } }
    public bool _isRock { get => isRock; set { isRock = value; } }

    protected RayData nowRay = new RayData();
    protected RayData lastRay = new RayData();
    protected Transform lastObject = null;
    protected bool isRock = false;

    protected LineRenderer lr = null;
    protected List<RayData> RayDataList = new List<RayData>();

    protected Transform headTr = null;
    protected Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);


    void Awake()
    {
        if (transform.Find("Head") != null)
            headTr = transform.Find("Head").transform;      
    }

    void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.2f;
    }

    // LayerMask = (-1) -(1 << 2));의 경우 '2. Ignore Laycast' 레이어를 Ray 충돌 대상에서 제외 함.
    // 작성 이유 : 들어올리는 애니메이션 도중 Ray와 dummyHole의 충돌이 일어나 Unrock이 돼서 dummyHole에 LayerMask를 부여.
    public void CreateRay(Vector3 startPos, Vector3 rayDir)
    {
        nowRay.nowRayReset(startPos, rayDir);
        Physics.Raycast(startPos, rayDir, out nowRay.hitRay, 9999f, (-1) -(1 << 2));
        RayHitScan();
    }

    // 라이프 주기 종료.
    protected void ResetRayDataList()
    {
        RayDataList.Clear();
    }

    protected void RayHitScan()
    {
        if (!isRock && nowRay != null)
        {
            if (nowRay.hitRay.collider != null)
            {
                // 현재 Ray 거리를 충돌 포인트까지로 제한.
                nowRay.rayDistance = Vector3.Distance(nowRay.hitRay.point, nowRay.hitPoint); 
                lr.SetPosition(1, nowRay.hitRay.point);

                RayData newRayData = new RayData();
                newRayData.PushRayData(nowRay);
                RayDataList.Add(newRayData);

                // player가 조작 가능한 Object에 nowRay가 닿으면 Object에 enum type별 기능 호출.
                if (nowRay.hitRay.collider.CompareTag("p_obj")) 
                {
                    if (nowRay.hitRay.collider.GetComponent<Object>() == null) 
                    {
                        Debug.Log("None Component Obj");
                        return;
                    }

                    nowRay.hitRay.collider.GetComponent<Object>().ObjActive(this);
                }
            }
        }

        if (isRock && RayDataList.Count != 0)
        {
            for (int i = 0; i < RayDataList.Count; i++)
            {
                RayData _rayData = RayDataList[i];

                // List의 마지막 Ray.
                if (i == RayDataList.Count - 1)
                {
                    if (_nowRay.hitRay.collider.transform != lastObject)
                    {
                        Unrock();
                        return;
                    }

                    RaycastHit RayCheck;
                    Vector3 hitRayPoint;
                    Vector3 hitRayDir;

                    // Ray가 거울에 한번 이상 반사 됐을 경우
                    if (RayDataList.Count > 1) 
                    {
                        // Ray를 수평으로 만들기 위해서 도착점의 y좌표를 Ray의 y로 보정.
                        hitRayPoint = lastObject.position;
                        hitRayPoint.y = _nowRay.hitRay.point.y;

                        // Object에서 반대 방향으로 다시 한번 Ray를 쏴주고 도착점이 마지막으로 쏘아진 Ray의 시작점과 일치하는지 검사.
                        hitRayDir = _nowRay.hitPoint - hitRayPoint;
                        Physics.Raycast(hitRayPoint, hitRayDir, out RayCheck);

                        if (RayCheck.point != _nowRay.hitPoint)
                        {
                            Unrock();
                            return;
                        }
                    }

                    lastRay.PushRayData(_rayData);
                }
            }
        }
    }

    protected void Unrock()
    {
        if (lastObject != null)
        {
            if (lastObject.GetComponent<RobotMotion>() != null)
            {
                lastObject.GetComponent<RobotMotion>().AllReset();
                lastObject = null;
                lr.enabled = false;
                _isRock = false;
            }
            else if (lastObject.GetComponent<Antenna>() != null)
            {
                if (lastObject.GetComponent<Antenna>().lastObject == null)
                {
                    lastObject.GetComponent<Antenna>().AllReset();
                    lastObject = null;
                    lr.enabled = false;
                    _isRock = false;
                }
                else
                    lastObject.GetComponent<Antenna>().Unrock();
            }
        }
    }

    // 생성된 Ray의 갯수만큼 LineRenderer의 Count를 올리고 RayDataList의 도착점을 활용해서 point 연결,
    // Line을 그려내는 함수.
    protected void CreateLine()
    {
        lr.enabled = true;
        lr.positionCount = RayDataList.Count + 1;

        for (int i = 0; i < RayDataList.Count; i++)
        {
            lr.SetPosition(i, RayDataList[i].hitPoint);

            if (i == RayDataList.Count - 1)
                lr.SetPosition(i + 1, RayDataList[i].hitRay.point);
        }
    }

    // PlayerRotation, AntennaRotation을 병합했음.
    protected void SetForwardToMousePos(Transform obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDist;

        if (zeroPlane.Raycast(ray, out rayDist))
        {
            Vector3 targetPos = ray.origin + ray.direction * rayDist;
            targetPos.y = transform.position.y;

            Vector3 finDir = (targetPos - transform.position).normalized;
            obj.forward = finDir;
        }
    }
}
