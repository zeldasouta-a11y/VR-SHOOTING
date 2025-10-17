using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class RandomTable
{
    private System.Random rng;
    private List<float> values;
    private int index;

    public RandomTable(int seed, int size = 1000)
    {
        rng = new System.Random(seed);
        values = new List<float>(size);
        for (int i = 0; i < size; i++)
            values.Add((float)rng.NextDouble()); // 0.0f ～ 1.0f
        index = 0;
    }

    // 0.0～1.0の乱数を取得
    public float NextFloat()
    {
        if (values.Count == 0) return 0f;
        float v = values[index];
        index = (index + 1) % values.Count; // 周回可能
        return v;
    }

    // 指定範囲の乱数を取得
    public float Range(float min, float max)
    {
        return min + (max - min) * NextFloat();
    }

    public int RangeInt(int min, int max)
    {
        return min + Mathf.FloorToInt(NextFloat() * (max - min));
    }

    // テーブルを再生成（シード変更時）
    public void Rebuild(int seed, int size = -1)
    {
        if (size < 0) size = values.Count;
        rng = new System.Random(seed);
        values.Clear();
        for (int i = 0; i < size; i++)
            values.Add((float)rng.NextDouble());
        index = 0;
    }
}
