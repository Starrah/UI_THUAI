using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using StartScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneControl : MonoBehaviour
{

    private int _myAi;
    public GameObject RedFlagObj;
    public GameObject BlueFlagObj;
    public GameObject Settings;
    public GameObject Loadings;
    public Text LoadingText;
    public GameObject LoadingSureButton;
    
    private GameDataSource _gameDataSource;
    private bool _flagLoadingDataProcessing = false;

    private bool FlagLoadingDataProcessing
    {
        get => _flagLoadingDataProcessing;
        set
        {
            _flagLoadingDataProcessing = value;
            LoadingSureButton.SetActive(!value);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        OnClickRed();
    }

    // Update is called once per frame
    void Update()
    {
        if (FlagLoadingDataProcessing && _gameDataSource != null)
            LoadingText.text = "读取游戏数据中......" + Math.Round(_gameDataSource.ProcessedRate * 100) + "%";
    }

    public void OnClickExceptionReaded()
    {
        Settings.SetActive(true);
        Loadings.SetActive(false);
    }
    
    public void OnClickRed()
    {
        Debug.Log("RedClicked");
        _myAi = 0;
        RedFlagObj.transform.localScale = new Vector3(1.5f, 1.5f,  1);
        BlueFlagObj.transform.localScale = new Vector3(1, 1, 1);
        RedFlagObj.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        BlueFlagObj.GetComponent<Image>().color = new Color(255, 255, 255, 100);
    }

    public void OnClickBlue()
    {
        Debug.Log("BlueClicked");
        _myAi = 1;
        RedFlagObj.transform.localScale = new Vector3(1, 1,  1);
        BlueFlagObj.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        RedFlagObj.GetComponent<Image>().color = new Color(255, 255, 255, 100);
        BlueFlagObj.GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }

    public void OnClickGameStart()
    {
        StartCoroutine(OnClickGameStartAsync());
    }
    
    public IEnumerator OnClickGameStartAsync()
    {
        Debug.Log("clk");
        Settings.SetActive(false);
        Loadings.SetActive(true);
        LoadingText.text = "读取游戏数据中......0%";
        OpenFileName fileInfo = null;

        try
        {
            fileInfo = FileManager.OpenFile("打开文件", "游戏数据文件(*.json)\0*.json");
            if (fileInfo == null || string.IsNullOrEmpty(fileInfo.file)) throw new Exception("打开文件失败，请检查文件是否存在！");
            _gameDataSource = new GameDataSource();
            FlagLoadingDataProcessing = true;
        }
        catch (Exception e)
        {
            FlagLoadingDataProcessing = false;
            LoadingText.text = "错误：" + e.Message;
            yield break;
        }
        
        IEnumerator enumerator = null;
        try
        {
            enumerator = _gameDataSource.ReadFileAsync(fileInfo.file);
        }
        catch (Exception e)
        {
            FlagLoadingDataProcessing = false;
            LoadingText.text = "错误：" + e.Message;
            yield break;
        }
        yield return null;
        
        bool res = true;
        while (res)
        {
            try
            {
                res = enumerator.MoveNext();
            }
            catch (Exception e)
            {
                FlagLoadingDataProcessing = false;
                LoadingText.text = "错误：" + e.Message;
                yield break;
            }
            yield return null;
        }

        try
        {
            GameControl.DataPassedFromStartScene = (_gameDataSource, _myAi);
            SceneManager.LoadScene("Main");
        }catch (Exception e)
        {
            FlagLoadingDataProcessing = false;
            LoadingText.text = "错误：" + e.Message;
            yield break;
        }
    }
}
