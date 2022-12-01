using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentView : UIBehaviour
{
    [SerializeField] private RectTransform containerRectTransform;
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [SerializeField] private TextMeshProUGUI contentText;

    public void ShowWithData(ContentData contentData)
    {
        var serializedData = JsonConvert.SerializeObject(contentData, Formatting.Indented);
        this.ShowMessage(serializedData);
    }

    public void ShowMessage(string message)
    {
        contentText.text = message;
        AnimateIn();
    }

    private void AnimateIn()
    {
        gameObject.SetActive(true);
        containerCanvasGroup.alpha = 0;

        var anchoredPos = containerRectTransform.anchoredPosition;
        DOTween.Sequence()
            .Append(containerRectTransform
                .DOAnchorPosY(anchoredPos.y, 0.25f)
                .From(new Vector2(anchoredPos.x, anchoredPos.y - 100)))
            .Join(containerCanvasGroup.DOFade(1f, 0.25f));
    }
}