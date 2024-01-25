using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Button_Control : MonoBehaviour
{
    public enum ButtonPattern
    {
        Play = 0,
        Setting,
        Exit,
        Max
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushButton(int value)
    {
        string text = string.Empty;
        switch (value)
        {
            case 0:
                text = "Play";
                break;
            case 1:
                text = "Setting";
                break;
            case 2:
                text = "Exit";
                break;
            default:
                break;
        }
        UnityEngine.Debug.Log(text);
    }
}
