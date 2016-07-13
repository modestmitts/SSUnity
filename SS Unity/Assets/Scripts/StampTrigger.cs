using UnityEngine;
using System.Collections;

public class StampTrigger : MonoBehaviour 
{
    public float Delay = 0.25f;
    private GameObject AccessIcon;
    private Transform PlayerTrans;
    public bool PlayerClose;
    public bool LoggedIn;

    public GameObject shipInterface;
    public ShipData ShuttleInfo;


    void Start()
    {
        AccessIcon = this.transform.FindChild("Access Icon").gameObject;
        AccessIcon.SetActive(false);
        PlayerClose = false;
        LoggedIn = false;
        Canvas canvas = FindObjectOfType<Canvas>();
        shipInterface = canvas.transform.Find("Ship Interface").gameObject;
        if (shipInterface == null) print("Couldn't find Ship Interface");
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (!LoggedIn))
        {
            AccessIcon.SetActive(true);
            PlayerTrans = other.gameObject.transform;
            StartCoroutine(Blink(true));
            PlayerClose = true;
        }     
        
    }   

    void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (!LoggedIn))
        {
            PlayerTrans = null;
            StartCoroutine(Blink(false));            
            AccessIcon.SetActive(false);
            PlayerClose = false;
        }
        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerClose = true;
            if (!LoggedIn)
            {
                AccessIcon.SetActive(true);
                PlayerTrans = other.gameObject.transform;
            }
        }  
    }

    void Update()
    {
        if (!LoggedIn)
        {
            if (PlayerClose)
            {
                Vector3 Position = new Vector3(PlayerTrans.position.x, this.transform.position.y, PlayerTrans.position.z);
                transform.LookAt(Position);
            }
        }    
    }

    // This is called when the Ship Access Icon is clicked on with Player Raycast
    // It activates the Ship Interface and calls the Icon Ping function (
    public void OpenInterface()
    {
        shipInterface.SetActive(true);
        shipInterface.GetComponent<ShipInterfaceControl>().IconPing(ShuttleInfo, this.gameObject);
        AccessIcon.SetActive(false);
    }

    public void AccessToggle(bool isOn)
    {
        AccessIcon.SetActive(isOn);
    }

    public void LogIn()
    {
        LoggedIn = true;
    }

    public void LogOut()
    { 
        LoggedIn = false;
        //if (OnTriggerStay(Collider other))// If Player is within collider, this should turn on access icon
        // If not, Icon should not be visible.
    }
    IEnumerator Blink(bool onoff)
    {
            SpriteRenderer SR = AccessIcon.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(Delay);
            SR.enabled = !SR.enabled;
            yield return new WaitForSeconds(Delay * 0.75f);
            SR.enabled = !SR.enabled;
            yield return new WaitForSeconds(Delay * 0.5f);
            SR.enabled = !SR.enabled;
            yield return new WaitForSeconds(Delay * 0.25f);
            SR.enabled = onoff;           
    }




}
