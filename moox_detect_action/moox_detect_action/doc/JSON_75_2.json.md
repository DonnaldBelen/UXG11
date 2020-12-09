# 出力説明 2020/07/06版

## run_tm.py の 87 行目 dict_result の中身と標準出力の概要  

```run_tm.py(72-96)  
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

            # JSON出力用
            self.publish_data(dict_result, 75, char_id, sensing_time)

            # 標準出力
            mess = 'Seat_ID: {} face_ward_relative:{:.2f}'.format(
                char_id, dict_result['status']['face_direction']['face_direction_horizontal'])
            out_box.append(mess)
        print(out_box)
```

- dict_resultの中身  
  - sensing_time : 入力データの時刻  
  - type : JOSN保存のタイプ  
  - no : JSON保存ナンバー  
  - data : データ格納部  
    - body : 前処理後の器官点の座標格納部  
      - raw : ノイズ未除去データ  
        - 器官点略称  
          - x : x座標  
          - y : y座標  
          - z : z座標  
          - confidence : 器官点品質情報(0:欠測 1:予測 2:測定)  
      - smooth : 平滑化処理によりノイズ除去を行ったデータ  
        - raw と同様  
      - body_id : Azure Kinect のボディID  
      - raw_camera_ids : カメラ取得情報  
      - is_data : データが存在するかの判定結果  
    - status : 視界情報の格納部  
      - face_direction : 視線情報格納部  
        - face_direction_horizontal : 顔向き水平角  
        - face_direction_vertical : 顔向き鉛直角  
        - face_direction_bace_x : 顔向き基準点  
        - face_direction_bace_y : 顔向き基準点  
        - face_direction_bace_z : 顔向き基準点  
        - face_direction_bace_right_eyes_x : 右目座標  
        - face_direction_bace_right_eyes_y : 右目座標  
        - face_direction_bace_right_eyes_z : 右目座標  
        - face_direction_bace_left_eyes_x : 左目座標  
        - face_direction_bace_left_eyes_y : 左目座標  
        - face_direction_bace_left_eyes_z : 左目座標  
      - look_points : 視点情報格納部  
        - look_point_center_x : 視界中心点座標  
        - look_point_center_y : 視界中心点座標  
        - look_point_center_z : 視界中心点座標  
        - look_point_center_object : 視界中心点の当たる物体名称
        - look_point_center_distance : 深度  
        - look_point_left_eyes_x : 左目投射点座標  
        - look_point_left_eyes_y : 左目投射点座標  
        - look_point_left_eyes_z : 左目投射点座標  
        - look_point_left_eyes_object : 左目投射点の当たる物体名称  
        - look_point_left_eyes_distance : 深度  
        - look_point_right_eyes_x : 右目投射点座標  
        - look_point_right_eyes_y : 右目投射点座標  
        - look_point_right_eyes_z : 右目投射点座標  
        - look_point_right_eyes_object : 右目投射点の当たる物体名称  
        - look_point_right_eyes_distance : 深度  
        - look_point_left_x : 視界左端点座標  
        - look_point_left_y : 視界左端点座標  
        - look_point_left_z : 視界左端点座標  
        - look_point_left_object : 視界左端点の当たる物体名称  
        - look_point_left_distance : 深度  
        - look_point_right_x : 視界右端点座標  
        - look_point_right_y : 視界右端点座標  
        - look_point_right_z : 視界右端点座標  
        - look_point_right_object : 視界右端点の当たる物体名称  
        - look_point_right_distance : 深度  
        - look_point_top_x : 視界上端点座標  
        - look_point_top_y : 視界上端点座標  
        - look_point_top_z : 視界上端点座標  
        - look_point_top_object : 視界上端点の当たる物体名称  
        - look_point_top_distance : 深度  
        - look_point_bottom_x : 視界下端点座標  
        - look_point_bottom_y : 視界下端点座標  
        - look_point_bottom_z : 視界下端点座標  
        - look_point_bottom_object : 視界下端点の当たる物体名称  
        - look_point_bottom_distance : 深度  
      - rule_action : 行動判定結果  
        - is_hand_swing : 手を振っているか  
        - is_l_hand_swing : 左手を振っているか  
        - is_r_hand_swing : 右手を振っているか  
      - property : 属性判定結果  
        - height : 身長推定結果 cm  
        - is_children : 小人(1),大人(0)  
        - is_children2 : 小人(1),大人(0)  
        - head_shoulder_width_ratio : is_children2に使用する肩幅と頭幅の比  
    - fast_status : rawデータを用いた低遅延視界情報の格納部  
      - statusと同様  

