using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 playerPosition = Vector2.zero;

    public List<int> currentTilePortalVisibleCellList = null;
    public List<Rect> tileList = new List<Rect>();
    public int currentTileIndex = 0;
    public Rect currentPortalIndex;

    private void Update()
    {
        playerPosition.x = transform.position.x;
        playerPosition.y = transform.position.z;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one + Vector3.up * 100f);
    }
}
