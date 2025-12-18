using UnityEngine;

public class FallingNote : MonoBehaviour
{
    public float fallSpeed = 1f;

    void Update()
    {
        // 월드 기준 아래 방향으로만 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 너무 아래로 내려가면 삭제
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }
}
