using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int CARDS_PER_ROW = 6;
    [SerializeField]
    private Vector2 CARDS_SPACING = new Vector2(1f, 2f);
    [SerializeField]
    private Vector2 CARDS_DIMENSION = new Vector2(17f, 22f);
    [SerializeField]
    private Vector2 displayOffset = new Vector2(8f, -10f);

    private float scrollingOffset = 0f;

    public float scrollStrength = 0f;

    [SerializeField]
    private List<CardModel> _displayedCards = new List<CardModel>();

    private GameObject currentPanel;
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

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if(currentPanel != null)
        {
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.05f)
            {
                scrollingOffset -= Input.GetAxis("Mouse ScrollWheel") * scrollStrength;
                scrollingOffset = Mathf.Clamp(scrollingOffset, 0f, - displayOffset.y/2f - GetVerticalScreenPos(_displayedCards.Count - CARDS_PER_ROW));

                AdjustDisplayedCardsPos();
            }
            
        }
    }


    public void DisplayDeckCards(List<CardSO> cards, GameManager.Players possessor)
    {
        switch (possessor)
        {
            case GameManager.Players.Player:
                DisplayCards(_playerDeckPanel, cards);
                break;
            case GameManager.Players.Opponent:
                DisplayCards(_opponentDeckPanel, cards);
                break;

            default: break;
        }
        
    }

    public void DisplayDiscardPileCards(List<CardSO> cards, GameManager.Players possessor)
    {
        switch (possessor)
        {
            case GameManager.Players.Player:
                DisplayCards(_playerDiscardPilePanel, cards);
                break;
            case GameManager.Players.Opponent:
                DisplayCards(_opponentDiscardPilePanel, cards);
                break;

            default: break;
        }
    }

    private void DisplayCards(GameObject panel, List<CardSO> cards)
    {
        if(isDisplayingCards)
        {
            return;
        }

        OpenPanel(panel);
        for (int indexCard = 0; indexCard < cards.Count; ++indexCard)
        {
            _displayedCards.Add(GameManager.Instance.CreateCard(cards[indexCard], transform));
        }
        AdjustDisplayedCardsPos();
    }

    private void OpenPanel(GameObject panel)
    {
        scrollingOffset = 0f;

        panel.SetActive(true);
        currentPanel = panel;
        isDisplayingCards = true;
    }

    private void CloseCurrentPanel()
    {
        currentPanel.SetActive(false);
        currentPanel = null;
        isDisplayingCards = false;
    }

    [ContextMenu("Readjust cards")]
    private void AdjustDisplayedCardsPos()
    {
        for (int indexCard = 0; indexCard < _displayedCards.Count; ++indexCard)
        {
            AdjustDisplayedCardPos(indexCard);
        }
    }

    private void AdjustDisplayedCardPos(int index)
    {
        _displayedCards[index].transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        _displayedCards[index].transform.localPosition = GetScreenPos(currentPanel, index);
    }

    private Vector3 GetScreenPos(GameObject panel, int index)
    {
        int columnIndex = index % CARDS_PER_ROW;
        int rowIndex = index / CARDS_PER_ROW;

        float horizontalPos = GetHorizontalScreenPos(index);
        float verticalPos = scrollingOffset + GetVerticalScreenPos(index);


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
        
    }

}
