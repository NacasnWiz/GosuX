using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Deck : MonoBehaviour
{
    [field: SerializeField]
    public List<CardSO> cardsInDeck { get; private set; }

    [field: SerializeField]
    public List<CardSO> cardsInDeckShuffled { get; private set; }

    [field: SerializeField]
    public Player owner { get; private set; }


    public static UnityEvent<Deck> ev_DeckClicked = new();


    private void Start()
    {
        ShuffleDeck();
    }


    [ContextMenu("Shuffle Deck")]
    public void ShuffleDeck()
    {
        Shuffle(cardsInDeck);
    }

    public void AddCard(CardSO card)
    {
        cardsInDeck.Add(card);
    }

    public void AddCards(List<CardSO> cards)
    {
        foreach (CardSO card in cards)
        {
            AddCard(card);
            if (card.rank == CardSO.Ranks.Troupe)
            {
                AddCard(card);
            }
        }
    }

    public void AddCards(List<CardModel> cardModels)
    {
        foreach (CardModel card in cardModels)
        {
            cardsInDeck.Add(card._cardSO);
        }
        ShuffleDeck();
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
        ev_DeckClicked.Invoke(this);
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
