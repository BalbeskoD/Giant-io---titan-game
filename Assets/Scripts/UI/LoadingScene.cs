using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField] private AssetReference asset;
        [SerializeField] private Slider slider;
        private SceneInstance _handler;
        private Sequence _sequence;

        private async void Start()
        {
            FB.Init(() => SetFBAdvertiserTracking(true));

            var handler = Addressables.LoadSceneAsync(asset, LoadSceneMode.Single, false);
            await slider.DOValue(1f, 5f);
            await handler;
            await handler.Result.ActivateAsync();
        }

        private void SetFBAdvertiserTracking(bool value)
        {
            FB.Mobile.SetAdvertiserTrackingEnabled(value);
            FB.Mobile.SetAdvertiserIDCollectionEnabled(value);
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }
    }
}