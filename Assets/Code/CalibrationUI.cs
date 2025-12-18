using UnityEngine;
using TMPro;

public class CalibrationUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text valueText;          // 가운데 값 (예: 0)
    public TMP_InputField inputField;   // 아래 입력창

    [Header("Value Settings")]
    public int minValue = 0;
    public int maxValue = 100;

    [Header("EMS Calibration")]
    public EMSController emsController;   // 🔹 Unity 쪽 EMSController 참조
    public int calibrationPulseMs = 1000;  // 🔹 칼리브레이션용 짧은 EMS 길이
    public bool sendOnEveryChange = true; // 🔹 true면 +/− 바꿀 때마다 바로 EMS 발사

    private int currentValue = 0;

    void Start()
    {
        int.TryParse(valueText.text, out currentValue);
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        valueText.text = currentValue.ToString();
        inputField.text = currentValue.ToString();

        // 🔹 Calibration 시작 시 EMS를 0으로 리셋 (안전)
        ResetEMSToZero();
    }

    void OnDestroy()
    {
        // 🔹 씬 종료 시에도 EMS를 0으로 리셋 (안전)
        ResetEMSToZero();
    }

    // -----------------------------
    // EMS를 0으로 리셋하는 함수
    // -----------------------------
    private void ResetEMSToZero()
    {
        if (emsController == null)
        {
            Debug.LogWarning("[Calibration] EMSController가 연결되어 있지 않습니다.");
            return;
        }

        // intensity = 0, duration = 100ms (짧게)
        emsController.SendPulse(0, 100);
        Debug.Log("[Calibration] EMS Reset to 0");
    }

    // -----------------------------
    // 내부에서 EMS 한 번 쏘는 헬퍼 함수
    // -----------------------------
    private void SendCalibrationPulse()
    {
        if (emsController == null)
        {
            Debug.LogWarning("[Calibration] EMSController가 연결되어 있지 않습니다.");
            return;
        }

        // intensity = currentValue, duration = calibrationPulseMs
        emsController.SendPulse(currentValue, calibrationPulseMs);
        Debug.Log($"[Calibration] EMS Pulse sent. intensity={currentValue}, duration={calibrationPulseMs} ms");
    }

    // Apply 버튼 → InputField 값을 ValueText로 적용 (+ 필요시 EMS 전송)
    public void OnApplyButtonClicked()
    {
        if (int.TryParse(inputField.text, out int newValue))
        {
            newValue = Mathf.Clamp(newValue, minValue, maxValue);
            currentValue = newValue;

            valueText.text = currentValue.ToString();
            inputField.text = currentValue.ToString();

            // Apply 눌렀을 때는 항상 한 번 쏴주도록 (보통 여기서 피험자 확인)
            SendCalibrationPulse();
        }
        else
        {
            Debug.LogWarning("입력 값이 숫자가 아닙니다.");
        }
    }

    // + 버튼
    public void OnPlusButtonClicked()
    {
        currentValue = Mathf.Clamp(currentValue + 1, minValue, maxValue);
        valueText.text = currentValue.ToString();
        inputField.text = currentValue.ToString();

        if (sendOnEveryChange)
        {
            SendCalibrationPulse();
            Debug.Log("현재 값: " + currentValue);
        }
    }

    // - 버튼
    public void OnMinusButtonClicked()
    {
        currentValue = Mathf.Clamp(currentValue - 1, minValue, maxValue);
        valueText.text = currentValue.ToString();
        inputField.text = currentValue.ToString();

        if (sendOnEveryChange)
        {
            SendCalibrationPulse();
            Debug.Log("현재 값: " + currentValue);
        }
    }

    // Save 버튼 → 값 저장 후 EMS를 0으로 리셋
    public void OnSaveButtonClicked()
    {
        if (EMSSettings.Instance != null)
        {
            EMSSettings.Instance.calibratedIntensity = currentValue;
            Debug.Log($"[Calibration] Saved intensity: {currentValue}");
        }
        else
        {
            Debug.LogWarning("[Calibration] EMSSettings.Instance가 없습니다.");
        }

        // 🔹 Save 후 EMS를 0으로 리셋 (안전)
        ResetEMSToZero();

        Debug.Log("[Calibration] EMS reset to 0 after save.");
    }
}