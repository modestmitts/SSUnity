using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        text.CrossFadeAlpha(0, 0.8f, true);
    }

    void OnDisable()
    {
        text.CrossFadeAlpha(1.0f, 0.0f, true);
        
    }
}
