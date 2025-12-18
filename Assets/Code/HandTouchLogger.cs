using UnityEngine;
using System.Collections.Generic;

public class PianoKeyVisual : MonoBehaviour
{
    public string handTag = "Hand";
    public float pressDistance = 0.02f;
    public float moveSpeed = 20f;
    public Color normalColor = Color.white;
    public Color pressedColor = Color.cyan;

    private Renderer rend;
    private Vector3 originalPos;
    private Vector3 targetPos;
    private bool isPressed = false;

    // 여러 개의 콜라이더(손가락 등)를 지원하기 위해
    private HashSet<Collider> touchingHands = new HashSet<Collider>();

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalPos = transform.localPosition;
        targetPos = originalPos;

        if (rend != null)
            rend.material.color = normalColor;
    }

    private void Update()
    {
        // 부드럽게 이동 (튀는 느낌 방지)
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            Time.deltaTime * moveSpeed
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(handTag)) return;

        touchingHands.Add(other);

        if (!isPressed)   // 이미 눌려있으면 또 누르지 않음
        {
            isPressed = true;
            targetPos = originalPos + new Vector3(0, -pressDistance, 0);

            if (rend != null)
                rend.material.color = pressedColor;

            Debug.Log("Key Pressed: " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(handTag)) return;

        touchingHands.Remove(other);

        // 손/손가락이 전부 빠져나간 경우에만 키를 올림
        if (touchingHands.Count == 0 && isPressed)
        {
            isPressed = false;
            targetPos = originalPos;

            if (rend != null)
                rend.material.color = normalColor;

            Debug.Log("Key Released: " + gameObject.name);
        }
    }
}
