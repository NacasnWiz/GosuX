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
    public GameObject testSpawn;
    [SerializeField]
    private GameObject dummy;

    public bool isOnPlayer { get; private set; } = false;


    [field : SerializeField]
    public Deck playerDeck { get; private set; }
    [field : SerializeField]
    public Hand playerHand { get; private set; }
    [field: SerializeField]
    public CinemachineVirtualCamera playerCamera { get; private set; }


    [field: SerializeField]
    public Deck opponentDeck { get; private set; }
    //[field: SerializeField]
    //public Hand opponentHand { get; private set; }
    [field: SerializeField]
    public CinemachineVirtualCamera opponentCamera { get; private set; }


    [field: SerializeField]
    public CardModel cardPrefab { get; private set; }


    [SerializeField]
    private CardSO.Clan[] playerClans = new CardSO.Clan[3];
    [SerializeField]
    private CardSO.Clan[] opponentClans = new CardSO.Clan[3];
    [SerializeField]
    private CardSO.Clan[] unplayedClans = new CardSO.Clan[2];


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
        FillDeck(playerDeck, playerClans);
        playerDeck.ShuffleDeck();
        FillDeck(opponentDeck, opponentClans);
        opponentDeck.ShuffleDeck();

        playerHand.DrawCards(7);
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


    public CardModel CreateCard(CardSO so, Transform parent)
    {
        CardModel card = Instantiate(cardPrefab, parent);
        card.Set(so);

        return card;
    }

    public CardModel CreateCard(CardSO so)
    {
        return CreateCard(so, transform);
    }


}
