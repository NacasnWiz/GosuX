using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] GameObject _board;

    private float PLANE_BASE_SIZE = 10f;
    //based on Plane's base width = 10
    public static float GAP_SIZE_HORIZONTAL = 0.25f;
    public static float HOLDER_WIDTH = 1.7f;
    //Could serialize and make  5 * HOLDER_WIDTH + 6 * GAP_SIZE_HORIZONTAL = 100  but let's not yet. (Canvas scaling management laterrr)
    public static float GAP_SIZE_VERTICAL = 0.05f;
    public static float HOLDER_HEIGHT = 1.6f;
    //Could serialize and make  3 * HOLDER_HEIGHT + 4 * GAP_SIZE_VERTICAL = 50  but let's not yet. (Canvas scaling management laterrr)

    
    Vector3[,] cardSpots = new Vector3[3, 5];

    [SerializeField]
    GameObject dummy;


    private void Awake()
    {
        SetCardSpots();
    }

    // Start is called before the first frame update
    void Start()
    {
        DummyShowCardSpots();
    }

    private void DummyShowCardSpots()
    {
        foreach (var cardSpot in cardSpots)
        {
            Instantiate(dummy, cardSpot, Quaternion.identity, _board.transform);
        }
    }

    private void SetCardSpots()
    {
        for (int i = 0; i < cardSpots.GetLength(0); ++i)
        {
            for (int j = 0; j < cardSpots.GetLength(1); ++j)
            {
                SetCardSpot(i, j);
            }
        }
    }

    private void SetCardSpot(int i, int j)
    {
        cardSpots[i, j] = transform.position + new Vector3(transform.localScale.x * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_HORIZONTAL * (j + 1) + HOLDER_WIDTH * (j + 0.5f)), 0.05f, transform.localScale.z * (-PLANE_BASE_SIZE / 2f + GAP_SIZE_VERTICAL * (i + 1) + HOLDER_HEIGHT * (i + 0.5f)));
    }




}
