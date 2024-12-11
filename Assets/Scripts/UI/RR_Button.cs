using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class RR_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public RectTransform border;
    private Animator animator;

    private Vector3 borderOpen;
    private Vector3 borderDef;


    private Vector3 btnOpen;
    private Vector3 btnDef;


    private Vector3 btnClicked;

    private bool animReady = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        borderOpen = border.localScale * 1.7f;
        borderDef = border.localScale;

        btnOpen = transform.localScale * 1.1f;
        btnDef = transform.localScale;
        btnClicked = btnOpen / 1.3f;
    }

    private void Start()
    {
        if (GetComponent<Button>())
            GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TriggerButtonAnim(btnOpen, borderOpen, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TriggerButtonAnim(btnDef, borderDef, false);
    }

    private bool animationDone = true;
    public void OnClick()
    {
        transform.DOScale(btnClicked, 0.2f).OnComplete(delegate
        {
            transform.DOScale(btnOpen, 0.1f).OnComplete(delegate { animationDone = true; });
        }).SetEase(Ease.InBack);

        /*if (!animationDone) return;

        animationDone = false;
        transform.DOScale(btnClicked, 0.2f).OnComplete(delegate
        {
            transform.DOScale(btnOpen, 0.1f).OnComplete(delegate { animationDone = true; });
        });*/
    }

    public void TriggerButtonAnim(Vector3 target, Vector3 borderTarget, bool isActive)
    {
        animator.SetBool("Active", isActive);
        transform.DOScale(target, 0.4f).SetEase(Ease.OutBack);
        border.DOScale(borderTarget, 0.4f).SetEase(Ease.OutBack);
    }   

    public void OnSelect(BaseEventData eventData)
    {
        //TriggerButtonAnim(btnOpen, borderOpen, true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //TriggerButtonAnim(btnDef, borderDef, false);
    }
}
