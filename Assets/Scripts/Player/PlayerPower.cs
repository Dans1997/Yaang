using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPower : MonoBehaviour
{
    [Tooltip("Duration the tile keeps lit until starting to turn off.")]
    [SerializeField] float duration = 0f; 
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TilePath tilePath = collision.gameObject.GetComponent<TilePath>();
        if(tilePath != null)
        {
            if(!tilePath.WasVisited())
                StartCoroutine(tilePath.LightUpTemporarily(duration));
        }
    }
}
