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

    [SerializeField]
    public GamePhases currentPhase = GamePhases.PlayPhase;

    public Dictionary<GameManager.Players, int> toDiscard { get; private set; } = new Dictionary<GameManager.Players, int>();



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
        if(IsPendingDiscards() && currentPhase != GamePhases.DiscardPhase)//Highly sub-optimal to check this at every frame
        {
            EnterDiscardPhase();
        }
        if(!IsPendingDiscards() && currentPhase == GamePhases.DiscardPhase)
        {
            ExitDiscardPhase();
        }
    }

    private void OnCardClicked(CardModel card)
    {

        if (card.isInHand)
        {
            switch(currentPhase)
            {
                case GamePhases.PlayPhase:
                    PlayCardFromHand(card);
                    break;
                case GamePhases.DiscardPhase:
                    PutCardInToDiscard(card);
                    break;

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
            GameManager.Instance.hands[card.owner].PutCardToDiscard(card);
        }
    }

    private void PlayCardFromHand(CardModel card)
    {
        if (GameManager.Instance.currentPlayer == card.owner)
        {
            PlayCard(card);

            GameManager.Instance.hands[card.owner].RemoveCard(card);

        }
    }

    private void PlayCard(CardModel card)
    {
        if (BoardManager.Instance.CanReceiveCard(card))
        {
            BoardManager.Instance.ReceiveCardPlayed(card);
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
