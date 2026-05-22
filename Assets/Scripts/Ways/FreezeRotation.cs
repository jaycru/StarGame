using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：
//*************************

public class FreezeRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public static void FreezeXAndZ(GameObject gameObject)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
