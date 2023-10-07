using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CardModel : MonoBehaviour
{
    [field : SerializeField]
    public CardSO _cardSO { get; private set; }

    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;

    [field : SerializeField]
    public Vector3 baseScale { get; private set; } = new Vector3(17f, 1f, 22f);

    public bool isHovered = false;
    public bool isInHand = false;


    public int currentActivations;
    public int battleValue;


    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Set(CardSO so)
    {
        _cardSO = so;
        meshRenderer.material = _cardSO.spriteMaterial;
    }

    [ContextMenu("Set to assigned SO (Inspector only intended")]
    private void Set()
    {
        Set(_cardSO);
    }

    private void OnMouseEnter()
    {
        isHovered = true;

        if (isInHand)
        {
            GameManager.Instance.playerHand.ShowCardOver(transform);
        }
        
        Debug.Log("Hovered " + gameObject.name);
    }

    private void OnMouseExit()
    {
        isHovered = false;

        if (isInHand)
        {
            GameManager.Instance.playerHand.AdjustCardsPos();//maybe bad architecture idk
        }

        Debug.Log("No longer hovers " + _cardSO.cardName);
    }

    private void OnMouseDown()
    {
        if (isInHand)
        {
            GameManager.Instance.playerHand.AdjustCardsPos();//maybe bad architecture idk
            GameManager.Instance.playerHand.PlayCard(this);
        }
    }

}
