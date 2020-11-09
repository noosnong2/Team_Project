using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Obj_pad : Object
{
    public override void ObjActive()
    {
        GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>().isClear = true;
    }
}
