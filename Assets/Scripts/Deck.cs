using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [field : SerializeField]
    public List<CardSO> cardsInDeck {get; private set;}

    [SerializeField]
    private List<CardSO> cardsInDeckShuffled;

    [SerializeField]
    private int DECK_STARTING_SIZE = 20;//dummy


    private void Start()
    {
        FillDeckDummy();
        ShuffleDeck();
    }


    [ContextMenu("Shuffle Deck")]
    public void ShuffleDeck()
    {
        Shuffle(cardsInDeck);
    }

    public void FillDeckDummy()
    {
        for (int i = 0; i < DECK_STARTING_SIZE; i++)
        {
            cardsInDeck.Add(GameManager.Instance.ALL_CARDS_LIST[i % GameManager.Instance.ALL_CARDS_LIST.Length]);
        }
    }

    public CardSO Draw()
    {
        CardSO drawnedCard = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);

        return drawnedCard;
    }

    private void OnMouseDown()
    {
        ActualiseShuffledDeck();
        UIManager.Instance.DisplayDeckCards(cardsInDeckShuffled);
    }

    private void ActualiseShuffledDeck()
    {
        cardsInDeckShuffled = cardsInDeck;
        Shuffle(cardsInDeckShuffled);
    }


    /*-- To shuffle an array a of n elements (indices 0..n-1): Fisher-Yates Durstenfield Algorithm
    for i from 0 to n-2 do
    j <- random integer such that i <(inclusive) j <(exclusive) n
    exchange a[j] and a[i]*/
    public void Shuffle(List<CardSO> list)
    {
        for (int i = 0; i < list.Count - 1; ++i)
        {
            int j = Random.Range(i, list.Count);
            CardSO temp = list[i];

            list[i] = list[j];
            list[j] = temp;
        }
    }

}
