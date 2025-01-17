using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour
{
    [Header("Bet Section")]
    [SerializeField]
    private List<BetButtonInfo> allBets;
    public Button betMinus, betPlus;
    public TextMeshProUGUI balanceText, betText, totalWinText;

    [Header("Spin Section")]
    public Button autoSpinButton;
    public Button spinButton;
    public Button stopButton;

    [Header("Starting Values")]
    [SerializeField]
    private double balance = 10000d;

    private double activeBet = 2;
    private double totalWin = 0;
    private List<double> betList;
    private bool lockBet = false;
    private bool autoSpin = false;
    private void Start()
    {
        betList = new List<double>();
        betList = allBets.Select(betInfo =>
        {
            betInfo.SelectorBtn.onClick.AddListener(() => BetJumpSelect(betInfo.betAmount));
            return betInfo.betAmount;
        }).ToList();
        //Add buttons OnClick listners
        betMinus.onClick.AddListener(BetMinus);
        betPlus.onClick.AddListener(BetPlus);
        autoSpinButton.onClick.AddListener(AutoSpinEnable);
        spinButton.onClick.AddListener(Spin);
        stopButton.onClick.AddListener(Stop);

        //Setting up default state
        BetJumpSelect(activeBet);
        balanceText.text = balance.ToString("0.00");
        totalWinText.text = totalWin.ToString("0.00");

    }

    private void BetMinus()
    {
        if (lockBet)
            return;

        int betIndex = betList.IndexOf(activeBet);
        if (betIndex > 0)
            betIndex--;
        else
            return;

        activeBet = betList[betIndex];

        UpdateMinusPlusButtonInteractable(betIndex);
    }
    private void BetPlus()
    {
        if (lockBet)
            return;

        int betIndex = betList.IndexOf(activeBet);
        if (betIndex >= betList.Count - 1)
            return;
        else
            betIndex++;

        activeBet = betList[betIndex];

        UpdateMinusPlusButtonInteractable(betIndex);
    }
    private void BetJumpSelect(double newBet)
    {
        if (lockBet)
            return;

        activeBet = newBet;
        int betIndex = betList.IndexOf(activeBet);

        UpdateMinusPlusButtonInteractable(betIndex);
    }
    private void UpdateMinusPlusButtonInteractable(int betIndex)
    {
        foreach (var item in allBets)
        {
            item.UpdateButtonInteractable(activeBet);
        }
        betText.text = activeBet.ToString("0.00");
        betMinus.interactable = betIndex > 0;
        betPlus.interactable = betIndex < betList.Count - 1;
    }
    private void AutoSpinEnable()
    {
        autoSpin = true;
        autoSpinButton.interactable = false;
        Spin();
    }
    private void Spin()
    {
        if (balance < activeBet)
        {
            SlotController.Instance.SetMessage("Insufficient Balance");
            spinButton.interactable = false;
            Invoke(nameof(MakeSpinButtonInteractable), 5f);
        }
        else
        {
            lockBet = true;
            balance -= activeBet;
            balanceText.text = balance.ToString("0.00");

            SlotController.Instance.StartSpin(activeBet);
            spinButton.gameObject.SetActive(false);
            stopButton.interactable = false;
            stopButton.gameObject.SetActive(true);
            Invoke(nameof(MakeStopButtonInteractable), 1f);
        }
    }
    private void Stop()
    {
        SlotController.Instance.StopSpin();
        stopButton.interactable = false;

        if (autoSpin)
        {
            autoSpin = false;
            Invoke(nameof(MakeAutoSpinButtonInteractable), 1f);
        }
    }
    public void SpinStopUIUpdate()
    {
        lockBet = false;
        spinButton.interactable = false;
        spinButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        if (autoSpin)
            Invoke(nameof(Spin), 0.8f);   //Respin
        else
            Invoke(nameof(MakeSpinButtonInteractable), 1f);
    }
    private void MakeSpinButtonInteractable() => spinButton.interactable = true;
    private void MakeStopButtonInteractable() => stopButton.interactable = true;
    private void MakeAutoSpinButtonInteractable() => autoSpinButton.interactable = true;

    public void AddWinReward(double winAmount)
    {
        //Validating winAmount
        if ((winAmount * 5) % activeBet != 0)
        {
            Debug.Log("Invalid winAmount * 5 : " + (winAmount * 5) + "\n betPerLine : " + activeBet);
            return;
        }
        SlotController.Instance.SetMessage($"You Win {winAmount:0.00}");
        balance += winAmount;
        totalWin += winAmount;
        balanceText.text = balance.ToString("0.00");
        totalWinText.text = totalWin.ToString("0.00");
    }
}

[Serializable]
public class BetButtonInfo
{
    public double betAmount;
    public Button SelectorBtn;

    public void UpdateButtonInteractable(double betAmount)
    {
        SelectorBtn.interactable = this.betAmount != betAmount;
    }
}