- 標準出力  
  - char_id : 椅子のID  
  - dict_result['status']['face_direction']['face_direction_horizontal'] : 顔向きの水平角度  
  - out_box : 椅子ID全部の結果を貯めるリスト  

## dict_resultの出力例(JSON)  

```2019-11-04***_75_2.json  
{ "sensing_time": "2020-06-22 03:41:39.338019", 
  "type": "75", 
  "no": "3", 
  "data": {"body": {
    "raw": {
      "pelvis": {"x": 96.41448669359056, "y": -115.76483070285951, "z": -444.2952902384045, "confidence": 2}, 
      "naval": {"x": 105.21412456103826, "y": 21.409586101009154, "z": -541.8961139837465, "confidence": 2}, 
      "chest": {"x": 111.0906021679109, "y": 148.27502909440898, "z": -587.7164419225389, "confidence": 2}, 
      "neck": {"x": 116.49877248735675, "y": 352.48808669297057, "z": -612.174377522294, "confidence": 2}, 
      "l_clavicle": {"x": 146.9866737679497, "y": 318.78264358270894, "z": -603.8140347014078, "confidence": 2}, 
      "l_shoulder": {"x": 277.1215751922957, "y": 292.20571615306847, "z": -584.0295825687608, "confidence": 2}, 
      "l_elbow": {"x": 369.1038072192757, "y": 48.026690032054944, "z": -567.4493218551338, "confidence": 2}, 
      "l_wrist": {"x": 278.2118211166719, "y": -14.69783325672529, "z": -379.04947309388103, "confidence": 2}, 
      "l_hand": {"x": 226.6993712333176, "y": 45.81022581270872, "z": -334.9900450540382, "confidence": 2}, 
      "l_handtip": {"x": 197.57483117147694, "y": 15.847996411949225, "z": -242.2111543381634, "confidence": 2}, 
      "l_thumb": {"x": 164.09902709908056, "y": 22.414605958469565, "z": -402.2116714510422, "confidence": 2}, 
      "r_clavicle": {"x": 84.64721232681859, "y": 318.4623494631784, "z": -614.365852282227, "confidence": 2}, 
      "r_shoulder": {"x": -32.13654544021301, "y": 286.5833518377359, "z": -639.8474293770469, "confidence": 2}, 
      "r_elbow": {"x": -145.08463476596694, "y": 55.64823914429894, "z": -710.901206101943, "confidence": 2}, 
      "r_wrist": {"x": -76.32307038823853, "y": 64.48390455861602, "z": -500.8284689168147, "confidence": 2}, 
      "r_hand": {"x": -31.48499839593069, "y": 15.705501933562118, "z": -429.10802450868835, "confidence": 2}, 
      "r_handtip": {"x": 7.695356784518026, "y": -27.341314827175324, "z": -347.7782477714725, "confidence": 2}, 
      "r_thumb": {"x": -42.06774089960004, "y": -42.07705874832163, "z": -463.9790964253948, "confidence": 2}, 
      "l_hip": {"x": 181.93060160110144, "y": -118.01871039537946, "z": -439.90817023008776, "confidence": 2}, 
      "l_knee": {"x": 160.21775630372667, "y": 30.992884293585348, "z": -93.47112584250624, "confidence": 2}, 
      "l_ankle": {"x": 134.17521089188585, "y": -330.7785874741194, "z": -90.01681136616435, "confidence": 1}, 
      "l_foot": {"x": 153.02011209669422, "y": -399.3115207044834, "z": 71.50506119046088, "confidence": 1}, 
      "r_hip": {"x": 19.30981616995382, "y": -113.73851033249593, "z": -448.26057542797787, "confidence": 2}, 
      "r_knee": {"x": 134.72758562429, "y": -5.871035368794537, "z": -105.83985269449897, "confidence": 2}, 
      "r_ankle": {"x": 62.43375171940693, "y": -362.7231879761948, "z": -148.50462119299993, "confidence": 1}, 
      "r_foot": {"x": 38.47242928103333, "y": -438.6299025641092, "z": -6.915781568018474, "confidence": 1}, 
      "head": {"x": 116.42459979038017, "y": 429.8623309721656, "z": -608.8919165487093, "confidence": 2}, 
      "nose": {"x": 144.91489995166603, "y": 380.7722185783676, "z": -468.6108224822126, "confidence": 2}, 
      "l_eyes": {"x": 152.6726446595094, "y": 432.35723970695767, "z": -478.1612242435997, "confidence": 2}, 
      "l_ear": {"x": 179.7444214363643, "y": 497.7805394374127, "z": -574.4494408306059, "confidence": 2}, 
      "r_eyes": {"x": 108.0297444298867, "y": 412.34179788149765, "z": -472.78799355187437, "confidence": 2}, 
      "r_ear": {"x": 31.570726915713067, "y": 450.47513876133473, "z": -561.6376017835489, "confidence": 2}
    }, 

    "smooth": {
      "pelvis": {"x": 96.41448669359056, "y": -115.76483070285951, "z": -444.2952902384045, "confidence": 2}, 
      "naval": {"x": 105.21412456103826, "y": 21.409586101009154, "z": -541.8961139837465, "confidence": 2}, 
      "chest": {"x": 111.0906021679109, "y": 148.27502909440898, "z": -587.7164419225389, "confidence": 2}, 
      "neck": {"x": 116.49877248735675, "y": 352.48808669297057, "z": -612.174377522294, "confidence": 2}, 
      "l_clavicle": {"x": 146.9866737679497, "y": 318.78264358270894, "z": -603.8140347014078, "confidence": 2}, 
      "l_shoulder": {"x": 277.1215751922957, "y": 292.20571615306847, "z": -584.0295825687608, "confidence": 2}, 
      "l_elbow": {"x": 369.1038072192757, "y": 48.026690032054944, "z": -567.4493218551338, "confidence": 2}, 
      "l_wrist": {"x": 278.2118211166719, "y": -14.69783325672529, "z": -379.04947309388103, "confidence": 2}, 
      "l_hand": {"x": 226.6993712333176, "y": 45.81022581270872, "z": -334.9900450540382, "confidence": 2}, 
      "l_handtip": {"x": 197.57483117147694, "y": 15.847996411949225, "z": -242.2111543381634, "confidence": 2}, 
      "l_thumb": {"x": 164.09902709908056, "y": 22.414605958469565, "z": -402.2116714510422, "confidence": 2},
      "r_clavicle": {"x": 84.64721232681859, "y": 318.4623494631784, "z": -614.365852282227, "confidence": 2}, 
      "r_shoulder": {"x": -32.13654544021301, "y": 286.5833518377359, "z": -639.8474293770469, "confidence": 2}, 
      "r_elbow": {"x": -145.08463476596694, "y": 55.64823914429894, "z": -710.901206101943, "confidence": 2},
      "r_wrist": {"x": -76.32307038823853, "y": 64.48390455861602, "z": -500.8284689168147, "confidence": 2}, 
      "r_hand": {"x": -31.48499839593069, "y": 15.705501933562118, "z": -429.10802450868835, "confidence": 2}, 
      "r_handtip": {"x": 7.695356784518026, "y": -27.341314827175324, "z": -347.7782477714725, "confidence": 2}, 
      "r_thumb": {"x": -42.06774089960004, "y": -42.07705874832163, "z": -463.9790964253948, "confidence": 2}, 
      "l_hip": {"x": 181.93060160110144, "y": -118.01871039537946, "z": -439.90817023008776, "confidence": 2}, 
      "l_knee": {"x": 160.21775630372667, "y": 30.992884293585348, "z": -93.47112584250624, "confidence": 2}, 
      "l_ankle": {"x": 134.17521089188585, "y": -330.7785874741194, "z": -90.01681136616435, "confidence": 1}, 
      "l_foot": {"x": 153.02011209669422, "y": -399.3115207044834, "z": 71.50506119046088, "confidence": 1}, 
      "r_hip": {"x": 19.30981616995382, "y": -113.73851033249593, "z": -448.26057542797787, "confidence": 2}, 
      "r_knee": {"x": 134.72758562429, "y": -5.871035368794537, "z": -105.83985269449897, "confidence": 2}, 
      "r_ankle": {"x": 62.43375171940693, "y": -362.7231879761948, "z": -148.50462119299993, "confidence": 1}, 
      "r_foot": {"x": 38.47242928103333, "y": -438.6299025641092, "z": -6.915781568018474, "confidence": 1}, 
      "head": {"x": 116.42459979038017, "y": 429.8623309721656, "z": -608.8919165487093, "confidence": 2}, 
      "nose": {"x": 144.91489995166603, "y": 380.7722185783676, "z": -468.6108224822126, "confidence": 2}, 
      "l_eyes": {"x": 152.6726446595094, "y": 432.35723970695767, "z": -478.1612242435997, "confidence": 2}, 
      "l_ear": {"x": 179.7444214363643, "y": 497.7805394374127, "z": -574.4494408306059, "confidence": 2}, 
      "r_eyes": {"x": 108.0297444298867, "y": 412.34179788149765, "z": -472.78799355187437, "confidence": 2}, 
      "r_ear": {"x": 31.570726915713067, "y": 450.47513876133473, "z": -561.6376017835489, "confidence": 2}
    }, 

    "body_id": 0, 
    "raw_camera_ids": {"0": 1, "1": 1}, 
    "is_data": true, "is_body_error": false
    }, 

  "status": {
    "face_direction": {
      "face_direction_horizontal": 5.387787546938483, 
      "face_direction_vertical": -41.13018359352964, 
      "face_direction_bace_x": 130.35119454469805, 
      "face_direction_bace_y": 422.34951879422766, 
      "face_direction_bace_z": -475.474608897737, 
      "face_direction_bace_right_eyes_x": 108.0297444298867, 
      "face_direction_bace_right_eyes_y": 412.34179788149765, 
      "face_direction_bace_right_eyes_z": -472.78799355187437, 
      "face_direction_bace_left_eyes_x": 152.6726446595094, 
      "face_direction_bace_left_eyes_y": 432.35723970695767, 
      "face_direction_bace_left_eyes_z": -478.1612242435997
    }, 

    "look_points": {
      "look_point_center_x": -69.96, 
      "look_point_center_y": -653.08, 
      "look_point_center_z": 620.0, 
      "look_point_center_object": "right_body", 
      "look_point_center_distance": 904.0311097169669, 
      "look_point_left_eyes_x": -48.13, 
      "look_point_left_eyes_y": -645.71, 
      "look_point_left_eyes_z": 620.0, 
      "look_point_left_eyes_object": "right_body", 
      "look_point_left_eyes_distance": 926.0407694094001, 
      "look_point_right_eyes_x": -91.79, 
      "look_point_right_eyes_y": -660.46, 
      "look_point_right_eyes_z": 620.0, 
      "look_point_right_eyes_object": "right_body", 
      "look_point_right_eyes_distance": 882.5292557709097, 
      "look_point_left_x": 49.6, 
      "look_point_left_y": -632.79, 
      "look_point_left_z": 620.0, 
      "look_point_left_object": "right_body", 
      "look_point_left_distance": 964.7244588280147, 
      "look_point_right_x": -192.2, 
      "look_point_right_y": -682.06, 
      "look_point_right_z": 620.0, 
      "look_point_right_object": "right_body", 
      "look_point_right_distance": 863.7669565836836, 
      "look_point_top_x": -69.96, 
      "look_point_top_y": -479.46, 
      "look_point_top_z": 620.0, 
      "look_point_top_object": "right_body", 
      "look_point_top_distance": 758.5364221594041, 
      "look_point_bottom_x": -58.11, 
      "look_point_bottom_y": -783.0, 
      "look_point_bottom_z": 555.22, 
      "look_point_bottom_object": "floor_body", 
      "look_point_bottom_distance": 1012.945302626179
    }, 
    "property": {
      "height": 179, 
      "is_children": 0, 
      "is_children2": 0, 
      "head_shoulder_width_ratio": 2.0138928620294
    }
  }, 

  "fast_status": {
    "face_direction": {
      "face_direction_horizontal": 5.387787546938483, 
      "face_direction_vertical": -41.13018359352964, 
      "face_direction_bace_x": 130.35119454469805, 
      "face_direction_bace_y": 422.34951879422766, 
      "face_direction_bace_z": -475.474608897737, 
      "face_direction_bace_right_eyes_x": 108.0297444298867, 
      "face_direction_bace_right_eyes_y": 412.34179788149765, 
      "face_direction_bace_right_eyes_z": -472.78799355187437, 
      "face_direction_bace_left_eyes_x": 152.6726446595094, 
      "face_direction_bace_left_eyes_y": 432.35723970695767, 
      "face_direction_bace_left_eyes_z": -478.1612242435997
  }, 

  "look_points": {
    "look_point_center_x": -69.96, 
    "look_point_center_y": -653.08, 
    "look_point_center_z": 620.0, 
    "look_point_center_object": "right_body", 
    "look_point_center_distance": 904.0311097169669, 
    "look_point_left_eyes_x": -48.13, 
    "look_point_left_eyes_y": -645.71, 
    "look_point_left_eyes_z": 620.0, 
    "look_point_left_eyes_object": "right_body", 
    "look_point_left_eyes_distance": 926.0407694094001, 
    "look_point_right_eyes_x": -91.79, 
    "look_point_right_eyes_y": -660.46, 
    "look_point_right_eyes_z": 620.0, 
    "look_point_right_eyes_object": "right_body", 
    "look_point_right_eyes_distance": 882.5292557709097, 
    "look_point_left_x": 49.6, 
    "look_point_left_y": -632.79, 
    "look_point_left_z": 620.0, 
    "look_point_left_object": "right_body", 
    "look_point_left_distance": 964.7244588280147, 
    "look_point_right_x": -192.2, 
    "look_point_right_y": -682.06, 
    "look_point_right_z": 620.0, 
    "look_point_right_object": "right_body", 
    "look_point_right_distance": 863.7669565836836, 
    "look_point_top_x": -69.96, 
    "look_point_top_y": -479.46, 
    "look_point_top_z": 620.0, 
    "look_point_top_object": "right_body", 
    "look_point_top_distance": 758.5364221594041, 
    "look_point_bottom_x": -58.11, 
    "look_point_bottom_y": -783.0, 
    "look_point_bottom_z": 555.22, 
    "look_point_bottom_object": "floor_body", 
    "look_point_bottom_distance": 1012.945302626179
  }, 

  "property": {
    "height": 179, 
    "is_children": 0, 
    "is_children2": 0, 
    "head_shoulder_width_ratio": 2.0138928620294
  }
  }}
}
```

以上
