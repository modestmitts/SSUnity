﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleButton : Toggle
{
    new void Start()
    {
        base.Start();
        HighlightEffect();
    }

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        HighlightEffect();
        
    }

    void HighlightEffect()
    {
        // override the color such that the toggle state of the button is obvious
        // by its color. 
        if (isOn)
        {
            image.color = this.colors.pressedColor;
        }
        else
        {
            image.color = this.colors.normalColor;
        }
    }
}