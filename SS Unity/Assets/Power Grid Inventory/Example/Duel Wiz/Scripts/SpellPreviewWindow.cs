using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PowerGridInventory;

public class SpellPreviewWindow : MonoBehaviour
{
    public bool DisableHover;
    public Text SpellName;
    public Text SpellDescription;
    public Text SpellCost;
    public Text SpellStr;
    public Text SpellDur;

    public void SetPreviewDetails(Spell spell)
    {
        SpellName.text = spell.Name;
        SpellDescription.text = spell.Description;
        SpellCost.text = Entity.CommaSeparatedString(spell.Cost);
        SpellStr.text = spell.Strength.ToString();
        if (spell.Type == Spell.SpellType.Status || spell.Type == Spell.SpellType.Defense)
            SpellDur.text = spell.Duration.ToString();
        else SpellDur.text = "-";
    }

    /// <summary>
    /// Hook this to the PGIView.OnEndHoverSlot.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="slot"></param>
    public void OnHoverSpell(PointerEventData data, PGISlot slot)
    {
        if (slot.Item != null && !DisableHover)
        {
            RectTransform rect = transform as RectTransform;
            //Vector3 pos = rect.position;

            if (PowerGridInventory.Utility.PGIPointer.GetPosition().x > Screen.width / 2)
            {
                //rect.position = new Vector3(pos.x, -1.5f, pos.z);
                rect.anchoredPosition3D = new Vector3(-rect.rect.width/2, rect.anchoredPosition3D.y, 0.0f);
            }
            else rect.anchoredPosition3D = new Vector3(rect.rect.width/2, rect.anchoredPosition3D.y, 0.0f);
            //else rect.position = new Vector3(pos.x, 1.5f, pos.z);
            Spell spell = slot.Item.GetComponent<Spell>();
            gameObject.SetActive(true);
            SetPreviewDetails(spell);
        }
    }

    /// <summary>
    /// Hook this to the PGIView.OnHoverSlot event.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="slot"></param>
    public void OnEndHoverSpell(PointerEventData data, PGISlot slot)
    {
        gameObject.SetActive(false);
    }
}
