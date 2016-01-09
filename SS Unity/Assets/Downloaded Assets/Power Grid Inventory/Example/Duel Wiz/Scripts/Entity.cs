using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using PowerGridInventory;


[RequireComponent(typeof(PGIModel))]
public class Entity : MonoBehaviour
{
    public enum ClassType
    {
        Wizard,
        Mage,
        Warlock,
        Sorceress,
        Witch,
    }

    public ClassType Type = ClassType.Wizard;
    public int DefaultStartingHealth = 100;
    public int DefaultStartingGold = 100;
    [Range(0.1f, 3.0f)]
    public float DifficultyScale = 1.0f;
    public int Speed;
    public Image Avatar;

    
    [HideInInspector]
    public Spell.SpellElement LastBurnType = Spell.SpellElement.None;
    [HideInInspector]
    public int ResistRoundsLeft = 0;
    [HideInInspector]
    public int ShieldRoundsLeft = 0;
    [HideInInspector]
    public int BurnRoundsLeft = 0;
    [HideInInspector]
    public int PierceRoundsLeft = 0;
    [HideInInspector]
    public int SpeedRoundsLeft = 0;

    [HideInInspector]
    public Spell.StatusType BuffType;
    [HideInInspector]
    public int ResistStr = 0;
    [HideInInspector]
    public int ShieldStr = 0;
    [HideInInspector]
    public int BurnStr = 0;
    [HideInInspector]
    public int PierceStr = 0;
    [HideInInspector]
    public int SpeedStr = 0;

    
    //[HideInInspector]
    public int CurrentHealth;
    //[HideInInspector]
    public float MaxHealth;
    //[HideInInspector]
    public int Gold;
    //[HideInInspector]
    public List<Spell> MobSpells = new List<Spell>();



    /// <summary>
    /// Describes common stats for generating mobs based on classtype.
    /// </summary>
    [Serializable]
    public class ClassInfo
    {
        public ClassType Type;
        public Sprite Avatar;
        public Color Tint;
        public int HealthMod;
        public Spell.SpellElement[] PreferredElements;
        public List<Spell> SpellsList;
    }
    public ClassInfo[] MobLists;

    [HideInInspector]
    public PGIModel Inventory;

    void Start()
    {
        Inventory = GetComponent<PGIModel>();
        ResetForGame();
    }

    /// <summary>
    /// Drops duration for all status effects by 1.
    /// Should be used at the end of each round.
    /// 
    /// NOTE: Remember to give all 'duration' effects 1 more round
    /// than needed or they won't last to the end of the turn.
    /// </summary>
    public void IncrementRoundEffects()
    {
        if (ResistRoundsLeft > 0) ResistRoundsLeft--;
        else ResistRoundsLeft = 0;
        if (ShieldRoundsLeft > 0) ShieldRoundsLeft--;
        else ShieldStr = 0;
        if (BurnRoundsLeft > 0) BurnRoundsLeft--;
        else
        {
            LastBurnType = Spell.SpellElement.None;
            BurnStr = 0;
        }
        if (PierceRoundsLeft > 0) PierceRoundsLeft--;
        else PierceStr = 0;
        if (SpeedRoundsLeft > 0) SpeedRoundsLeft--;
        else SpeedStr = 0;
        

    }

    /// <summary>
    /// Ensures everything is ready for the next battle scene.
    /// </summary>
	public void ResetForBattle()
    {
        //this is designed to set health for a player (hard difficulty = less life)
        //use 'GenerateStats()' to set health for mobs.
        //MaxHealth = (int)(((float)GetTotalGoldAssets()/2) * (1.0f / DifficultyScale));

        //player health goes up by 1/2 each time this is called
        //MaxHealth += 0.5f;
        MaxHealth = DefaultStartingHealth;
        CurrentHealth = (int)MaxHealth;

        ResistStr = 0;
        ShieldStr = 0;
        BurnStr = 0;
        PierceStr = 0;
        SpeedStr = 0;

        LastBurnType = Spell.SpellElement.None;

        ResistRoundsLeft = 0;
        ShieldRoundsLeft = 0;
        BurnRoundsLeft = 0;
        PierceRoundsLeft = 0;
        SpeedRoundsLeft = 0;
    }

    /// <summary>
    /// Resets all stats to new-game conditions.
    /// </summary>
    public void ResetForGame()
    {
        MaxHealth = (float)DefaultStartingHealth;
        Gold = DefaultStartingGold;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetSpeed()
    {
        return Speed + SpeedStr;
    }

    /// <summary>
    /// Returns total gold assets including gold and cost of all equipped spells.
    /// This will be used often as a measure of experience the player has.
    /// </summary>
    /// <returns></returns>
    public int GetTotalGoldAssets()
    {
        int gold = 0;
        if (Inventory != null)
        {
            foreach (var item in Inventory.AllItems)
            {
                var spell = item.GetComponent<Spell>();
                gold += spell.Cost;
            }
        }
        if(MobSpells != null)
        {
            foreach(var spell in MobSpells)
            {
                gold += spell.Cost;
            }
        }

        return gold + this.Gold;
    }

    /// <summary>
    /// Returns all spell available to this entity.
    /// </summary>
    /// <returns></returns>
    public int GetSpellCount()
    {
        int spellCount = 0;
        if (Inventory != null)
        {
            spellCount += Inventory.AllItems.Length;
        }
        if (MobSpells != null)
        {
            spellCount += MobSpells.Count;
            
        }

        return spellCount;
    }

    /// <summary>
    /// Infers a 'level' based on total wealth and HP.
    /// </summary>
    /// <returns></returns>
    int GetEffectiveLevel()
    {
        int lev = GetTotalGoldAssets() % 500;
        //Debug.Log("Effective Level for: " + gameObject.name + "  " + lev);
        return 1 + lev;
    }

    /// <summary>
    /// Aids in converting gold integer value to a comma-seperated string.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string CommaSeparatedString(int count)
    {
        return String.Format("{0:#,##0}", count);
    }

    /// <summary>
    /// Intended for use on mob entity. It generates a mob and all stats and spells
    /// based on the 'Total Gold Assets' that is provided. Gold is used as an
    /// indication of xp for the player in this game.
    /// </summary>
    /// <param name="xp"></param>
    public void GenerateStats(Entity opponent)
    {
        CurrentHealth = (int)MaxHealth;

        ResistStr = 0;
        ShieldStr = 0;
        BurnStr = 0;
        PierceStr = 0;
        SpeedStr = 0;

        ResistRoundsLeft = 0;
        ShieldRoundsLeft = 0;
        BurnRoundsLeft = 0;
        PierceRoundsLeft = 0;
        SpeedRoundsLeft = 0;

        //Debug.Log("Mob level: " + level);
        //first, determine what kind of mob we are making
        this.Type = (ClassType)UnityEngine.Random.Range(0, (int)ClassType.Witch+1);
        
        foreach(var mob in MobLists)
        {
            if(mob.Type == Type)
            {
                //give this mob a spell list based on the mob type's preferred spells
                MobSpells = mob.SpellsList;
                this.Type = mob.Type;
                //generate health for mobs
                //MaxHealth = (int)((float)((float)opponent.GetTotalGoldAssets() / 2) * ((float)DifficultyScale / 2));

                this.CurrentHealth += mob.HealthMod;
                Avatar.sprite = mob.Avatar;
                Avatar.color = mob.Tint;
                break;
            }
        }

        

    }


}
