using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerDiscardPilePanel;
    [SerializeField]
    private GameObject _playerDeckPanel;
    [SerializeField]
    private GameObject _opponentDiscardPilePanel;
    [SerializeField]
    private GameObject _opponentDeckPanel;
    [SerializeField]
    private GameObject _toDiscardPanel;
    [SerializeField]
    private TMP_Text toDiscardText;
    [SerializeField]
    private Button confirmDiscardButton;
    [SerializeField] private Button seeOpponentButton;
    [SerializeField] private Button returnToPlayerButton;


    [SerializeField]
    private int CARDS_PER_ROW = 6;
    [SerializeField]
    private Vector2 CARDS_SPACING = new Vector2(1f, 2f);
    [SerializeField]
    private Vector2 CARDS_DIMENSION = new Vector2(17f, 22f);
    [SerializeField]
    private Vector2 displayOffset = new Vector2(8f, -10f);

    private float scrollingOffset = 0f;

    [SerializeField, Range(0f, 100f)]
    private float scrollStrength = 0f;
    [SerializeField, Range(0f, 100f)]
    private float toDiscardBaseOffsetX = 1f;

    [SerializeField]
    private List<CardModel> _displayedCards = new List<CardModel>();

    [SerializeField]
    private List<CardModel> cardsDiscarding = new List<CardModel>();
    private int nb_cardsToDiscard;


    private GameObject currentPanel;
    private GameManager.Players currentPlayerDisplayedCards = GameManager.Players.Player;
    private bool isDisplayingCards = false;


    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manager is Null !!!");
            }

            return _instance;
        }
    }


    private void Start()
    {
        RulesManager.Instance.ev_EnterDiscardPhase.AddListener(() => DisplayToDiscardPanel());
        Deck.ev_DeckClicked.AddListener((deck) => DisplayDeckCards(deck));
        DiscardPile.ev_DiscardPileClicked.AddListener((discardPile) => DisplayDiscardPileCards(discardPile));
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if(currentPanel != null)
        {
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.05f && _displayedCards.Count > 2 * CARDS_PER_ROW)
            {
                scrollingOffset -= Input.GetAxis("Mouse ScrollWheel") * scrollStrength;
                float maxScrollingOffset = -displayOffset.y / 2f - GetVerticalScreenPos(_displayedCards.Count - CARDS_PER_ROW);
                scrollingOffset = Mathf.Clamp(scrollingOffset, 0f, maxScrollingOffset);

                AdjustDisplayedCardsTransform();
            }
            
        }
    }


    public void DisplayDeckCards(Deck deck)
    {
        scrollingOffset = 0f;

        switch (deck.owner.ID)
        {
            case GameManager.Players.Player:
                DisplayCards(_playerDeckPanel, deck.cardsInDeckShuffled, deck.owner.ID);
                break;
            case GameManager.Players.Opponent:
                DisplayCards(_opponentDeckPanel, deck.cardsInDeckShuffled, deck.owner.ID);
                break;

            default: break;
        }
        
    }

    public void DisplayDiscardPileCards(DiscardPile discardPile)
    {
        scrollingOffset = 0f;

        switch (discardPile.owner.ID)
        {
            case GameManager.Players.Player:
                DisplayCards(_playerDiscardPilePanel, discardPile.cardsInDiscardPile, discardPile.owner.ID);
                break;
            case GameManager.Players.Opponent:
                DisplayCards(_opponentDiscardPilePanel, discardPile.cardsInDiscardPile, discardPile.owner.ID);
                break;

            default: break;
        }
    }

    private void DisplayCards(GameObject displayPanel, List<CardSO> cards, GameManager.Players owner)
    {
        if(isDisplayingCards)
        {
            return;
        }

        currentPlayerDisplayedCards = owner;

        OpenPanel(displayPanel);
        isDisplayingCards = true;
        for (int indexCard = 0; indexCard < cards.Count; ++indexCard)
        {
            _displayedCards.Add(GameManager.Instance.CreateCardModel(cards[indexCard], transform));
        }
        AdjustDisplayedCardsTransform();
    }

    private void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        currentPanel = panel;
    }

    private void CloseCurrentPanel()
    {
        currentPanel.SetActive(false);
        currentPanel = null;
    }

    [ContextMenu("Readjust cards")]
    private void AdjustDisplayedCardsTransform()
    {
        for (int indexCard = 0; indexCard < _displayedCards.Count; ++indexCard)
        {
            AdjustDisplayedCardTransform(indexCard);
        }
    }

    private void AdjustDisplayedCardTransform(int index)
    {
        _displayedCards[index].transform.rotation = GetOnScreenRotation();
        _displayedCards[index].transform.localPosition = GetScreenPos(index);
    }

    private Quaternion GetOnScreenRotation()
    {
        Quaternion displayRotation = Quaternion.Euler(90f, 0f, 0f);
        if (currentPlayerDisplayedCards == GameManager.Players.Opponent)
            displayRotation *= Quaternion.Euler(0f, 0f, 180f);
        return displayRotation;
    }

    private Vector3 GetScreenPos(int index)
    {
        float horizontalPos = GetHorizontalScreenPos(index);
        float verticalPos = scrollingOffset + GetVerticalScreenPos(index);

        horizontalPos *= (int)currentPlayerDisplayedCards;
        verticalPos *= (int)currentPlayerDisplayedCards;

        Vector3 screenPos = Camera.main.transform.position + new Vector3(horizontalPos, -15f, verticalPos);


        return screenPos;
    }

    private float GetHorizontalScreenPos(int index)
    {
        int columnIndex = index % CARDS_PER_ROW;

        return displayOffset.x - Camera.main.orthographicSize * Camera.main.aspect + (1 + columnIndex) * CARDS_SPACING.x + (columnIndex + 0.5f) * CARDS_DIMENSION.x;
    }

    private float GetVerticalScreenPos(int index)
    {
        int rowIndex = index / CARDS_PER_ROW;

        return displayOffset.y + Camera.main.orthographicSize - (1 + rowIndex) * CARDS_SPACING.y - (rowIndex + 0.5f) * CARDS_DIMENSION.y;
    }

    public void OnExitButtonClick()
    {
        FlushDisplayedCards();
        isDisplayingCards = false;
        CloseCurrentPanel();
    }

    private void FlushDisplayedCards()
    {
        foreach (CardModel card in _displayedCards)
        {
            Destroy(card.gameObject);
        }
        _displayedCards.Clear();
    }


    public void OnSeeOpponentButtonClicked()
    {
        seeOpponentButton.gameObject.SetActive(false);
        GameManager.Instance.SeeOpponent();
        returnToPlayerButton.gameObject.SetActive(true);
    }

    public void ReturnToPlayerButtonClicked()
    {
        returnToPlayerButton.gameObject.SetActive(false);
        GameManager.Instance.ReturnToPlayer();
        seeOpponentButton.gameObject.SetActive(true);
    }

    public void DisplayToDiscardPanel()
    {
        nb_cardsToDiscard = RulesManager.Instance.toDiscard[GameManager.Instance.currentPlayer.ID];
        OpenPanel(_toDiscardPanel);
        AdjustToDiscardText();
    }

    public bool NeedToDiscardMoreCards()
    {
        return (nb_cardsToDiscard - cardsDiscarding.Count) > 0;
    }

    public void ReceiveCardToDiscard(CardModel card)
    {
        if (!NeedToDiscardMoreCards())
        {
            Debug.Log("You don't need to discard more cards");

            return;
        }

        cardsDiscarding.Add(card);
        card.isInToDiscard = true;

        AdjustDiscardingDisplay();
    }

    private void AdjustDiscardingDisplay()
    {
        AdjustDiscardingCardsPos();
        AdjustToDiscardText();
        AdjustConfirmDiscardButtonDisplay();
    }

    private void AdjustConfirmDiscardButtonDisplay()
    {
         confirmDiscardButton.gameObject.SetActive(!NeedToDiscardMoreCards());
    }



    [ContextMenu("Adjust Discarding Cards Pos")]
    private void AdjustDiscardingCardsPos()
    {
        for (int index = 0; index < cardsDiscarding.Count; ++index)
        {
            float offsetX = toDiscardBaseOffsetX * nb_cardsToDiscard * 0.5f;
            Debug.Log("Bump");
            cardsDiscarding[index].SetPos((2f+index * 0.1f) * Vector3.up + _toDiscardPanel.transform.position + offsetX * Vector3.left + (3+index * toDiscardBaseOffsetX) * Vector3.right);
        }
    }

    private void AdjustToDiscardText()
    {
        if(nb_cardsToDiscard > 0)
        {
            toDiscardText.text = "Discard " + nb_cardsToDiscard + " cards. (" + (nb_cardsToDiscard - cardsDiscarding.Count) + " more)";
        }
        else
        {
            toDiscardText.text = "Clic button to confirm discard.";
        }
    }

    public void OnConfirmDiscardButtonClick()
    {
        RulesManager.Instance.RegisterDiscard(GameManager.Instance.currentPlayer.ID, nb_cardsToDiscard);
        FlushCardsDiscarding();

        confirmDiscardButton.gameObject.SetActive(false);
        CloseCurrentPanel();
    }

    private void FlushCardsDiscarding()
    {      
        foreach(CardModel card in cardsDiscarding)
        {
            GameManager.Instance.players[GameManager.Instance.currentPlayer.ID]._hand.RemoveCard(card);
            GameManager.Instance.players[GameManager.Instance.currentPlayer.ID]._discardPile.AddCard(card);
            Destroy(card.gameObject);
        }
        cardsDiscarding.Clear();
    }


    public void ReplaceCardInhand(CardModel card)
    {
        cardsDiscarding.Remove(card);
        card.isInToDiscard = false;
        GameManager.Instance.players[GameManager.Instance.currentPlayer.ID]._hand.AdjustCardsPos();
        AdjustDiscardingDisplay();
    }

}
