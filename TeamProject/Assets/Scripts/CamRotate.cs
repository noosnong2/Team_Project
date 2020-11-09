using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    // Vector3.zero를 중점으로 Q,E버튼을 눌러서 메인 카메라를 회전, 씬마다 있는 메인 카메라의 position 초기값 설정과
    // 컴포넌트 부착을 수동으로 해야하는데 main 카메라를 prefab화하거나 dontdestroy로 설정하는게 좋을 듯?
    private float elapsedTime = 0f;
    private float rotTime = 1f;
    private float rotAngle = 90f;
    private bool isRot = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRot)
            ResetRotInfo(1);

        if (Input.GetKeyDown(KeyCode.E) && !isRot)
            ResetRotInfo(-1);

        if (isRot)
        {
            elapsedTime = Mathf.Clamp(elapsedTime + Time.deltaTime, 0f, rotTime);
            transform.RotateAround(Vector3.zero, Vector3.up, rotAngle * Time.deltaTime);

            if (elapsedTime >= rotTime)
                isRot = false;
        }
    }

    private void ResetRotInfo(int rotDir)
    {
        elapsedTime = 0f;
        isRot = true;
        rotAngle = rotDir * Mathf.Abs(rotAngle);
    }
}
