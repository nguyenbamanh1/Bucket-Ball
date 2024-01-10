using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
public class SliceChooseView : MonoBehaviour
{
    [SerializeField] int numLevel1View;
    [SerializeField] Vector2 sizeView;
    [SerializeField] private float transform_X;
    [SerializeField] private bool isScroll;
    [SerializeField] private float speedSlice = 200f;
    [SerializeField] private Button btnLeft;
    [SerializeField] private Button btnRight;

    [SerializeField] Transform groupView;
    [SerializeField] GameObject groupPrefabs;
    [SerializeField] GameObject levelPrefabs;
    [SerializeField] MapDataList mapDataList;

    [Header("Sprite")]
    [SerializeField] Sprite LevelActive;
    [SerializeField] Sprite LevelLock;

    private RectTransform rectTransform;
    public static GameObject[] groupLevels;
    private Vector2 vecTarget;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        int levelActive = PlayerPrefs.GetInt("levelActive", 1);
        if (groupLevels == null || groupLevels.Where(a => a != null && a.activeSelf).Count() == 0)
        {
            int numPage = mapDataList.mapDatas.Count / numLevel1View + 1;

            groupLevels = new GameObject[numPage];

            for (int i = 0; i < numPage; i++)
            {
                groupLevels[i] = Instantiate(groupPrefabs, groupView);

                int count = Mathf.Min(numLevel1View, mapDataList.mapDatas.Count - numLevel1View * i);

                for (int j = 0; j < count; j++)
                {
                    var g = Instantiate(levelPrefabs, groupLevels[i].transform);
                    g.GetComponentInChildren<TMP_Text>().text = (j + 1 + numLevel1View * i).ToString();
                    int level = (j + 1 + numLevel1View * i);

                    if (level <= levelActive)
                    {
                        g.GetComponent<Image>().sprite = LevelActive;
                        g.GetComponentInChildren<Button>().onClick.AddListener(() => {
                            int _level = level;
                            play(_level);
                        });
                    }
                }

            }
        }
        else
        {
            refreshLevel();
        }

        vecTarget = rectTransform.anchoredPosition;

        if (vecTarget.x <= -((groupLevels.Length - 1) * (sizeView.x - 200 + sizeView.y / 2)))
            btnRight.interactable = false;
        if (vecTarget.x >= 0)
            btnLeft.interactable = false;
    }

    public void refreshLevel()
    {
        int levelActive = PlayerPrefs.GetInt("levelActive", 1);
        for (int i = 0; i < groupLevels.Length; i++)
        {
            groupLevels[i].transform.SetParent(this.transform, false);
            int count = Mathf.Min(numLevel1View, mapDataList.mapDatas.Count - numLevel1View * i);

            for (int j = 0; j < count; j++)
            {
                var g = groupLevels[i].transform.GetChild(j);
                int level = (j + 1 + numLevel1View * i);

                g.GetComponentInChildren<TMP_Text>().text = level.ToString();

                if (level <= levelActive)
                {
                    g.GetComponent<Image>().sprite = LevelActive;
                }
                else
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                    g.GetComponent<Image>().sprite = LevelLock;
                }
            }
        }
    }

    public void play(int levelSelect)
    {
        for (int i = 0; i < groupLevels.Length; i++)
        {
            groupLevels[i].transform.SetParent(null, false);
            DontDestroyOnLoad(groupLevels[i]);
        }
        PlayerPrefs.SetInt("level", levelSelect);
        SceneManager.LoadScene(1);
    }

    void FixedUpdate()
    {
        if(Vector2.Distance(rectTransform.anchoredPosition, vecTarget) > 0)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, vecTarget, speedSlice);
        }
    }

    public void nextPage()
    {
        vecTarget = new Vector2(rectTransform.anchoredPosition.x - (sizeView.x - 200 + sizeView.y / 2), 0);

        if (vecTarget.x <= -((groupLevels.Length - 1) * (sizeView.x - 200 + sizeView.y / 2)))
        {
            btnLeft.interactable = true;
            btnRight.interactable = false;
        }      
    }

    public void prePage()
    {
        vecTarget = new Vector2(rectTransform.anchoredPosition.x + (sizeView.x - 200 + sizeView.y / 2), 0);

        if (vecTarget.x >= 0)
        {
            btnLeft.interactable = false;
            btnRight.interactable = true;
        }
    }
}
