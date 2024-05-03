using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工具类
/// </summary>
public static class ToolsE
{
    /// <summary>
    /// 文件重命名
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="source">原名</param>
    /// <param name="hundred">百位</param>
    //[UnityEditor.MenuItem("Tools/FileRename")]
    /*public static void FileRename(string path, string source, string extension = ".bmp", bool hundred = true)
    {
        //ToolsE.FileRename("/Resources/Item/", "1");

        string oldName, newName;

        for (int i = 0; i != 666; i++)
        {
            oldName = Application.dataPath + path + (i + 1) + "-" + source + extension;

            LogWarning(File.Exists(oldName) + " | " + oldName);

            if (File.Exists(oldName))
            {
                newName = Application.dataPath + path;
                if (i < 10) newName += "0";
                if (hundred && i < 100) newName += "0";
                newName += i + extension;

                File.Move(oldName, newName);
            }
            else return;
        }
    }*/

    /*private ref int SSS { get { return ref _sss; } }

    private int _sss;

    private void III(ref int aa)
    {
        aa = 0;

        III(ref SSS);
    }*/

    //Debug
    #region
    public static void Log(object log)
    {
#if UNITY_EDITOR
        Debug.Log(log);
#endif
    }

    public static void Log(object log, UnityEngine.Object gameObject)
    {
#if UNITY_EDITOR
        Debug.Log(log, gameObject);
#endif
    }

    public static void LogWarning(object log)
    {
#if UNITY_EDITOR
        Debug.LogWarning(log);
#endif
    }

    public static void LogWarning(string[] logArray)
    {
#if UNITY_EDITOR
        LogWarning(SA2S(logArray));
#endif
    }

    public static void LogWarning(int[] logArray)
    {
#if UNITY_EDITOR
        LogWarning(IA2S(logArray));
#endif
    }

    public static void LogWarning(object log, UnityEngine.Object gameObject)
    {
#if UNITY_EDITOR
        Debug.LogWarning(log, gameObject);
#endif
    }

    public static void LogError(object log)
    {
#if UNITY_EDITOR
        Debug.LogError(log);
#endif
    }

    public static void LogError(object log, UnityEngine.Object gameObject)
    {
#if UNITY_EDITOR
        Debug.LogError(log, gameObject);
#endif
    }

    public static void Assert(bool condition, string log)
    {
#if UNITY_EDITOR
        Debug.Assert(condition, log);
#endif
    }

    private static int _bugCount;

    /// <summary>
    /// 实机Debug
    /// </summary>
    /// <param name="text">Text</param>
    public static void RuntimeDebug(string text)
    {
        Root_.ScreenLog(text);

        File.WriteAllText(Application.persistentDataPath + "/Bug_" + _bugCount++ + ".txt", text);
    }

    /// <summary>
    /// 实机Debug
    /// </summary>
    /// <param name="e">Exception</param>
    public static void RuntimeDebug(Exception e, string text = null)
    {
        RuntimeDebug(e.GetType() + Const.SPLIT_RN + Const.SPLIT_RN +
                e.InnerException + Const.SPLIT_RN + Const.SPLIT_RN +
                e.Message + Const.SPLIT_RN + Const.SPLIT_RN +
                e.StackTrace + Const.SPLIT_RN + Const.SPLIT_RN +
                e.TargetSite + Const.SPLIT_RN + Const.SPLIT_RN +
                e.ToString() + Const.SPLIT_RN + Const.SPLIT_RN +
                text);

        File.WriteAllText(Application.persistentDataPath + "/Bug_" + _bugCount++ + ".txt", text);
    }
    #endregion

    /*/// <summary>
    /// 字符转向量
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="splitC">切割符</param>
    /// <returns>向量</returns>
    public static Vector3 S2V(this string data, char splitC = Const.SPLIT_2)
    {
        float[] tempSF = S2F(data.Split(splitC));

        return new(tempSF[0], tempSF[1], tempSF[2]);
    }*/

    /// <summary>
    /// 字符转整形
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>整形</returns>
    public static int[] S2IA(this string data, char split = Const.SPLIT_1) => SA2IA(data.Split(split));

    /// <summary>
    /// 字符转整形
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>整形</returns>
    public static int[] SA2IA(this string[] data)
    {
        int[] tempIA = new int[data.Length];

        for (int i = 0; i != tempIA.Length; i++)
            tempIA[i] = int.Parse(data[i]);

        return tempIA;
    }

    /// <summary>
    /// 字符转列表
    /// </summary>
    /// <param name="list">列表</param>
    /// <param name="data">字符</param>
    public static void SA2LI(this List<int> list, string[] data)
    {
        list.Clear();

        for (int i = 0; i != data.Length; i++)
        {
            list.Add(int.Parse(data[i]));
        }
    }

