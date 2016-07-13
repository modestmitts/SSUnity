using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GetShipIcon : MonoBehaviour 
{
    Image img;
    
    void Start()
    {
        img = this.GetComponent<Image>();

    }

    public void PushImage(Sprite spr)
    {
        img.sprite = spr;
    }

}
