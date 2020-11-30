using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using System;
using MiniJSON;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Ros2WebSocketHeartRate: MonoBehaviour
{
    public String m_ros2WebsocketHost;

    public int seatSelect;

    public TextMeshProUGUI m_stat;


    private Ros2SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    private WebSocket ws;
    private bool isWsUse = false;
    private int test_val = 0;
    private float HeartRate_var;


    [Serializable]
    public class Ros2WebsocketRequest
    {
        public string op = "subscribe";
        public string topic;
        public string type = "std_msgs/msg/String";
    }

    // Start is called before the first frame update
    void Start()
    {
        m_skeletalTrackingProvider = new Ros2SkeletalTrackingProvider();
        isWsUse = true;
        Task.Run(() => startSocket());
    }

    void startSocket()
    {
        // WebSocketのクライアントの生成
        // ws://192.168.100.224:9090/
        // ws://192.168.11.73:9090/
        ws = new WebSocket(m_ros2WebsocketHost);

        // 接続時に呼ばれる
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("open");

            if (seatSelect == 2)
            {
                subscribe("/moox/data/sensor/heart_rate/c_2");
            }
            else if (seatSelect == 3)
            {
                subscribe("/moox/data/sensor/heart_rate/c_3"); 
            }
            
        };

        // サーバからのデータ受信時に呼ばれる
        ws.OnMessage += (sender, e) =>
        {
            //Debug.Log(e.Data);
            try
            {
                onMessage(e);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        };

        // クローズ時に呼ばれる
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("close");

            if (isWsUse)
            {
                Thread.Sleep(1000);
                Task.Run(() => startSocket());
            }
        };

        // エラー時に呼ばれる
        ws.OnError += (sender, e) =>
        {
            Debug.Log(e.Message);

            if (isWsUse)
            {
                Thread.Sleep(1000);
                Task.Run(() => startSocket());
            }
        };

        // 接続
        ws.ConnectAsync();
    }

    void Update()
    {
        m_stat.text = HeartRate_var;
    }

    // 破棄時に呼ばれる
    void OnDestroy()
    {
        isWsUse = false;

        if (null != ws)
        {
            ws.CloseAsync();
        }
        ws = null;
    }

    void onMessage(MessageEventArgs e)
    {
        RosData rosdata = JsonConvert.DeserializeObject<RosData>(e.Data);
        RosData2 rosdata2 = JsonConvert.DeserializeObject<RosData2>(rosdata.msg.data);
        //Debug.Log(rosdata2.data.heart_rate.ToString());
        HeartRate_var = rosdata2.data.heart_rate;
    }

    void onAzureKinectMessage(Dictionary<string, object> data)
    {
        m_skeletalTrackingProvider.onAzureKinectMessage(data);
    }

    void subscribe(string topic)
    {
        if(null == ws)
        {
            return;
        }

        Ros2WebsocketRequest req = new Ros2WebsocketRequest();
        req.topic = topic;
        var sendStr = JsonUtility.ToJson(req);
        ws.Send(sendStr);
    }


    [Serializable]
    public class Msg    {
        public string data { get; set; } 
    }

    public class RosData    {
        public string op { get; set; } 
        public string topic { get; set; } 
        public Msg msg { get; set; } 
    }

    [Serializable]
    public class RosData2
    {
        public RosData_data data;
        public string sensing_time;
    }
    [Serializable]
    public class RosData_data
    {
        public public float heart_rate;
    }

}
