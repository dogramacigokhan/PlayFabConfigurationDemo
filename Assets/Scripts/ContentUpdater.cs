using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingMessageText;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private ContentView contentView;
    [SerializeField] private GameObject loadButtonsParent;

    public void LoadLocalData()
    {
        this.HideButtonsAndShowLoadingBar();
        contentView.ShowWithData(ContentData.Default);
    }

    public void LoadRemoteData()
    {
        this.HideButtonsAndShowLoadingBar();
        this.contentView.ShowMessage("Downloaded data will be shown here.");
    }

    private void HideButtonsAndShowLoadingBar()
    {
        loadButtonsParent.SetActive(false);
        loadingSlider.gameObject.SetActive(true);
    }

    private void SetLoadingMessage(string message)
    {
        loadingMessageText.text = message;
        loadingSlider.value = Mathf.Min(1f, loadingSlider.value + 0.33f); // Auto increment the slider value
    }
}
