using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：
//*************************

public class GameSceneManager : MonoBehaviour
{
    private Camera mainCamera;
    public static GameSceneManager instance;
    public TextAsset intoText;//剧情沉浸式带入的文本
    public GameObject chatText;//对话文本UI块
    private TextOut textOut;//对话文本UI块上的TextOut脚本
    private CameraClearFlags originalClearFlags;//主摄像机原始清除标志
    private Color originalBackgroundColor;//主摄像机原始背景颜色
    private int originalCullingMask;//主摄像机原始剔除层

    void Awake()
    {
        mainCamera = Camera.main;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //初始化对话文本UI块
        textOut = chatText.GetComponent<TextOut>();
        //初始化摄像机原始设置
        originalClearFlags = mainCamera.clearFlags;
        originalBackgroundColor = mainCamera.backgroundColor;
        originalCullingMask = mainCamera.cullingMask;

        //剧情沉浸式带入
        StartIntoDialogue();
    }

    void Update()
    {
        
    }

    private void StartIntoDialogue()
    {
        //mainCamera.enabled = false;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
        mainCamera.cullingMask = 0;
        SetGamePlayActive(true, intoText);
    }
    /// <summary>
    /// 设置游戏模式或者剧情（对话）模式
    /// 如若set为true，则进入游戏模式，玩家可以移动；如若set为false，则进入剧情模式，玩家无法移动，视角锁定，开始对话
    /// </summary>
    public void SetGamePlayActive(bool set, TextAsset text)
    {
        if (set)
        {
            //锁定时间戳
            Time.timeScale = 0;
            //设定文本
            textOut.SetText(text);
            //显示文本
            chatText.transform.parent.gameObject.SetActive(true);
            chatText.SetActive(true);
            //显示鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //打印日志
            Debug.Log("进入剧情模式");
        }
        else
        {
            //如果主摄像机未激活，则激活主摄像机
            if (mainCamera.cullingMask == 0)
            {
                mainCamera.clearFlags = originalClearFlags;
                mainCamera.backgroundColor = originalBackgroundColor;
                mainCamera.cullingMask = originalCullingMask;
                textOut.AwakeBrother(false);
            }
            //解锁时间戳
            Time.timeScale = 1;
            //隐藏文本
            chatText.SetActive(false);
            //隐藏鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //打印日志
            Debug.Log("进入游戏模式");
        }
    }
}
