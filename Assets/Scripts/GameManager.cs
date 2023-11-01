using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("Game Manager is Null !!!");
            }

            return _instance;
        }
    }


    public enum Players
    {
        Player = 1,
        None = 0,
        Opponent = -1
    }

    [field: SerializeField]
    public CardSO[] ALL_CARDS_LIST {get; private set;}

    [SerializeField]
    private Dictionary<CardSO.Clans, List<CardSO>> ALL_CLANS_CARDS = new();

    //[SerializeField]
    //private CardSO.Clan[] playerClans = new CardSO.Clan[3];
    //[SerializeField]
    //private CardSO.Clan[] opponentClans = new CardSO.Clan[3];
    [SerializeField]
    private CardSO.Clans[] unplayedClans = new CardSO.Clans[2];

    public int nb_maxCardsInHand = 7;
    public int nb_maxTurnsAfterPass = 3;


    [field: SerializeField] public Board _board { get; private set; }

    [SerializeField]
    public GameObject testSpawn;
    [SerializeField]
    private GameObject dummy;

    //public bool isOnPlayer { get; private set; } = false;
    private int nb_turnsAfterPassed;

    [SerializeField]
    private Player _player;

    [field: SerializeField]
    public CinemachineVirtualCamera playerCamera { get; private set; }


    [SerializeField]
    private Player _opponent;

    [field: SerializeField]
    public CinemachineVirtualCamera opponentCamera { get; private set; }


    public Dictionary<Players, Player> players { get; private set; } = new();



    [field: SerializeField]
    public CardModel cardPrefab { get; private set; }

    [field: SerializeField]
    public Player currentPlayer { get; private set; }

    public bool currentPlayerHasPlayed = false;


    public UnityEvent<Player> ev_TurnEnded = new();


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

        SetClansCardsLists();
    }

    private void SetClansCardsLists()
    {
        int clanCount = 0;
        foreach (CardSO.Clans clan in Enum.GetValues(typeof(CardSO.Clans)))
        {
            ALL_CLANS_CARDS.Add(clan, new List<CardSO>());
            for (int i = 0; i < 11; ++i)
            {
                ALL_CLANS_CARDS[clan].Add(ALL_CARDS_LIST[i + clanCount * 11]);
            }
            ++clanCount;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreatePlayersDictionary();
        RulesManager.Instance.ev_CurrentPlayerHasPlayed.AddListener(() => { Debug.Log("current player has played."); currentPlayerHasPlayed = true; });
        RulesManager.Instance.ev_RoundEnded.AddListener((winner) => Debug.Log("The round ended! " + winner.ID + " wins the round. (Sacrifice Phase is to be implemented)"));
        RulesManager.Instance.ev_GameEnded.AddListener((winner) => Debug.Log("The game has ended. Winner is" + winner.ID));
        RulesManager.Instance.ev_ExitSacrificePhase.AddListener(() => StartNewRound());
        RulesManager.Instance.ev_CardSacrificed.AddListener((card) => Destroy(card.gameObject));

        FillDeck(_player);
        _player.ShuffleDeck();
        FillDeck(_opponent);
        _opponent.ShuffleDeck();

        _player.DrawCards(nb_maxCardsInHand);
        _opponent.DrawCards(nb_maxCardsInHand);
    }

    private void LateUpdate()
    {
        if(currentPlayerHasPlayed && RulesManager.Instance.currentPhase == RulesManager.GamePhases.PlayPhase)
        {
            EndTurn();
        }
    }

    private void StartNewRound()
    {
        _player.DrawCards(nb_maxCardsInHand - _player._hand._size);
        _opponent.DrawCards(nb_maxCardsInHand - _opponent._hand._size);

        _player.hasPassed = false;
        _opponent.hasPassed = false;
    }

    private void CreatePlayersDictionary()
    {
        players.Add(Players.Player, _player);
        players.Add(Players.Opponent, _opponent);
    }

    private void FillDeck(Player player)
    {
        List<CardSO.Clans> clans = player.clansList;

        foreach (CardSO.Clans clan in clans)
        {
            player.AddCardsToDeck(ALL_CLANS_CARDS[clan]);
        }
    }



    public CardModel CreateCardModel(CardSO so, Transform parent)
    {
        CardModel card = Instantiate(cardPrefab, parent);
        card.Set(so);

        return card;
    }

    public CardModel CreateCardModel(CardSO so)
    {
        return CreateCardModel(so, transform);
    }

    public void SeeOpponent()
    {
        playerCamera.Priority = 10;
        opponentCamera.Priority = 15;
    }

    public void ReturnToPlayer()
    {
        opponentCamera.Priority = 10;
        playerCamera.Priority = 15;
    }

    private Player GetOtherPlayer(Player player)
    {
        if (player.ID == Players.Player)
        {
            return players[Players.Opponent];
        }
        else
        {
            return players[Players.Player];
        }

        //could also use
        //currentPlayer = players[(Players)(-(int)currentPlayer)];
    }

    public void Pass()
    {
        if (!(RulesManager.Instance.currentPhase == RulesManager.GamePhases.PlayPhase))
        {
            Debug.Log("Can't Pass, it's not PlayPhase");
            return;
        }
        currentPlayer.hasPassed = true;
        Debug.Log(currentPlayer + " has passed.");
        EndTurn();
    }

    public void EndTurn()
    {
        ev_TurnEnded.Invoke(currentPlayer);

        if (players[GetOtherPlayer(currentPlayer).ID].hasPassed)
        {
            ++nb_turnsAfterPassed;
        }
        else
        {
            currentPlayer = GetOtherPlayer(currentPlayer);
        }

        currentPlayerHasPlayed = false;

        if(nb_turnsAfterPassed >= nb_maxTurnsAfterPass || (currentPlayer.hasPassed && GetOtherPlayer(currentPlayer).hasPassed))
        {
            RulesManager.Instance.EndRound();
        }
    }

    public Player GetBestArmyPlayer()
    {
        int _playerBattleScore = _player._army._battleScore;
        int _opponentBattleScore = _opponent._army._battleScore;
        if (_playerBattleScore > _opponentBattleScore)
        {
            return _player;
        }
        else if (_playerBattleScore < _opponentBattleScore)
        {
            return _opponent;
        }
        else
        {
            Debug.Log("Both players have same battleScore of " + _opponentBattleScore);
            return null;
        }
    }


}
