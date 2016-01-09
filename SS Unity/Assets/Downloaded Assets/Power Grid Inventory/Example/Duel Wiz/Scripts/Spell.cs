using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PowerGridInventory;

[RequireComponent(typeof(PGISlotItem))]
public class Spell : MonoBehaviour
{
    public enum SpellType
    {
        Attack,
        Defense,
        Status,
        Heal,
    }

    public enum SpellElement
    {
        None,
        Fire,
        Air,
        Earth,
        Water,
    }

    public enum StatusType
    {
        Speed,
        Resist,
        Burn,
        Pierce,
    }

    //public ParticleSystem PreCastEffect;
    public ParticleSystem CastEffect;

    [Description(3)]
    public string Description = "Default spell description.";
    public string Name = "Default Spell Name";
    public int LevelRequirement;
    public int Cost;
    public SpellType Type;
    public SpellElement Element;
    public int Strength = 5;
    public int Duration = 1;
    public bool TargetsCaster = false;
    public StatusType StatusClass = StatusType.Speed;
    public float Lob = 0.0f;
    public float Grav = 0.0f;
    public float Speed = 10.0f;
    public Sprite Icon;
    public AudioClip Sound;

    PGISlotItem Item;
    AudioSource Sfx;

    void Start()
    {
        Item = GetComponent<PGISlotItem>();
        Item.Icon = Icon;
        Sfx = GameObject.FindGameObjectWithTag("Sfx").GetComponent<AudioSource>();
    }

    /// <summary>
    /// This will determine the effect of the spell based on its stats, and the
    /// stats of the caster and target. It will automatically destroy itself when complete.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="enemy"></param>
    public int Cast(Entity caster, Entity enemy, Text casterText, Text enemyText)
    {
        if (Sfx != null)
        {
            Sfx.clip = Sound;
            Sfx.Play();
        }
        Entity target = (this.TargetsCaster) ? caster : enemy;
        Text targetText = (this.TargetsCaster) ? casterText : enemyText;

        switch(Type)
        {
            case SpellType.Attack:
                {
                    //simple calc for damage
                    int pierce = (caster.PierceRoundsLeft > 0) ? caster.PierceStr : 0;
                    int shield = (enemy.ShieldRoundsLeft > 0) ? enemy.ShieldStr : 0;
                    int dmg = this.Strength - shield;
                    dmg += (int)((float)(dmg) * (float)((float)pierce/100.0f));//convert to percentage and apply that to damage
                    dmg = Random.Range(dmg - (dmg / 2), dmg + (dmg / 2));
                    //add some synergy if this spell element is equal to burn type
                    if (enemy.LastBurnType == this.Element) dmg += (int)((float)dmg * 0.10f);
                    if (dmg <= 0) dmg = 1;
                    enemy.CurrentHealth -= dmg;

                    enemyText.text = dmg.ToString();
                    enemyText.color = Color.red;
                    enemyText.gameObject.SetActive(true);
                    return dmg;
                }
            case SpellType.Defense:
                {
                    caster.ShieldRoundsLeft = this.Duration;
                    caster.ShieldStr = this.Strength;

                    casterText.text = "Shielded";
                    casterText.color = Color.green;
                    casterText.gameObject.SetActive(true);
                    return caster.ShieldStr;
                }
            case SpellType.Status:
                {
                    //NOTE: Burn is applied to the to the enemy, not the caster
                    if (this.StatusClass == StatusType.Burn)
                    {
                        //Let's make burn damage stack! >:)
                        //make it even worse if it's the same type as last time
                        if (enemy.LastBurnType == this.Element && enemy.BurnStr > 0) enemy.BurnStr += this.Strength * 2;
                        else enemy.BurnStr += this.Strength;
                        enemy.LastBurnType = this.Element;

                        //but duration doesn't get added until we have none left
                        if (enemy.BurnRoundsLeft < 1) enemy.BurnRoundsLeft = this.Duration;
                        else enemy.BurnRoundsLeft++;

                        enemyText.text = "Afflicted";
                        enemyText.color = Color.yellow;
                        enemyText.gameObject.SetActive(true);
                        return enemy.BurnStr;
                    }
                    else if(this.StatusClass == StatusType.Pierce)
                    {
                        caster.PierceRoundsLeft = this.Duration;
                        caster.PierceStr = this.Strength;

                        casterText.text = "Amped";
                        casterText.color = Color.blue;
                        casterText.gameObject.SetActive(true);
                        return caster.PierceStr;
                    }
                    else if(this.StatusClass == StatusType.Speed)
                    {
                        //this can target caster or enemey
                        target.SpeedRoundsLeft = this.Duration;
                        target.SpeedStr = this.Strength;

                        if (this.Strength > 0)
                        {
                            targetText.color = Color.blue;
                            targetText.text = "Haste";
                        }
                        else
                        {
                            targetText.color = Color.yellow;
                            targetText.text = "Slow";
                        }
                        targetText.gameObject.SetActive(true);

                        return target.SpeedStr;
                    }
                    else if(this.StatusClass == StatusType.Resist)
                    {
                        //this can target cast or enemy (if used with negative numbers
                        //it basically acts as another kind of shield with a very different effect)
                        target.ResistRoundsLeft = this.Duration;
                        target.ResistStr = this.Strength;

                        if (this.Strength > 0)
                        {
                            targetText.color = Color.blue;
                            targetText.text = "Resist Up";
                        }
                        else
                        {
                            targetText.color = Color.yellow;
                            targetText.text = "Resist Lowered";
                        }
                        targetText.gameObject.SetActive(true);
                        return target.ResistStr;
                    }
                    break;
                }
            case SpellType.Heal:
                {
                    //healing is strengthened by 'pierce'
                    //FUTURE NOTE: Would it be interesting to make 'shields' also weaken healing?
                    int pierce = (caster.PierceRoundsLeft > 0) ? caster.PierceStr : 0;
                    int dmg = this.Strength + pierce;
                    dmg = Random.Range(dmg - (dmg / 2), dmg + (dmg / 2));
                    if (dmg <= 0) dmg = 1;
                    target.CurrentHealth += dmg;
                    targetText.color = Color.green;
                    targetText.text = "+" + dmg;
                    targetText.gameObject.SetActive(true);
                    return dmg;
                }
        }

        return 0;
    }

    /// <summary>
    /// Removes this spell from it's associated model and then destroys it.
    /// </summary>
    public void DiscardSpell()
    {
        if(Item != null && Item.Model != null)
        {
            Item.Model.Drop(Item);
            GameObject.DestroyObject(this.gameObject);
        }
    }
}
