using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NotificationBase))]
public class OrientationText : OrientationBase
{
    public int LetterPerString = 29;

    private NotificationBase notification;

    private void Awake()
    {
        notification = GetComponent<NotificationBase>();
    }

    private void Update()
    {
        if (!IsAvailableOrientation)
        {
            return;
        }

        string[] phrases = notification.text.text.Split(new string[] { "</b></size>\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string result = "";
        for (int i = 0; i < phrases.Length; i++)
        {
            if(i< phrases.Length - 1)
            {
                phrases[i] += "</b></size>\n\n";
            }
            string[] words = phrases[i].Replace("\n", " ").Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
            int lettersInString = 0;
            foreach (var word in words)
            {
                int wordLength = GetWorldLength(word);
                int useSpace = lettersInString > 0 ? 1 : 0;

                if (lettersInString > 0 && lettersInString + wordLength > LetterPerString)
                {
                    useSpace = 0;
                    lettersInString = 0;
                    result += "\n";
                }

                result += useSpace == 1 ? " " : "";
                result += word;
                lettersInString += wordLength + useSpace;
            }

            if (i < phrases.Length - 1)
            {
                result += "\n\n";
            }
        }

        if (notification.UseCustomLineSpacing)
        {
            notification.text.lineSpacing = notification.LineSpacing;
        }
        notification.text.text = result;
    }

    private int GetWorldLength(string s)
    {
        int count = 0;
        bool findNonLiteralSymbol = false;
        for (int i = 0; i < s.Length; i++)
        {
            findNonLiteralSymbol |= s[i] == '<';

            if (!findNonLiteralSymbol)
            {
              //  Debug.Log(s[i]);
                count++;
            }

            findNonLiteralSymbol &= s[i] != '>';

        }

        return count;
    }
}
