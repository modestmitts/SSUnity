using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PowerGridInventory;

public class Battle : MonoBehaviour
{
    public Rigidbody2D Projectile;
    public Text Earnings;
    public Text MobHP;
    public Text PlayerHP;
    public Text MobText;
    public Text PlayerText;
    public Text ResultsText;
    public Text BattleText;
    public SpellSelectionWindow SpellSelection;
    public SpellPreviewWindow SpellPreview;
    public PGISlot PlayerSpellSlot;
    GameObject PlayerAvatar;
    GameObject MobAvatar;

    public PGISlot PlayerSelectionSlot;
    public int SpellCountdown = 5;
    public int VictoryReward = 150;
    public int DefeatPunish = 0;
    public float AttackMoveDist = 1.0f;

    int CurrentCountdown;
    //PGIModel PlayerInventory;
    Entity PlayerData;
    Entity MobData;
    GameState StateManager;
    Phase BattlePhase;

    public enum Phase
    {
        PreBattle,
        SpellSelect,
        PreAttack,
        Attack1,
        Attack2,
        PostAttack,
        BattleEnd,
    }


    void Awake()
    {
        //PlayerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PGIModel>();
        PlayerData = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        MobData = GameObject.FindGameObjectWithTag("Mob").GetComponent<Entity>();
        StateManager = GameObject.FindGameObjectWithTag("Game State Manager").GetComponent<GameState>();
    }

    void OnEnable()
    {
        if (StateManager.CurrentState == GameState.State.Battle)
        {
            BattlePhase = Phase.PreBattle;
            SpellSelection.gameObject.SetActive(false);
            StartCoroutine(BattleHeartBeat());
        }
    }

