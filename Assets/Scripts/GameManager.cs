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


    [SerializeField]
    public GameObject testSpawn;
    [SerializeField]
    private GameObject dummy;

    public bool isOnPlayer { get; private set; } = false;


    [field : SerializeField]
    public Deck playerDeck { get; private set; }
    [field : SerializeField]
    public Hand playerHand { get; private set; }
    [field: SerializeField]
    public DiscardPile playerDiscardPile { get; private set; }
    [field: SerializeField]
    public CinemachineVirtualCamera playerCamera { get; private set; }


    [field: SerializeField]
    public Deck opponentDeck { get; private set; }
    [field: SerializeField]
    public Hand opponentHand { get; private set; }
    [field: SerializeField]
    public DiscardPile opponentDiscardPile { get; private set; }
    [field: SerializeField]
    public CinemachineVirtualCamera opponentCamera { get; private set; }

    
    public Dictionary<Players, Deck> decks { get; private set; } = new Dictionary<Players, Deck>();
    public Dictionary<Players, DiscardPile> discardPiles { get; private set; } = new Dictionary<Players, DiscardPile>();
    public Dictionary<Players, Hand> hands { get; private set; } = new Dictionary<Players, Hand>();
    public Dictionary<Players, CinemachineVirtualCamera> cameras { get; private set; } = new Dictionary<Players, CinemachineVirtualCamera>();



    [field: SerializeField]
    public CardModel cardPrefab { get; private set; }

    [field: SerializeField]
    public Players currentPlayer { get; private set; }

    public bool currentPlauerHasPlayed = false;

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
        CreatePlayersDictionaries();

        FillDeck(playerDeck, playerClans);
        playerDeck.ShuffleDeck();
        FillDeck(opponentDeck, opponentClans);
        opponentDeck.ShuffleDeck();

        playerHand.DrawCards(nb_cardsDrawnStart);
        opponentHand.DrawCards(nb_cardsDrawnStart);
    }

    private void CreatePlayersDictionaries()
    {
        decks.Add(Players.Player, playerDeck);
        decks.Add(Players.Opponent, opponentDeck);
        discardPiles.Add(Players.Player, playerDiscardPile);
        discardPiles.Add(Players.Opponent, opponentDiscardPile);
        hands.Add(Players.Player, playerHand);
        hands.Add(Players.Opponent, opponentHand);
        cameras.Add(Players.Player, playerCamera);
        cameras.Add(Players.Opponent, opponentCamera);
    }

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

    public void EndTurn(Players who)
    {
        if(who == Players.Player)
        {
            currentPlayer = Players.Opponent;
        }
        else if(who == Players.Opponent)
        {
            currentPlayer = Players.Player;
        }
    }

}
