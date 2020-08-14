using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator ShakeCamera(float duration, float strength)
    {
        Vector3 startPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * strength;
            float offsetY = Random.Range(-1f, 1f) * strength;

            transform.localPosition = new Vector3(offsetX, offsetY, startPos.z);
            elapsed += Time.deltaTime;
            yield return null; // every frame once
        }

        transform.localPosition = startPos;
    }
}
