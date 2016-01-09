using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PowerGridInventory;

public class Shop : MonoBehaviour
{
    
    public SpellPreviewWindow ShopPreview;
    public SpellPreviewWindow SpellbokPreview;
    public AudioClip BuyClip;
    public AudioClip SellClip;
    public AudioClip ErrorClip;
    

    Entity PlayerData;
    Entity MobData;
    PGIModel PlayerInventory;
    PGIModel ShopInventory;
    GameState StateManager;
    AudioSource SoundSource;
    Text GoldText;

    void Awake()
    {
        SoundSource = GetComponentInParent<AudioSource>();

        PlayerData = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        MobData = GameObject.FindGameObjectWithTag("Mob").GetComponent<Entity>();
        PlayerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PGIModel>();
        ShopInventory = GameObject.FindGameObjectWithTag("Shop").GetComponent<PGIModel>();
        StateManager = GameObject.FindGameObjectWithTag("Game State Manager").GetComponent<GameState>();
    }

    void OnEnable()
    {
        if(GoldText == null) GoldText = GameObject.Find("Gold Count").GetComponent<Text>(); ;
        GoldText.text =  Entity.CommaSeparatedString(PlayerData.Gold);

        SpellbokPreview.DisableHover = false;
        ShopPreview.DisableHover = false;
    }

    /// <summary>
    /// Hook this to the PGIOnClickSlot event of the player's inventory.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="slot"></param>
    public void OnSellSpell(PointerEventData data, PGISlot slot)
    {
        //don't try to buy or sell if the shop obvject isn't even open
        if (!this.gameObject.activeSelf) return;
        if (slot == null || slot.Item == null) return;
        Spell spell = slot.Item.GetComponent<Spell>();
        if (spell == null) return;

        if (slot.Model != PlayerInventory)
        {
            //trying to sell from an inventory that is not the player's
            PlaySound(ErrorClip);
            return;
        }

        //if we don't do this our spellbook preview window will stay open
        SpellbokPreview.OnEndHoverSpell(null, slot);

        PlayerInventory.Remove(slot.Item, true);
        PlayerData.Gold += spell.Cost/2;
        GoldText.text = Entity.CommaSeparatedString(PlayerData.Gold);
        PlaySound(SellClip);

        GameObject.Destroy(slot.Item.gameObject);
        StateManager.CheckForGameOver(PlayerData, MobData);
    }

    /// <summary>
    /// Hook this to the PGIView.OnClickSlot event of the shop's inventory.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="slot"></param>
    public void OnBuySpell(PointerEventData data, PGISlot slot)
    {
        //don't try to buy or sell if the shop obvject isn't even open
        if (!this.gameObject.activeSelf) return;
        if (slot == null || slot.Item == null) return;
        Spell spell = slot.Item.GetComponent<Spell>();
        if (spell == null) return;

        if(slot.Model != ShopInventory)
        {
            //trying to buy from an inventory that is not the shop's
            PlaySound(ErrorClip);
            return;
        }
        
        if (PlayerInventory.HasRoomForItem(slot.Item, false, false) && PlayerData.Gold >= spell.Cost)
        {
            //make copy of the spell object and place *that* in the player's inventory
            GameObject go = GameObject.Instantiate(slot.Item.gameObject) as GameObject;
            PlayerInventory.Pickup(go.GetComponent<PGISlotItem>());

            //adjust other stats and play a confirmation sound
            PlayerData.Gold -= spell.Cost;
            GoldText.text = Entity.CommaSeparatedString(PlayerData.Gold);
            PlaySound(BuyClip);
        }
        else
        {
            PlaySound(ErrorClip);
            return;
        }
    }

    /// <summary>
    /// Helper used to play audio clips.
    /// </summary>
    /// <param name="clip"></param>
    void PlaySound(AudioClip clip)
    {
        if(clip != null && SoundSource != null)
        {
            SoundSource.clip = clip;
            SoundSource.Play();
        }
    }

    
}
