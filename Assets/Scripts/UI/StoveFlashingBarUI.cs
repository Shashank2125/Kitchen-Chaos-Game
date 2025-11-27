using UnityEngine;

public class StoveFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "isFlashing";
    [SerializeField] private StoveCounter stoveCounter;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        animator.SetBool(IS_FLASHING, false);
    }
    private void StoveCounter_OnProgressChanged(object sender, IProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
        animator.SetBool(IS_FLASHING, show);
    }
   
}
