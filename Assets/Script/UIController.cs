using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class UIController : MonoBehaviour
{   
    //Socket控制器
    public SocketManager mSocketMgr;
    //消息队列
    public Queue<string> mMsgQueue ;
    // Start is called before the first frame update
    void Start()
    {   
        mMsgQueue = new Queue<string>();
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
    }

    public void ReceiveMsgCallBack(string jsonmsg)
    {
        Debug.Log("UIController  Receivemsg:" + jsonmsg);
        mMsgQueue.Enqueue(jsonmsg);
    }
    
    public void OnLogin()
    {
        MsgData data = new MsgData();
        data.msgType = MsgType.Login;
        data.msg = "login";
        State.number = 1;
        string jmsg = JsonMapper.ToJson(data);
        mSocketMgr.SendMsgToServer(jmsg);
    }
    
    public void OnLogout()
    {
        MsgData data = new MsgData();
        data.msgType = MsgType.LoginOut;
        data.msg = "logout";
        State.number = 0;
        string jmsg = JsonMapper.ToJson(data);
        mSocketMgr.SendMsgToServer(jmsg);
    }

    public void OnBtnClick()
    {   
        MsgData data = new MsgData();
        data.msgType = MsgType.HideCube;
        data.msg = "hidecube";
        string jmsg = JsonMapper.ToJson(data);
        mSocketMgr.SendMsgToServer(jmsg);
    }
    
    public void OnDestroy()
    {
        mSocketMgr.mConnectSuccess -= ConnectSuccess;
        mSocketMgr.mReceiveMsgCallBack -= ReceiveMsgCallBack;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