    /// <summary>
    /// 字符转字典
    /// </summary>
    /// <param name="dic">字典</param>
    /// <param name="data">数据</param>
    /// <param name="split_0">切割符0</param>
    /// <param name="split_1">切割符1</param>
    public static void S2DI(this Dictionary<int, int> dic, string data, char split_0 = Const.SPLIT_1, char split_1 = Const.SPLIT_2)
    {
        dic.Clear();

        string[] tempSA = data.Split(split_0);
        int[] tempIA;

        for (int i = 0; i != tempSA.Length; i++)
        {
            tempIA = SA2IA(tempSA[i].Split(split_1));

            dic.Add(tempIA[0], tempIA[1]);
        }
    }

    /// <summary>
    /// 字符转浮点
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>浮点</returns>
    public static float[] S2F(this string[] data)
    {
        float[] tempIA = new float[data.Length];

        for (int i = 0; i != tempIA.Length; i++)
            tempIA[i] = float.Parse(data[i]);

        return tempIA;
    }

    /// <summary>
    /// 字符转游戏事件
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="split_0">切割0</param>
    /// <param name="split_1">切割1</param>
    /// <returns>游戏事件</returns>
    public static GameEventData S2GE(this string data, char split_0 = Const.SPLIT_1, char split_1 = Const.SPLIT_2)
    {
        string[] tempSA = data.Split(split_0);

        return new GameEventData(tempSA[0].S2E<GameEventType>(), tempSA[1].Split(split_1));
    }

    /// <summary>
    /// 字符转游戏事件集合
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="split_0">切割0</param>
    /// <param name="split_1">切割1</param>
    /// <returns>游戏事件集合</returns>
    public static GameEventData[] S2GA(this string data, char split_0 = Const.SPLIT_2, char split_1 = Const.SPLIT_3)
    {
        string[] tempSA = data.Split(Const.SPLIT_0);
        GameEventData[] eventArray = new GameEventData[tempSA.Length];

        for (int i = 0; i != eventArray.Length; i++)
            eventArray[i] = tempSA[i].S2GE(split_0, split_1);

        return eventArray;
    }

