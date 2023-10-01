using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    [SerializeField] private CardSO cardSO;

    [SerializeField] private MeshCollider meshCollider;

    [SerializeField] private MeshRenderer meshRenderer;
    
    public void Set(CardSO so)
    {
        cardSO = so;
    }

    private void OnMouseOver()
    {
        transform.position += Vector3.up * 5f;
    }

}
