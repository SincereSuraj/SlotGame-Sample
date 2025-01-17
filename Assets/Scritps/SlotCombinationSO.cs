using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotCombinationSO", menuName = "SO/Slot Combination")]
public class SlotCombinationSO : ScriptableObject
{
    public int columnCount = 3;
    public CombinationEntry[] combinations;

    public int[] GetCombinationInfo(int code)
    {
        return combinations.Where(combination => combination.combinationCode == code).Select(combination => combination.rowIndex).FirstOrDefault();
    }
}

[System.Serializable]
public class CombinationEntry
{
    public int combinationCode;
    public int[] rowIndex;
}
