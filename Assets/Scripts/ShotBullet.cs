using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 発射済みの弾の情報を管理するための構造体
internal struct ShotBullet
{
    internal int id;
    internal float shotTime;
    internal Vector3 targetDirection;
    internal GameObject bullet;

    internal ShotBullet(int id, Vector3 targetDirection, GameObject bullet)
    {
        this.id = id;
        shotTime = Time.time;
        this.targetDirection = targetDirection;
        this.bullet = bullet;
    }
}
