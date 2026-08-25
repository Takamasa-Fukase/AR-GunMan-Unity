using System;
using UnityEngine;

// UnitySendMessageを受信するためにMonoBehaviorである必要があるので継承
public class AndroidMessageCenter : MonoBehaviour
{
    public static AndroidMessageCenter Instance;

    public event Action RenderWeaponFiringEvent;

    void Awake()
    {
        Instance = this;
    }

    public void SendMessageToAndroid(UnityToAndroidMessage message)
    {
        print($"SendMessageToAndroid message: {message}");
        AndroidJavaObject unityMessageCenter = new("com.takamasafukase.ar_gunman_android.UnityMessageCenter");
        // 構造体からJSON文字列に変換
        string jsonStringMessage = JsonUtility.ToJson(message);

        print($"SendMessageToAndroid jsonStringMessage: {jsonStringMessage}");
        unityMessageCenter.Call("onReceivedMessageFromUnity", jsonStringMessage);
    }

    // Android側から呼び出される
    public void OnReceivedMessageFromAndroid(string message)
    {
        print($"OnReceivedMessageFromAndroid message: {message}");
        // JSON文字列から構造体に変換
        AndroidToUnityMessage fromAndroidMessage = JsonUtility.FromJson<AndroidToUnityMessage>(message);
        print($"OnReceivedMessageFromAndroid fromAnddroidMessage: {message}");

        switch (fromAndroidMessage.eventType)
        {
            case AndroidToUnityMessage.EventType.showWeapon:
                print("showWeapon");
                // TODO: ピストルを表示＆FPS視点に固定（座標と角度をUpdate()内で移動）
                break;
            case AndroidToUnityMessage.EventType.fireWeapon:
                print("fireWeapon");
                RenderWeaponFiringEvent.Invoke();
                break;
        }
    }
}
