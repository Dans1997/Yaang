using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager GameManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manager is null!");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    #endregion Singleton

    // State
    Vector3 destination;
    TilePath[] tiles;

    // Start is called before the first frame update
    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Finish").transform.position;
        tiles = FindObjectsOfType<TilePath>();
    }

    public bool IsTileValid(Vector3 position)
    {
        foreach (TilePath tile in tiles)
        {
            if (tile.transform.position == position)
                return true;
        }
        return false;
    }

    public bool IsDestination(Vector3 position)
    {
        return position == destination;
    }

}
