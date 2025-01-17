using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotController : MonoBehaviour
{
    public static SlotController Instance;

    [SerializeField] private MenuBar menuBar;
    [SerializeField] private List<ColumnSpinHandler> spinColumns;
    public RollItemImagesSO rollItemsSO;
    public SlotCombinationSO combinationSO;

    public TextMeshProUGUI currentStateMessageText;
    public float spinDuration = 3f;
    private Coroutine spinCoroutine;
    List<string> allItemName = new();
    private double winAmount = 0;
    private void Awake()
    {
        Instance = this;
    }
    Coroutine messageHighlight;
    public void SetMessage(string message)
    {
        currentStateMessageText.text = message;
        if (messageHighlight != null)
            StopCoroutine(messageHighlight);

        messageHighlight = StartCoroutine(IEMessageHighlight());
    }
    private IEnumerator IEMessageHighlight()
    {
        Color messageTextColor = currentStateMessageText.color;
        for (int i = 0; i < 3; i++)
        {
            messageTextColor.a = 0;
            currentStateMessageText.color = messageTextColor;
            yield return new WaitForSeconds(0.3f);
            messageTextColor.a = 1;
            currentStateMessageText.color = messageTextColor;
            yield return new WaitForSeconds(2f);
        }
    }
    void Start()
    {
        SetMessage("Ready, set, Spin");
        allItemName = rollItemsSO.GetALLItemsName();
    }

    public void StartSpin(double bet)
    {
        if (spinCoroutine != null)      //Handle Spam Spin request
            return;

        SetMessage("Best of Luck");
        int[,] resultMatrix = new int[3, 3];
        int colCount = 0;
        foreach (var item in spinColumns)
        {
            List<string> resItems = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                int generateItemIndex = Random.Range(0, allItemName.Count);
                resItems.Add(allItemName[generateItemIndex]);
                resultMatrix[colCount, i] = generateItemIndex;
            }
            item.StartSpin(resItems);
            colCount++;
        }
        spinningColumnCount = spinColumns.Count;
        spinCoroutine = StartCoroutine(IESpinTimer());
        CalculateCombination(bet / combinationSO.combinations.Length, resultMatrix);
    }
    private int winMultiplier = 0, combinationCode = -1;
    private async void CalculateCombination(double bet, int[,] combination)
    {
        //Calculate win by analyzing combination
        await Task.Run(() => GetHighestCombinationReward(combination));
        winAmount = bet * winMultiplier;
    }
    private IEnumerator IESpinTimer()
    {
        yield return new WaitForSeconds(spinDuration);
        StopSpin();
        spinCoroutine = null;
    }
    public void StopSpin()
    {
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
        }
        foreach (var item in spinColumns)
        {
            item.StopSpin();
        }
        StartCoroutine(IEAfterSpinSeq());
    }
    private int spinningColumnCount = 0;
    public void NotifySpinComplete()
    {
        spinningColumnCount--;
    }
    private IEnumerator IEAfterSpinSeq()
    {
        yield return null;
        yield return null;
        yield return new WaitUntil(() => spinningColumnCount <= 0); //wait until all column come to stop
        yield return new WaitForSeconds(0.5f);
        //Give Reward if any
        if (winAmount > 0)
        {
            //Show special effects using coroutines
            int[] winCombination = combinationSO.GetCombinationInfo(combinationCode);
            for (int i = 0; i < winCombination.Length; i++)
            {
                spinColumns[i].AnimateSpinItem(winCombination[i]);
            }
            yield return new WaitForSeconds(0.8f);  //Wait until Animation
            //give reward
            menuBar.AddWinReward(winAmount);
            yield return new WaitForSeconds(1f);
        }
        else
        {
            //Lose case
            SetMessage("Lost, Try Again");
        }
        menuBar.SpinStopUIUpdate();
    }

    #region Checking Combination
    private void GetHighestCombinationReward(int[,] combination)
    {
        //Set Default values
        int rewardingItemIndex = -1;
        combinationCode = -1;   
        winMultiplier = 0;

        if (combinationSO.combinations[0].rowIndex.Length != combination.GetLength(1))
        {
            Debug.Log("Given Combination data doesnot meet required Combination column count");
            winMultiplier = 0;
        }

        foreach (var item in combinationSO.combinations)
        {
            bool isMatch = true;
            int currentItemIndex = combination[0, item.rowIndex[0]];
            for (int i = 0; i < item.rowIndex.Length; i++)
            {
                isMatch &= currentItemIndex == combination[i, item.rowIndex[i]];
            }
            if (isMatch && currentItemIndex > rewardingItemIndex)
            {
                rewardingItemIndex = currentItemIndex;
                combinationCode = item.combinationCode;
            }
        }

        Debug.Log("rewarding item : " + rewardingItemIndex + " \n rewardCode : " + combinationCode);
        if (rewardingItemIndex > -1)    //Has Reward
        {
            winMultiplier = rollItemsSO.GetRewardMultiplier(allItemName[rewardingItemIndex]);
        }
    }
    #endregion
}
