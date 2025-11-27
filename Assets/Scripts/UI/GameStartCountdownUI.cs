using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{

    private const string NUMBER_POPUP = "NumberPopup";
    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator animator;
    private int previousCountdownNumber;

     private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        GameManager.Instance.OnStateChange += GameManager_OnStateChange;
        Hide();
    }
    private void GameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();

        }

    }
    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountDownToStartTimer());

        countdownText.text = countdownNumber.ToString();
        if (previousCountdownNumber != countdownNumber)
        {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountDownSound();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
