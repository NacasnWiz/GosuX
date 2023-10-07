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
        H�ros = 3,
        Immortel = 5
    }

    public enum Clan
    {
        Abhilasha,
        Galmi,
        Goan_Sul,
        Justice,
        Narashima,
        Phoenix,
        Tomorrow,
        Xi_an
    }

    public enum KeyWord
    {
        Prime,
        Veteran,
        �th�r�,
        Echo,
        Rel�ve_Secr�te,
        Merveilleux,
        Recruteur,
    }

    public Material spriteMaterial;
    //public Material spriteBack;


    public Sprite sprite;


    public string cardName;
    public string nom;
    public Rank rank;
    public Clan clan;

    [TextArea]
    public string texte;
    [TextArea]
    public string passif;
    public bool forceDeVolont�;
    public KeyWord[] keyWords;
    public int rel�veCost;
    public int maxActivations;



}
