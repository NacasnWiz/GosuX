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
        PlayPhase = 1,
        DiscardPhase = 2,
        BattlePhase = 3,//a cinematic, kind of
        SacrificePhase = 4,
        GameEndedPhase = 5,
    }

    public static int CARDS_IN_ROW { get; private set; } = 5;
    public static int NUMBER_OF_ROWS { get; private set; } = 3;

    public int costOfNewClanTroupe { get; private set; } = 2;

    [field: SerializeField]
    public GamePhases currentPhase { get; private set; } = GamePhases.PlayPhase;

    public Dictionary<GameManager.Players, int> toDiscard { get; private set; } = new();
    public Dictionary<GameManager.Players, int> toSacrifice { get; private set; } = new();

    public Player lastRoundWinner { get; private set; }


    public UnityEvent ev_EnterDiscardPhase = new();
    public UnityEvent ev_EnterSacrificePhase = new();
    public UnityEvent<CardModel> ev_CardSacrificed = new();
    public UnityEvent ev_ExitSacrificePhase = new();
    public UnityEvent ev_CurrentPlayerHasPlayed = new();
    public UnityEvent<Player> ev_RoundEnded = new();
    public UnityEvent<Player> ev_GameEnded = new();



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CardModel.ev_CardClicked.AddListener((card) => OnCardClicked(card));

        CreateDictionnaries();
    }

    private void CreateDictionnaries()
    {
        toDiscard.Add(GameManager.Players.Player, 0);
        toDiscard.Add(GameManager.Players.Opponent, 0);

        toSacrifice.Add(GameManager.Players.Player, 0);
        toSacrifice.Add(GameManager.Players.Opponent, 0);
    }

    private bool IsPendingDiscards()
    {
        return toDiscard[GameManager.Players.Player] > 0 || toDiscard[GameManager.Players.Opponent] > 0;
    }

    private bool IsPendingSacrifices()
    {
        return toSacrifice[GameManager.Players.Player] > 0 || toSacrifice[GameManager.Players.Opponent] > 0;
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
                    if (GameManager.Instance.currentPlayer == card.owner)
                    {
                        PutCardInToDiscard(card);
                    }
                    return;

                default:
                    return;
            }
        }
        else if (card.isInArmy)
        {
            switch (currentPhase)
            {
                case GamePhases.SacrificePhase:
                    if (card.owner._army.IsLIBRE(card))
                    {
                        Sacrifice(card);
                    }
                    else
                    {
                        Debug.Log("You can't sacrifice a card that's not LIBRE.");
                    }
                    return;
            }
        }
    }

    private void Sacrifice(CardModel cardToSacrifice)
    {
        if(toSacrifice[cardToSacrifice.owner.ID] <= 0)
        {
            Debug.Log(cardToSacrifice.owner.ID + "Doesn't need to sacrifice any more card.");
            return;
        }
        cardToSacrifice.owner._army.RemoveCard(cardToSacrifice);
        cardToSacrifice.owner._discardPile.AddCard(cardToSacrifice);
        --toSacrifice[cardToSacrifice.owner.ID];

        ev_CardSacrificed.Invoke(cardToSacrifice);


        if (!IsPendingSacrifices())
        {
            ExitSacrificePhase();
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
            card.owner._hand.PutCardToDiscard(card);
        }
    }

    private void PlayCardFromHand(CardModel card)
    {
        if (card.owner._army.CanReceiveCard(card))
        {
            card.owner._army.ReceiveCardPlayed(card);
            card.owner._hand.RemoveCard(card);


            //card.PlayCardEffect();//NEXT FEATURE TO IMPLEMENT
            if (currentPhase != GamePhases.DiscardPhase && IsPendingDiscards())//I highly dislike to check this at every frame
            {
                EnterDiscardPhase();
            }
            else
            {
                ev_CurrentPlayerHasPlayed.Invoke();
            }

        }
    }

    private void EnterDiscardPhase()
    {
        currentPhase = GamePhases.DiscardPhase;
        ev_EnterDiscardPhase.Invoke();
    }

    private void EnterSacrificePhase()
    {
        Debug.Log("Sacrifice Phase just begun.");
        ev_EnterSacrificePhase.Invoke();
        currentPhase = GamePhases.SacrificePhase;
    }

    private void ExitDiscardPhase()
    {
        currentPhase = GamePhases.PlayPhase;
    }

    private void ExitSacrificePhase()
    {
        //Start new round, probably
        ev_ExitSacrificePhase.Invoke();
        Debug.Log("Sacrifice Phase is over.");

        currentPhase = GamePhases.PlayPhase;
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
            ev_CurrentPlayerHasPlayed.Invoke();
        }
    }

    public void EndRound()
    {
        Debug.Log("The round has ended.");

        CrownRoundWinner();
        
        if(lastRoundWinner.SupremacyPoint > 1)//if the player who've just won the round has more than one Supremacy point
        {
            ev_GameEnded.Invoke(lastRoundWinner);//He wins the game and the game ends here.
            currentPhase = GamePhases.GameEndedPhase;
            return;
        }

        ev_RoundEnded.Invoke(lastRoundWinner);

        toSacrifice[GameManager.Players.Player] += CalculateToSacrifice(GameManager.Instance.players[GameManager.Players.Player]);
        toSacrifice[GameManager.Players.Opponent] += CalculateToSacrifice(GameManager.Instance.players[GameManager.Players.Opponent]);

        EnterSacrificePhase();
    }

    private int CalculateToSacrifice(Player player)
    {
        int output = 0;
        int playerArmySize = player._army._size;

        output = playerArmySize / 2 + playerArmySize % 2;

        return output;
    }

    private void CrownRoundWinner()
    {
        lastRoundWinner = GameManager.Instance.GetBestArmyPlayer();
        if (lastRoundWinner == null)//there have been a stalemate
        {
            foreach (Player player in GameManager.Instance.players.Values)
            {
                player.SupremacyPoint += 1;
            }
            Debug.Log("There is a stalemate. Stalemates are not handled yet.");
            //ev_ThereHaveBeenAStalemate.Invoke();
        }
        else
        {
            lastRoundWinner.SupremacyPoint += 1;
        }
    }
}
