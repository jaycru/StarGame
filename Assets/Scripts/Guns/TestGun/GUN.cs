using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：枪械父类
//*************************

public class GUN : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual int GetMaxBullets()
    {
        return 0;
    }

    public virtual void AddBullets(int addBullets)
    {

    }
}
