using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameControl gameControlInstance;

    // for play/pause button (used to change the sprite currently show)
    public Button playPauseBtn;
    public Sprite[] spritePlayPause;
    private int spriteNum = 1;

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
    private int preTurn;
    private int preRedCoin, preBlueCoin;
    public Text redCoinChange, blueCoinChange;

    // initialize the player: PAUSE playing
    // change to PLAY playing: set IsPlaying = true; spriteNum in the declaration should be 0
    private void Start()
    {
        gameControlInstance.IsPlaying = false;
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
        gameControlInstance.PlaySpeed = value;
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
        Debug.Log("Turn Slide value changed to :" + (int)turn);
        gameControlInstance.ChangeTurn((int)turn);
    }
       

    private void Update()
    {
        speedValue.text = gameControlInstance.PlaySpeed.ToString();
        speedSlider.value = gameControlInstance.PlaySpeed;

        currentTurn = gameControlInstance.CurrentTurn;
        turnSlider.value = currentTurn;
        turnNumber.text = gameControlInstance.CurrentTurn.ToString();

        coinValue = gameControlInstance.GetMoneys();
        scoreValue = gameControlInstance.GetScores();

        coin[0].text = coinValue[0].ToString();
        coin[1].text = coinValue[1].ToString();
        score[0].text = scoreValue[0].ToString();
        score[1].text = scoreValue[1].ToString();

        if(preTurn != currentTurn)
        {
            redCoinChange.enabled = false;
            blueCoinChange.enabled = false;
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
        }
        preTurn = currentTurn;

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
