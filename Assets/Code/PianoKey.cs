using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // AudioSource가 할당 안 되어있으면 자동으로 찾기
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 그래도 없으면 경고
        if (audioSource == null)
        {
            Debug.LogError($"[PianoKey] {name}에 AudioSource가 없습니다!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log($"[PianoKey] OnTriggerEnter: {other.name}, tag={other.tag}");

            // audioSource가 null이 아닐 때만 재생
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // PianoGameManager가 있을 때만 호출
            if (PianoGameManager.Instance != null)
            {
                PianoGameManager.Instance.KeyPressed(this.name);
            }
        }
    }
}