using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetter : MonoBehaviour
{
    void OnEnable()
    {
        if (AndroidMessageCenter.Instance != null)
        {
            AndroidMessageCenter.Instance.ResetGameSceneEvent += ResetScene;
        }
    }

    void OnDisable()
    {
        if (AndroidMessageCenter.Instance != null)
        {
            AndroidMessageCenter.Instance.ResetGameSceneEvent -= ResetScene;
        }
    }

    private void ResetScene()
    {
        print("ログUnity SceneResetter ResetScene");

        // 現在のシーンを再読み込みして完全に初期化する
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}