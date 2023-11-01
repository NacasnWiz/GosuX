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

    public enum Clan//careful not to have a clan equal zero
    {
        Abhilasha = 2,
        Galmi = 3,
        Goan_Sul = 5,
        Justice = 7,
        Narashima = 11,
        Phoenix = 13,
        Tomorrow = 17,
        Xi_an = 19,
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


    public void OnPlayEffect()
    {
        Debug.Log(nom + " has been played.");
    }

}
