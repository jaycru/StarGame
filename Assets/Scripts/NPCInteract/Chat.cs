using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：挂载于NPC上，当主角靠近时打开AskPanel
//***************************************** 
public class Chat : MonoBehaviour
{
    public TextAsset chatText;
    private string player = "Player";
    private Transform askPanel;//对话按钮

    private void Start()
    {
        askPanel = GameObject.Find("InventoryCanvas").transform.GetChild(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == player)
        {
            askPanel.gameObject.SetActive(true);
            askPanel.GetChild(0).gameObject.SetActive(true);
            askPanel.GetChild(0).GetComponent<BeginChat>().SetText(chatText);
            Debug.Log("靠近！");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (askPanel.gameObject.activeSelf)
        {
            askPanel.gameObject.SetActive(false);
            Debug.Log("退出！");
        }
    }
}
