using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour
{
    public GameObject player;
    public GameObject inventoryPanel;
    public GameObject shipInterface;
    public GameObject reticle;
    public GameObject loggedShipIcon;

    public bool loggedin;
  
    private RectTransform InvRT;

    // Inventory visible transform Top = -200, Bottom = 20
    // Inventory hidden Top -16, Bottom -164


    void Awake()
    {
        GUICon(false);
        inventoryPanel.SetActive(false);
        shipInterface.SetActive(false);
        loggedin = false;
        loggedShipIcon.SetActive(false);

    }

    private object Find(string p)
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventoryPanel.activeSelf)
            {                
                inventoryPanel.SetActive(true);
                shipInterface.SetActive(false);
                GUICon(true);
            }
            else
            {
                inventoryPanel.SetActive(false);
                shipInterface.SetActive(false);
                GUICon(false);            
            }
        }
        /// If Logged in, then hitting 'L' will open the ship interface
        if (Input.GetKeyDown(KeyCode.L) && (loggedin))
        {
            if (!shipInterface.activeSelf)
            {
                inventoryPanel.SetActive(false);
                shipInterface.SetActive(true);                       
                GUICon(true);
            }
            else
            {
                inventoryPanel.SetActive(false);
                shipInterface.SetActive(false);
                GUICon(false);
            }
        }
    }

    public void DataCloseButton()
    {
        inventoryPanel.SetActive(false);
        shipInterface.SetActive(false);
        GUICon(false);
    }

    // This disables player movement, hides the reticle and re-enables the mouse
    public void GUICon(bool GUIon)
    {
        player.GetComponent<CharacterMotor>().enabled = !GUIon;
        player.GetComponent<MouseLook>().enabled = !GUIon;
        reticle.SetActive(!GUIon);
        Cursor.visible = GUIon;
    }

    //Loggle is a toggle that is pinged when the Login button is pressed on the Public page of a ship
    // and when the player logs out or logs into a different ship
    ///THis denotes that the player is now logged and and the Access that this refers to is switched on or
    /// off depending on which this is.
    // 
    public void LogIn()
    {
        loggedin = true;
    }

    public void LogOut()
    {
        loggedin = false;
        GUICon(false);
    }
}
