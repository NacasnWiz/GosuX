using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    private static BoardManager _instance;
    public static BoardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Board Manager is Null !!!");
            }

            return _instance;
        }
    }

    [SerializeField] GameObject _board;

    private float PLANE_BASE_SIZE = 10f;
    //based on Plane's base width = 10
    public float GAP_SIZE_HORIZONTAL { get; private set; } = 0.25f;
    public float HOLDER_WIDTH { get; private set; } = 1.7f;
    //[field : SerializeField] //Could serialize and make  5 * HOLDER_WIDTH + 6 * GAP_SIZE_HORIZONTAL = 100  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_VERTICAL { get; private set; } = 0.05f;
    public float HOLDER_HEIGHT { get; private set; } = 1.6f;
    //Could serialize and make  3 * HOLDER_HEIGHT + 4 * GAP_SIZE_VERTICAL = 50  but let's not yet. (Canvas scaling management laterrr)

    public static int CARDS_IN_ROW { get; private set; } = 5;
    public static int NUMBER_OF_ROWS { get; private set; } = 3;


    Vector3[,] cardSpots = new Vector3[NUMBER_OF_ROWS, CARDS_IN_ROW];

    private Dictionary<CardSO.Rank, List<CardModel>> armyRows = new Dictionary<CardSO.Rank, List<CardModel>>();


    [SerializeField]
    GameObject dummy;

    [SerializeField]
    private Vector3 cardScaleOnBoard = new Vector3(14.75f, 19f, 1f);


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

        SetArmySlots();
    }

    // Start is called before the first frame update
    void Start()
    {
        DummyShowCardSpots();
        
    }

    private void DummyShowCardSpots()
    {
        foreach (var cardSpot in cardSpots)
        {
            Instantiate(dummy, cardSpot, Quaternion.identity, transform);
        }
    }

    private void SetArmySlots()
    {
        SetArmyRows();
        SetCardSpots();
    }

    private void SetArmyRows()
    {
        armyRows.Add(CardSO.Rank.Troupe, new List<CardModel>());
        armyRows.Add(CardSO.Rank.Héros, new List<CardModel>());
        armyRows.Add(CardSO.Rank.Immortel, new List<CardModel>());
    }

    private void SetCardSpots()
    {
        for (int i = 0; i < cardSpots.GetLength(0); ++i)
        {
            for (int j = 0; j < cardSpots.GetLength(1); ++j)
            {
                SetCardSpot(i, j);
            }
        }
    }

    private void SetCardSpot(int i, int j)
    {
        cardSpots[i, j] = _board.transform.position + new Vector3(_board.transform.localScale.x * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_HORIZONTAL * (j + 1) + HOLDER_WIDTH * (j + 0.5f)), 1.05f, _board.transform.localScale.z * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_VERTICAL * (i + 1) + HOLDER_HEIGHT * (i + 0.5f)));
    }

    public void ReceiveCardPlayed(CardModel cardModel)
    {
        if (!CanReceiveCard(cardModel._cardSO))
        {
            return;
        }

        Vector2Int spot = DetermineSpotToPlay(cardModel._cardSO);

        cardModel.transform.position = cardSpots[spot[0], spot[1]];

        cardModel.transform.SetParent(transform);
        cardModel.transform.localScale = cardScaleOnBoard;
        armyRows[cardModel._cardSO.rank].Add(cardModel);
    }



    public bool CanReceiveCard(CardSO cardSO)
    {
        if (!armyRows.ContainsKey(cardSO.rank))
        {
            Debug.Log("The board doesn't have a row for this card's rank. " + cardSO.cardName + " Cannot be played");
            return false;
        }
        if(armyRows[cardSO.rank].Count() >= CARDS_IN_ROW)
        {
            Debug.Log("The " + cardSO.rank.ToString() + " army row is full. It already contains " + armyRows[cardSO.rank].Count() + " cards, for a maximal capacity of " + CARDS_IN_ROW);
            return false;
        }


        return true;

    }

    private Vector2Int DetermineSpotToPlay(CardSO cardSO)
    {
        Vector2Int spot = new Vector2Int();

        switch (cardSO.rank)
        {
            case (CardSO.Rank.Troupe):
                spot[0] = 0;
                break;
            case (CardSO.Rank.Héros):
                spot[0] = 1;
                break;
            case (CardSO.Rank.Immortel):
                spot[0] = 2;
                break;

            default:
                spot[0] = -1;
                break;
        }

        spot[1] = armyRows[cardSO.rank].Count() < 5 ? armyRows[cardSO.rank].Count : -1;

        return spot;
    }


}
