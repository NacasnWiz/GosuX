using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Board", menuName = "ScriptableObects/Board Dimensions")]
public class BoardDimensionsSO : ScriptableObject
{
    public float PLANE_BASE_SIZE = 10f;

    //based on Plane's base width = 10
    //[field : SerializeField] //Could serialize and make  5 * HOLDER_WIDTH + 6 * GAP_SIZE_HORIZONTAL = 100  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_HORIZONTAL = 0.25f;
    public float HOLDER_WIDTH = 1.7f;
    //[field : SerializedField]//Could serialize and make  3 * HOLDER_HEIGHT + 4 * GAP_SIZE_VERTICAL = 50  but let's not yet. (Canvas scaling management laterrr)
    public float GAP_SIZE_VERTICAL = 0.05f;
    public float HOLDER_HEIGHT = 1.6f;

    //Could make it depend on values above.
    public Vector3 cardScaleOnBoard = new(14f, 19f, 1f);
}
