using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Army : MonoBehaviour
{
    [SerializeField] private BoardDimensionsSO boardDimensions;

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

    private Dictionary<CardSO.Rank, List<CardModel>> rows = new();

    [field : SerializeField]
    public Player owner { get; private set; }

    [SerializeField]
    private GameObject dummy;


    private void Awake()
    {
        SetConstants();

        SetCardSlots();
    }

    private void SetConstants()
    {
        GAP_SIZE_HORIZONTAL = boardDimensions.GAP_SIZE_HORIZONTAL;
        GAP_SIZE_VERTICAL = boardDimensions.GAP_SIZE_VERTICAL;
        HOLDER_WIDTH = boardDimensions.HOLDER_WIDTH;
        HOLDER_HEIGHT = boardDimensions.HOLDER_HEIGHT;
        
        CARDS_IN_ROW = RulesManager.CARDS_IN_ROW;
        NUMBER_OF_ROWS = RulesManager.NUMBER_OF_ROWS;
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
        Board board = GameManager.Instance._board;

        float horizontalPos = board.transform.localScale.x * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_HORIZONTAL * (j + 1) + HOLDER_WIDTH * (j + 0.5f));
        float verticalPos = board.transform.localScale.z * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_VERTICAL * (i + 1) + HOLDER_HEIGHT * (i + 0.5f));

        horizontalPos *= owner.ID == GameManager.Players.Player ? 1f : -1f;
        verticalPos *= owner.ID == GameManager.Players.Player ? 1f : -1f;

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

        if (rows[CardSO.Rank.Troupe].Count > 0 && !ContainsClan(CardSO.Rank.Troupe, cardToReceive._cardSO.clan))
        {
            RulesManager.Instance.RequireDiscard(owner.ID, RulesManager.Instance.costOfNewClanTroupe);
        }

        Vector2Int spot = DetermineSpotToPlay(cardToReceive._cardSO);
        cardToReceive.transform.position = cardSpots[spot[0], spot[1]];

        cardToReceive.transform.SetParent(transform);
        cardToReceive.transform.localScale = boardDimensions.cardScaleOnBoard;
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


        bool containsTroupeOfSameClan = ContainsClan(CardSO.Rank.Troupe, cardToReceive._cardSO.clan);
        switch (cardToReceive._cardSO.rank)
        {
            case CardSO.Rank.Troupe:
                if (rows[CardSO.Rank.Troupe].Count > 0 && !containsTroupeOfSameClan)
                {
                    return RulesManager.Instance.costOfNewClanTroupe < owner._hand.GetHandSize();
                }
                else
                {
                    return true;
                }

            case CardSO.Rank.Héros:
                return containsTroupeOfSameClan && rows[CardSO.Rank.Troupe].Count > rows[CardSO.Rank.Héros].Count;

            case CardSO.Rank.Immortel:
                bool containsHérosOfSameClan = ContainsClan(CardSO.Rank.Héros, cardToReceive._cardSO.clan);
                return containsTroupeOfSameClan && containsHérosOfSameClan && rows[CardSO.Rank.Troupe].Count >= rows[CardSO.Rank.Héros].Count && rows[CardSO.Rank.Héros].Count > rows[CardSO.Rank.Immortel].Count;
        }

        return true;
    }

    private bool ContainsClan(CardSO.Rank rank, CardSO.Clan clan)
    {
        bool output = false;

        List<CardModel> row = rows[rank];

        foreach (CardModel card in row)
        {
            if (card._cardSO.clan == clan)
            {
                output = true;
            }
        }

        return output;
    }

    private bool ContainsClan(CardSO.Clan clan)
    {
        return ContainsClan(CardSO.Rank.Troupe, clan) && ContainsClan(CardSO.Rank.Héros, clan) && ContainsClan(CardSO.Rank.Immortel, clan);
    }

    public int CalculateBattleScore()
    {
        int score = 0;

        foreach(var row in rows.Values)
        {
            foreach(var card in row)
            {
                score += card.battleValue;
            }
        }

        return score;
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
