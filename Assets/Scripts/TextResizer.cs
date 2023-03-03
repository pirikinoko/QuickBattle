using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextResizer : MonoBehaviour
{
    Text text;
    int countLetter;
    int defaultSize;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
        defaultSize = text.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        countLetter =   text.text.Length; 
        text.fontSize = defaultSize - countLetter * 2;   
    }
}
