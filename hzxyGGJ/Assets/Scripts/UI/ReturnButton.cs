using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    public Button returnButton;
    public static ReturnButton instance;
    public GameObject leaderboardPanel;

    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        returnButton.gameObject.SetActive(false);
    }
    public void OnReturnButtonClick()
    {
        returnButton.gameObject.SetActive(false);
        StartButton.instance.gameObject.SetActive(true);
        RankButton.instance.gameObject.SetActive(true);
        leaderboardPanel.gameObject.SetActive(false);
    }
}
