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
    private float speed = 10f;//子弹移动速度
    private Rigidbody rb;//子弹的刚体
    private float lifeTime = 5f;//最长存活时间（销毁保护）
    private bool isHit;//是否碰撞
    private string enemyTag;//敌人的标记
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
    /// 控制碰撞（造成伤害）
    /// </summary>
    public void ControlHit()
    {
        if (isHit)
        {
            Hit();
        }
    }
    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <returns></returns>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == enemyTag)
        {
            Debug.Log("Successfully Hit!");
            isHit = true;
        }
    }
    /// <summary>
    /// 管理碰撞
    /// </summary>
    private void Hit()
    {
        Destroy(gameObject);
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
        //拿到敌人的标签
        enemyTag = "Enemy";
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
