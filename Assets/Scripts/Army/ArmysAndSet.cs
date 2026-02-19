using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmysAndSet : MonoBehaviour
{
    // Start is called before the first frame update
    private struct Armys
    {
        private int HP;
        private int ATK;
        private float ATKinterval;
        private float Speed;
        public string Name;
        public System.Type Behaviour;
        public void setInitial(int hp, int atk,float atkinterval,float speed,string name, System.Type behaviour)
        {
            HP=hp;
            ATK=atk;
            ATKinterval=atkinterval;
            Speed=speed;
            Name=name;
            Behaviour = behaviour;
        }
    }
    
    private int flag = 0;
    private bool isdown = false;
    private int lengthOfArmys = 2;
    private string string1;
    Armys[] armys=new Armys[100];
    void Start()
    {
        
        //List<Armys> armysList = new List<Armys>();
        Armys armys1=new Armys();
        armys[1] = armys1;
        armys[1].setInitial(47, 47, 1f, 1f, "SkeletonArmy",typeof(SkeletonBehaviour));
        armys[2] = armys1;
        armys[2].setInitial(400, 205, 1f, 0.7f, "MusketeerArmy", typeof(SkeletonBehaviour));//暂时挂上
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(armys[1].Name);
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray1);
            if (hits.Length > 0)
            {
                //Debug.Log("okk");
                RaycastHit hit = hits[0];
                for(int i=1;i<=lengthOfArmys;i++)
                {
                    if(hit.collider.tag == armys[i].Name)
                    {
                        flag = i;
                        isdown = true;
                        //Debug.Log("okk");
                    }
                }
                //if (hits[0].collider.name=="Skeleton")//改搜索方式
                //{
                //    flag = 1;
                //    isdown = true;
                //    //Debug.Log("okk");
                //}
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray2);
            if (isdown == true)
            {
                if (hits.Length > 0)
                {
                    if (hits[0].collider.tag == "Plane")
                    {
                        RaycastHit hit = hits[0];
                        //GameObject gameObject=hit.collider.gameObject;
                        Create(hit.point);
                        isdown = false;
                        
                    }
                }
            }
            else
            {
                Debug.Log("请先选择卡牌");
            }
        }
    }
    public void Create(Vector3 point)
    {
        string1 = armys[flag].Name;
        GameObject army1 = GameObject.Find(string1);
        Quaternion quaternion = army1.transform.rotation;
        GameObject newskeleton = Instantiate(army1, point, quaternion);
        newskeleton.AddComponent(armys[flag].Behaviour);
    }
}
