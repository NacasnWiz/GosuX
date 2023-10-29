using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private float MAX_HEIGHT = 0.5f;
    [SerializeField]
    private float MIN_HEIGHT = -0.5f;

    private List<CardModel> cardsHeld =  new List<CardModel>();

    [field : SerializeField]
    public Player owner { get; private set; }

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
        AddCard(owner._deck.Draw());
    }

    [ContextMenu("Redraw")]
    public void ReDraw()
    {
        owner._deck.AddCards(cardsHeld);
        
        foreach (CardModel card in cardsHeld)
        {
            Destroy(card.gameObject);
        }
        cardsHeld.Clear();

        DrawCards(7);
    }

    private void AddCard(CardSO so)
    {
        CardModel card = GameManager.Instance.CreateCardModel(so);
        AddCard(card);
    }

    public void AddCard(CardModel card)
    {
        card.owner = owner;
        card.transform.SetParent(transform, false);
        card.isInHand = true;

        cardsHeld.Add(card);

        AdjustCardsPos();
    }

    [ContextMenu("Adjust Cards Pos")]
    public void AdjustCardsPos()
    {
        for (int i = 0; i < cardsHeld.Count; ++i)
        {
            if (!cardsHeld[i].isInToDiscard)
                AdjustCardPos(i);
        }
    }

    private void AdjustCardPos(int indexInHand)//TODO: problem when holding 8+ cards, the top one goes a bit out of screen
    {
        float offsetZ = (MAX_HEIGHT - MIN_HEIGHT) / cardsHeld.Count;
        float offsetY = 0.1f;

        cardsHeld[indexInHand].transform.localPosition = (MIN_HEIGHT + indexInHand * offsetZ) * Vector3.forward - indexInHand * offsetY * Vector3.up;
    }

    private void RemoveCard(int index)
    {
        if(index < 0 || index >= cardsHeld.Count)
        {
            return;
        }
        cardsHeld[index].isInHand = false;
        cardsHeld.RemoveAt(index);
        AdjustCardsPos();
    }

    public void RemoveCard(CardModel cardInHand)
    {
        RemoveCard(cardsHeld.IndexOf(cardInHand));
    }


    public void PutCardToDiscard(CardModel cardInHand)
    {
        if (UIManager.Instance.NeedToDiscardMoreCards())
        {
            UIManager.Instance.ReceiveCardToDiscard(cardInHand);
        }
    }

    public int GetHandSize()
    {
        return cardsHeld.Count;
    }
}
