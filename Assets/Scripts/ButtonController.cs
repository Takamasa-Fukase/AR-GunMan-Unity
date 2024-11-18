using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnTapButton);
    }

    void OnTapButton()
    {
        print("OnTapButton");
        ARController.instance.ShootBullet();
        print("OnTapButton ShootBulletを実行した");        
    }

    // デバッグ用
    void SendDummyMessageFromAndroid()
    {
        var fromAndroidMessage = new AndroidToUnityMessage(
            eventType: AndroidToUnityMessage.EventType.fireWeapon,
            weaponType: AndroidToUnityMessage.WeaponType.pistol
        );
        string dummyJsonString = JsonUtility.ToJson(fromAndroidMessage);
        ARController.instance.OnReceiveMessageFromAndroid(stringMessage: dummyJsonString);
    }

    public void print(string message)
    {
        Debug.Log($"<color=cyan>Unityログ: {message}</color>");
    }
}
