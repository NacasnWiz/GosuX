using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField]
    public GameManager.Players ID { get; private set; }

    public List<CardSO.Clan> clansList = new();

    [field: SerializeField]
    public Deck _deck { get; private set; }
    [field: SerializeField]
    public DiscardPile _discardPile { get; private set; }
    [field: SerializeField]
    public Hand _hand { get; private set; }
    [field: SerializeField]
    public Army _army { get; private set; }

    public bool hasPassed = false;

    public int SupremacyPoint = 0;

    //Camera ?

    public void AddCardsToDeck(List<CardSO> cards)
    {
        _deck.AddCards(cards);
    }

    public void AddCard(CardSO card)
    {
        _deck.AddCard(card);
    }

    public void ShuffleDeck()
    {
        _deck.ShuffleDeck();
    }

    public void DrawCards(int nb_toDraw)
    {
        _hand.DrawCards(nb_toDraw);
    }

    [ContextMenu("Calculate Battle Score")]
    public int CalculateArmyScore()
    {
        int score = _army.CalculateBattleScore();
        Debug.Log(ID.ToString() + " currently totalizes " + score + " battle points");
        return score;
    }

}
