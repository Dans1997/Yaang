using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(TilePath))]
public class PathEditor : MonoBehaviour
{
    // Cached Components
    PlayerController player;

    // State
    float moveUnitX = 0f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        Vector2 playerMoveUnits = player.GetMoveUnits();
        moveUnitX = playerMoveUnits.x;
    }

    // Update is called once per frame
    void Update()
    {
        SnapToPosition();
        RenameGameObjectAndLabel();
    }

    private void RenameGameObjectAndLabel()
    {
        string labelText = $"{transform.position.x},{transform.position.y}";
        gameObject.name = labelText;
        if (gameObject.tag == "Start") gameObject.name = "Start";
        if (gameObject.tag == "Finish") gameObject.name = "Finish";
    }

    private void SnapToPosition()
    {
        float x = RoundToNearest(transform.position.x, moveUnitX);
        float y = RoundToNearest(transform.position.y, 0.25f);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    float RoundToNearest(float n, float x) => Mathf.Round(n / x) * x;
}
