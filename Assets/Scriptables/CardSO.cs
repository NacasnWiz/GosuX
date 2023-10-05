using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObects/Card")]
public class CardSO : ScriptableObject
{
    public int ID;

    public enum Rank
    {
        Troupe = 2,
        Héros = 3,
        Immortel = 5
    }

    public enum Clan
    {

    }

    public Material spriteMaterial;
    //public Material spriteBack;

    public string cardName;
    public Rank rank;


}
