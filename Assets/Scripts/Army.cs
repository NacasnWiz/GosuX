using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour
{
    private float PLANE_BASE_SIZE = 10f;
    //based on Plane's base width = 10
    public float GAP_SIZE_HORIZONTAL { get; private set; } = 0.25f;
    public float HOLDER_WIDTH { get; private set; } = 1.7f;
    //[field : SerializeField] //Could serialize and make  5 * HOLDER_WIDTH + 6 * GAP_SIZE_HORIZONTAL = 100  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_VERTICAL { get; private set; } = 0.05f;
    public float HOLDER_HEIGHT { get; private set; } = 1.6f;
    //Could serialize and make  3 * HOLDER_HEIGHT + 4 * GAP_SIZE_VERTICAL = 50  but let's not yet. (Canvas scaling management laterrr)
    public int CARDS_IN_ROW { get; private set; } = 5;
    public int NUMBER_OF_ROWS { get; private set; } = 3;

    public Vector3[,] cardSpots { get; private set; }

    private Dictionary<CardSO.Rank, List<CardModel>> rows = new Dictionary<CardSO.Rank, List<CardModel>>();

    [field : SerializeField]
    public GameManager.Players owner { get; private set; }

    [SerializeField]
    private GameObject dummy;


    private void Awake()
    {
        SetConstants();

        SetCardSlots();
    }

    private void SetConstants()
    {
        GAP_SIZE_HORIZONTAL = BoardManager.Instance.GAP_SIZE_HORIZONTAL;
        GAP_SIZE_VERTICAL = BoardManager.Instance.GAP_SIZE_VERTICAL;
        HOLDER_WIDTH = BoardManager.Instance.HOLDER_WIDTH;
        HOLDER_HEIGHT = BoardManager.Instance.HOLDER_HEIGHT;
        
        CARDS_IN_ROW = BoardManager.CARDS_IN_ROW;
        NUMBER_OF_ROWS = BoardManager.NUMBER_OF_ROWS;
    }


    // Start is called before the first frame update
    void Start()
    {
        DummyShowCardSpots();
    }

    private void SetCardSlots()
    {
        cardSpots = new Vector3[NUMBER_OF_ROWS, CARDS_IN_ROW];
        SetArmyRows();
        SetCardSpots();
    }

    private void SetArmyRows()
    {
        rows.Add(CardSO.Rank.Troupe, new List<CardModel>());
        rows.Add(CardSO.Rank.Héros, new List<CardModel>());
        rows.Add(CardSO.Rank.Immortel, new List<CardModel>());
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
        GameObject board = BoardManager.Instance._board;

        float horizontalPos = board.transform.localScale.x * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_HORIZONTAL * (j + 1) + HOLDER_WIDTH * (j + 0.5f));
        float verticalPos = board.transform.localScale.z * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_VERTICAL * (i + 1) + HOLDER_HEIGHT * (i + 0.5f));

        horizontalPos *= (int)transform.rotation.y == 0 ? 1f : -1f;
        verticalPos *= (int)transform.rotation.y == 0 ? 1f : -1f;

        cardSpots[i, j] = board.transform.position + new Vector3(horizontalPos, 1.05f, verticalPos);
    }

    private void DummyShowCardSpots()
    {
        foreach (var cardSpot in cardSpots)
        {
            Instantiate(dummy, cardSpot, Quaternion.identity, transform);
        }
    }

    public void ReceiveCardPlayed(CardModel cardToReceive)
    {
        if (!CanReceiveCard(cardToReceive))
        {
            return;
        }

        RulesManager.Instance.RequireDiscard(owner, cardToReceive.costToEnter);

        Vector2Int spot = DetermineSpotToPlay(cardToReceive._cardSO);
        cardToReceive.transform.position = cardSpots[spot[0], spot[1]];

        cardToReceive.transform.SetParent(transform);
        cardToReceive.transform.localScale = BoardManager.Instance.cardScaleOnBoard;
        rows[cardToReceive._cardSO.rank].Add(cardToReceive);
    }



    public bool CanReceiveCard(CardModel cardToReceive)
    {
        if (!rows.ContainsKey(cardToReceive._cardSO.rank))
        {
            Debug.Log("The board doesn't have a row for this card's rank. " + cardToReceive._cardSO.cardName + " Cannot be played");
            return false;
        }
        if (rows[cardToReceive._cardSO.rank].Count >= CARDS_IN_ROW)
        {
            Debug.Log("The " + cardToReceive._cardSO.rank.ToString() + " army row is full. It already contains " + rows[cardToReceive._cardSO.rank].Count + " cards, for a maximal capacity of " + CARDS_IN_ROW);
            return false;
        }


        bool containsTroupeOfSameClan = ContainsClan(rows[CardSO.Rank.Troupe], cardToReceive._cardSO.clan);
        switch (cardToReceive._cardSO.rank)
        {
            case CardSO.Rank.Troupe:
                if (rows[CardSO.Rank.Troupe].Count > 0 && !containsTroupeOfSameClan)
                {
                    cardToReceive.costToEnter = 2;
                    //RulesManager.Instance.RequireDiscard(owner, 2);
                }
                return true;

            case CardSO.Rank.Héros:
                return containsTroupeOfSameClan && rows[CardSO.Rank.Troupe].Count > rows[CardSO.Rank.Héros].Count;

            case CardSO.Rank.Immortel:
                bool containsHérosOfSameClan = ContainsClan(rows[CardSO.Rank.Héros], cardToReceive._cardSO.clan);
                return containsTroupeOfSameClan && containsHérosOfSameClan && rows[CardSO.Rank.Troupe].Count >= rows[CardSO.Rank.Héros].Count && rows[CardSO.Rank.Héros].Count > rows[CardSO.Rank.Immortel].Count;
        }

        return true;
    }

    private bool ContainsClan(List<CardModel> row, CardSO.Clan clan)
    {
        bool output = false;

        foreach (CardModel card in row)
        {
            if (card._cardSO.clan == clan)
            {
                output = true;
            }
        }

        return output;
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

        spot[1] = rows[cardSO.rank].Count < 5 ? rows[cardSO.rank].Count : -1;

        return spot;
    }

}
