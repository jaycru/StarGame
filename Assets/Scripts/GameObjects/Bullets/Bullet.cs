using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：
//*************************

public class Bullet : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Initialize();
        Invoke("ReturnMySelf", 5f);
        //Invoke("Die", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    public void ReturnMySelf()
    {
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
        PoolManager.instance.ReturnBullet(this.gameObject);
    }
}
