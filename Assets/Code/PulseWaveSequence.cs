using System.Collections;
using UnityEngine;

public class PulseWaveSequence : MonoBehaviour
{
    [Header("Playhead")]
    public RectTransform playhead;

    [Header("Pulse Visual")]
    public GameObject pulsePrefab;
    public RectTransform trackStart;
    public RectTransform trackEnd;

    [Header("EMS")]
    public EMSController emsController;
    public int calibratedIntensity = 40;

    [Header("Pattern Settings (ms)")]
    public int initialWaitMs = 3000;
    public int longPulseMs = 800;   // long pulse duration
    public int shortPulseMs = 400;  // short pulse duration
    public int gapMs = 500;
    public int repetitions = 4;

    [Header("Animator Clip Settings")]
    public AnimationClip pulseClip; // PulseAnim 애니메이션 클립 넣기 (길이 읽어오기)

    Coroutine playRoutine;
    GameObject currentPulse;

    float clipLength = 1.0f; // 기본값 1초

    void Awake()
    {
        if (pulseClip != null)
            clipLength = pulseClip.length;
    }

    int TotalPatternMs()
    {
        int oneCycle = longPulseMs + gapMs + shortPulseMs + gapMs;
        return initialWaitMs + repetitions * oneCycle;
    }

    IEnumerator MovePlayhead()
    {
        if (playhead == null || trackStart == null || trackEnd == null)
            yield break;

        Vector3 startPos = trackStart.position;
        Vector3 endPos = trackEnd.position;

        float totalSec = TotalPatternMs() / 1000f;   // ← 전체 시퀀스 시간
        float elapsed = 0f;

        while (elapsed < totalSec)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / totalSec);

            playhead.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
    }



    public void StartSequence()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlaySequence());
    }

    public void StopSequence()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        if (currentPulse != null)
        {
            Destroy(currentPulse);
            currentPulse = null;
        }
    }

    IEnumerator PlaySequence()
    {
        StartCoroutine(MovePlayhead());

        yield return new WaitForSeconds(initialWaitMs / 1000f);

        for (int i = 0; i < repetitions; i++)
        {
            yield return StartCoroutine(PlayOnePulse(longPulseMs));
            yield return new WaitForSeconds(gapMs / 1000f);

            yield return StartCoroutine(PlayOnePulse(shortPulseMs));
            yield return new WaitForSeconds(gapMs / 1000f);
        }

        playRoutine = null;
    }

    IEnumerator PlayOnePulse(int durationMs)
    {
        Debug.Log($"[Wave] PlayOnePulse start, durationMs = {durationMs}");

        if (pulsePrefab == null || trackStart == null)
        {
            Debug.LogError("[Wave] pulsePrefab / trackStart 누락");
            yield break;
        }

        float durationSec = durationMs / 1000f;

        // 1) PulseVisual 생성 (WavePanel 밑, 트랙 시작 위치 또는 Playhead 위치)
        currentPulse = Instantiate(pulsePrefab, trackStart.parent);
        RectTransform pulseRT = currentPulse.GetComponent<RectTransform>();
        Animator anim = currentPulse.GetComponent<Animator>();

        if (pulseRT != null)
        {
            // ★ Playhead 위치에 맞추고 싶으면 이 줄을 사용
            pulseRT.SetParent(playhead, false);

            pulseRT.anchoredPosition = Vector2.zero;
            pulseRT.localScale = Vector3.one;
        }

        // 2) EMS 발사
        emsController?.SendPulse(calibratedIntensity, durationMs);

        // 3) 애니메이션 속도 = 0.8초 / 0.4초에 맞게 자동 설정
        if (anim != null)
        {
            float desiredTime = durationSec;   // 0.8 또는 0.4
            float speed = clipLength / desiredTime;
            anim.speed = speed;
            anim.SetTrigger("Pulse");
        }

        // 4) duration 동안 그냥 기다리기 (이제 위치 이동 없음)
        yield return new WaitForSeconds(durationSec);

        // 5) 끝나면 PulseVisual 제거
        if (currentPulse != null)
        {
            Destroy(currentPulse);
            currentPulse = null;
        }
    }
}