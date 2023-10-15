using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    private static RulesManager _instance;
    public static RulesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Rules Manager is Null !!!");
            }

            return _instance;
        }
    }


    public enum GamePhases
    {
        PlayPhase,
        DiscardPhase,

    }

    [SerializeField]
    public GamePhases currentPhase = GamePhases.PlayPhase;



    public Dictionary<GameManager.Players, int> toDiscard { get; private set; } = new Dictionary<GameManager.Players, int>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        CreateToDiscardDictionnary();
    }

    private void Update()
    {
        if(IsPendingDiscards() && currentPhase != GamePhases.DiscardPhase)//Highly sub-optimal to check this at every frame
        {
            EnterDiscardPhase();
        }
        if(!IsPendingDiscards() && currentPhase == GamePhases.DiscardPhase)
        {
            ExitDiscardPhase();
        }
    }

    private void ExitDiscardPhase()
    {
        currentPhase = GamePhases.PlayPhase;
    }

    private bool IsPendingDiscards()
    {
        return toDiscard[GameManager.Players.Player] > 0 || toDiscard[GameManager.Players.Opponent] > 0;
    }

    private void CreateToDiscardDictionnary()
{
        toDiscard.Add(GameManager.Players.Player, 0);
        toDiscard.Add(GameManager.Players.Opponent, 0);
    }

    public void RequireDiscard(GameManager.Players who, int cost)
    {
        toDiscard[who] += cost;
    }

    public void RegisterDiscard(GameManager.Players who, int nbDiscarded)
    {
        toDiscard[who] -= nbDiscarded;
    }

    private void EnterDiscardPhase()
    {
        currentPhase = GamePhases.DiscardPhase;
        UIManager.Instance.DisplayToDiscardPanel();
    }

}
