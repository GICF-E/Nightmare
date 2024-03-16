using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // 位置
    public Vector3 position;
    // 旋转角度
    public Quaternion rotation;
    // 是否被全图标记
    public bool isMarked;
    // 玩家的生命值
    public float playerHealth;
    // 枪械是否被拾取
    public List<bool> isItemPick = new List<bool>();
    // 食物是否被食用
    public List<bool> isFoodEaten = new List<bool>();
    // 油桶是否被引爆
    public List<bool> isExplosiveBarrelsExplode = new List<bool>();
    // 气瓶是否被引爆
    public List<bool> isGasTanksHit = new List<bool>();
    // 武器数据列表
    public List<WeaponData> weaponsData = new List<WeaponData>();
    // 敌人数据列表
    public List<EnemyData> enemiesData = new List<EnemyData>();
}

[System.Serializable]
public class WeaponData
{
    // 是否被启用
    public bool isActive;
    // 当前弹夹剩余子弹数
    public int currentBullet;
    // 当前备弹数
    public int bulletLeft;
}

[System.Serializable]
public class EnemyData
{
    // 位置
    public Vector3 position;
    // 敌人的下一个巡逻点
    public int patrolIndex;
    // 敌人实际血量
    public float currentHealth;
    // 敌人是否死亡
    public bool isDead;
}

public static class SaveSystem
{
    // 保存数据
    public static void SaveData(PlayerData data)
    {
        // 将数据转化为JSON
        string json = JsonUtility.ToJson(data);
        // 将数据写入文件
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    // 读取数据
    public static PlayerData LoadData()
    {
        // 读取文件位置
        string path = Application.persistentDataPath + "/savefile.json";
        // 判断文件是否存在
        if (File.Exists(path))
        {
            // 读取文件
            string json = File.ReadAllText(path);
            // 将JSON转化为数据
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }

    // 清除数据
    public static void ClearData()
    {
        // 读取文件位置
        string path = Application.persistentDataPath + "/savefile.json";
        // 判断文件是否存在
        if (File.Exists(path))
        {
            // 删除文件
            File.Delete(path);
        }
    }
}