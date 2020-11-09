using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] stageObj = null;
    public GameObject[] triggerObj = null;
    public GameObject player = null;

    private StageSceneManager ssm = null;

    // 스테이지를 구성하는 데이터를 관리하는 class.
    public class StageData 
    {
        //Object의 위치/각도/크기.
        public class ObjectInfo 
        {
            public Vector3 nowPos;
            public Vector3 nowRot;
            public Vector3 nowScale;

            public ObjectInfo(Transform tr)
            {
                nowPos = tr.position;
                nowRot = tr.rotation.eulerAngles;
                nowScale = tr.localScale;
            }

            public void ChangePos(Vector3 pos)
            {
                nowPos = pos;
            }

            public void ChangeRot(Vector3 rt)
            {
                nowRot = rt;
            }

            public void ChangeScale(Vector3 sc)
            {
                nowScale = sc;
            }
        }

        // Player의 위치/각도/크기/isRock.
        public class PlayerInfo 
        {
            public Vector3 nowPos;
            public Vector3 nowRot;
            public Vector3 nowScale;
            public bool isRock;

            public PlayerInfo(Transform tr)
            {
                nowPos = tr.position;
                nowRot = tr.rotation.eulerAngles;
                nowScale = tr.localScale;
                isRock = false;
            }

            public PlayerInfo(Transform tr, bool rock)
            {
                nowPos = tr.position;
                nowRot = tr.rotation.eulerAngles;
                nowScale = tr.localScale;
                isRock = rock;
            }
        }

        public enum EState { FIRST, PLAYING, CLEAR };
        public EState stageState = EState.FIRST;

        public List<ObjectInfo> stageObj = new List<ObjectInfo>();
        public List<ObjectInfo> triggerObj = new List<ObjectInfo>();
        public PlayerInfo player;
        
        public int stageLev;
    }


    void Start()
    {
        if (GameObject.Find("StageSceneManager") != null)
        {
            ssm = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
            LoadStageData(ssm.stageLev);
        }
    }

    // StageSceneManager로 세이브할 데이터를 저장하는 함수.
    public void SaveStageData(int _stageLev) 
    {
        StageData stageData = new StageData();

        for (int i = 0; i < stageObj.Length; i++)
            stageData.stageObj.Add(new StageData.ObjectInfo(stageObj[i].transform));

        for (int i = 0; i < triggerObj.Length; i++)
            stageData.triggerObj.Add(new StageData.ObjectInfo(triggerObj[i].transform));

        stageData.stageLev = _stageLev;
        stageData.stageState = StageData.EState.PLAYING;

        stageData.player = new StageData.PlayerInfo(player.transform);

        ssm.stageDatas[_stageLev] = stageData; 
    }

    // 매개변수로 받아온 Level로 StageData를 가져옴.
    public void LoadStageData(int _stageLev) 
    {
        if (ssm.stageDatas[_stageLev] != null)
        {
            if (ssm.stageDatas[_stageLev].stageState == StageData.EState.PLAYING && 
                ssm.stageDatas[_stageLev].stageLev == _stageLev)
            {
                StageData sd = ssm.stageDatas[_stageLev];

                for (int i = 0; i < stageObj.Length; i++)
                    stageObj[i].transform.position = sd.stageObj[i].nowPos;

                for (int i = 0; i < triggerObj.Length; i++)
                    triggerObj[i].transform.position = sd.triggerObj[i].nowPos;

                player.transform.position = sd.player.nowPos;
            }
        }
    }

    public void ClearStage(int _stageLev)
    {
        StageData stagedata = new StageData();

        stagedata.stageLev = _stageLev;

        //해당 스테이지를 클리어 했다는 정보만 저장
        //(클리어시 해당 스테이지는 초기화를 하기 때문에 클리어 유무만 판단하면됨)
        stagedata.stageState = StageData.EState.CLEAR;
        ssm.stageDatas[_stageLev] = stagedata;

        //빌드 번호를 맞춰주기 위해 2를 더한다.
        ssm.MoveStage(_stageLev + 2); 
    }
}
