using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //[field : SerializeField] public Camera _camera { get; private set; }//will have public methods accessed via _instance


    //===== Info in Editor ====
    [field: SerializeField]
    public CardSO[] ALL_CARDS_LIST {get; private set;}

    /*
    public enum Area
    {
        InHand,
        OnBoard,
        InDeck,
        InDiscardPile

    }
    *///Enum of game Areas, wrong architechture I think

    [SerializeField]
    public GameObject testSpawn;
    [SerializeField]
    private GameObject dummy;
    //========================

    //==== Faking it Material ====
    //[field : SerializeField]
    //public Transform playerBoardAnchor { get; private set; }


    //========================

    [field : SerializeField]
    public Deck playerDeck { get; private set; }
    [field : SerializeField]
    public Hand playerHand { get; private set; }

    [field: SerializeField]
    public CardModel cardPrefab { get; private set; }


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

    // Start is called before the first frame update
    void Start()
    {
        playerHand.DrawCards(7);
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
