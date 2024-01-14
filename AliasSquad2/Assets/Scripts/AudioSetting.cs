using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;

    private VolumeData volumeData = new VolumeData(); 

    private DataFileManager dataFileManager;

    private string jsonData;

    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSE;

    private float volumeTempBGM;
    private float volumeTempSE;

    void Start () {
        dataFileManager = GameObject.Find("DataFileManager").GetComponent<DataFileManager>();
        VolumeLoad();
        if (jsonData == null)
        {
            VolumeSave();
        }
        volumeData = JsonUtility.FromJson<VolumeData>(jsonData);
        sliderBGM.value = volumeData.value[0];
        sliderSE.value = volumeData.value[1];
    }
	
	void Update () {
		
	}

    public void SetMasterVolume(float f)
    {
        audioMixer.SetFloat("Master", Mathf.Lerp(-20, 5, f));
    }

    public void SetBGMVolume(float f)
    {
        audioMixer.SetFloat("BGM", Mathf.Lerp(-20, 5, f));
    }

    public void SetSEVolume(float f)
    {
        audioMixer.SetFloat("SE", Mathf.Lerp(-20, 5, f));
    }

    public void RevertVolume()
    {
        sliderBGM.value = volumeTempBGM;
        sliderSE.value = volumeTempSE;
    }

    public void VolumeTemp()
    {
        volumeTempBGM = sliderBGM.value;
        volumeTempSE = sliderSE.value;
    }

    public void VolumeSave()
    {
        volumeData.value[0] = sliderBGM.value;
        volumeData.value[1] = sliderSE.value;
        jsonData = JsonUtility.ToJson(volumeData);
        dataFileManager.FileSave("option",jsonData);
    }

    public void VolumeLoad()
    {
        jsonData = dataFileManager.FileLoad("option");
    }

}

[System.Serializable]
public class VolumeData
{
    public float[] value = new float[2];
}
