using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    [Header("GameObject")]
    private GameObject Side;

    [Space(5)]
    [Header("Android Keys")]
    public string AndroidBannerID;
    public string AndroidInterstitialID;
    public string AndroidRewardvideoID;

    [Space(5)]
    [Header("iOS Keys")]
    public string iOSBannerID;
    public string iOSInterstitialID;
    public string iOSRewardvideoID;

    [Header("String")]
    private string SideID;

    [Header("Int")]
    [Range(1, 120)]
    public int TimerBetweenAds;

    [Header("Bool")]
    [HideInInspector]
    public bool BannerStatus;
    private bool ShowAds = true;

    [Header("AdMob")]
    private BannerView Banner;
    private InterstitialAd Interstitial;
    private RewardedAd RewardVideo;

    public static AdManager Instance;

    public UnityAction InterstitialClosed { get; private set; }
    public UnityAction<bool> CompleteMethod { get; private set; }

    private void Awake()
    {
        Advertisements.Instance.Initialize();

        if (Instance != this)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                .SetSameAppKeyEnabled(true).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        //RequestBanner();
        Advertisements.Instance.ShowBanner(BannerPosition.TOP, BannerType.Banner);

        RequestInterstitial();
        RequestRewardVideo();
    }
    public void Request()
    {
        RequestBanner();
        RequestInterstitial();
        RequestRewardVideo();
    }
    #region Banner
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidBannerID;
#elif UNITY_IPHONE
            string adUnitId = "iOSBannerID";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.Banner = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        this.Banner.OnAdLoaded += this.HandleBannerOnAdLoaded;
        // Called when an ad request failed to load.
        this.Banner.OnAdFailedToLoad += this.HandleBannerOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.Banner.OnAdOpening += this.HandleBannerOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.Banner.OnAdClosed += this.HandleBannerOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.Banner.LoadAd(request);

    }
    public void HandleBannerOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleBannerOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.LoadAdError.GetMessage());
    }

    public void HandleBannerOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleBannerOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    #endregion
    #region Interstitial
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidInterstitialID;
#elif UNITY_IPHONE
        string adUnitId = iOSInterstitialID;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.Interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.Interstitial.OnAdLoaded += HandleInterstitialOnAdLoaded;
        // Called when an ad request failed to load.
        this.Interstitial.OnAdFailedToLoad += HandleInterstitialOnAdFailedToLoad;
        // Called when an ad is shown.
        this.Interstitial.OnAdOpening += HandleInterstitialOnAdOpening;
        // Called when the ad is closed.
        this.Interstitial.OnAdClosed += HandleInterstitialOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.Interstitial.LoadAd(request);

    }
    public void ShowInterstitial()
    {
        if (ShowAds)
        {
            if (Advertisements.Instance.IsInterstitialAvailable())
            {
                StopAllCoroutines();
                Advertisements.Instance.ShowInterstitial(InterstitialClosed);


            }
           
        }
    }
    public void HandleInterstitialOnAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received");
    }

    public void HandleInterstitialOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("Interstitial failed to load: ");
    }

    public void HandleInterstitialOnAdOpening(object sender, EventArgs args)
    {
        print("HandleAdOpening event received");
    }

    public void HandleInterstitialOnAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
        ShowAds = false;
        StartCoroutine(ReactiveAds());
        RequestInterstitial();
    }

    #endregion
    #region RewardVideo
    private void RequestRewardVideo()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = AndroidRewardvideoID;
#elif UNITY_IPHONE
            adUnitId = iOSRewardvideoID;
#else
            adUnitId = "unexpected_platform";
#endif

        this.RewardVideo = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.RewardVideo.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.RewardVideo.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.RewardVideo.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.RewardVideo.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.RewardVideo.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.RewardVideo.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.RewardVideo.LoadAd(request);
    }
    public void ShowRewardVideo()
    {
        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            StopAllCoroutines();
            Advertisements.Instance.ShowRewardedVideo(CompleteMethod);

        }
        else
        {
            Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
        }
    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleRewardedAdFailedToLoad event received with message: ");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        print("HandleRewardedAdFailedToShow event received with message: ");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        print("HandleRewardedAdClosed event received");
        ShowAds = false;
        StartCoroutine(ReactiveAds());
        RequestRewardVideo();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        print("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
    }

    #endregion
    private IEnumerator ReactiveAds()
    {
        yield return new WaitForSeconds(TimerBetweenAds);

        ShowAds = true;
    }
}
