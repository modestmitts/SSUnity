using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameState : MonoBehaviour
{
    #region Data
    public enum State
    {
        Title,
        Shop,
        BattlePreview,
        Battle,
        BattleReview,
        GameOver,
        Invalid,
    }

    [Serializable]
    public class StateObjects
    {
        public State State;
        public GameObject[] ActiveObjects;
    }
    #endregion


    #region Members
    [Tooltip("If the player goes below this in total assets (gold and spells) then it is instatly Game Over.")]
    public int MinGold = 10;
    public State CurrentState = State.Title;
    public StateObjects[] StateObjs;
    public GameObject[] ManagedObjects;

    float OldVolume;

    private State LastState = State.Invalid;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public bool CheckForGameOver(Entity player, Entity mob)
    {
        if (player.GetTotalGoldAssets() < MinGold || mob.MaxHealth < 1 || player.MaxHealth < 1)
        {
            SetState(GameState.State.GameOver);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Provided mostly for use in the UI event system.
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(State newState)
    {
        CurrentState = newState;
    }

    public void SetState(int stateIndex)
    {
        CurrentState = (State)stateIndex;
    }

    void Start()
    {
        OldVolume = AudioListener.volume;
        AudioListener.volume = 0.0f;
        //activate all managed objects so that they are pre-initialized
        foreach(var obj in ManagedObjects)  obj.SetActive(true);
        
        StartCoroutine(SyncState());
    }

    /// <summary>
    /// Sets the game state and manages active states for any associated GameObjects.
    /// </summary>
    /// <param name="newState"></param>
    void UpdateStateSettings(State newState)
    {
        foreach(var go in ManagedObjects) go.SetActive(false);

        foreach(var state in StateObjs)
        {
            if(state.State == newState)
            {
                foreach (var go in state.ActiveObjects) go.SetActive(true);
            }
        }
        CurrentState = newState;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator SyncState()
    {

        //we yield immediately because we want at least 1 frame to occur where all of our
        //gamestate objects are active. That way they have time to perform any end-of-frame updates
        //before we begin tracking the current gamestate.
        yield return new WaitForEndOfFrame();
        AudioListener.volume = OldVolume;
        
        while(true)
        {
            if (CurrentState != LastState)
            {
                UpdateStateSettings(CurrentState);
                LastState = CurrentState;
            }
            yield return null;
        }
    }

}
