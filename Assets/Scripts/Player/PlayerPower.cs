using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPower : MonoBehaviour
{
    [Tooltip("Maximum number of tiles the power lights up.")]
    [SerializeField] int maxTiles = 4; 

    // State
    int tilesLit = 0;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TilePath tilePath = collision.gameObject.GetComponent<TilePath>();
        if (tilePath != null)
        {
            if (!tilePath.WasVisited())
            {
                if (++tilesLit <= maxTiles)
                {
                    StartCoroutine(tilePath.LightUpTemporarily());
                }
            }

        }
    }
}
