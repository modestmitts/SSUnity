using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PublicPageData : MonoBehaviour {

    private Image img;    
    public Sprite sprt;

    /*
    public string TitleText;
    public string OwnerText;
    public string IDText;
    public string SecStat;
    public string OverMsg;
     */ 
    void Start()
    {
        img = this.transform.Find("Ship Owner Icon").gameObject.GetComponent<Image>();
        

        /*
        temp = this.transform.Find("Ship Title").gameObject;
        TitleText = temp.GetComponent<Text>().text;

        temp = this.transform.Find("Ship Owner").gameObject;
        OwnerText = temp.GetComponent<Text>().text;

        temp = this.transform.Find("Ship ID").gameObject;
        IDText = temp.GetComponent<Text>().text;

        // I will need to put in a color change here, depending on the status
        temp = this.transform.Find("Security Status").gameObject;
        SecStat = temp.GetComponent<Text>().text;

        temp = this.transform.Find("Overriding Status").gameObject;
        OverMsg = temp.GetComponent<Text>().text;
         */ 
    }

}
