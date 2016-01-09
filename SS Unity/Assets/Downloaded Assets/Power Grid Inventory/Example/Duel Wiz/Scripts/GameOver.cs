using UnityEngine;
using System.Collections;
using PowerGridInventory;

public class GameOver : MonoBehaviour
{
    PGIModel PlayerInventory;
    Entity PlayerData;
    Entity MobData;
    GameState StateManager;
    bool CanGameOver;

    void OnDisable()
    {
        CanGameOver = false;
    }

    void OnEnable()
    {
        //this is so that player's don't accidentally skip the GameOver screen immdiately if they are clicking rapidly
        StartCoroutine(WaitForClick());
    }

    void Start()
    {
        PlayerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PGIModel>();
        PlayerData = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        MobData = GameObject.FindGameObjectWithTag("Mob").GetComponent<Entity>();
        StateManager = GameObject.FindGameObjectWithTag("Game State Manager").GetComponent<GameState>();
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0) && CanGameOver)
        {
            DoGameOver();
        }
    }

    IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(1.0f);
        CanGameOver = true;
    }

    void DoGameOver()
    {
        PlayerData.ResetForGame();
        MobData.ResetForGame();
        var list = PlayerInventory.AllItems;
        foreach (var item in list) PlayerInventory.Remove(item, false);

        StateManager.SetState(GameState.State.Title);
    }
    
}
