using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandRayPointer : MonoBehaviour
{
    [Header("설정")]
    public OVRHand ovrHand; // OVRHand 컴포넌트
    public LineRenderer lineRenderer;
    public float rayLength = 5f;
    public LayerMask uiLayer;

    [Header("제스처 설정")]
    public bool requirePinchToShow = true; // 핀치할 때만 레이저 보이기

    private bool isPinching = false;
    private bool wasPinching = false;

    void Start()
    {
        // LineRenderer 설정
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (ovrHand == null) return;

        // 핀치 제스처 확인 (검지와 엄지가 붙으면)
        isPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        // 제스처에 따라 레이저 표시
        if (requirePinchToShow)
        {
            lineRenderer.enabled = isPinching;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        if (lineRenderer.enabled)
        {
            CastRay();
        }

        wasPinching = isPinching;
    }

    void CastRay()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Vector3 endPoint = transform.position + transform.forward * rayLength;

        if (Physics.Raycast(ray, out hit, rayLength, uiLayer))
        {
            endPoint = hit.point;

            // UI 버튼 클릭 처리
            if (isPinching && !wasPinching)
            {
                Button button = hit.collider.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    Debug.Log("버튼 클릭: " + button.name);
                }
            }
        }

        // 레이저 라인 그리기
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}