    /// <summary>
    /// 字符转枚举
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="origin">源</param>
    /// <returns>枚举</returns>
    public static T S2E<T>(this string origin) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), origin);
    }

    /// <summary>
    /// 字符数组转向量
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>向量</returns>
    public static Vector2 SA2V2(this string[] data) => new(float.Parse(data[1]), float.Parse(data[2]));

    /// <summary>
    /// 字符数组转向量
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>向量</returns>
    public static Vector3 SA2V3(this string[] data) => new(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));

    /// <summary>
    /// 字符数组整合
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>字符</returns>
    public static string SA2S(this string[] data, char split = Const.SPLIT_3)
    {
        string tempS = string.Empty;

        for (int i = 0; i != data.Length; i++) tempS += data[i] + split;

        return tempS.Remove(tempS.Length - 1, 1);
    }

    /// <summary>
    /// 字符数组剪切
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="length">长度</param>
    /// <returns>数据</returns>
    public static string[] SACut(this string[] data, int length)
    {
        string[] tempSA = new string[length];

        for (int i = 0; i != length; i++)
        {
            tempSA[i] = data[i];
        }

        return tempSA;
    }

    /// <summary>
    /// 字符数组剪切
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="length">长度</param>
    /// <returns>数据</returns>
    public static string SACut2S(this string[] data, int length) => SA2S(SACut(data, length));

    /// <summary>
    /// 整形数组整合
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>字符集合</returns>
    public static string[] IA2SA(this int[] data)
    {
        string[] tempSA = new string[data.Length];

        for (int i = 0; i != tempSA.Length; i++)
        {
            tempSA[i] = data[i].ToString();
        }

        return tempSA;
    }

    /// <summary>
    /// 整形数组整合
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="split">拼接符</param>
    /// <returns>字符集合</returns>
    public static string IA2S(this int[] data, char split = Const.SPLIT_1) => SA2S(IA2SA(data), split);

    /// <summary>
    /// 数字转枚举
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="origin">源</param>
    /// <returns>枚举</returns>
    public static T I2E<T>(this int origin) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), origin);
    }

    /// <summary>
    /// 向量转字符
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="split">切割符</param>
    /// <returns>字符</returns>
    public static string V2S(this Vector3 origin, char split = Const.SPLIT_2) => origin.x.ToString() + split + origin.y + split + origin.z;

    /// <summary>
    /// 向量转字符
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="split">切割符</param>
    /// <returns>字符</returns>
    public static string V2S_Y(this Vector3 origin, char split = Const.SPLIT_2) => origin.x.ToString() + split + 0 + split + origin.z;

    /// <summary>
    /// X轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="x">X</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyX(this Vector3 origin, float x)
    {
        origin.x = x;

        return origin;
    }

    /// <summary>
    /// X轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="x">X</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyXAdd(this Vector3 origin, float x)
    {
        origin.x += x;

        return origin;
    }

    /// <summary>
    /// Y轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="y">Y</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyY(this Vector3 origin, float y)
    {
        origin.y = y;

        return origin;
    }

    /// <summary>
    /// Y轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="y">Y</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyYAdd(this Vector3 origin, float y)
    {
        origin.y += y;

        return origin;
    }

    /// <summary>
    /// Z轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="z">Z</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyZ(this Vector3 origin, float z)
    {
        origin.z = z;

        return origin;
    }

    /// <summary>
    /// Z轴修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="z">Z</param>
    /// <returns>结果</returns>
    public static Vector3 V3ModifyZAdd(this Vector3 origin, float z)
    {
        origin.z += z;

        return origin;
    }

    /// <summary>
    /// 向量修改，每帧调用的自行缓存Vector
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    /// <param name="z">z</param>
    /// <returns>结果</returns>
    public static Vector3 V3Modify(this Vector3 origin, float x, float y, float z)
    {
        origin.x = x;
        origin.y = y;
        origin.z = z;

        return origin;
    }

    /// <summary>
    /// 交换
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="t0">左值</param>
    /// <param name="t1">右值</param>
    public static void Swap<T>(ref T t0, ref T t1) => (t1, t0) = (t0, t1);

    /// <summary>
    /// 末尾
    /// </summary>
    public static int Last(this Array array) => array.Length - 1;

    /// <summary>
    /// 末尾
    /// </summary>
    public static int Last<T>(this List<T> list) => list.Count - 1;

    /// <summary>
    /// 末尾
    /// </summary>
    public static int Last<T0, T1>(this Dictionary<T0, T1> dic) => dic.Count - 1;

    /// <summary>
    /// 有效验证
    /// </summary>
    public static bool Valid(this Array array, int index) => 0 <= index && index < array.Length;

    /// <summary>
    /// 有效验证
    /// </summary>
    public static bool Valid<T>(this List<T> list, int index) => 0 <= index && index < list.Count;

    /// <summary>
    /// Z轴修正，不明原因的所有素材需要垂直拉伸1.2倍，猜测发售前临时修改
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static Vector3 ZFixed(this Vector3 origin)
    {
        origin.z *= 1.2f;

        return origin;
    }

    /// <summary>
    /// 平面化
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>向量</returns>
    public static Vector3 Planarization(this Vector3 origin)
    {
        origin.y = 0;

        return origin;
    }

    /// <summary>
    /// 透明修改
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="alpha">透明度</param>
    /// <returns>颜色</returns>
    public static Color ColorModifyA(this Color origin, float alpha)
    {
        origin.a = alpha;

        return origin;
    }

    /// <summary>
    /// 绝对值
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>结果</returns>
    public static int ABS(this int origin) => (origin ^ (origin >> 31)) - (origin >> 31);

    /// <summary>
    /// 绝对值
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>结果</returns>
    public static float ABS(this float origin) => 0 < origin ? origin : -origin;

    /// <summary>
    /// 单数判断
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>单/双</returns>
    public static bool IsOddNumber(this int origin) => 1 == (origin & 1);

    /// <summary>
    /// 保留小数
    /// </summary>
    /// <param name="origin">源</param>
    /// <param name="roundCount">位数</param>
    /// <returns>结果</returns>
    public static float Round(this float origin, int roundCount = 0)
    {
        float tempF0 = 1, tempF1 = origin.ABS();

        for (int i = 0; i != roundCount; i++)
            tempF0 *= 10;

        tempF1 = ((int)(tempF1 * tempF0 + 0.5f)) / tempF0;

        return 0 < origin ? tempF1 : -tempF1;
    }

    /// <summary>
    /// 是否整数
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>是否</returns>
    public static bool IsInt(this float origin) => 0 == origin - (int)origin;

    /// <summary>
    /// 向上取整
    /// </summary>
    /// <param name="origin">源</param>
    /// <returns>整数</returns>
    public static int Ceil(this float origin) => (int)(origin + 0.9999999f);

    /// <summary>
    /// 物体路径获取
    /// </summary>
    /// <param name="origin"></param>
    /// <returns>路径</returns>
    public static string GOPathGet(this Transform origin)
    {
        string path = origin.name;

        Transform tempT = origin;

        while (GameManager_.MAP_ROOT != tempT.parent.name)
            path = (tempT = tempT.parent).name + Const.SPLIT_3 + path;

        return path.TrimEnd(Const.SPLIT_3);
    }
}