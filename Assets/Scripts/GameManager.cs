using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Opponent = -1
    }

    [field: SerializeField]
    public CardSO[] ALL_CARDS_LIST {get; private set;}

    [SerializeField]
    private Dictionary<CardSO.Clan, List<CardSO>> ALL_CLANS_CARDS = new Dictionary<CardSO.Clan, List<CardSO>>();

    [SerializeField]
    private CardSO.Clan[] playerClans = new CardSO.Clan[3];
    [SerializeField]
    private CardSO.Clan[] opponentClans = new CardSO.Clan[3];
    [SerializeField]
    private CardSO.Clan[] unplayedClans = new CardSO.Clan[2];

    public int nb_cardsDrawnStart = 7;


    [field: SerializeField] public Board _board { get; private set; }

    [SerializeField]
    public GameObject testSpawn;
    [SerializeField]
    private GameObject dummy;

    public bool isOnPlayer { get; private set; } = false;


    [SerializeField]
    private Player _player;

    [field : SerializeField]
    public Deck playerDeck { get; private set; }
    [field: SerializeField]
    public DiscardPile playerDiscardPile { get; private set; }
    [field : SerializeField]
    public Hand playerHand { get; private set; }
    [field: SerializeField]
    public Army playerArmy { get; private set; }

    [field: SerializeField]
    public CinemachineVirtualCamera playerCamera { get; private set; }


    [SerializeField]
    private Player _opponent;
    [field: SerializeField]
    public Deck opponentDeck { get; private set; }
    [field: SerializeField]
    public DiscardPile opponentDiscardPile { get; private set; }
    [field: SerializeField]
    public Hand opponentHand { get; private set; }
    [field: SerializeField]
    public Army opponentArmy { get; private set; }

    [field: SerializeField]
    public CinemachineVirtualCamera opponentCamera { get; private set; }

    
    //public Dictionary<Players, Deck> decks { get; private set; } = new();
    //public Dictionary<Players, DiscardPile> discardPiles { get; private set; } = new();
    //public Dictionary<Players, Hand> hands { get; private set; } = new();
    //public Dictionary<Players, Army> armies { get; private set; } = new();
    //public Dictionary<Players, CinemachineVirtualCamera> cameras { get; private set; } = new();

    public Dictionary<Players, Player> players { get; private set; } = new();



    [field: SerializeField]
    public CardModel cardPrefab { get; private set; }

    [field: SerializeField]
    public Players currentPlayer { get; private set; }

    public bool currentPlayerHasPlayed = false;

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
        foreach (CardSO.Clan clan in Enum.GetValues(typeof(CardSO.Clan)))
        {
            ALL_CLANS_CARDS.Add(clan, new List<CardSO>());
            for (int i = 0; i < 11; ++i)
            {
                ALL_CLANS_CARDS[clan].Add(ALL_CARDS_LIST[i + (int)clan * 11]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //CreatePlayersDictionaries();
        CreatePlayersDictionary();
        RulesManager.Instance.ev_CurrentPlayerHasPlayed.AddListener(() => { Debug.Log("current player has played."); currentPlayerHasPlayed = true; });

        FillDeck(playerDeck, playerClans);
        playerDeck.ShuffleDeck();
        FillDeck(opponentDeck, opponentClans);
        opponentDeck.ShuffleDeck();

        playerHand.DrawCards(nb_cardsDrawnStart);
        opponentHand.DrawCards(nb_cardsDrawnStart);
    }

    private void LateUpdate()
    {
        if(currentPlayerHasPlayed && RulesManager.Instance.currentPhase == RulesManager.GamePhases.PlayPhase)
        {
            EndTurn();
        }
    }

    private void CreatePlayersDictionary()
    {
        players.Add(Players.Player, _player);
        players.Add(Players.Opponent, _opponent);
    }

    //private void CreatePlayersDictionaries()
    //{
    //    decks.Add(Players.Player, playerDeck);
    //    decks.Add(Players.Opponent, opponentDeck);

    //    discardPiles.Add(Players.Player, playerDiscardPile);
    //    discardPiles.Add(Players.Opponent, opponentDiscardPile);

    //    hands.Add(Players.Player, playerHand);
    //    hands.Add(Players.Opponent, opponentHand);

    //    armies.Add(Players.Player, playerArmy);
    //    armies.Add(Players.Opponent, opponentArmy);

    //    cameras.Add(Players.Player, playerCamera);
    //    cameras.Add(Players.Opponent, opponentCamera);
    //}

    private void FillDeck(Deck deck, CardSO.Clan[] clans)
    {
        foreach (CardSO.Clan clan in clans)
        {
            AddCardsToDeck(deck, ALL_CLANS_CARDS[clan]);
        }
    }

    private void AddCardsToDeck(Deck deck, List<CardSO> cards)
    {
        deck.AddCards(cards);
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

    public void EndTurn()
    {
        if(currentPlayer == Players.Player)
        {
            currentPlayer = Players.Opponent;
        }
        else if(currentPlayer == Players.Opponent)
        {
            currentPlayer = Players.Player;
        }

        //is equivalent to
        //currentPlayer = (Players)(-(int)currentPlayer);

        currentPlayerHasPlayed = false;
    }

}
