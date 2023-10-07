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

    [field : SerializeField] public GameObject _board { get; private set; }

    public float PLANE_BASE_SIZE { get; private set; } = 10f;

    //based on Plane's base width = 10
    //[field : SerializeField] //Could serialize and make  5 * HOLDER_WIDTH + 6 * GAP_SIZE_HORIZONTAL = 100  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_HORIZONTAL { get; private set; } = 0.25f;
    public float HOLDER_WIDTH { get; private set; } = 1.7f;
    //[field : SerializedField]//Could serialize and make  3 * HOLDER_HEIGHT + 4 * GAP_SIZE_VERTICAL = 50  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_VERTICAL { get; private set; } = 0.05f;
    public float HOLDER_HEIGHT { get; private set; } = 1.6f;
    

    public static int CARDS_IN_ROW { get; private set; } = 5;
    public static int NUMBER_OF_ROWS { get; private set; } = 3;


    [SerializeField]
    private Army playerArmy;
    [SerializeField]
    private Army opponentArmy;


    [SerializeField]
    private GameObject dummy;

    [SerializeField]
    public Vector3 cardScaleOnBoard { get; private set; } = new Vector3(14f, 19f, 1f);


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
        
    }


    public void ReceiveCardPlayed(CardModel cardModel, GameManager.Players owner)
    {
        switch (owner)
        {
            case GameManager.Players.Player:
                playerArmy.ReceiveCardPlayed(cardModel);
                break;
            case GameManager.Players.Opponent:
                opponentArmy.ReceiveCardPlayed(cardModel);
                break;

            default:
                Debug.Log("default ReceiveCardPlayed from BoardManager returns immediately (does nothing)");
                return;
        }
    }



    public bool CanReceiveCard(CardSO cardSO, GameManager.Players owner)
    {
        switch (owner)
        {
            case GameManager.Players.Player:
                return playerArmy.CanReceiveCard(cardSO);
            case GameManager.Players.Opponent:
                return opponentArmy.CanReceiveCard(cardSO);

            default:
                Debug.Log("default case CanReceiveCard from BoardManager : false.");
                return false;
        }
    }

}
