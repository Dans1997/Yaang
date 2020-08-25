using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(TilePath))]
public class PathEditor : MonoBehaviour
{
    // Cached Components
    PlayerController player;
    TilePath tilePath;

    // State
    float moveUnitX = 0f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        tilePath = GetComponent<TilePath>();
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem)
        {
            ParticleSystem.MainModule particles = particleSystem.main;
            particles.simulationSpeed = 0f;
        }

        Vector2 playerMoveUnits = player.GetMoveUnits();
        moveUnitX = playerMoveUnits.x;

        RenameGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        SnapToPosition();
    }

    private void RenameGameObject()
    {
        gameObject.name = $"{tilePath.GetName()}";
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
