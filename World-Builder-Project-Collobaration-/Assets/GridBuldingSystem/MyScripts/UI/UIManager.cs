using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject arrowsPanel;
    [SerializeField] GameObject iconsPanel;
    [SerializeField] List<GameObject> icons;
    [SerializeField] GameObject amountOfGemsUI;
    public TMP_Text textAmount;
    int amountOfGems = 0;
    public LocalLevelState prefabsState;
    public static UIManager Instance { get; private set; }
    public event Action OnChangedGrid;

    private void Awake()
    {
        Instance = this;
        textAmount = amountOfGemsUI.GetComponent<TMP_Text>();
        
    }

    private void Start()
    {
        HidePanels();
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }

        textAmount.SetText("0");
    }

    public void LocalSetupUIIcons()
    {
        //prefabsState = BuildingManager.blockPrefab.gameObject.GetComponent<LocalLevelState>();
        if (prefabsState != null)
        {
            switch (prefabsState.GetCurrentLevelState())
            {
                case LevelState.Pond:
                    foreach (var icon in icons)
                    {
                        icon.SetActive(false);
                    }
                    break;

                case LevelState.Desert:
                    foreach (var icon in icons)
                    {
                        icon.SetActive(false);
                    }
                    icons[0].SetActive(true);
                    break;

                case LevelState.Forest:
                    foreach (var icon in icons)
                    {
                        icon.SetActive(true);
                    }
                    break;

                default:
                    Debug.Log("NOTHING");
                    break;
            }
        }       
    }
    public void ShowPanels()
    {
        arrowsPanel.SetActive(true);
        iconsPanel.SetActive(true);
    }

    public void HidePanels()
    {
        arrowsPanel.SetActive(false);
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }
        iconsPanel.SetActive(false);
    }

    public void BlockUp(int addedV)
    {
        BuildingManager.blockPrefab.ChangeHeight(addedV);
        OnChangedGrid?.Invoke();
    }

    public void BlockDown(int subtractedV)
    {
       BuildingManager.blockPrefab.ChangeHeight(subtractedV);
       OnChangedGrid?.Invoke();
    }

    public void CollectGem()
    {
        amountOfGems++;
        textAmount.SetText(amountOfGems.ToString());
    }
}
