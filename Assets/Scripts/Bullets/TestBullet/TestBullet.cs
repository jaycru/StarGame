using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：测试用子弹
//***************************************** 
public class TestBullet : MonoBehaviour
{
    private int hit;//子弹造成的伤害
    private float speed = 1f;//子弹移动速度
    private Rigidbody rb;//子弹的刚体
    private float lifeTime = 5f;//最长存活时间（销毁保护）
    void Start()
    {
        Debug.Log("Successfully Instantiate! Now hit is " + hit);
        Initial();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        Move();
    } 
    /// <summary>
    /// 初始化面朝方向模块
    /// </summary>
    public void InitialFace(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    /// <summary>
    /// 初始化物体
    /// </summary>
    public void Initial()
    {
        //拿到自身的引用
        rb = GetComponent<Rigidbody>();
    }
    /// <summary>
    /// 移动模块
    /// </summary>
    public void Move()
    {
        Vector3 direction = transform.forward;
        Debug.Log(direction);
        Debug.Log(rb);
        rb.velocity = direction * speed;
    }
    /// <summary>
    /// 设定子弹参数
    /// </summary>
    /// <param name="hit"></param>
    public void SetBullet(int hit)
    {
        this.hit = hit;
    }
}
