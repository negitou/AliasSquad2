using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_FocusAnimation : MonoBehaviour
{
    [SerializeField] private float changeValue;
    RectTransform MyTransform;
    float DefSizeX , DefSizeY ;
    // Start is called before the first frame update
    void Start()
    {
        MyTransform = GetComponent<RectTransform>();
        DefSizeX = MyTransform.rect.width;
        DefSizeY = MyTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterEvent()
    {
        StartCoroutine("ScaleUp");
    }

    public void ExitEvent()
    {
        StartCoroutine("ScaleDown");
    }

    IEnumerator ScaleUp()
    {
        for(int i = 1; i < 5; i++)
        {
            float tmp = (float)1 + ((float)changeValue * i);
            MyTransform.localScale = new Vector3(tmp , tmp, 1);
            //MyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, DefSizeX * (float)(1 + (i * 0.04))) ;
            //MyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, DefSizeY * (float)(1 + (i * 0.04)));
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ScaleDown()
    {
        for (int i = 5; i >= 0; i--)
        {
            float tmp = (float)1 + ((float)changeValue * i);
            MyTransform.localScale = new Vector3(tmp, tmp, 1);
            //MyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, DefSizeX * (float)(1 + (i * 0.04)));
            //MyTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, DefSizeY * (float)(1 + (i * 0.04)));
            yield return new WaitForSeconds(0.01f);
        }
    }
}
