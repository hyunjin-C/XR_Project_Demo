using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// Unity → Arduino(ESP / Nano 33 IoT 등) 로
/// Wi-Fi(UDP) 패킷을 보내는 EMS 컨트롤러.
/// </summary>
public class EMSController : MonoBehaviour
{
    [Header("Network Settings (Arduino Wi-Fi)")]
    [Tooltip("Arduino 보드의 IP 주소")]
    public string arduinoIp = "192.168.7.10";

    [Tooltip("Arduino 스케치에서 열어둔 UDP 포트 번호")]
    public int arduinoPort = 5005;

    [Header("Debug")]
    public bool logToConsole = true;

    // Unity 쪽 UDP 소켓
    UdpClient udpClient;

    void Awake()
    {
        InitUdpClient();
    }

    void OnDestroy()
    {
        CloseUdpClient();
    }

    /// <summary>
    /// UDP 클라이언트 초기화
    /// </summary>
    void InitUdpClient()
    {
        try
        {
            udpClient = new UdpClient();
            udpClient.Connect(arduinoIp, arduinoPort);

            if (logToConsole)
                Debug.Log($"[EMSController] UDP 연결 준비 완료 → {arduinoIp}:{arduinoPort}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EMSController] UDP 초기화 실패: {e.Message}");
            udpClient = null;
        }
    }

    /// <summary>
    /// UDP 소켓 정리
    /// </summary>
    void CloseUdpClient()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
    }

    /// <summary>
    /// EMS 자극 한 번 보내기.
    /// intensity, durationMs 정보가 포함된 문자열을
    /// UDP 패킷으로 Arduino에 전송.
    ///
    /// 실제 EMS ON/OFF 타이밍은 Arduino 쪽에서 durationMs 기준으로 처리하는 걸 권장.
    /// </summary>
    /// <param name="intensity">자극 세기 (0~100 가정)</param>
    /// <param name="durationMs">자극 지속 시간 (ms)</param>
    public void SendPulse(int intensity, int durationMs)
    {
        if (logToConsole)
        {
            Debug.Log($"[EMSController] SendPulse called. " +
                      $"Intensity={intensity}, Duration={durationMs} ms");
        }

        if (udpClient == null)
        {
            InitUdpClient();
            if (udpClient == null)
            {
                Debug.LogError("[EMSController] UDP 클라이언트가 준비되지 않았습니다.");
                return;
            }
        }

        // 간단한 텍스트 프로토콜 예시:
        // "PULSE,세기,지속시간"
        // e.g., "PULSE,40,800"
        string message = $"PULSE,{intensity},{durationMs}";
        byte[] data = Encoding.ASCII.GetBytes(message);

        try
        {
            udpClient.Send(data, data.Length);

            if (logToConsole)
                Debug.Log($"[EMSController] UDP Sent → {message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EMSController] UDP 전송 실패: {e.Message}");
        }
    }

    // 만약 Unity에서 ON/OFF 두 번 보내고 싶다면 이렇게도 쓸 수 있음.
    // 현재는 사용하지 않고, 필요하면 PulseWaveSequence 쪽에서 호출하면 됨.
    public void SendPulseWithOnOff(int intensity, int durationMs)
    {
        StartCoroutine(SendPulseCoroutine(intensity, durationMs));
    }

    private IEnumerator SendPulseCoroutine(int intensity, int durationMs)
    {
        // 1) EMS ON
        SendRawCommand($"ON,{intensity}");

        // 2) durationMs 만큼 대기
        yield return new WaitForSeconds(durationMs / 1000f);

        // 3) EMS OFF
        SendRawCommand("OFF,0");

        if (logToConsole)
        {
            Debug.Log($"[EMSController] Pulse finished. " +
                      $"Intensity={intensity}, Duration={durationMs} ms");
        }
    }

    /// <summary>
    /// 임의 문자열을 그대로 Arduino로 보내는 유틸 함수
    /// </summary>
    public void SendRawCommand(string command)
    {
        if (udpClient == null)
        {
            InitUdpClient();
            if (udpClient == null) return;
        }

        byte[] data = Encoding.ASCII.GetBytes(command);
        try
        {
            udpClient.Send(data, data.Length);

            if (logToConsole)
                Debug.Log($"[EMSController] Raw UDP Sent → {command}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[EMSController] Raw UDP 전송 실패: {e.Message}");
        }
    }
}
