using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARController : MonoBehaviour
{    
    public static ARController instance;
    [SerializeField] GameObject pistolParent;
    [SerializeField] GameObject sphere;
    [SerializeField] GameObject target;
    Camera cam;
    private List<ShotBullet> shotBullets = new();
    private int nextBulletId = 1;
    // 2秒で10メートル移動するための速度を計算（5メートル/秒）
    private float distancePerSecond = 10f / 2f;
    private List<GameObject> targets = new();
    private GameObject currentPistolParent;
    // private Vector3 pistolFPSishPosition = new Vector3(1.651289f, -1.85f, 5.788595f);
    // private Quaternion pistolFPSishRotaion = Quaternion.Euler(-3.908f, 75.45f, 2.119f);
    private GameObject pistol;
    private Vector3 pistolFPSishEulerAngles = new();

    // デバッグ用
    float previousCheckedTime = 0f;

    private void Awake()
    {
        // 他のスクリプトファイルからアクセス可能にするためのインスタンスを変数に格納
        instance = this;

        // カメラのインスタンスを変数に格納
        cam = Camera.main;

        // スケールを調整
        sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        target.transform.localScale = new Vector3(0.2f, 0.2f, 0.142f);

        // 当たり判定のための設定
        sphere.AddComponent<SphereCollider>();
        sphere.AddComponent<Rigidbody>();
        sphere.GetComponent<Rigidbody>().useGravity = false;

        GenerateTargetsToRandomPositions();

        currentPistolParent = Instantiate(pistolParent, cam.transform.position, cam.transform.rotation);
        pistol = currentPistolParent.gameObject.transform.Find("colt1911").gameObject;
        pistolFPSishEulerAngles = pistol.transform.localEulerAngles;
    }

    private void Update()
    {
        // 発射済みの弾があれば移動（座標の更新）と2秒経過チェックをする
        foreach (var shotBullet in shotBullets)
        {
            // targetDirectionに沿って移動
            Vector3 moveDirection = shotBullet.targetDirection.normalized * distancePerSecond * Time.deltaTime;
            shotBullet.bullet.transform.Translate(moveDirection);

            // 2秒以上経過した弾を削除
            if (Time.time - shotBullet.shotTime > 2f)
            {
                if (shotBullet.bullet != null) Destroy(shotBullet.bullet);
                shotBullets.Remove(shotBullet);
            }
        }

        // 全ての的をカメラ目線キープになる様、角度をカメラに合わせ続ける
        foreach (var clonedTarget in targets)
        {
            clonedTarget.transform.rotation = cam.transform.rotation;
        }

        // ピストルをFPS風視点キープになる様、座標と角度をカメラに合わせ続ける
        KeepPistolPositionAndAngleToFPSIsh();

        // デバッグ用（1秒ごとに実行される）
        CheckPerSecond();
    }

    private void GenerateTargetsToRandomPositions()
    {
        for (int i = 0; i < 50; i++)
        {
            Vector3 randomPosition = GetRandomTargetPosition();
            GameObject clonedTarget = Instantiate(target, randomPosition, cam.transform.rotation);
            targets.Add(clonedTarget);
        }
    }

    private Vector3 GetRandomTargetPosition()
    {
        // 原点からの至近距離とみなす境界値
        float closeRangeBorder = 0.167f;

        float randomX = Random.Range(-1.0f, 1.0f);
        float randomY = Random.Range(-0.5f, 0.7f);
        float randomZNegativeAndExceptCloseRange = Random.Range(-1.0f, -(closeRangeBorder));
        float randomZPositiveAndExceptCloseRange = Random.Range(closeRangeBorder, 1.0f);
        float randomZAllRange = Random.Range(-1.0f, 1.0f);

        float randomZ;

        // 原点0（プレーヤーの初期位置）の超至近距離に的を集中させずに、
        // なるべくばらけさせることができる様に制御する
        if (randomX < -(closeRangeBorder) || randomX > closeRangeBorder || randomY < -(closeRangeBorder) || randomY > closeRangeBorder)
        {
            // XとYのどちらかが原点0から0.5より離れている場合は、
            // 原点0に近い値（-0.5 ~ 0.5）も含めてユーザーの近くも遠くも両方含めた範囲のZのランダム値を渡す
            randomZ = randomZAllRange;
        }
        else
        {
            // それに対し、XもYも両方原点0に近い値（-0.5 ~ 0.5）になった場合はZだけは、
            // （-0.5 ~ 0.5）の範囲を除外した値から選ばせて、最低でもユーザーから0.5より離れさせる
            // 50%の確率で分岐（Random.valueは0~1の範囲を生成するので0.5を境にしている）
            randomZ = Random.value < 0.5f ? randomZNegativeAndExceptCloseRange : randomZPositiveAndExceptCloseRange;
        }

        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
        return randomPosition;
    }

    private void SendMessageToAndroid(UnityToAndroidMessage message)
    {
        print($"SendMessageToAndroid message: {message}");
        AndroidJavaObject unityToAndroidMessenger = new AndroidJavaObject("com.takamasafukase.ar_gunman_android.UnityToAndroidMessenger");
        // 構造体からJSON文字列に変換
        string jsonStringMessage = JsonUtility.ToJson(message);

        print($"SendMessageToAndroid jsonStringMessage: {jsonStringMessage}");
        unityToAndroidMessenger.Call("sendMessage", jsonStringMessage);
    }

    // Android側から呼び出すメソッドなのでpublicにしている
    public void OnReceiveMessageFromAndroid(string stringMessage)
    {
        print($"OnReceiveMessageFromAndroid stringMessage: {stringMessage}");
        // JSON文字列から構造体に変換
        AndroidToUnityMessage message = JsonUtility.FromJson<AndroidToUnityMessage>(stringMessage);
        print($"OnReceiveMessageFromAndroid message: {message}");

        switch (message.eventType)
        {
            case AndroidToUnityMessage.EventType.showWeapon:
                print("showWeapon");
                // TODO: ピストルを表示＆FPS視点に固定（座標と角度をUpdate()内で移動）
                break;
            case AndroidToUnityMessage.EventType.fireWeapon:
                print("fireWeapon");
                HandleFireWeapon(weaponType: message.weaponType);
                break;
        }
    }

    private void HandleFireWeapon(AndroidToUnityMessage.WeaponType weaponType)
    {
        print($"HandleFireWeapon weaponType: {weaponType}");
        switch (weaponType)
        {
            case AndroidToUnityMessage.WeaponType.pistol:
                ShootBullet();
                break;
            case AndroidToUnityMessage.WeaponType.bazooka:
                break;
        }
    }

    // ピストルの弾の発射を描画
    // TODO: Unityデバッグ用にpublicにしてみた。本来はprivateにしたい
    public void ShootBullet()
    {
        print("ShootBullet");
        // 発射のスタート地点としてカメラ位置に設置
        GameObject bulletInstance = Instantiate(sphere, cam.transform.position, Quaternion.identity);

        // 発射済みの球を管理する為の構造体を生成
        ShotBullet newBullet = new ShotBullet(
            id: nextBulletId++,
            targetDirection: cam.transform.forward,
            bullet: bulletInstance
        );

        // 発射済みの弾リストに格納
        shotBullets.Add(newBullet);

        // ピストルオブジェクトに対して発砲時の銃口跳ね上げアニメーションを実行させる
        PistolAnimation.instance.ExecuteShootingAnimation();
    }

    public void HandleOnCollisionEnterToTargetObject(Collision collision, GameObject targetObject)
    {
        print("HandleOnCollisionEnterToTargetObject");
        // 衝突したオブジェクトが弾かどうか確認
        ShotBullet? hitBullet = shotBullets.Find(b => b.bullet == collision.gameObject);
        if (hitBullet != null)
        {
            print("弾を削除します");
            // 弾を削除
            Destroy(hitBullet?.bullet);
            shotBullets.Remove((ShotBullet)hitBullet);

            var message = new UnityToAndroidMessage(
                eventType: UnityToAndroidMessage.EventType.targetHit
            );
            // 的に弾が当たったことをAndroid側に通知
            SendMessageToAndroid(message: message);
        }
        GameObject? hitTargetObject = targets.Find(target => target == targetObject);
        if (hitTargetObject != null)
        {
            print("該当の的のオブジェクトを配列から削除します");
            targets.Remove((GameObject)hitTargetObject);
        }
    }

    private void KeepPistolPositionAndAngleToFPSIsh()
    {
        // ピストルをラップしている空の親オブジェクトに対する操作
        currentPistolParent.transform.position = cam.transform.position;
        currentPistolParent.transform.rotation = cam.transform.rotation;

        // ピストル自体のオブジェクトに対する操作（ユーザー操作によってはズレる場合があるため補正する）
        pistol.transform.localEulerAngles = pistolFPSishEulerAngles;
    }

    // デバッグ用（1秒ごとに実行される）
    private void CheckPerSecond()
    {
        if ((Time.time - previousCheckedTime) > 1)
        {
            previousCheckedTime = Time.time;

            // ここに1秒ごとに実行したい処理を書く
            // print($"Update\n- pistol.transform.localEulerAngles: {pistol.transform.localEulerAngles}\n");
        }
    }

    public void print(string message)
    {
        Debug.Log($"<color=cyan>Unityログ: {message}</color>");
    }
}
