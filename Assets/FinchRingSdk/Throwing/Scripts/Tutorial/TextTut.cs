using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
using UnityEngine.UI;

public class TextTut : MonoBehaviour
{
    private Text TextOutput;
    public Chirality Chirality;
    public string CurrentText;
    private bool right;
    private bool left;
    private bool up;
    private bool down;



    private void Awake()
    {
        TextOutput = GetComponent<Text>();
    }

    void Update()
    {
        TextOutput.text = CurrentText;
        
    }
}
