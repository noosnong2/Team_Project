using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonManager : MonoBehaviour
{
    public Image[] selectBtn; 


    void Start()
    {
        StageSceneManager ssm = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
        
        if (ssm.stageDatas[0] != null)
        {
            for (int i = 0; i < selectBtn.Length; i++)
            {
                if (ssm.stageDatas[i] != null)
                {
                    //FIRST(0),PLAYING(1),CLEAR(2)로 구성, 조건에 맞는 버튼의 상태 변경.
                    switch (ssm.CheckStageState(i)) 
                    {
                        case 1:
                            if (!selectBtn[i].GetComponent<Button>().enabled)
                                selectBtn[i].GetComponent<Button>().enabled = true;

                            selectBtn[i].color = Color.yellow;
                            break;

                        case 2:
                            if (!selectBtn[i].GetComponent<Button>().enabled)
                                selectBtn[i].GetComponent<Button>().enabled = true;

                            selectBtn[i].color = Color.green;
                            break;
                    }
                }
            } 
        }    
    }
}