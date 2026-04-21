using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：实现文本输出
//***************************************** 
public class TextOut : MonoBehaviour
{
    private TextAsset textAsset;
    private GameObject nameText;//名字
    private GameObject chatText;//聊天
    private TextMeshProUGUI namePro;
    private TextMeshProUGUI chatPro;
    private GameObject[] options = new GameObject[4];
    private TextMeshProUGUI[] optionsPro = new TextMeshProUGUI[4];//可选择项
    private string[] lines;
    private int count;//聊天行数
    private int optionCount;//选择数量
    private int nextAddCount;//下一次增加的行数
    private bool optionStage = false;//选择状态
    void Start()
    {
        nameText = transform.GetChild(0).gameObject;
        chatText = transform.GetChild(1).gameObject;
        namePro = nameText.GetComponent<TextMeshProUGUI>();
        chatPro = chatText.GetComponent<TextMeshProUGUI>();
        InitialOptions();
        lines = ReadLine();
        namePro.text = lines[0];
        chatPro.text = lines[1];
        count = 1;
    }

    void Update()
    {
        ChangeText();
    } 

    private void InitialOptions()
    {
        int maxCount = transform.childCount;
        for (int i = 2; i < maxCount; i++) 
        {
            options[i - 2] = transform.GetChild(i).gameObject;
            optionsPro[i - 2] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
    }

    private void ClearOptions(int optionCount)
    {
        for (int i = 0;i < optionCount; i++)
        {
            options[i].SetActive(false);
            optionsPro[i].text = string.Empty;
        }
    }

    public void SetText(TextAsset textAsset)
    {
        this.textAsset = textAsset;
    }

    private string[] ReadLine()
    {
        string[] lines = textAsset.text.Split(new char[]
            {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("Length:" +  lines.Length);
        return lines;
    }

    private void ChangeText()
    {
        int hit;
        //非选择状态，按下左键或F键时，跳转下一行
        if (!optionStage && (Input.GetButtonUp("Interact") || Input.GetMouseButtonUp(0)))
        {
            count++;
            if (count >= lines.Length)
            {
                ExitChat();
            }
            else
            {
                ChangeLine();
            }
        }
        else if (optionStage && ButtonOption(out hit))
        {
            Debug.Log("Into Option");
            if (hit <= optionCount)
            {
                count += hit;
                ChangeLine();
                ClearOptions(optionCount);
                count += (optionCount - hit);
                optionStage = false;
            }
        }
    }
    /// <summary>
    /// 切换为下一行
    /// </summary>
    private void ChangeLine()
    {
        string line = lines[count];
        if (line[0] != '[')
        {
            chatPro.text = lines[count];
        }
        //包含可选项
        else
        {
            optionStage = true;
            string[] option = line.Split(new char[] {'['}, System.StringSplitOptions.RemoveEmptyEntries);
            int optionsCount = option.Length;
            optionCount = optionsCount;
            for (int i = 0; i < optionsCount; i++)
            {
                string optionString = option[i].Substring(3);
                options[i].SetActive(true);
                optionsPro[i].text = optionString;
            }
        }
    }
    /// <summary>
    /// 退出聊天状态并初始化
    /// </summary>
    private void ExitChat()
    {
        chatPro.text = lines[1];
        count = 1;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
        AwakeBrother();
    }
    /// <summary>
    /// 唤醒兄弟物体（即按钮）
    /// </summary>
    public void AwakeBrother()
    {
        transform.parent.GetChild(0).gameObject.SetActive(true);
    }

    private bool ButtonOption(out int hit)
    {
        hit = 0;
        if (Input.GetButtonUp("Option1"))
        {
            hit = 1;
        }
        else if (Input.GetButtonUp("Option2"))
        {
            hit = 2;
        }
        else if (Input.GetButtonUp("Option3"))
        {
            hit = 3;
        }
        return Input.GetButtonUp("Option1") | Input.GetButtonUp("Option2") | Input.GetButtonUp("Option3");
    }
}
