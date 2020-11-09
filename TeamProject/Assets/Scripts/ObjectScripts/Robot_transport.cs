using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using UnityEngine;

public class Robot_transport : Robot
{ 
    public Transform leftHand = null, rightHand = null;

    private bool isLifting = false;
    public bool _isLifting { get => isLifting; }

    private bool trigger = false;
    public bool _trigger { get => trigger; set { trigger = value; } }

    private GameObject dummyHole = null;
    private Transform targetTr = null;


    void Awake() 
    {
        // dummy의 위치를 고정시키기 위한 collider
        dummyHole = GetComponentInChildren<CheckTrigger>().gameObject;
        dummyHole.SetActive(false);
    }

    protected override void Update() 
    {
        robotRayDir = transform.forward;
        base.Update();

        if (hitRay.collider && hitRay.collider.name == "Robot Dummy") 
        {
            if (hitRay.collider.GetComponent<Object>()) 
            {
                object_ = hitRay.collider.GetComponent<Object>();
                targetTr = object_.transform;
            } 
            else
                Debug.Log("None Script On Dummy");
        }
        else
            object_ = null;

        // dummyHole의 위치는 양 손 사이로 고정.
        if (dummyHole.activeSelf) 
        {
            Vector3 lhsFromRhs = rightHand.transform.position - leftHand.transform.position;
            Vector3 clampPos = leftHand.transform.position + lhsFromRhs.normalized
                * (Vector3.Distance(leftHand.transform.position, rightHand.transform.position) / 2);

            dummyHole.transform.position = clampPos;
        }
    }

    public IEnumerator ClampTargetPos() 
    {
        isLifting = true;
        dummyHole.SetActive(true);
        targetTr.GetComponent<SphereCollider>().enabled = false;

        // dummyHole이 땅에 닿았을 때 부터 target의 위치를 dummyHole의 위치로 고정.
        // G키를 누르면 anim_put 시작, 그 후 dummyHole이 다시 땅에 닿거나 Unrock 호출로 RobotMotion이 꺼지게 되면
        // target의 위치를 바닥에 맞게 보정하고 collider를 켜줌. 
        yield return new WaitUntil(() => trigger);

        while (true) 
        {
            targetTr.position = dummyHole.transform.position;

            if (Input.GetKeyDown(KeyCode.G))
                GetComponent<RobotMotion>().LiftORPut();

            yield return new WaitForFixedUpdate();

            if (!trigger || !GetComponent<RobotMotion>().enabled) 
            {
                isLifting = false;
                dummyHole.SetActive(false);
                targetTr.position = new Vector3(targetTr.position.x, -0.2f, targetTr.position.z);
                targetTr.GetComponent<SphereCollider>().enabled = true;
                break;
            }
        }
    }
}
