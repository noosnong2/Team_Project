using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_dummy : Robot
{
    protected override void Update()
    {
        robotRayDir = -transform.up;
        base.Update();

        if (hitRay.collider && hitRay.collider.name == "Pad")
        {
            if (hitRay.collider.GetComponent<Object>())
                object_ = hitRay.collider.GetComponent<Object>();
            else
                Debug.Log("None Script On Pad");
        }
        else
            object_ = null;
    }
}
