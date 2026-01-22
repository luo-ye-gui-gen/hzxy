using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankButton : MonoBehaviour
{
    public Button rankButton;
    public static RankButton instance;
    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        rankButton.gameObject.SetActive(true);
    }
    public void OnRanktButtonClick()
    {
        rankButton.gameObject.SetActive(false);
        StartButton.instance.gameObject.SetActive(false);
        ReturnButton.instance.gameObject.SetActive(true);
    }
}
