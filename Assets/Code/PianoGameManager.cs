using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoGameManager : MonoBehaviour
{
    // 싱글톤 인스턴스 (어디서나 접근용)
    public static PianoGameManager Instance;

    private void Awake()
    {
        // 씬에 하나만 존재하도록 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 피아노 키가 눌렸을 때 PianoKey에서 호출할 함수
    public void KeyPressed(string keyName)
    {
        Debug.Log($"KeyPressed 호출됨: {keyName}");
        // 나중에 여기에서 판정, 점수, EMS 트리거 등 처리하면 됨
    }
}
