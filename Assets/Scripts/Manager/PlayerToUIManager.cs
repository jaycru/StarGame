using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：4/25 13:06
//描述：建立主角状态到UI管理器的通信桥梁，负责将主角状态数据传递给UI管理器，以便UI能够正确显示主角的状态信息
//*************************

public class PlayerToUIManager : MonoBehaviour
{
    public static PlayerToUIManager Instance;//单例实例
    // Start is called before the first frame update
    public GameObject player;//主角对象

    private HUDManager hud;//拿到UI管理器的单例

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;//设置单例实例
        }
        else
        {
            Destroy(gameObject);//如果已经存在实例，销毁当前对象
        }
        hud = HUDManager.Instance;//获取UI管理器的单例实例
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 变化血量时调用此方法，传递新的血量值和最大血量值给UI管理器，以更新UI显示
    /// </summary>
    /// <param name="newHp">新血量</param>
    /// <param name="maxHp">最大血量</param>
    /// <param name="isUp">是否是增加血量（true）还是减少血量（false）</param>
    public void ChangeHealth(int newHp, int maxHp, bool isUp)
    {
        hud.UpdateHealth(newHp, maxHp);
        if (!isUp)
        {
            hud.PlayDamageEffect();
        }
    }
}
