using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CardModel : MonoBehaviour
{
    public GameManager.Players owner;

    [field : SerializeField]
    public CardSO _cardSO { get; private set; }

    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;

    [field : SerializeField]
    public Vector3 baseScale { get; private set; } = new Vector3(17f, 22f, 1f);

    public bool isShownOver = false;
    public bool isInHand = false;
    public bool isInToDiscard = false;


    public int currentActivations;
    public int battleValue;

    [SerializeField]
    private float hoverHeight = 5f;
    private float lastPosY;
    [SerializeField]
    private float hoverScaleFactor = 1.1f;
    private float lastScaleFactor;


    public static UnityEvent<CardModel> ev_CardClicked = new();

    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Set(CardSO so)
    {
        _cardSO = so;

        meshRenderer.material.mainTexture = _cardSO.sprite.texture;
    }

    [ContextMenu("Set to assigned SO (Inspector only intended")]
    private void Set()
    {
        Set(_cardSO);
    }

    private void OnMouseEnter()
    {
        if (isInHand || isInToDiscard)
        {
            ShowCardOver();
        }
        
        Debug.Log("Hovered " + _cardSO.cardName);
    }

    private void OnMouseExit()
    {
        if (isInHand || isInToDiscard)
        {
            StopShowCardOver();
        }

        Debug.Log("No longer hovers " + _cardSO.cardName);
    }

    private void OnMouseDown()
    {
        ev_CardClicked.Invoke(this);
    }

    public void ShowCardOver()
    {
        if(isShownOver)
        {
            return;
        }

        lastPosY = transform.position.y;
        transform.position += Vector3.up * hoverHeight;
        lastScaleFactor = transform.localScale.x / baseScale.x;
        transform.localScale = baseScale * hoverScaleFactor;

        isShownOver = true;
    }

    public void StopShowCardOver()
    {
        if (!isShownOver)
        {
            return;
        }

        transform.position = new Vector3 (transform.position.x, lastPosY, transform.position.z);
        transform.localScale = baseScale * lastScaleFactor;
        isShownOver = false;
        if(isInHand)
        {
            GameManager.Instance.players[owner]._hand.AdjustCardsPos();//Not best
        }
    }

    public void SetPos(Vector3 position)
    {
        transform.position = position;
        lastPosY = position.y;
    }

    public void PlayCardEffect()
    {
        _cardSO.OnPlayEffect();
    }

}
