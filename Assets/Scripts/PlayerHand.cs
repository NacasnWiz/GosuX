using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private float MAX_HEIGHT = 0.5f;
    [SerializeField]
    private float MIN_HEIGHT = -0.5f;

    [SerializeField]
    private float hoverHeight = 5f;
    [SerializeField]
    private float hoverScaleFactor = 1.1f;

    [SerializeField]
    private List<CardModel> cardsHeld =  new List<CardModel>();


    private void Start()
    {
        AdjustCardsPos();
    }

    public void DrawCards(int number)
    {
        for (int i = 0; i < number; ++i)
        {
            DrawFromDeck();
        }
    }

    private void DrawFromDeck()
    {
        AddCard(GameManager.Instance.playerDeck.Draw());
    }

    private void AddCard(CardSO so)
    {
        CardModel card = GameManager.Instance.CreateCard(so, transform);
        card.transform.SetParent(transform);
        card.isInHand = true;

        cardsHeld.Add(card);
    }

    private void AddCard(CardModel card)
    {
        AddCard(card._cardSO);
    }

    public void AdjustCardsPos()
    {

        for (int i = 0; i < cardsHeld.Count; ++i)
        {
            AdjustCardPos(i);
        }
    }

    private void AdjustCardPos(int indexInHand)
    {
        float offsetZ = (MAX_HEIGHT - MIN_HEIGHT) / cardsHeld.Count;
        float offsetY = 5f;

        cardsHeld[indexInHand].transform.localPosition = (MIN_HEIGHT + indexInHand * offsetZ) * Vector3.forward - indexInHand * offsetY * Vector3.up;
        cardsHeld[indexInHand].transform.localScale = new Vector3 (17f,22f,1f);//NEED TO WORK ON THE WHOLE SCALING THING.
    }

    public void ShowCardOver(Transform cardTransform)
    {
        cardTransform.position = new Vector3(cardTransform.position.x, hoverHeight, cardTransform.position.z);

        cardTransform.localScale *= hoverScaleFactor;
    }

    public void PlayCard(CardModel cardInHand)
    {
        if (!BoardManager.Instance.CanReceiveCard(cardInHand._cardSO))
        {
            return;
        }
        BoardManager.Instance.ReceiveCardPlayed(cardInHand);

        cardInHand.isInHand = false;
        RemoveCard(cardsHeld.IndexOf(cardInHand));
    }

    private void RemoveCard(int index)
    {
        cardsHeld.RemoveAt(index);
    }


}
