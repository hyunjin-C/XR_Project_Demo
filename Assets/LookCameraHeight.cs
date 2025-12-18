using UnityEngine;

public class LockCameraHeight : MonoBehaviour
{
    public float fixedHeight = 1.95f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = fixedHeight;
        transform.position = pos;
    }
}