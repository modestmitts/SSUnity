using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ToggleInventory : MonoBehaviour 
{
    public GameObject player;
    public GameObject invPanel;
    public Image ret;
    private bool invActive;
 
    private RectTransform InvRT;

    // Inventory visible transform Top = -200, Bottom = 20
    // Inventory hidden Top -16, Bottom -164

    
    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            InvRT = transform as RectTransform;
        }

        invActive = false;        
        Cursor.visible = false;
        invPanel.SetActive(invActive);
                
    }

    private object Find(string p)
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {

            /// Get
            if (InvRT == null) return;

            
            ///
            invActive = !invActive;
            invPanel.SetActive(invActive);
            ret.enabled = !ret.enabled;
            Cursor.visible = !Cursor.visible;

            // Disable Player movement
            player.GetComponent<CharacterMotor>().enabled = !invActive;
            player.GetComponent<MouseLook>().enabled = !invActive;                
        }



    }

	
}
