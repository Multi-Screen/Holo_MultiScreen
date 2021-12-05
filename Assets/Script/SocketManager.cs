using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
using System.IO;

public class SocketManager : MonoBehaviour
{   
    public Socket mSocket;
    //数据缓冲池
    public byte[] mBuffer = new byte[1024];
    //数据拼接池
    public MemoryStream mMemoryStream = new MemoryStream();
    //接收消息队列
    public Queue<string> mRecvMsgQueue = new Queue<string>();
    //连接成功回调
    public Action mConnectSuccess;
    //接收消息回调
    public Action<string> mReceiveMsgCallBack;


    public void ConnectServer()
    {
        Debug.Log("开始连接服务器");
        //创建Socket
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //绑定IP地址和端口
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("172.22.20.231"), 9000);
        //开始连接服务器
        mSocket.BeginConnect(iPEndPoint, ConnectCallBack, "");
    }
    public void Update()
    {
        if (mRecvMsgQueue.Count > 0)
        {
            string jsonmsg = mRecvMsgQueue.Dequeue();
            if (mReceiveMsgCallBack != null)
            {
                mReceiveMsgCallBack.Invoke(jsonmsg);
            }
        }
    }
    /// <summary>
    /// 连接服务器回调
    /// </summary>
    /// <param name="ar">异步结果</param>
    public void ConnectCallBack(IAsyncResult ar)
    {
        mSocket.EndConnect(ar);
        if (mSocket.Connected == true)
        {
            Debug.Log("服务器连接成功！");
            if (mConnectSuccess != null)
            {
                mConnectSuccess.Invoke();
            }
            mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, ReceiveMsgCallBack, "");
        }
        else
        {
            Debug.LogError("服务器连接失败，请注意检查！");
        }

    }
    /// <summary>
    /// 发送消息到服务器
    /// </summary>
    /// <param name="msg"></param>
    public void SendMsgToServer(string msg)
    {
        byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(msg);
        //取出消息体的长度 +4  4代表的是包体在Byte字节内所占的长度
        int msglength = msgBytes.Length + 4;
        //把包头转为byte数组
        byte[] headBytes = BitConverter.GetBytes(msglength);
        MemoryStream stream = new MemoryStream();
        //写入包头
        stream.Write(headBytes, 0, headBytes.Length);
        //写入包体
        stream.Write(msgBytes, 0, msgBytes.Length);
        //要发送的字节
        byte[] sendbytes = stream.ToArray();
        stream.Dispose();
        stream.Close();
        mSocket.BeginSend(sendbytes, 0, sendbytes.Length, SocketFlags.None, SendMsgCallBack, "");
    }
    /// <summary>
    /// 发送消息回调
    /// </summary>
    /// <param name="ar"></param>
    public void SendMsgCallBack(IAsyncResult ar)
    {
        int length = mSocket.EndSend(ar);
        Debug.Log("消息发送完成 长度：" + length);
    }
    /// <summary>
    /// 接收消息回调
    /// </summary>
    /// <param name="ar"></param>
    public void ReceiveMsgCallBack(IAsyncResult ar)
    {
        int length = mSocket.EndReceive(ar);

        //写入数据拼接池
        mMemoryStream.Write(mBuffer, 0, length);
        //读取到的下标
        int redIndex = 0;
        //是否开始拆包
        bool isUnpackFinsh = false;
        //接收的数据
        byte[] recvMsgBytes = mMemoryStream.ToArray();
        //消息内容大小
        int msgContentSize = 0;
        //开始拆包
        while (isUnpackFinsh == false)
        {
            //判断当前接收到的数据长度，是否小于包头的长度
            if (recvMsgBytes.Length < 4 + redIndex)
            {
                isUnpackFinsh = true;
            }
            else
            {
                //取出包头 得到消息的长度
                msgContentSize = BitConverter.ToInt32(recvMsgBytes, redIndex);
                //如果接收的数据的长度，小于消息的长度 说明消息不完整
                //获取剩余的长度
                int overLength = recvMsgBytes.Length - redIndex;
                if (overLength < msgContentSize)
                {
                    isUnpackFinsh = true;
                }
                else
                {
                    //说明数据是完整的 
                    string Jsonmsg = System.Text.Encoding.UTF8.GetString(recvMsgBytes, redIndex + 4, recvMsgBytes.Length - 4);
                    mRecvMsgQueue.Enqueue(Jsonmsg);
                    //读取下标往拨
                    redIndex += msgContentSize;
                }
            }
        }
        if (isUnpackFinsh == true)
        {
            //如果小于 拆包完成 进行数据拼接
            mMemoryStream.Dispose();
            mMemoryStream.Close();
            mMemoryStream = new MemoryStream();
            mMemoryStream.Write(recvMsgBytes, redIndex, recvMsgBytes.Length - redIndex);
            mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, ReceiveMsgCallBack, "");
        }


    }
    public void OnDestroy()
    {
        mSocket.Close();
    }
}
