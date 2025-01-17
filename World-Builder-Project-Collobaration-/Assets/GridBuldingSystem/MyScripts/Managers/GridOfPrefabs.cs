﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.AI.Navigation;
using UnityEngine.EventSystems;


public class GridOfPrefabs : MonoBehaviour
{
    [SerializeField] GameObject blockPrefabObj;
    public int width = 3;
    public int height = 5;
    [SerializeField] Color colorOfHighlightedOblock = new Color();
    //[SerializeField] Color materialOfSelectedOblock = new Color();
    [SerializeField] Material materialOfSelectedOblock;
    [SerializeField] float heightScale = 40.0f;
    [SerializeField] float xScale = 16.0f;
    public static GridOfPrefabs Instance { get; private set; }
    public static bool IsValidGridPos = false;
    public MyGridXZ<PrefabGridObject> globalGrid;
    public NavMeshSurface[] horizontalSurfaces; //TO ADD SURFACES FOR ANOTHER NAVMESHAGENTS

    private void Awake()
    {
        Instance = this;
        horizontalSurfaces = GetComponents<NavMeshSurface>();
    }
    private void OnEnable()
    {
        UIManager.Instance.OnChangedGrid += RebuildNavMesh;
    }

    private void Start()
    {
        globalGrid = new MyGridXZ<PrefabGridObject>(width, height, 15f, Vector3.zero, (MyGridXZ<PrefabGridObject> g, int x, int y) => new PrefabGridObject(g, x, y));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BlockPrefab blockPrefab = BlockPrefab.Create(globalGrid.GetWorldPosition(x, y), blockPrefabObj);
                blockPrefab.gameObject.transform.parent = gameObject.transform;
                globalGrid.GetGridObject(x, y).SetPlacedObject(blockPrefab);
                float height = heightScale * Mathf.PerlinNoise(UnityEngine.Random.Range(0.1f, 10) * xScale, 0.0f);
                blockPrefab.transform.localScale = new Vector3(1, Mathf.RoundToInt(height), 1);
                int newHeight = Mathf.FloorToInt(height);
                if (blockPrefab.transform.localScale.y <= 4) //Water
                {
                    blockPrefab.transform.localScale = new Vector3(1, 1, 1);
                    blockPrefab.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                    blockPrefab.ChangeHeight(0);

                }

                else if (blockPrefab.transform.localScale.y > 5 && blockPrefab.transform.localScale.y <= 6)
                {
                    blockPrefab.transform.localScale = new Vector3(1, blockPrefab.transform.localScale.y - 2, 1);
                    blockPrefab.transform.localRotation = Quaternion.Euler(new Vector3(0, RandomRotation(), 0));
                    blockPrefab.ChangeHeight(0);
                }

                else
                {
                    blockPrefab.ChangeHeight(0);
                    blockPrefab.blockInside.gameObject.SetActive(true);
                    blockPrefab.transform.localRotation = Quaternion.Euler(new Vector3(0, RandomRotation(), 0));

                }
            }
        }

        RebuildNavMesh();
    }


    private void RebuildNavMesh()
    {
        for (int i = 0; i < horizontalSurfaces.Length; i++)
        {
            horizontalSurfaces[i].BuildNavMesh();
        }
        
    }

    float RandomRotation()
    {
        int randomAngle = UnityEngine.Random.Range(1, 4);
        float angle = 0;
        switch (randomAngle)
        {
            case 1:
                angle = 90;
                break;
            case 2:
                angle = 180;
                break;
            case 3:
                angle = 270;
                break;
            default:
                angle = 0;
                break;
        }
        return angle;
    }
    public Transform GetCenterObjInGrid()
    {
        int halfWidth = Mathf.RoundToInt(width / 2);
        int halfHeight = Mathf.RoundToInt(height / 2);
        return globalGrid.GetGridObject(halfWidth, halfHeight).GetPlacedObject().transform;
    }

    public Color GetColorOfHighlightedBlocks()
    {
        return colorOfHighlightedOblock;
    }
    public Material GetMaterialOfSelectedBlocks()
    {
        return materialOfSelectedOblock;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 9999f))
            {

                if (raycastHit.collider.gameObject.GetComponentInParent<BlockPrefab>())
                {
                    BlockPrefab placedObject = raycastHit.collider.gameObject.GetComponentInParent<BlockPrefab>();
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            globalGrid.GetGridObject(x, z).GetPlacedObject().IsThisBlockWasSelected = false;
                            placedObject.ChangeColorBack();
                            placedObject.ChangeMaterialBack();
                            UIManager.Instance.HidePanels();
                        }
                    }
                    if (!placedObject.GetComponent<BlockHealth>().IsBlockDead)
                    {
                        placedObject.IsThisBlockWasSelected = true;
                        placedObject.ChangeSelectedMaterial();
                        UIManager.Instance.ShowPanels();
                        UIManager.Instance.prefabsState = placedObject.GetComponent<LocalLevelState>();
                        UIManager.Instance.LocalSetupUIIcons();
                    }
                    else
                    {
                        Bubble.Instance.CreatePopupText(GetMouseWorldPosition(), " I am dead.. ☹ ");
                    }
                    
                }
            }
        }




    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {

            IsValidGridPos = true;
            return raycastHit.point;
        }
        else
        {
            IsValidGridPos = false;
            return Vector3.zero;

        }
    }

    public class PrefabGridObject
    {

        private const int MIN = 0;
        private const int MAX = 255;

        private MyGridXZ<PrefabGridObject> grid;
        private int x;
        private int y;
        private int value;

        private BlockPrefab blockPrefab;
        public PrefabGridObject(MyGridXZ<PrefabGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            blockPrefab = null;
        }

        public void ChangeValue(int addValue)
        {
            value += addValue;
            value = Mathf.Clamp(value, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }


        public float GetValueNormalized()
        {
            return (float)value / MAX;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + blockPrefab;
            //return value.ToString();
        }

        public void SetPlacedObject(BlockPrefab blockPrefab)
        {
            this.blockPrefab = blockPrefab;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            blockPrefab = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public BlockPrefab GetPlacedObject()
        {
            return blockPrefab;
        }

        public bool CanBuild()
        {
            return blockPrefab == null;
        }
    }


}