using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // ゲームオブジェクト同士が接触したタイミングで呼ばれる
    void OnCollisionEnter(Collision collision)
    {
        print($"OnCollisionEnter collision.gameObject.name: {collision.gameObject.name}, this.gameObject.name: {this.gameObject.name}");
        ARController.instance.HandleOnCollisionEnterToTargetObject(collision: collision, targetObject: this.gameObject);
        Destroy(this.gameObject);
    }

    public void print(string message)
    {
        Debug.Log($"<color=cyan>Unityログ: CubeController {message}</color>");
    }
}
