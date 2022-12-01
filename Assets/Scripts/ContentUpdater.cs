using System;
using Newtonsoft.Json;
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
        LoginToPlayFab(FetchTitleData);
    }

    private void LoginToPlayFab(Action onLoggedIn)
    {
        SetLoadingMessage("Logging in");

        var request = new PlayFab.ClientModels.LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };

        PlayFab.PlayFabClientAPI.LoginWithCustomID(
            request,
            _ => onLoggedIn?.Invoke(),
            error => contentView.ShowMessage($"Error:\n {error.ErrorMessage}"));
    }

    private void FetchTitleData()
    {
        SetLoadingMessage("Fetching content");

        // Request ALL title data key-value pairs as Dictionary<string, string>
        var request = new PlayFab.ClientModels.GetTitleDataRequest();

        PlayFab.PlayFabClientAPI.GetTitleData(
            request,
            result =>
            {
                SetLoadingMessage("Processing the result");

                var downloadedData = new ContentData
                {
                    IntValue = int.Parse(result.Data["IntValue"]),
                    BoolValue = bool.Parse(result.Data["BoolValue"]),
                    StringValue = result.Data["StringValue"],
                    ComplexValue = JsonConvert.DeserializeObject<ContentData.ComplexData>(result.Data["ComplexValue"]),
                };
                contentView.ShowWithData(downloadedData);
            },
            error => contentView.ShowMessage($"Error:\n {error.ErrorMessage}"));
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
