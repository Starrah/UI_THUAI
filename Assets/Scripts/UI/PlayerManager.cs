using System;
using System.Collections.Generic;
using GameData.GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameControl gameControlInstance;

    // for play/pause button (used to change the sprite currently show)
    public Button playPauseBtn;
    public Sprite[] spritePlayPause;
    private int spriteNum = 0;

    // for speed slider and speed value (used to synchronize the number shown and the handle place)
    public Slider speedSlider;
    public Text speedValue;

    // for turn slider and turn number (used to synchronize the number shown and the handle place)
    public Slider turnSlider;
    public Text turnNumber, totalTurnNumber;
    private int currentTurn;

    // for coins and point
    public Text[] coin, score;
    private int[] coinValue, scoreValue;

    // for enable identifier (flag) according to "MyAi"
    public Image[] flag;

    // for pop-up coin change message
    private int preTurn = -1;
    private int preRedCoin, preBlueCoin;
    public Text redCoinChange, blueCoinChange;

    // for message panel
    public int maxMessage = 80;
    public GameObject messagePanel, textObject;
    public Color[] messageColor;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    // initialize the player: playing
    private void Start()
    {
        gameControlInstance.IsPlaying = true;
        playPauseBtn.image.sprite = spritePlayPause[spriteNum];

        flag[0].enabled = false;
        flag[1].enabled = false;
        flag[gameControlInstance.MyAi].enabled = true;

        redCoinChange.enabled = false;
        blueCoinChange.enabled = false;

        preRedCoin = 0;
        preBlueCoin = 0;

        int totalTurn = gameControlInstance.DataSource.GetStartData().ActualRoundNum;
        totalTurnNumber.text = "/" + totalTurn.ToString();
        turnSlider.maxValue = totalTurn;
    }

    // change playing state when the play/pause button "OnClick"
    // spriteNum: 1:play 0:pause
    public void changePlaying()
    {
        Debug.Log("Change Playing");
        gameControlInstance.IsPlaying = !gameControlInstance.IsPlaying;
        spriteNum = (spriteNum == 1) ? 0 : 1;
        playPauseBtn.image.sprite = spritePlayPause[spriteNum];
    }

    // change playing speed when the faster/slower button "OnClick"
    public void multiplySpeed(float value)
    {
        Debug.Log("Faster/Slower clicked");
        gameControlInstance.PlaySpeed *= value;
    }

    // change the playing speed when the speedSlider "OnValueChanged"
    public void slideSpeed(float value)
    {
        Debug.Log("Speed Slide value changed");
        //速度与滑块之间的对应改为分段线性插值算法
        gameControlInstance.PlaySpeed = value >= 0 ? Mathf.Lerp(1, 5, value) : Mathf.Lerp(1, 0.2f, -value);
    }

    // change current turn when next(number = 5)/previous(number = -5) button "OnClick"
    public void changeTurn(int number)
    {
        Debug.Log("Next/Previous Unit");
        gameControlInstance.ChangeTurn(gameControlInstance.CurrentTurn + number);
    }

    // change current turn when the speedSlider "OnValueChanged"
    public void slideTurn(float turn)
    {
        gameControlInstance.ChangeTurn((int)turn);
    }

    // generate event message and then send to updateMessage
    private void generateMessage()
    {
        // get the total number of the events happening in the current turn
        List<GameEventBase> curTurnEvent = gameControlInstance.DataSource.GetTurnData(currentTurn).Events;
        int turnEventNumber = curTurnEvent.Count;
        int AiNumber = gameControlInstance.DataSource.GetTurnData(currentTurn).Ai;
        string text = "";

        updateMessage("\n------ 回合" + (currentTurn + 1).ToString() + " ------", 2);

        if (turnEventNumber > 0)
        {        
            text = "\n 玩家" + AiNumber.ToString() + ":";
            updateMessage(text, AiNumber);
        }

        // add all event messages to the message list and message panel
        for (int i = 0; i < turnEventNumber; ++i)
        {
            // get event[i] and generate Chinese message according to the event type and event result
            if (curTurnEvent[i] is BidResultEvent bre)
            {
                if (bre.Success)
                {
                    text = " 地皮拍卖成功！";
                }
                else
                {
                    text = " 发起的地皮拍卖因竞拍方余额不足而流拍。";
                }
            }

            if (curTurnEvent[i] is NewBidEvent nbe)
            {
                text = " 发起金额 " + nbe.Bid.money.ToString() + " 的拍卖。";
            }

            if (curTurnEvent[i] is PutDetectorEvent pde)
            {
                text = " 设置检测设备，共找到 " + pde.Result.Count + " 个疫区。";
            }

            if (curTurnEvent[i] is PutProcessorEvent ppe)
            {
                text = " 设置治理设备，共治理 " + ppe.Result.Count + " 个疫区。";
            }

            if (curTurnEvent[i] is TipsterEvent te)
            {
                if (te.Success)
                {
                    text = " 超级侦察机在( " + te.Result.x + ", " + te.Result.y + ")找到未被检测的疫区";
                }
                else
                {
                    text = " 侦察机没有找到未被发现的疫区。所有疫区已被发现。";
                }
            }
            updateMessage(text, AiNumber); 
        }
    }

    // update turn event to message panel
    // colorType: 0: player 0, 1: player 1, 2: system
    private void updateMessage(string text, int colorType)
    {
        if (text.Length > 0)
        {
            if (messageList.Count > maxMessage)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }
            
            Message newMessage = new Message();
            newMessage.text = text;
            GameObject newText = Instantiate(textObject, messagePanel.transform);
            newMessage.textObject = newText.GetComponent<Text>();
            newMessage.textObject.text = newMessage.text;
            if(colorType == 2)
            {
                newMessage.textObject.alignment = TextAnchor.MiddleCenter;
            }
            newMessage.textObject.color = messageColor[colorType];
            messageList.Add(newMessage);
        }   
        
    }

    private void Update()
    {
        speedValue.text = gameControlInstance.PlaySpeed.ToString("0.00");

        currentTurn = gameControlInstance.CurrentTurn;
        turnSlider.value = currentTurn;
        turnNumber.text = (gameControlInstance.CurrentTurn+1).ToString();//用户习惯回合从1开始

        coinValue = gameControlInstance.GetMoneys();
        scoreValue = gameControlInstance.GetScores();

        coin[0].text = coinValue[0].ToString();
        coin[1].text = coinValue[1].ToString();
        score[0].text = scoreValue[0].ToString();
        score[1].text = scoreValue[1].ToString();

        if (preTurn != currentTurn && preTurn < GameControl.Instance.StartData.ActualRoundNum)
        {
            redCoinChange.enabled = false;
            blueCoinChange.enabled = false;
            generateMessage();

            if (coinValue[0] != preRedCoin)
            {
                int sub = coinValue[0] - preRedCoin;
                redCoinChange.text = (sub > 0) ? "+" + sub.ToString() : sub.ToString();
                redCoinChange.enabled = true;
                preRedCoin = coinValue[0];
            }
            if (coinValue[1] != preBlueCoin)
            {
                int sub = coinValue[1] - preBlueCoin;
                blueCoinChange.text = (sub > 0) ? "+" + sub.ToString() : sub.ToString();
                blueCoinChange.enabled = true;
                preBlueCoin = coinValue[1];
            }

            preTurn = currentTurn;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            changePlaying();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(KeyCode.L);
            gameControlInstance.ChangeTurn(gameControlInstance.CurrentTurn - 5);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(KeyCode.O);
            multiplySpeed((float)0.8);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(KeyCode.P);
            multiplySpeed((float)1.25);
        }
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}