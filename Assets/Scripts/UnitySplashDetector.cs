using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitySplashDetector : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitForSplashScreenEnd());
    }

    private IEnumerator WaitForSplashScreenEnd()
    {
        // 無料版のスプラッシュ画面が完全に終わるまで待機
        while (!SplashScreen.isFinished)
        {
            yield return null;
        }

        var toAndroidMessage = new UnityToAndroidMessage(
            eventType: UnityToAndroidMessage.EventType.splashFinished
        );
        AndroidMessageCenter.Instance.SendMessageToAndroid(message: toAndroidMessage);
    }
}