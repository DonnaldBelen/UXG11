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

public class Ros2WebSocketSection: MonoBehaviour
{
    public String m_ros2WebsocketHost;

    public int seatSelect;

    public TextMeshProUGUI m_stat;
    public GameObject GazePanel;
    public GameObject GazePanel_leyes;
    public GameObject GazePanel_reyes;

    private Ros2SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    private WebSocket ws;
    private bool isWsUse = false;
    private int test_val = 0;
    private int rInt;
    private float coords1;
    private float coords2;
    private float coords3;
    private float coords4;
    private float coords5;
    private float coords6;
    private float coords7;
    private float coords8;
    private float coords9;
    public float screen_x;
    public float screen_y;
    private float nose_offset_x;
    private float nose_offset_y;
    private float l_ears_offset_x;
    private float l_ears_offset_y;
    private float r_ears_offset_x;
    private float r_ears_offset_y;


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
                subscribe("/moox/data/sensor/detect_action/c_3");
            }
            else if (seatSelect == 3)
            {
                subscribe("/moox/data/sensor/detect_action/c_3");
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
        RectTransform rt = GazePanel.GetComponent<RectTransform>();
        RectTransform rt_leyes = GazePanel_leyes.GetComponent<RectTransform>();
        RectTransform rt_reyes = GazePanel_reyes.GetComponent<RectTransform>();
        rt.transform.localPosition = new Vector3(coords1,coords2,0);
        rt_leyes.transform.localPosition = new Vector3(coords4,coords5,0);
        rt_reyes.transform.localPosition = new Vector3(coords7,coords8,0);
        m_stat.text = "Looking Coordinate: " + "[ " + coords1 + "," + coords2 + " ]";
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
        nose_offset_x = 0;
        nose_offset_y = 0;
        l_ears_offset_x = -100;
        l_ears_offset_y = 0;
        r_ears_offset_x = 100;
        r_ears_offset_y = 0;
        //Debug.Log(rosdata2.data.status.look_points.look_point_center_x.ToString());
        coords1 = screen_x + nose_offset_x - rosdata2.data.status.look_points.look_point_center_x;
        coords2 = screen_y + nose_offset_y + rosdata2.data.status.look_points.look_point_center_y;
        coords4 = screen_x + l_ears_offset_x - rosdata2.data.status.look_points.look_point_left_eyes_x;
        coords5 = screen_y + l_ears_offset_y + rosdata2.data.status.look_points.look_point_left_eyes_y;
        coords7 = screen_x + r_ears_offset_x - rosdata2.data.status.look_points.look_point_right_eyes_x;
        coords8 = screen_y + r_ears_offset_y + rosdata2.data.status.look_points.look_point_right_eyes_y;
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
        public RosData_data_status status;
    }
    [Serializable]
    public class RosData_data_status
    {
        public RosData_data_status_look_points look_points ;
    }
    [Serializable]
    public class RosData_data_status_look_points
    {
        public float look_point_center_x;
        public float look_point_center_y;
        public float look_point_center_z;
        public float look_point_left_eyes_x;
        public float look_point_left_eyes_y;
        public float look_point_left_eyes_z;
        public float look_point_right_eyes_x;
        public float look_point_right_eyes_y;
        public float look_point_right_eyes_z;
    }
}
