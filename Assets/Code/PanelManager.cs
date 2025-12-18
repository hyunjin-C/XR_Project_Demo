using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject calibrationPanel;
    public GameObject mainPanel;

    void Start()
    {
        ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        calibrationPanel.SetActive(false);
        mainPanel.SetActive(false);
    }

    public void ShowCalibrationPanel()
    {
        startPanel.SetActive(false);
        calibrationPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void ShowMainPanel()
    {
        startPanel.SetActive(false);
        calibrationPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
