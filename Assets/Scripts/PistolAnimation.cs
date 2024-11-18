using UnityEngine;
using System.Collections;

public class PistolAnimation : MonoBehaviour
{
    public static PistolAnimation instance;
    private bool isExecutingPistolShootAnimation = false;
    float destinationAngleValue = (Mathf.PI / 6) * Mathf.Rad2Deg;
    float rotationTime = 0.06f; // 回転にかける時間

    void Awake()
    {
        instance = this;
    }

    public void ExecuteShootingAnimation()
    {
        StartCoroutine(ShootingAnimation());
    }

    IEnumerator ShootingAnimation()
    {
        // まだ前回のアニメーションが実行中なら弾く
        if (!isExecutingPistolShootAnimation)
        {
            // アニメーション実行中なのでtrueに切り替え
            isExecutingPistolShootAnimation = true;

            // MEMO: ピストルの3Dモデルのデフォルト角度がそういえば横向きに倒した様な角度になっていた。
            // なのでFPS視点に設置した時点では倒れているピストルを起こした（回転させた）状態になっている。
            // 従って、カメラから見た角度だと銃口の跳ね上げはX軸の回転に思えるが、
            // 実際にはZ軸の逆方向回転を行うことで銃口の跳ね上げアニメショーンの見た目になる。

            // 最初の回転
            yield return RotateOverTime(
                target: gameObject.transform,
                rotation: new Vector3(0, 0, -destinationAngleValue),
                duration: rotationTime
            );

            // 元の位置に戻る回転
            yield return RotateOverTime(
                target: gameObject.transform,
                rotation: new Vector3(0, 0, destinationAngleValue),
                duration: rotationTime
            );

            // アニメーション実行が完了したのでfalseに切り替え
            isExecutingPistolShootAnimation = false;
        }
    }

    IEnumerator RotateOverTime(Transform target, Vector3 rotation, float duration)
    {
        Quaternion originalRotation = target.rotation;
        Quaternion finalRotation = originalRotation * Quaternion.Euler(rotation);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            target.rotation = Quaternion.Lerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.rotation = finalRotation;
    }

    public void print(string message)
    {
        Debug.Log($"<color=cyan>Unityログ: {message}</color>");
    }
}