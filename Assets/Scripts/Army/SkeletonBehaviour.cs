using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private int HP;
    private int ATK;
    private float Speed;
    private float ATKinterval;
    private float AttackRange;
    private float SeeRange;
    void Start()
    {
        HP = 47;
        ATK = 47;
        Speed = 1f;
        ATKinterval = 1f;
        AttackRange = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(HP<=0)
        {
            Destroy(this);
            return;
        }
        else
        {
            //this.transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)//改变判断条件为持续条件，明天再来吧，累了
    {
        if(other.gameObject.name!="Plane")
            Debug.Log("chufa0");
        if (other.gameObject.name == "SeeBehaviour" && other.tag == "Red" && this.gameObject.tag == "SkeletonArmy") 
        {
            //iSeeYou(other.gameObject);//开始识别动作
            Debug.Log("识别成功");
        }
        else if (other.gameObject.name == "ATKBehaviour" && other.tag == "Red") 
        {
            //Attack(other.gameObject);//开始攻击
            Debug.Log("开始攻击");
        }
    }
}
