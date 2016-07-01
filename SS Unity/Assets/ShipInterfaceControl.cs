using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipInterfaceControl : MonoBehaviour 
{

    /* Ship Interface Controls
     * This does several things
     * 1. Recieves the ShipData Script for the currently logged in ship.
     * 2. Disseminates the ShipData and fills the Data tabs accordingly
     * 3. Controls the state between the Public Page, Login Page and the Data Page
     */
 
    public bool LoggedIn;
    public GameObject PublicPage;
    public GameObject DataTabs;
    public GameObject SecurityPage;
    public GameObject LogoutDialogue;

    private ShipData QSD; /// Quick Ship Data (Not logged in)
    private ShipData SD; // Ship Data you've logged into
    public Sprite spt;

    public enum DataState {NoShip, Public, LogOutCheck, Security, DataTabs};
    private bool dataFilled;  // Have the Datatabs been filled with relevant data?
    public DataState DState;
    private DataState newState;

    private StampTrigger STrigger;
    private StampTrigger QSTrigger;

    private GameObject ShipLOIndicator;

    void Start()
    {
        DState = DataState.NoShip;
        dataFilled = false;
    }

    // THis function displays the Public Page, and then calls the Icon Ping on GUIcontrol to halt player movement
    public void IconPing(ShipData incoming, GameObject ST)
    {
        QSD = incoming; // Get the new ship's data object
        QSTrigger = ST.GetComponent<StampTrigger>();

        newState = DataState.Public; //Open the ship's Public Page
        this.GetComponentInParent<GUIControl>().GUICon(true); // Turn off player movement

        print("Shutting off the Access Icon");
        QSTrigger.GetComponent<StampTrigger>().LogIn();
    }

    /*
    public void AccessIconClosed()
    {
        STrigger.AccessToggle(true);
        SD = null;
        STrigger = null;
        newState = DataState.NoShip;
    }
     */

    public void LogOutButton()
    {
        newState = DataState.LogOutCheck;
    }

    public void LogOut()
    {

        LoggedIn = false;
        STrigger.LogOut();
        
        ShipLOIndicator.SetActive(false);
        
        SD = null;
        STrigger = null;
        newState = DataState.NoShip;
    }

    public void LogIn()
    {
        print("Login SIC");
        // If you are logged into a ship, then this will open the "Check Logout" dialogue
        // If you check YES, then it will run Logout, and then Login to the Security
        // If you check No, then it should go back to the public page
        if (LoggedIn) newState = DataState.LogOutCheck;    
        else newState = DataState.Security;
    }

    public void CheckLog(bool logout)
    {
        LogoutDialogue.SetActive(false);
        // If Logout, then are two conditions you can be in, (A) log out button, or trying to (B) login something else.
        if (logout)
        {
            if (QSD == null) // You hit the logout button
            {
                LogOut();
                this.GetComponentInParent<GUIControl>().GUICon(false); // Turn off player movement
            }
            else // You are logging into a different shipp
            {
                LogOut();
                LogIn();
            }
        }
        else // YOu want to stay logged in
        {
            newState = DataState.Public;
        }        
    }

    // This is called when the "Close" is hit
    public void ClearQuickShipData()
    {
        QSD = null;
        newState = DataState.NoShip;
    }

    public void CheckPassword(string PWenter)
    {
        if (PWenter == QSD.password)
        {
            LoggedIn = true;
            SD = QSD;
            QSD = null;
            STrigger = QSTrigger;
            // Turn on "Ship Login Indicator"
            PushLoginIndicatorData();
            // Turn off "Access Icon"
            STrigger.GetComponent<StampTrigger>().LogIn();
            STrigger.GetComponent<StampTrigger>().AccessToggle(false);
            newState = DataState.DataTabs;
            this.GetComponentInParent<GUIControl>().LogIn(); 
        }
        else newState = DataState.Public;    
    }

    public void CancelPassword()
    {
        newState = DataState.Public;
    }

    void Update()
    {
        if (DState != newState)
        {
            switch (newState) 
            {
                case DataState.NoShip : 
                {
                    if (QSTrigger) QSTrigger.GetComponent<StampTrigger>().LogOut();
                        
                    SD = null; // I wonder if I have to do anything else here
                    PublicPage.SetActive(false);
                    DataTabs.SetActive(false);
                    SecurityPage.SetActive(false);
                    dataFilled = false;
                    DState = newState;
                    break;
                }

                case DataState.DataTabs:
                {
                    if (SD)
                    {
                        PublicPage.SetActive(false);
                        SecurityPage.SetActive(false);
                        DataTabs.SetActive(true);
                        PushShipData();
                        DState = newState;
                        break;
                    }
                    else newState = DataState.NoShip;
                    break;
                }

                case DataState.Public:
                {
                    DataTabs.SetActive(false);
                    SecurityPage.SetActive(false);
                    PublicPage.SetActive(true);
                    PushPublicData();                                 
                    DState = newState;
                    break;
                }

                case DataState.LogOutCheck:
                {
                    LogoutDialogue.SetActive(true);               

                    break;
                }

                case DataState.Security:
                {
                    if (QSD.password != "NA")
                    {
                        PublicPage.SetActive(false);
                        DataTabs.SetActive(false);
                        
                        // Security page will have the Password dialogue, which I am hoping can be pure UI
                        SecurityPage.SetActive(true);
                        DState = newState;
                    }
                    else
                    {
                        // Open Dialogue asking if they want to log out of current ship or not
                        LoggedIn = true;
                        SD = QSD;
                        QSD = null;
                        STrigger = QSTrigger;
                        // Turn on "Ship Login Indicator"
                        PushLoginIndicatorData();
                        // Turn off "Access Icon"
                        STrigger.GetComponent<StampTrigger>().LogIn();
                        STrigger.GetComponent<StampTrigger>().AccessToggle(false);
                        newState = DataState.DataTabs;
                        this.GetComponentInParent<GUIControl>().LogIn(); 
                    }
                    break;
                }

                default: return;             
            }    
        }
    }

    private void PushPublicData()
    {
        PublicPage.transform.Find("Ship Title").gameObject.GetComponent<Text>().text = QSD.shipTitle;
        PublicPage.transform.Find("Ship Owner Icon").gameObject.GetComponent<Image>().sprite = QSD.shipIcon;
        PublicPage.transform.Find("Ship Owner").gameObject.GetComponent<Text>().text = QSD.shipOwner;
        PublicPage.transform.Find("Ship ID").gameObject.GetComponent<Text>().text = QSD.shipID;
        PublicPage.transform.Find("Security Status").gameObject.GetComponent<Text>().text = QSD.shipSecStatus;
        PublicPage.transform.Find("Overriding Status").gameObject.GetComponent<Text>().text = QSD.shipOverMsg;
    }

    private void PushLoginIndicatorData()
    {
        ShipLOIndicator = this.transform.parent.transform.Find("Ship Login Indicator").gameObject;
        ShipLOIndicator.SetActive(true);
        ShipLOIndicator.transform.Find("Ship Owner Icon").gameObject.GetComponent<Image>().sprite = SD.shipIcon;
        ShipLOIndicator.transform.Find("Ship Title").gameObject.GetComponent<Text>().text = SD.shipTitle;
        ShipLOIndicator.transform.Find("Ship ID").gameObject.GetComponent<Text>().text = SD.shipID;
    }

    private void PushShipData()
    {
        PublicPage.transform.Find("Ship Title").gameObject.GetComponent<Text>().text = SD.shipTitle;
        PublicPage.transform.Find("Ship Owner Icon").gameObject.GetComponent<Image>().sprite = SD.shipIcon;
        PublicPage.transform.Find("Ship ID").gameObject.GetComponent<Text>().text = SD.shipID;
    }
}
