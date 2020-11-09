using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RobotMotion : MonoBehaviour
{
    private Vector3 rot = Vector3.zero;
    private float rotSpeed = 40f;
    private float moveSpeed = 10f;
    private bool isWalkable = false;

    private Animator anim = null;
    private RuntimeAnimatorController rac = null;
    private float openAnimLen = 0f;
    private float liftAnimLen = 0f;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rot = transform.eulerAngles;
        rac = anim.runtimeAnimatorController;

        foreach (AnimationClip ac in rac.animationClips) 
        { 
            if (ac.name == "anim_open")
                openAnimLen = ac.length;

            if (ac.name == "anim_lift")
                liftAnimLen = ac.length;
        }
    }

    void OnEnable() 
    {
        // Robot.ObjActive()로 Enable이 true가 됐을 때 anim_open 시작, 종료되고나면 이동 가능.
        anim.SetBool("Open_Anim", true);
        CancelInvoke();
        Invoke("AproveMotionChange", openAnimLen);
    }

    void Update() 
    {
        if (isWalkable) 
            CheckKey();

        if (anim.GetBool("Walk_Anim"))
            transform.position += transform.forward * Time.deltaTime * moveSpeed;

        transform.eulerAngles = rot;
    }

    public void AllReset() 
    {
        anim.SetBool("Walk_Anim", false);
        anim.SetBool("Open_Anim", false);
        isWalkable = false;

        if (GetComponent<Robot_transport>())
            anim.SetBool("Lifting", false);

        GetComponent<RobotMotion>().enabled = false;
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionX;
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionZ;
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationY;
    }

    public void LiftORPut()
    {
        isWalkable = false;
        CancelInvoke();
        Invoke("AproveMotionChange", liftAnimLen);

        if (!anim.GetBool("Lifting"))
            anim.SetTrigger("Lift_Anim");
        else
            anim.SetTrigger("Put_Anim");

        anim.SetBool("Lifting", !anim.GetBool("Lifting"));
    }

    private void AproveMotionChange()
    {
        isWalkable = true;
    }

    private void CheckKey() 
    {
        // Walk
        if (Input.GetKey(KeyCode.W))
            anim.SetBool("Walk_Anim", true);
        else if (Input.GetKeyUp(KeyCode.W))
            anim.SetBool("Walk_Anim", false);

        // Rotate Left
        if (Input.GetKey(KeyCode.A))
            rot[1] -= rotSpeed * Time.fixedDeltaTime;

        // Rotate Right
        if (Input.GetKey(KeyCode.D))
            rot[1] += rotSpeed * Time.fixedDeltaTime;
    }
}
