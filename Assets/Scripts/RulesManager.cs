using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public static int CARDS_IN_ROW { get; private set; } = 5;
    public static int NUMBER_OF_ROWS { get; private set; } = 3;

    public int costOfNewClanTroupe { get; private set; } = 2;

    [SerializeField]
    public GamePhases currentPhase = GamePhases.PlayPhase;

    public Dictionary<GameManager.Players, int> toDiscard { get; private set; } = new Dictionary<GameManager.Players, int>();


    public UnityEvent ev_EnterDiscardPhase = new();
    public UnityEvent ev_CurrentPlayerHasPlayed = new();


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        CardModel.ev_CardClicked.AddListener((card) => OnCardClicked(card));

        CreateToDiscardDictionnary();
    }

    private void Update()
    {
        if(currentPhase != GamePhases.DiscardPhase && IsPendingDiscards())//I highly dislike to check this at every frame
        {
            EnterDiscardPhase();
        }
    }

    private bool IsPendingDiscards()
    {
        return toDiscard[GameManager.Players.Player] > 0 || toDiscard[GameManager.Players.Opponent] > 0;
    }

    private void OnCardClicked(CardModel card)
    {
        if (card.isInHand)
        {
            switch(currentPhase)
            {
                case GamePhases.PlayPhase:
                    if (GameManager.Instance.currentPlayer == card.owner)
                    {
                        PlayCardFromHand(card);
                    }
                    return;
                case GamePhases.DiscardPhase:
                    PutCardInToDiscard(card);
                    return;

                default:
                    return;
            }
        }
    }

    private void PutCardInToDiscard(CardModel card)
    {
        if (card.isInToDiscard)
        {
            UIManager.Instance.ReplaceCardInhand(card);
        }
        else
        {
            GameManager.Instance.players[card.owner]._hand.PutCardToDiscard(card);
            //GameManager.Instance.hands[card.owner].PutCardToDiscard(card);
        }
    }

    private void PlayCardFromHand(CardModel card)
    {
        if (GameManager.Instance.players[card.owner]._army.CanReceiveCard(card))
        {
            GameManager.Instance.players[card.owner]._army.ReceiveCardPlayed(card);
            GameManager.Instance.players[card.owner]._hand.RemoveCard(card);


            //card.PlayCardEffect();//NEXT FEATURE TO IMPLEMENT


            ev_CurrentPlayerHasPlayed.Invoke();
        }
    }

    private void ExitDiscardPhase()
    {
        currentPhase = GamePhases.PlayPhase;
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

        if (!IsPendingDiscards())
        {
            ExitDiscardPhase();
        }
    }

    private void EnterDiscardPhase()
    {
        currentPhase = GamePhases.DiscardPhase;
        ev_EnterDiscardPhase.Invoke();
    }

}
