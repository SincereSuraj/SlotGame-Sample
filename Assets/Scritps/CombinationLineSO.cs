using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "CombinationLineSO", menuName = "SO/combinationLineSO")]
public class CombinationLineSO : ScriptableObject
{
    public List<combination> combinationsList;

    public int[] GetCombinationInfo(int code)
    {
        return combinationsList.Where(combination=> combination.combinationCode== code).Select(combination=> combination.itemIndex.line).FirstOrDefault();
    }
}
[Serializable]
public class combination
{
    public int combinationCode;
    public HorizontalArray itemIndex;
}
[Serializable]
public class HorizontalArray
{
    public int[] line;
    public int Count = 3;
}