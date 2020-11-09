using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneManager : MonoBehaviour
{
    // 스테이지 정보를 저장하는 배열, Index = 스테이지 갯수.
    public StageManager.StageData[] stageDatas = new StageManager.StageData[3]; 

    public float animTime = 2.5f;
    private bool isAnim = false;

    public int stageLev = 0;
    public bool isClear = false;


    void Start()
    {
        // 스테이지에서 메인메뉴로 왔을 때의 데이터 동기화.
        StageSceneManager[] checkSSM = FindObjectsOfType<StageSceneManager>(); 

        if (checkSSM.Length > 1)
        {
            for(int i = 0; i < checkSSM[0].stageDatas.Length; i++)
                stageDatas[i] = checkSSM[0].stageDatas[i];

            Destroy(checkSSM[0].gameObject);
        }
    }

    void Update()
    {
        // if)ESC를 누르면 메인메뉴로 씬 이동.
        // else if)클리어 시 다음 스테이지로 이동, 클리어 조건은 Object의 클리어 트리거를 가진 오브젝트들이 넘겨주게 해야 함.
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            StageManager sm = GameObject.Find("StageManager").GetComponent<StageManager>();
            sm.SaveStageData(stageLev);
            LeaveStage();
        }
        else if (isClear) 
        {
            StageManager sm = GameObject.Find("StageManager").GetComponent<StageManager>();
            sm.ClearStage(stageLev);
            isClear = false;
        }
    }

    // 다음 스테이지로 이동.
    public void MoveStage(int _stageLev) 
    {
        if (!isAnim)
        {
            isAnim = true;
            StageSceneManager[] ssm = FindObjectsOfType<StageSceneManager>();

            if (ssm.Length == 1)
                DontDestroyOnLoad(this);

            StartCoroutine(TransitionAnimCoroutine(_stageLev));
        }
    }

    // 외부 class에서 enum값을 이용해 스테이지의 상태를 확인하기 위한 함수.
    public int CheckStageState(int stageLv) 
    {
        if (stageDatas[stageLv] != null)
        {
            switch (stageDatas[stageLv].stageState)
            {
                case StageManager.StageData.EState.FIRST:
                    return 0;

                case StageManager.StageData.EState.PLAYING:
                    return 1;

                case StageManager.StageData.EState.CLEAR:
                    return 2;
            }
        }

        return -1;
    }

    // Transition 애니메이션을 위한 코루틴.
    protected IEnumerator TransitionAnimCoroutine(int _stageLev)
    {
        Animator transition = GameObject.Find("Transition").GetComponent<Animator>();
        transition.SetTrigger("TransitionTrigger");
        yield return new WaitForSeconds(animTime);

        isAnim = false;

        SceneManager.LoadScene(_stageLev);
        stageLev = _stageLev - 1;
    }

    // 스테이지에서 메인메뉴로 이동.
    protected void LeaveStage() 
    {
        if (!isAnim)
        {
            isAnim = true;
            StartCoroutine(TransitionAnimCoroutine(0));
        }
    }
}
