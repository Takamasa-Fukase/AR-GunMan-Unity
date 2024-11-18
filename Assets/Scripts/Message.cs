using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Android => Unity方向へのメッセージ送信の為の構造体
public struct AndroidToUnityMessage
{
    // MEMO: JsonUtilityでToJson(), FromJson()を使う場合はプロパティはinternalではなくpublicである必要がある模様（struct自体はinternalでもOKだった）
    public EventType eventType;
    public WeaponType weaponType;
    // TODO: Androidに組み込む時にはコンストラクタ不要になるので消す
    public AndroidToUnityMessage(EventType eventType, WeaponType weaponType)
    {
        this.eventType = eventType;
        this.weaponType = weaponType;
    }
    public enum EventType{
        showWeapon,
        fireWeapon,
    }
    public enum WeaponType{
        pistol,
        bazooka,
    }
}

// Unity => Android方向へのメッセージ送信の為の構造体
public struct UnityToAndroidMessage
{
    // MEMO: JsonUtilityでToJson(), FromJson()を使う場合はプロパティはinternalではなくpublicである必要がある模様（struct自体はinternalでもOKだった）
    public EventType eventType;
    public UnityToAndroidMessage(EventType eventType)
    {
        this.eventType = eventType;
    }
    public enum EventType
    {
        targetHit,
    }
}
