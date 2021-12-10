using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class UIController : MonoBehaviour
{   

    public GameObject obj;
    //Socket控制器
    public SocketManager mSocketMgr;
    //消息队列
    public Queue<string> mMsgQueue = new Queue<string>();

    // 显示状态
    private int mShowState = 0;
 

    public void Start()
    {   
        mSocketMgr.mConnectSuccess += ConnectSuccess;
        mSocketMgr.mReceiveMsgCallBack += ReceiveMsgCallBack;
        mSocketMgr.ConnectServer();
    }

    /// <summary>
    /// 连接服务器成功回调
    /// </summary>
    public void ConnectSuccess()
    {
        Debug.Log("UIController 连服务器成功回调");
        // 自动登录
        MsgData data = new MsgData();
        data.msgType = MsgType.Login;
        data.msg = "login";
        data.name = "holo";
        string jmsg = JsonMapper.ToJson(data);
        mSocketMgr.SendMsgToServer(jmsg);
    }

    public void ReceiveMsgCallBack(string jsonmsg)
    {
        Debug.Log("UIController  Receivemsg:" + jsonmsg);
        mMsgQueue.Enqueue(jsonmsg);
    }
    
    public void OnDestroy()
    {
        mSocketMgr.mConnectSuccess -= ConnectSuccess;
        mSocketMgr.mReceiveMsgCallBack -= ReceiveMsgCallBack;
    }

    // Update is called once per frame
    public void Update()
    {   
        if (mMsgQueue.Count > 0)
        {
            string jsonmsg = mMsgQueue.Dequeue();
            // //把json字符串转为数据类
            MsgData msgData = JsonMapper.ToObject<MsgData>(jsonmsg);
            Debug.Log("msgType:" + msgData.msgType);
            switch (msgData.msgType)
            {   
                case MsgType.Login:
                    Debug.Log("登录成功");
                    break;
                case MsgType.HideCube:
                if (mShowState == 1){
                    mShowState = 0;
                }else{  
                    mShowState = 1;
                }
                    break;
                default:
                    break;
            }
        }


        // 主线程和子线程的问题
        if(mShowState==0){
            obj.SetActive(true);
        }else{
            obj.SetActive(false);
        }
    }
}
