using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField]
    public Deck _deck { get; private set; }
    [field: SerializeField]
    public DiscardPile _discardPile { get; private set; }
    [field: SerializeField]
    public Hand _hand { get; private set; }
    [field: SerializeField]
    public Army _army { get; private set; }

    //Camera ?



}
