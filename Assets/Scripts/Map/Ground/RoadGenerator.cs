using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private GameObject bossAreaPrefab;
    [SerializeField] private GameObject miniBossAreaPrefab;
    [SerializeField] private GameObject startingAreaPrefab;

    [SerializeField] private GameObject cellObj;

    private int allowedEnds;

    private List<GameObject> gridComponents;
    private List<GameObject> groundComponents;
    private List<Cell> cellsToGenerate;
    private List<GameObject> cells;

    private bool isComplete;
    private bool isReady;

    private List<GameObject> tempGrid;


    private void Awake()
    {
        gridComponents = new List<GameObject>();
        groundComponents = new List<GameObject>();
        cellsToGenerate = new List<Cell>();
        cells = new List<GameObject>();
        tempGrid = new List<GameObject>();
        isComplete = false;
        isReady = false;
        allowedEnds = 1;


        if (startingAreaPrefab.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
        {
            gridComponents.Add(Instantiate(startingAreaPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)));
            groundComponents.Add(gridComponents[0]);
            GroundBase.lastTransform = gridComponents[0].transform;
            InitializeNextGrid(gridComponents[0]);
        }
    }
    private void Update()
    {
        if (isReady)
            FillRandomCell();
    }

    void InitializeNextGrid(GameObject groundToGenerateAround)
    {
        if (!isComplete)
        {
            GroundBase groundScript = (GroundBase)groundToGenerateAround.GetComponentAtIndex(1);
            foreach (Direction direction in groundScript.PlaceableDirections)
            {
                GameObject newCell = Instantiate(cellObj, groundScript.GetNextPosition(direction), new Quaternion(0, 0, 0, 0));
                Cell cellScript = newCell.GetComponentAtIndex(newCell.GetComponentCount() - 1) as Cell;
                cellScript.CreateCell(false, roadPrefabs);
                ValidateCellPosition(ref newCell);

                if (newCell)
                {
                    gridComponents.Add(newCell);
                    cells.Add(newCell);
                    cellsToGenerate.Add(cellScript);
                }
            }

            UpdateGeneration();
        }
        else if (cellsToGenerate.Count > 1)
        {
            UpdateGeneration();
        }
        else
        {
            Instantiate(bossAreaPrefab, cellsToGenerate[0].gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            DestroyImmediate(cellsToGenerate[0]);
        }
    }

    void ValidateCellPosition(ref GameObject cell)
    {
        int num = 0;
        for (int i = 0; i < gridComponents.Count; i++)
        {
            num++;
            if (cell.transform.position == gridComponents[i].transform.position)
            {
                gridComponents.Remove(cell);
                cells.Remove(cell);
                cellsToGenerate.Remove((Cell)cell.GetComponentAtIndex(cell.GetComponentCount() - 1));
                DestroyImmediate(cell);
                break;
            }
        }
    }

    void UpdateGeneration()
    {
        List<Vector3> groundTransforms = new List<Vector3>();
        foreach (GameObject component in groundComponents)
            groundTransforms.Add(component.transform.position);

        for (int i = 0; i < cellsToGenerate.Count; i++)
        {
            List<RoadTypeDirection> options = new List<RoadTypeDirection>();
            
            for (int x = 0; x < groundComponents.Count; x++)
            {
                GroundBase groundScript = groundComponents[x].GetComponentAtIndex(1) as GroundBase;
                List<Vector3> componentAvailablePositions = groundScript.GetAvailablePositions();
                int n = 0;
                foreach (Vector3 transform in componentAvailablePositions)
                    if (groundTransforms.Contains(transform))
                        n++;
                if (n == componentAvailablePositions.Count)
                    groundComponents.Remove(groundComponents[x]);
                else
                {
                    if (componentAvailablePositions.Contains(cellsToGenerate[i].transform.position))
                    {
                        // Is current groundComponent the cell's South object
                        if (groundScript.GetNextPosition(Direction.North) == cellsToGenerate[i].transform.position)
                        {
                            Debug.Log($"Hit North with {cellsToGenerate.Count} cellsToGenerate");
                            Debug.Log($"GroundComponent #{x}");
                            foreach (RoadTypeDirection prefab in groundScript.NorthRoadPrefabs)
                            {
                                for (int r = 0; r < prefab.rotations.Count; r++)
                                {
                                    //Debug.Log(prefab.rotations[r]);
                                    prefab.rotations[r] = (int)((prefab.rotations[r] + groundComponents[x].transform.rotation.y) % 360);
                                    //Debug.Log(prefab.rotations[r]);
                                    //Debug.Log($"Float: {groundComponents[x].transform.rotation.y} + Int Variant: {(int)groundComponents[x].transform.rotation.y}");
                                }
                            }
                            CheckValidity(ref options, groundScript.NorthRoadPrefabs);
                        }

                        // Is current groundComponent the cell's West object
                        else if (groundScript.GetNextPosition(Direction.East) == cellsToGenerate[i].transform.position)
                        {
                            Debug.Log($"Hit East with {cellsToGenerate.Count} cellsToGenerate");
                            Debug.Log($"GroundComponent #{x}");
                            foreach (RoadTypeDirection prefab in groundScript.EastRoadPrefabs)
                            {
                                for (int r = 0; r < prefab.rotations.Count; r++)
                                {
                                    prefab.rotations[r] = (int)((prefab.rotations[r] + groundComponents[x].transform.rotation.y) % 360);
                                }
                            }
                            CheckValidity(ref options, groundScript.EastRoadPrefabs);
                        }

                        // Is current groundComponent the cell's North object
                        else if (groundScript.GetNextPosition(Direction.South) == cellsToGenerate[i].transform.position)
                        {
                            Debug.Log($"Hit South with {cellsToGenerate.Count} cellsToGenerate");
                            Debug.Log($"GroundComponent #{x}");
                            foreach (RoadTypeDirection prefab in groundScript.SouthRoadPrefabs)
                            {
                                for (int r = 0; r < prefab.rotations.Count; r++)
                                {
                                    prefab.rotations[r] = (int)((prefab.rotations[r] + groundComponents[x].transform.rotation.y) % 360);
                                }
                            }
                            CheckValidity(ref options, groundScript.SouthRoadPrefabs);
                        }

                        //Current groundComponent is the cell's East object
                        else
                        {
                            Debug.Log($"Hit West with {cellsToGenerate.Count} cellsToGenerate");
                            Debug.Log($"GroundComponent #{x}");
                            foreach (RoadTypeDirection prefab in groundScript.WestRoadPrefabs)
                            {
                                for (int r = 0; r < prefab.rotations.Count; r++)
                                {
                                    prefab.rotations[r] = (int)((prefab.rotations[r] + groundComponents[x].transform.rotation.y) % 360);
                                }
                            }
                            CheckValidity(ref options, groundScript.WestRoadPrefabs);
                        }
                    }
                }
            }

            cellsToGenerate[i].tileOptions = options;
            //string s = $"CellTransform: {cellsToGenerate[i].transform.ToString()}";
            //foreach (RoadTypeDirection option in options)
            //{
            //    s += $" + {option.prefab}";
            //    foreach (int rotation in option.rotations)
            //        s += $" + {rotation}";
            //}
            //Debug.Log(s);
        }

        if (gridComponents.Count >= 20)
            isComplete = true;

        StartCoroutine(CheckEntropy());
    }

    void CheckValidity(ref List<RoadTypeDirection> optionList, List<RoadTypeDirection> validOptions)
    {
        Debug.Log($"OptionList: {optionList.Count}");
        if (optionList.Count == 0)
        {
            List<GameObject> validOptionsPrefabs = new List<GameObject>();
            Debug.Log($"ValidOptions: {validOptions.Count}");
            for (int x = 0; x < validOptions.Count; x++)
            {
                if (allowedEnds <= 1 && (validOptions[x].prefab.GetComponent<RoadCulDeSac>() != null || validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() != null))
                {
                    validOptions.Remove(validOptions[x]);
                }
                else
                {
                    validOptionsPrefabs.Add(validOptions[x].prefab);
                    if (validOptions[x].prefab.GetComponent<RoadCulDeSac>() != null || validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() != null)
                        allowedEnds--;
                }   
            }

            for (int x = roadPrefabs.Count - 1; x >= 0; x--)
            {
                GameObject element = roadPrefabs[x];
                if (validOptionsPrefabs.Contains(element))
                {
                    int index = Array.FindIndex(validOptionsPrefabs.ToArray(), obj => obj == element);
                    optionList.Add(validOptions[index]);
                }
            }
        }
        else
        {
            for (int x = optionList.Count - 1; x >= 0; x--)
            {
                if (!validOptions.Contains(optionList[x]))
                {
                    optionList.RemoveAt(x);
                }
            }
        }
    }

    IEnumerator CheckEntropy()
    {
        List<GameObject> tempGrid1 = new List<GameObject>(cells);
        tempGrid1.Sort((a, b) => ((Cell)a.GetComponentAtIndex(a.GetComponentCount() - 1)).tileOptions.Count - ((Cell)b.GetComponentAtIndex(b.GetComponentCount() - 1)).tileOptions.Count);
        tempGrid1.RemoveAll(a => ((Cell)a.GetComponentAtIndex(a.GetComponentCount() - 1)).tileOptions.Count != ((Cell)tempGrid1[0].GetComponentAtIndex(tempGrid1[0].GetComponentCount() - 1)).tileOptions.Count);
        tempGrid = tempGrid1;

        yield return new WaitForSeconds(1.125f);

        isReady = true;
    }

    void FillRandomCell()
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        
        GameObject cell = tempGrid[randIndex];
        Cell cellToFill = (Cell)tempGrid[randIndex].GetComponentAtIndex(tempGrid[randIndex].GetComponentCount() - 1);

        int random = UnityEngine.Random.Range(0, cellToFill.tileOptions.Count);
        RoadTypeDirection selectedRoad = cellToFill.tileOptions[random];
        cellToFill.tileOptions = new List<RoadTypeDirection> { selectedRoad };

        RoadTypeDirection foundRoad = cellToFill.tileOptions[0];
        float y = foundRoad.rotations[UnityEngine.Random.Range(0, foundRoad.rotations.Count)];
        GameObject obj = Instantiate(foundRoad.prefab, cellToFill.transform.position, Quaternion.Euler(0, y, 0).normalized);
        GroundBase.lastTransform = obj.transform;

        allowedEnds += ((GroundBase)foundRoad.prefab.GetComponentAtIndex(1)).extraEnds;

        gridComponents.Remove(cell);
        GameObject cellActual = cells[Array.FindIndex(cells.ToArray(), go => go == cell)];
        cells.Remove(cellActual);
        cellsToGenerate.Remove(cellToFill);
        DestroyImmediate(cell);
        DestroyImmediate(cellActual);

        groundComponents.Add(obj);
        gridComponents.Add(obj);

        isReady = false;

        InitializeNextGrid(obj);
    }
}
