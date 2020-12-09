import rclpy
import configparser
import time
from std_msgs.msg import String
from rclpy.node import Node
from rclpy.qos import QoSProfile, QoSHistoryPolicy, QoSReliabilityPolicy, QoSDurabilityPolicy
import traceback
import glob
import os
import sys
sys.path.append(os.path.dirname(os.path.abspath(__file__)) + '/')
from moox_common.mooxdds import MooxSubscriberNode
from moox_common.mooxdds import MooxPublisherNode
from moox_common.mooxdds import MooxQoS
from moox_common.common import MyEncoder
from rclpy.executors import MultiThreadedExecutor
import mysql.connector
import mysql.connector.pooling
import subprocess
import json
from datetime import date, datetime, timedelta
import re
import argparse

# 前処理用
sys.path.append(os.path.dirname(os.path.abspath(__file__)))
from Detect_rule.detect_rule import Detect_rule
from Preprocessing.Preprocessor import Preprocessor

class MooxDetectAction(Node):
    def __init__(self):
        super().__init__('MooxDetectAction')

        parser = argparse.ArgumentParser(description='MooxDetectAction')
        parser.add_argument('--no', type=str, default="") # NO
        args = parser.parse_args()

        self.no = args.no

        print("MooxDetectAction NO => {}".format(self.no))

        # 設定読み込み
        inifile = configparser.ConfigParser()
        inifile.read(os.path.dirname(os.path.abspath(__file__)) + '/../../../../config.ini', 'UTF-8')

        qos_profile = MooxQoS.create(MooxQoS.QoSType.SENSOR)
        self.pubnode = {
            '75-{}'.format(self.no) : MooxPublisherNode('/moox/data/sensor/detect_action/c_{}'.format(self.no), String, qos_profile)
        }

        # 計算対象被験者設定
        self.char_ids = char_ids = [self.no]

        self.prep = Preprocessor()

        self.detect_actions = []
        for i in range(len(char_ids)):
            self.detect_actions.append(Detect_rule())

    def run(self):
        qos_profile = MooxQoS.create(MooxQoS.QoSType.SENSOR)
        
        subnode1 = MooxSubscriberNode('/moox/data/sensor/sitting_seat',
            String, qos_profile, self.callback_subscribe)
        
        executor = MultiThreadedExecutor(num_threads=10)
        executor.add_node(subnode1)
        executor.add_node(self)

        try:
            executor.spin()
        except KeyboardInterrupt:
            print("KeyboardInterrupt!")

        executor.shutdown()

        subnode1.destroy_node()

    def callback_subscribe(self, msg):
        json_data = json.loads(msg.data)

        tic = time.time()

        # 入力データ取得
        sensing_time = json_data['sensing_time']
        dic_data = json_data['data']

        out_box = []
        for i, char_id in enumerate(self.char_ids):
            _key = 'seat' + char_id

            input_dict = self.prep.Calculate(dic_data[_key])
            self.detect_actions[i].Calculate(input_dict, sensing_time)
            dict_result = self.detect_actions[i].dict_result

            # 処理時間計測
            time_info = dic_data["time_info"]

            d_time = datetime.utcnow()
            s_time = datetime.strptime(sensing_time, "%Y-%m-%d %H:%M:%S.%f")
            c_time = datetime.strptime(time_info["camera_sensing_time"]["0"], "%Y-%m-%d %H:%M:%S.%f")

            time_info["detect_action_time"] = d_time.strftime("%Y-%m-%d %H:%M:%S.%f")
            time_info["detect_action_latency"] = (d_time - s_time).total_seconds()
            time_info["total_latency"] = (d_time - c_time).total_seconds()

            dict_result["time_info"] = time_info

            # JSON出力用
            self.publish_data(dict_result, 75, char_id, sensing_time)

            # 標準出力
            mess = 'Seat_ID: {} face_ward_relative:{:.2f}'.format(
                char_id, dict_result['status']['face_direction']['face_direction_horizontal'])
            out_box.append(mess)
        print(out_box)

    def publish_data(self, data, sensor_type, sensor_no, sensing_time):
        frame_data = {
            'sensing_time': sensing_time,
            'type': sensor_type,
            'no': sensor_no,
            'data': data
        }

        json_str = json.dumps(frame_data, cls = MyEncoder)
        self.pubnode["{}-{}".format(sensor_type, sensor_no)].publish(json_str)


def main():
    args = []
    rclpy.init(args=args)

    node = MooxDetectAction()
    node.run()

    rclpy.shutdown()

if __name__ == '__main__':
    main()