    IEnumerator BattleHeartBeat()
    {
        float waitTime = 1.0f;
        bool Loop = true;
        Victory status = Victory.Continue;

        Entity first = MobData, second = PlayerData;

        while(Loop)
        {
            if (!this.gameObject.activeSelf) break;

            switch(BattlePhase)
            {
                case Phase.PreBattle:
                    {
                        //setup mob
                        //NOTE: Both of these next two methods set the max health.
                        //'Reset' sets the health in a way that is designed for the player.
                        //'Generate' sets the health in a way intended for mobs.
                        PlayerData.ResetForBattle();
                        MobData.GenerateStats(PlayerData);
                        SpellPreview.DisableHover = true;
                        waitTime = 0.0f;
                        BattlePhase = Phase.SpellSelect;

                        PlayerHP.text = PlayerData.CurrentHealth.ToString();
                        MobHP.text = MobData.CurrentHealth.ToString();

                        //display battle text and play a sound
                        ResetAllBattleText();
                        BattleText.gameObject.SetActive(true);
                        BattleText.text = "Vs " + MobData.Type.ToString() + "";
                        yield return new WaitForSeconds(2.0f);
                        BattleText.gameObject.SetActive(false);
                        break;
                    }
                case Phase.SpellSelect:
                    {
                        //enable the spell selection window and start the countdown timer
                        waitTime = 0.0f;
                        SpellPreview.DisableHover = false;
                        CurrentCountdown = SpellCountdown;
                        SpellSelection.gameObject.SetActive(true);
                        SpellSelection.CountdownText.text = CurrentCountdown.ToString();
                        SpellSelection.CountdownText.gameObject.SetActive(false);
                        SpellSelection.CountdownText.gameObject.SetActive(true); //this will reset the fade effect attached to it

                        while(CurrentCountdown >= 0)
                        {
                            yield return new WaitForSeconds(1.0f);
                            CurrentCountdown--;
                            SpellSelection.CountdownText.text = CurrentCountdown.ToString();
                            SpellSelection.CountdownText.gameObject.SetActive(false);
                            SpellSelection.CountdownText.gameObject.SetActive(true); //this will reset the fade effect attached to it
                        }

                        SpellSelection.gameObject.SetActive(false);
                        BattlePhase = Phase.PreAttack;
                        break;
                    }
                case Phase.PreAttack:
                    {
                        //setup timing, next phase, and hide unwanted windows
                        waitTime = 1.0f;
                        BattlePhase = Phase.Attack1;
                        SpellPreview.DisableHover = true;
                        SpellPreview.OnEndHoverSpell(null, null);

                        //Firgure out who will be attacking first
                        if(PlayerData.GetSpeed() == MobData.GetSpeed())
                        {
                            //switch each round if speeds are same
                            var temp = first;
                            first = second;
                            second = temp;
                        }
                        else
                        {
                            first = (PlayerData.GetSpeed() > MobData.GetSpeed()) ? PlayerData : MobData;
                            second = (PlayerData.GetSpeed() < MobData.GetSpeed()) ? PlayerData : MobData;
                        }
                        
                        break;
                    }
                case Phase.Attack1:
                    {
                        ResetAllBattleText();
                        waitTime = 0.0f;

                        //move avatar forward
                        yield return new WaitForSeconds(0.5f);
                        Vector3 pos = first.Avatar.transform.position;
                        first.Avatar.transform.position = new Vector3(pos.x - (AttackMoveDist * first.Avatar.transform.localScale.x), pos.y, pos.z);
                        yield return new WaitForSeconds(1.0f);

                        //do attack and check for victory
                        if (!Attack(first, second)) yield return new WaitForSeconds(2.0f);
                        status = PostAttackCheck();
                        if(status == Victory.Continue) BattlePhase = Phase.Attack2;
                        else BattlePhase = Phase.BattleEnd;

                        //return pos, pause for spell effect, then update damage text
                        yield return new WaitForSeconds(2.0f);
                        first.Avatar.transform.position = pos;
                        PlayerHP.text = PlayerData.CurrentHealth.ToString();
                        MobHP.text = MobData.CurrentHealth.ToString();
                        ResetAllBattleText();
                        break;
                    }
                case Phase.Attack2:
                    {
                        ResetAllBattleText();
                        waitTime = 0.0f;

                        //move avatar forward
                        yield return new WaitForSeconds(0.5f);
                        Vector3 pos = second.Avatar.transform.position;
                        second.Avatar.transform.position = new Vector3(pos.x - (AttackMoveDist*second.Avatar.transform.localScale.x), pos.y, pos.z);
                        yield return new WaitForSeconds(1.0f);

                        //do attack and check for victory
                        if (!Attack(second, first)) yield return new WaitForSeconds(2.0f);
                        status = PostAttackCheck();
                        if (status == Victory.Continue) BattlePhase = Phase.PostAttack;
                        else BattlePhase = Phase.BattleEnd;

                        //return pos, pause for spell effect, then update damage text
                        yield return new WaitForSeconds(2.0f);
                        second.Avatar.transform.position = pos;
                        PlayerHP.text = PlayerData.CurrentHealth.ToString();
                        MobHP.text = MobData.CurrentHealth.ToString();
                        ResetAllBattleText();
                        break;
                    }
                case Phase.PostAttack:
                    {
                        ResetAllBattleText();
                        waitTime = 0.0f;

                        //Check for burn. If applied, give a slight delay for visuals
                        if (ApplyBurn())
                        {
                            yield return new WaitForSeconds(2.0f);
                            PlayerHP.text = PlayerData.CurrentHealth.ToString();
                            MobHP.text = MobData.CurrentHealth.ToString();
                            status = PostAttackCheck();
                            if (status == Victory.Continue) BattlePhase = Phase.SpellSelect;
                            else BattlePhase = Phase.BattleEnd;
                        }
                        else
                        {
                            BattlePhase = Phase.SpellSelect;

                            //check to see if player has anything in inventory
                            if (PlayerData.Inventory.AllItems.Length < 1 && status == Victory.Continue)
                            {
                                status = Victory.Mob;
                                BattlePhase = Phase.BattleEnd;
                                BattleText.gameObject.SetActive(true);
                                BattleText.text = "You have run out of spells!";
                                yield return new WaitForSeconds(2.0f);

                                break;
                            }
                        }

                        ResetAllBattleText();
                        PlayerData.IncrementRoundEffects();
                        MobData.IncrementRoundEffects();

                        //give a pause here before we start next round
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                case Phase.BattleEnd:
                    {
                        Loop = false;

                        ResetAllBattleText();
                        BattleText.gameObject.SetActive(true);
                        int earnings;
                        
                        if(status == Victory.Player)
                        {
                            earnings = VictoryReward + (int)MobData.MaxHealth - MobData.DefaultStartingHealth + PlayerData.CurrentHealth;
                            BattleText.text = "Victory";
                            ResultsText.text = "You were victorious.";
                            yield return new WaitForSeconds(2.0f);
                            BattleText.gameObject.SetActive(false);

                            PlayerData.Gold += earnings;
                            Earnings.text = "You earned " + earnings + " gold! Your enemies have gained strength.";

                            MobData.MaxHealth += 4.0f;
                            PlayerData.MaxHealth += 1.0f;
                        }
                        else if(status == Victory.Mob)
                        {
                            earnings = VictoryReward + (int)MobData.MaxHealth - MobData.DefaultStartingHealth;
                            BattleText.text = "Defeat";
                            ResultsText.text = "You were defeated.";
                            yield return new WaitForSeconds(2.0f);
                            BattleText.gameObject.SetActive(false);

                            PlayerData.Gold -= earnings;
                            Earnings.text = "You lost " + earnings + " gold!";

                            MobData.MaxHealth -= 2.0f;
                            PlayerData.MaxHealth -= 0.5f;
                        }
                        else
                        {
                            BattleText.text = "Tie";
                            ResultsText.text = "The duel was a tie.";
                            yield return new WaitForSeconds(2.0f);
                            BattleText.gameObject.SetActive(false);
                            Earnings.text = "You neither lost nor gained gold.";
                        }

                        if(!StateManager.CheckForGameOver(PlayerData, MobData))
                            StateManager.SetState(GameState.State.BattleReview);
                        break;
                    }
            }

            yield return new WaitForSeconds(waitTime);
        }

        BattlePhase = Phase.PreBattle;
    }
	
    enum Victory
    {
        Player,
        Mob,
        Tie,
        Continue,
    }

    void ResetAllBattleText()
    {
        BattleText.text = "";
        BattleText.gameObject.SetActive(false);

        MobText.text = "---";
        MobText.color = Color.white;
        MobText.gameObject.SetActive(false);

        PlayerText.text = "---";
        PlayerText.color = Color.white;
        PlayerText.gameObject.SetActive(false);

    }
    
    /// <summary>
    /// Applies duration burn damage. Bur is a bit counter-intuitive. It is stored
    /// as a status effect to the caster and then applied each turn to the opponent.
    /// </summary>
    /// <returns>Returns true if damage was dealt. This means we need to check for victory conditions again.</returns>
    bool ApplyBurn()
    {
        bool dmgApplied = false;
        
        if (MobData.BurnRoundsLeft > 0)
        {
            //TODO: Once 'resist' SpellSelection are added, this needs to Behaviour applied
            //int resist = (enemy.ResistRoundsLeft > 0) ? enemy.ResistStr : 0;
            //resist = (int)((float)this.Strength * ((float)resist / 100.0f));
            
            //apply resists to durn damage
            int dmg = MobData.BurnStr;
            dmg = Random.Range(dmg - (dmg / 2), dmg + (dmg / 2));
            BattleText.text = "Affliction!";
            BattleText.gameObject.SetActive(true);
            MobText.color = Color.red;
            MobText.text = dmg.ToString();
            MobText.gameObject.SetActive(true);
            MobData.CurrentHealth -= dmg;
            dmgApplied = true;
        }
        if (PlayerData.BurnRoundsLeft > 0)
        {
            int dmg = MobData.BurnStr;
            dmg = Random.Range(dmg - (dmg / 2), dmg + (dmg / 2));
            BattleText.text = "Affliction!";
            BattleText.gameObject.SetActive(true);
            PlayerText.color = Color.red;
            PlayerText.text = dmg.ToString();
            PlayerText.gameObject.SetActive(true);
            PlayerData.CurrentHealth -= dmg;
            dmgApplied = true;
        }
        
        return dmgApplied;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="target"></param>
    bool Attack(Entity agent, Entity target)
    {
        Spell spell = null;

        //Debug.Log(agent.gameObject.name + " is casting against" + target.gameObject.name);
        //if it is player attacking check for spell in Slot. if it is a mob, check for spell in spells list.
        if (agent == PlayerData)
        {
            if (PlayerSpellSlot.Item == null)
            {
                BattleText.gameObject.SetActive(true);
                BattleText.text = "You didn't cast a spell!";
                return false;
            }
            else spell = PlayerSpellSlot.Item.GetComponent<Spell>();
        }
        else if (agent == MobData)
        {
            if (MobData.MobSpells.Count < 1)
            {
                BattleText.gameObject.SetActive(true);
                BattleText.text = "Your enemy has no spells!";
                return false;
            }
            else
            {
                int index = UnityEngine.Random.Range(0, agent.MobSpells.Count);
                spell = agent.MobSpells[index];
                //BUG: This removes all instances of that spell!
                //Instead, just remove by index.
                //agent.MobSpells.RemoveAt(index);
            }
        }

        if (spell != null)
        {
            Text casterText = (agent == PlayerData) ? PlayerText : MobText;
            Text targetText = (target == PlayerData) ? PlayerText : MobText;
            //cast and discard spell.
            spell.Cast(agent, target, casterText, targetText);
            ResetProjectile();
            Entity targ = (spell.TargetsCaster) ? agent : target;
            ProjectileSpeed = spell.Speed;
            FireProjectile(agent, targ, spell);

            //we don't want mobs disposing of the shop spells!
            if(agent == PlayerData)spell.DiscardSpell();
        }

        return true;
    }

    /// <summary>
    /// Reset projectile to in-active state.
    /// </summary>
    public void ResetProjectile()
    {
        Projectile.gravityScale = 1.0f;
        Projectile.gameObject.SetActive(false);
        Projectile.position = Vector2.zero;
    }

    public float ProjectileSpeed = 10.0f;

    public Vector3 AssignVector(Transform dest, Transform source)
    {
        return new Vector3(dest.transform.position.x, dest.transform.position.y, source.position.z); ;
    }

    /// <summary>
    /// Fires projectile in an inferred direction.
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="target"></param>
    /// <param name="lob"></param>
    /// <param name="gravity"></param>
    public void FireProjectile(Entity agent, Entity target, Spell spell)
    {
        //Debug.Log("Agent: " + agent.gameObject.name + "   Target: " + target.gameObject.name);
        
        Projectile.gameObject.transform.position = AssignVector(agent.Avatar.transform, Projectile.gameObject.transform);
        Projectile.gameObject.SetActive(true);
        Projectile.gravityScale = spell.Grav;

        if (!spell.TargetsCaster)
        {
            Vector2 direction = target.Avatar.transform.position - agent.Avatar.transform.position;
            direction.y += spell.Lob;
            Projectile.velocity = direction * spell.Speed;
        }
        if (spell.CastEffect != null)
        {
            spell.CastEffect.gameObject.SetActive(true);
            spell.CastEffect.Stop();
            spell.CastEffect.Play();
        }
    }

    /// <summary>
    /// Check for various conditions such as CounterAttacks, death, etc.
    /// </summary>
    /// <returns>Returns true if the fight continues, false if the target died.</returns>
    Victory PostAttackCheck()
    {
        if (PlayerData.CurrentHealth <= 0 && MobData.CurrentHealth <= 0) return Victory.Tie;
        else if (PlayerData.CurrentHealth <= 0) return Victory.Mob;
        else if (MobData.CurrentHealth <= 0) return Victory.Player;
        //else if (PlayerData.Inventory.AllItems.Length < 1) return Victory.Mob;


        return Victory.Continue;
    }
}
