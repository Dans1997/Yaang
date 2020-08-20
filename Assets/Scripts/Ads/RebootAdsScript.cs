using UnityEngine;
using UnityEngine.Advertisements;

public class RebootAdsScript : MonoBehaviour, IUnityAdsListener
{
    #if UNITY_ANDROID
        private string gameId = "3778943";
    #elif UNITY_IOS
        private string gameId = "3778942";
    #endif

    string myPlacementId = "video";
    bool testMode = true;

    // Initialize the Ads listener and service:
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowRebootAd()
    {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(myPlacementId))
        {
            Advertisement.Show(myPlacementId);
        }
        else
        {
            Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == myPlacementId)
        {
            // Optional actions to take when the placement becomes ready(For example, enable the rewarded ads button)
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            Debug.Log("You finished the ad! Thanks!.");
        }
        else if (showResult == ShowResult.Skipped)
        {
            Debug.Log("You skipped the ad.");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }

        // Reload Scene Regardless 
        SceneLoader.SceneLoaderInstance.ReloadScene();
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
