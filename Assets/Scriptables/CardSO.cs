using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObects/Card")]
public class CardSO : ScriptableObject
{
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private string cardName;

}
