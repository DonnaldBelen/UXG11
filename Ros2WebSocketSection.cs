using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.IO;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Get ROS2 msgs")]

    public class CA_ROS2SubscriberLookpoints : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject ROS2GameObject;
        private ROS2Connector ros2connector;

        public FsmGameObject GazePanel;
        public FsmGameObject GazePanel_leyes;
        public FsmGameObject GazePanel_reyes;

        public FsmString topic;
        private RosData rosdata;

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

        public override void Reset()
        {
            topic = "/moox/data/sensor/detect_action/c_3";
        }

        public override void OnEnter()
        {
            ros2connector = ROS2GameObject.Value.GetComponent<ROS2Connector>();
            //ros2connector.subscribe(topic.Value);
        }

        public override void OnUpdate()
        {
            lock (ros2connector.message)
            {
                if (topic.Value==ros2connector.message.topic) // ToDo: 一度読んだフレームを捨てる処理（JSONデシリアライズが重い）
                {
                    //rosdata = JsonUtility.FromJson<RosData>(ros2connector.message.msg.data);
                    rosdata = JsonConvert.DeserializeObject<RosData>(ros2connector.message.msg.data);
                    Debug.Log(ros2connector.message.msg.data);
                    set_kinect_data(rosdata);
                }
            }
        }
        public void set_kinect_data(RosData rosdata)
        {
            RectTransform rt = GazePanel.GetComponent<RectTransform>();
            RectTransform rt_leyes = GazePanel_leyes.GetComponent<RectTransform>();
            RectTransform rt_reyes = GazePanel_reyes.GetComponent<RectTransform>();

            nose_offset_x = 0;
            nose_offset_y = 0;
            l_ears_offset_x = -100;
            l_ears_offset_y = 0;
            r_ears_offset_x = 100;
            r_ears_offset_y = 0;
            //Debug.Log(rosdata.data.status.look_points.look_point_center_x.ToString());
            coords1 = screen_x + nose_offset_x - rosdata.data.status.look_points.look_point_center_x;
            coords2 = screen_y + nose_offset_y + rosdata.data.status.look_points.look_point_center_y;
            coords4 = screen_x + l_ears_offset_x - rosdata.data.status.look_points.look_point_left_eyes_x;
            coords5 = screen_y + l_ears_offset_y + rosdata.data.status.look_points.look_point_left_eyes_y;
            coords7 = screen_x + r_ears_offset_x - rosdata.data.status.look_points.look_point_right_eyes_x;
            coords8 = screen_y + r_ears_offset_y + rosdata.data.status.look_points.look_point_right_eyes_y;

            rt.transform.localPosition = new Vector3(coords1,coords2,0);
            rt_leyes.transform.localPosition = new Vector3(coords4,coords5,0);
            rt_reyes.transform.localPosition = new Vector3(coords7,coords8,0);
        }
        public bool is_confident(kinect_point_struct kinect_point)
        {
            if (kinect_point.confidence != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Serializable]
        public class RosData
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
        }
    }
}

