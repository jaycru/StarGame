using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：
//*************************

public class GunController : MonoBehaviour
{
    public GameObject bulletContainer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject bullet = PoolManager.instance.RentBullet();
        bullet.transform.position = transform.position;
        bullet.transform.parent = bulletContainer.transform;
        bullet.transform.forward = transform.forward;
        bullet.SetActive(true);
    }
}
