using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private float MAX_HEIGHT = 0.5f;
    [SerializeField]
    private float MIN_HEIGHT = -0.5f;

    [SerializeField]
    private List<CardModel> cardsHeld =  new List<CardModel>();

    [SerializeField]//DO NOT KEEP
    CardModel dummyToDraw;


    public void AddCard(CardModel so)
    {
        cardsHeld.Add(Instantiate(dummyToDraw, transform));
    }

    private void AdjustCardsPos()
    {
        float offset = (MAX_HEIGHT - MIN_HEIGHT)/cardsHeld.Count;

        for (int i = 0; i < cardsHeld.Count; ++i)
        {
            cardsHeld[i].transform.localPosition = (MIN_HEIGHT + i * offset) * Vector3.forward - i * offset * Vector3.up;
        }
    }

    private void Start()
    {
        for (int i = 0; i < 5; ++i)
        {
            AddCard(dummyToDraw);
        }
    }

    private void Update()
    {
        AdjustCardsPos();
    }
}
