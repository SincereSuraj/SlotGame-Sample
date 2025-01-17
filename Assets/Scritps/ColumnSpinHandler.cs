using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ColumnSpinHandler : MonoBehaviour
{
    [Header("layout Setup")]
    public RollItemImagesSO rollItemsSO;
    public int verticalItemCount = 5;
    public RollItem rollItemPrefabRef;
    public Transform poolParent;
    public Transform activeRollLayout;
    public int columnIndex;

    [Header("Spin Properties")]
    public float spinSpeed = 1000;

    Queue<RollItem> spinItemPool = new();
    List<string> allItemName = new();
    List<RollItem> activeSpinItems = new List<RollItem>();
    private VerticalLayoutGroup verticalLayout;
    void Start()
    {
        verticalLayout = activeRollLayout.GetComponent<VerticalLayoutGroup>();
        allItemName = rollItemsSO.GetALLItemsName();
        PopulateItemPool();
        InitialFill();
    }
    private void PopulateItemPool()
    {
        for (int i = 0; i < 10; i++)
        {
            RollItem newItem = Instantiate(rollItemPrefabRef, poolParent);
            newItem.ResetToDefault();
            spinItemPool.Enqueue(newItem);
        }
    }
    private void InitialFill()
    {
        for (int i = 0; i < verticalItemCount; i++)
        {
            GenerateNewItem();
        }
    }
    private RollItem GenerateNewItem()
    {
        return GenerateNewItem(allItemName[Random.Range(0, allItemName.Count)]);
    }
    private RollItem GenerateNewItem(string itemName)
    {
        RollItem newItem = spinItemPool.Dequeue();
        newItem.transform.SetParent(activeRollLayout);
        newItem.transform.SetAsFirstSibling();
        newItem.SetNewData(itemName);
        activeSpinItems.Add(newItem);
        return newItem;
    }
    private List<string> requiredResult;
    private bool spin = false;
    public float maxSizeY = 170f;
    public void StartSpin(List<string> res)
    {
        requiredResult = res;
        StartSpin();
    }
    [ContextMenu("Spin")]
    public void StartSpin()
    {
        spin = true;
        StartCoroutine(IESpinSeq());
    }
    [ContextMenu("Stop")]
    public void StopSpin()
    {
        spin = false;
    }
    private IEnumerator IESpinSeq()
    {
        verticalLayout.enabled = true;
        yield return null;
        RectTransform spinItem;
        //Loop random generation
        while (spin)
        {
            spinItem = GenerateNewItem().GetComponent<RectTransform>(); //Generate new item for spin
            yield return StartCoroutine(IEItemSlider(spinItem));

            EnqueueLastItem();  //Remove last item from active spin , enqueue them to pool
        }
        // Add few rextra random to make each column stop separately
        for (int i = 0; i < 3 * columnIndex * Random.Range(1, 4); i++)
        {
            spinItem = GenerateNewItem().GetComponent<RectTransform>();
            yield return StartCoroutine(IEItemSlider(spinItem));

            EnqueueLastItem();
        }
        // push Result into the roll
        for (int i = requiredResult.Count - 1; i > -1; i--)
        {
            spinItem = GenerateNewItem(requiredResult[i]).GetComponent<RectTransform>();
            yield return StartCoroutine(IEItemSlider(spinItem));

            EnqueueLastItem();
        }
        // Add extra 1 item to make all the result items selected
        {
            spinItem = GenerateNewItem().GetComponent<RectTransform>();
            yield return StartCoroutine(IEItemSlider(spinItem));

            EnqueueLastItem();
        }
        yield return null;
        SlotController.Instance.NotifySpinComplete();
        verticalLayout.enabled = false;
    }
    private IEnumerator IEItemSlider(RectTransform spinItem)
    {
        Vector2 size = spinItem.sizeDelta;
        size.y = 0;
        spinItem.sizeDelta = size;
        while (size.y < maxSizeY)
        {
            size.y += Time.deltaTime * spinSpeed;
            size.y = Mathf.Clamp(size.y, 0, maxSizeY);
            spinItem.sizeDelta = size;

            yield return null;
        }
    }
    public void AnimateSpinItem(int index)
    {
        //(Count - 1)-> index, (Count-2)-> index after skipping invisible element,
        //(count-2)-index -> inverts the index from combinationIndex to index in activeSpinItem list
        int highlightIndex = verticalItemCount - 2 - index; 
        activeSpinItems[highlightIndex].AnimateWin();
    }
    public void EnqueueLastItem()
    {
        RollItem lastItem = activeSpinItems[0];
        activeSpinItems.RemoveAt(0);
        lastItem.ResetToDefault();
        if (!spinItemPool.Contains(lastItem))
        {
            lastItem.transform.SetParent(poolParent);
            spinItemPool.Enqueue(lastItem);
        }
    }
}

public enum RollItemType
{
    Lime,
    Cherry,
    Berries,
    Bell,
    Star,
    Joker
}
