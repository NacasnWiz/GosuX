using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    [field: SerializeField]
    public GameManager.Players owner { get; private set; }

    [field : SerializeField]
    public List<CardSO> cardsInDiscardPile { get; private set; } = new List<CardSO>();

    public void AddCards(List<CardSO> cards)
    {
        foreach (CardSO card in cards)
        {
            AddCard(card);
        }
    }

    public void AddCards(List<CardModel> cards)
    {
        foreach (CardModel card in cards)
        {
            AddCard(card);
        }
    }

    public void AddCard(CardSO card)
    {
        cardsInDiscardPile.Add(card);
    }

    public void AddCard(CardModel card)
    {
        AddCard(card._cardSO);
    }

    private void OnMouseDown()
    {
        UIManager.Instance.DisplayDiscardPileCards(cardsInDiscardPile, owner);
    }
}
