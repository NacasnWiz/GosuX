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

    private GameManager gameManager;

    public enum Cards
    {

    }

    public Dictionary<int, Cards> CardRegister { get; private set; } = new Dictionary<int, Cards>();//Nope, ce registre est déjà géré dans l'enum (Abhishala = 1, etc.) Il faut un registre des effets des cartes

    public Dictionary<GameManager.Players, int> toDiscard { get; private set; } = new Dictionary<GameManager.Players, int>();

    private void Awake()
    {
        _instance = this;
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        CreateToDiscardDictionnary();
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

}
