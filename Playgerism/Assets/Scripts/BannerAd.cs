using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour {

    public string gameId_android = "3317802";
    public string gameId_ios = "3317803";
    public string placementId = "banner1";
    public bool testMode = false;

    void Start()
    {
#if PLATFORM_IOS //|| UNITY_EDITOR
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId_ios, testMode, false);
        }
#endif
#if PLATFORM_ANDROID || UNITY_EDITOR
        Advertisement.Initialize(gameId_android, testMode);
#endif
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.isInitialized || !Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(placementId);
    }
}

