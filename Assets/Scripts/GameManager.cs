using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    [SerializeField] MapDataList mapData;
    [SerializeField] UnityEvent m_eventSuccess;
    [SerializeField] TMP_Text m_levelText;

    private int level;
    private GameObject map;

    void Start()
    {
        instance = this;
        Time.timeScale = 1f;
        level = PlayerPrefs.GetInt("level", 1);
        if (PlayerPrefs.HasKey("level"))
            PlayerPrefs.DeleteKey("level");
        if (level > mapData.mapDatas.Count)
            SceneManager.LoadScene(0);

        m_levelText.text = "LV." + level.ToString();
        map = mapData.GetMapData(level);
        Instantiate(map, new Vector3(0, 0, 0), Quaternion.identity);
    }


    public void GameSuccess()
    {
        if (PlayerPrefs.GetInt("levelActive") < level + 1)
            PlayerPrefs.SetInt("levelActive", level + 1);
        m_eventSuccess.Invoke();
    }

    public void GamePause()
    {
        Time.timeScale = 1 - Time.timeScale;
    }


    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene(1);
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("level", level + 1);
        SceneManager.LoadScene(1);
    }

    public void Music()
    {

    }

}
