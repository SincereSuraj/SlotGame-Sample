using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SpinItemImagesSO", menuName = "SO/spinItemSO")]
public class RollItemImagesSO : ScriptableObject
{
    [SerializeField]
    List<RollItemImageInfo> allSprites;
    public Sprite GetSprite(string type)
    {
        return allSprites.Where(res => res.type == type).Select(res => res.image).FirstOrDefault();
    }
    public List<string> GetALLItemsName()
    {
        return allSprites.Select(res=> res.type).ToList();
    }
    public int GetRewardMultiplier(string type)
    {
        return allSprites.Where(res => res.type == type).Select(res => res.rewardMultiplier).FirstOrDefault();
    }
}
[Serializable]
public class RollItemImageInfo
{
    public string type;
    public Sprite image;
    public int rewardMultiplier;
}
