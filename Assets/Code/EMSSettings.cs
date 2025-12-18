using UnityEngine;

public class EMSSettings : MonoBehaviour
{
    public static EMSSettings Instance;

    [Range(0, 100)]
    public int calibratedIntensity = 0;  // 캘리브레이션에서 결정된 값

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
