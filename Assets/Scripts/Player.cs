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

    //Camera ?

    public void ShuffleDeck()
    {
        _deck.ShuffleDeck();
    }

    public void DrawCards(int nb_toDraw)
    {
        _hand.DrawCards(nb_toDraw);
    }

}
