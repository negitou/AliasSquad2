using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RecoilTool : EditorWindow {

    public WeaponAsset recoilAsset;

	private float minTime;
	private float maxTime;

    private double beforeTime;
    private float currentTime;

    float frame = 5;
	float time = 0;

	private bool play = false;

    Keyframe[] keyframeX, keyframeY;
    int keyIndexX = 0, keyIndexY = 0;
    float keyTimeX = 0, keyTimeY = 0;
    float keyValueX = 0, keyValueY = 0;

    Rect area;

    [MenuItem("Window/RecoilTool")]
    static void Init()
    {
        RecoilTool recoilTool = (RecoilTool)EditorWindow.GetWindow(typeof(RecoilTool));
        recoilTool.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();//縦
        recoilAsset = EditorGUILayout.ObjectField("RecoilAsset", recoilAsset, typeof(WeaponAsset), false) as WeaponAsset;
        if (recoilAsset == null)
        {
            play = false;
            time = 0;
        }

        if (recoilAsset != null)
        {
			recoilAsset.x = EditorGUILayout.CurveField("X", recoilAsset.x);
			recoilAsset.y = EditorGUILayout.CurveField("Y",recoilAsset.y);

            GUILayout.Label("Preview");

            float margin = 20.0f;
            float size = 500.0f;

            EditorGUILayout.BeginHorizontal();//横
            area = GUILayoutUtility.GetRect(size + margin * 2, size + margin * 2);

            Handles.color = new Color(0f, 0.7f, 1f, 0.5f);
            const int div = 20;
            for (int i = 1; i < div; i++)
            {
                float y = size / div * i;
                float x = size / div * i;
                Handles.DrawLine(new Vector2(margin + area.x, margin + area.y + y),new Vector2(margin + size, margin + area.y + y));
                Handles.DrawLine(new Vector2(margin + area.x + x, margin + area.y),new Vector2(margin + area.x + x, -margin + area.yMax));
            }

            Handles.color = new Color(0f, 0.7f, 1f, 1f);
            Handles.DrawLine(new Vector2(margin + area.x, margin + area.y),new Vector2(margin + size, margin + area.y));
            Handles.DrawLine(new Vector2(margin + area.x, margin + area.y + size), new Vector2(margin + size, margin + area.y + size));
            Handles.DrawLine(new Vector2(margin + area.x, margin + area.y), new Vector2(margin + area.x, margin + area.y + size));
            Handles.DrawLine(new Vector2(margin + size, margin + area.y), new Vector2(margin + size, margin + area.y + size));

            Handles.color = new Color(1f, 1f, 1f, 1f);

			for (int i = 0; i <= time * frame; i++)
			{
				Handles.DrawLine(new Vector2(area.x + margin + size / 2 + -recoilAsset.x.Evaluate(i / frame) * 10, area.y + margin + size / 2 + -recoilAsset.y.Evaluate(i / frame) * 10), new Vector2(area.x + margin + size / 2 + -recoilAsset.x.Evaluate((i + 1) / frame) * 10, area.y + margin + size / 2 + -recoilAsset.y.Evaluate((i + 1) / frame) * 10));
			}

            EditorGUILayout.BeginVertical();//縦
            GUILayout.Label("Time を消すと削除できます");

            EditorGUILayout.BeginHorizontal();//横

            //X
            string[] keys = new string[recoilAsset.x.keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = "Index" + i;
            }

            GUILayout.Label("X");
            keyframeX = recoilAsset.x.keys;
            keyIndexX = EditorGUILayout.Popup(keyIndexX, keys);

            try
            {
                keyTimeX = keyframeX[keyIndexX].time;
                keyValueX = keyframeX[keyIndexX].value;
            }
            catch (System.IndexOutOfRangeException)
            {
                recoilAsset.y.AddKey(0, 0);
            }

            GUILayout.Label("Time");
            keyTimeX = EditorGUILayout.FloatField(keyTimeX);

            GUILayout.Label("Value");
            keyValueX = EditorGUILayout.FloatField(keyValueX);

            Keyframe keyframeTemp = new Keyframe();

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("AddKey"))
            {
                recoilAsset.x.AddKey(keyframeX[keyframeX.Length - 1].time + 1f, keyframeX[keyframeX.Length - 1].value);
            }
            try
            {
                if (keyTimeX != keyframeX[keyIndexX].time || keyValueX != keyframeX[keyIndexX].value)
                {
                    keyframeTemp.value = keyValueX;
                    keyframeTemp.time = keyTimeX;

                    recoilAsset.x.MoveKey(keyIndexX, keyframeTemp);
                }
            }
            catch (System.Exception)
            {
                recoilAsset.y.AddKey(0, 0);
            }


            EditorGUILayout.BeginHorizontal();//横

            //Y
            keys = new string[recoilAsset.y.keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = "Index" + i;
            }

            GUILayout.Label("Y");
            keyframeY = recoilAsset.y.keys;
            keyIndexY = EditorGUILayout.Popup(keyIndexY, keys);
            try
            {
                keyTimeY = keyframeY[keyIndexY].time;
                keyValueY = keyframeY[keyIndexY].value;
            }
            catch (System.IndexOutOfRangeException)
            {
                recoilAsset.y.AddKey(0, 0);
            }

            GUILayout.Label("Time");
            keyTimeY = EditorGUILayout.FloatField(keyTimeY);

            GUILayout.Label("Value");
            keyValueY = EditorGUILayout.FloatField(keyValueY);

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("AddKey"))
            {
                recoilAsset.y.AddKey(keyframeY[keyframeY.Length - 1].time + 1f, keyframeY[keyframeY.Length - 1].value);
            }

            try
            {
                if (keyTimeY != keyframeY[keyIndexY].time || keyValueY != keyframeY[keyIndexY].value)
                {
                    keyframeTemp.value = keyValueY;
                    keyframeTemp.time = keyTimeY;
                    recoilAsset.y.MoveKey(keyIndexY, keyframeTemp);
                }
            }
            catch (System.Exception)
            {
                recoilAsset.y.AddKey(0, 0);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            recoilAsset.damage = EditorGUILayout.IntField("Damage", recoilAsset.damage);

            recoilAsset.magazine = EditorGUILayout.IntField("Magazine", recoilAsset.magazine);

            recoilAsset.ammo = EditorGUILayout.IntField("Ammo", recoilAsset.ammo);

            recoilAsset.delayTime = EditorGUILayout.FloatField("DelayTime", recoilAsset.delayTime);
            
            if (GUILayout.Button("Play"))
            {
				play = !play;
				if (play) {
					time = 0;
					beforeTime = Time.realtimeSinceStartup;
				}	
            }


			if (GUILayout.Button("Save"))
			{
				EditorUtility.SetDirty (recoilAsset);
				AssetDatabase.SaveAssets();	
			}

            EditorGUILayout.EndVertical();

            EditorApplication.update += () => {
				if (play) {
					double timeSinceStartup = EditorApplication.timeSinceStartup;
					double deltaTime = timeSinceStartup - beforeTime;
					beforeTime = timeSinceStartup;
					time += (float)deltaTime;

					if (time > 10){
						play = false;
					}
				}
			};
        }

    }

}
