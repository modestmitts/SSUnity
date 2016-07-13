using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginIndData : MonoBehaviour {

    private Image img;
    private Text TitleText;
    private Text IDText;

	// Use this for initialization
    void Start()
    {
        GameObject temp = this.transform.Find("Ship Owner Icon").gameObject;
        img = temp.GetComponent<Image>();

        temp = this.transform.Find("Ship Title").gameObject;
        TitleText = temp.GetComponent<Text>();

        temp = this.transform.Find("Ship ID").gameObject;
        IDText = temp.GetComponent<Text>();       
    }

    public void PushAndFillIndicator(Sprite spr, string Title, string ID)
    {
        img.sprite = spr;
        TitleText.text = Title;
        IDText.text = ID;
    }

    public void Erase()
    {
        img.sprite = null;
        TitleText.text = null;
        IDText.text = null;
    }
}
