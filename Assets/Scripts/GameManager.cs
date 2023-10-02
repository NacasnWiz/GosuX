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

    [SerializeField] private Camera _camera;//will have public methods accessed via _instance

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
    GameObject dummy;


    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
