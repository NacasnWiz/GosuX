using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(SpriteRenderer))]//TODO: replace the MeshRenderer with a SpriteRenderer instead!
[RequireComponent(typeof(MeshCollider))]
public class CardModel : MonoBehaviour //TODO review encapsulation of public members once the script will be in function
{
    public Player owner;

    [field : SerializeField]
    public CardSO _cardSO { get; private set; }

    //public SpriteRenderer spriteRenderer;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;

    [field : SerializeField]
    public Vector3 baseScale { get; private set; } = new Vector3(17f, 22f, 1f);
    [SerializeField]
    private float hoverHeight = 5f;
    private float lastPosY;
    [SerializeField]
    private float hoverScaleFactor = 1.1f;
    private float lastScaleFactor;

    public bool isShownOver = false;
    public bool isInHand = false;
    public bool isInArmy = false;
    public bool isInToDiscard = false;


    public int currentActivations{ get; private set; }
    public int battleValue{ get; private set; }
    public string nom{ get; private set; }
    public int clan { get; private set; }
    public CardSO.Ranks rank { get; private set; }



    public static UnityEvent<CardModel> ev_CardClicked = new();

    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Set(CardSO so)
    {
        _cardSO = so;
        battleValue = (int)_cardSO.rank;
        nom = so.nom;
        clan = (int)so.clan;
        rank = so.rank;

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

        lastPosY = transform.position.y; //of
        transform.position += Vector3.up * hoverHeight;
        lastScaleFactor = transform.localScale.x / baseScale.x; //oof
        transform.localScale = baseScale * hoverScaleFactor; //oooof

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
            owner._hand.AdjustCardsPos();//Not best
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
