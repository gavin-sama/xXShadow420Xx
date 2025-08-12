using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using static UnityEditor.Rendering.FilterWindow;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private GameObject bossAreaPrefab;
    [SerializeField] private GameObject miniBossAreaPrefab;
    [SerializeField] private GameObject startingAreaPrefab;

    [SerializeField] private GameObject cellObj;

    private int allowedEnds;

    [SerializeField] private List<GameObject> gridComponents;
    [SerializeField] private List<GameObject> groundComponents;
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
                newCell.transform.position = new Vector3(
                        Mathf.Round(newCell.transform.position.x / 45f) * 45f,
                        newCell.transform.position.y,
                        Mathf.Round(newCell.transform.position.z / 45f) * 45f
                    );
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
        for (int i = 0; i < gridComponents.Count; i++)
        {
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
        List<RoadTypeDirection> validOptions;
        List<Vector3> groundTransforms = new List<Vector3>();
        foreach (GameObject component in groundComponents)
            groundTransforms.Add(component.transform.position);

        for (int i = 0; i < cellsToGenerate.Count; i++)
        {
            Debug.Log($"cellsToGenerate index: {i}");
            List<RoadTypeDirection> options = cellsToGenerate[i].tileOptions;

            int optionsNumOfUpdatedTimes = 0;
            for (int x = 0; x < groundComponents.Count; x++)
            {
                GroundBase groundScript = groundComponents[x].GetComponentAtIndex(1) as GroundBase;
                List<Vector3> componentAvailablePositions = new List<Vector3>();
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    componentAvailablePositions.Add(groundScript.GetNextPosition(direction));
                int n = 0;
                foreach (Vector3 transform in componentAvailablePositions)
                    if (groundTransforms.Contains(transform))
                        n++;
                if (n == 4)
                    groundComponents.Remove(groundComponents[x]);
                else
                {
                    if (componentAvailablePositions.Contains(cellsToGenerate[i].transform.position))
                    {
                        Debug.Log(groundComponents[x].GetComponentAtIndex(1).GetType());
                        // Is current groundComponent the cell's South object
                        if (groundScript.GetNextPosition(Direction.North) == cellsToGenerate[i].transform.position)
                        {
                            string s = "North";
                            validOptions = groundScript.NorthRoadPrefabs.Select(original => original.Clone()).ToList();
                            UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, s);
                        }

                        // Is current groundComponent the cell's West object
                        else if (groundScript.GetNextPosition(Direction.East) == cellsToGenerate[i].transform.position)
                        {
                            string s = "East";
                            validOptions = groundScript.EastRoadPrefabs.Select(original => original.Clone()).ToList();
                            UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, s);
                        }

                        // Is current groundComponent the cell's North object
                        else if (groundScript.GetNextPosition(Direction.South) == cellsToGenerate[i].transform.position)
                        {
                            string s = "South";
                            validOptions = groundScript.SouthRoadPrefabs.Select(original => original.Clone()).ToList();
                            UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, s);
                        }

                        //Current groundComponent is the cell's East object
                        else
                        {
                            string s = "West";
                            validOptions = groundScript.WestRoadPrefabs.Select(original => original.Clone()).ToList();
                            UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, s);
                        }
                    }
                }
            }
        }

        if (gridComponents.Count >= 150)
            isComplete = true;

        StartCoroutine(CheckEntropy());
    }

    void UpdateRules(ref int optionsNumOfUpdatedTimes, List<RoadTypeDirection> options, ref List<RoadTypeDirection> validOptions, int x, string s)
    {
        for (int p = validOptions.Count - 1; p >= 0; p--)
        {
            Debug.Log($"ValidOptions.Count: {validOptions.Count} & p: {p}");
            if (optionsNumOfUpdatedTimes > 0)
            {
                Debug.Log($"TileOptions Before: {validOptions.Count}");
                List<Type> optionsPrefabsTypes = new List<Type>();
                foreach (RoadTypeDirection obj in options)
                {
                    if (obj.prefab.GetComponentAtIndex(1).GetType() == validOptions[p].prefab.GetComponentAtIndex(1).GetType())
                    {
                        for (int r = validOptions[p].rotations.Count - 1; r >= 0; r--)
                        {
                            if (validOptions[p].rotationsChanged == false)
                            {
                                //validOptions[p].rotations[r] = (int)((validOptions[p].rotations[r] + groundComponents[x].transform.rotation.eulerAngles.y) % 360);

                                validOptions[p].rotations = validOptions[p].originalRotations
                                    .Select(rot => ((int)(rot + groundComponents[x].transform.rotation.eulerAngles.y)) % 360)
                                    .ToList();

                                validOptions[p].rotationsChanged = true;
                            }

                            if (!obj.rotations.Contains(validOptions[p].rotations[r]))
                            {
                                s += $" | {validOptions[p].prefab.GetComponentAtIndex(1).GetType()} -- Removed Rotation: {validOptions[p].rotations[r].ToString()}";
                                validOptions[p].rotations.RemoveAt(r);
                            }
                        }
                    }

                    optionsPrefabsTypes.Add(obj.prefab.GetComponentAtIndex(1).GetType());
                }
                if (!optionsPrefabsTypes.Contains(validOptions[p].prefab.GetComponentAtIndex(1).GetType()))
                    validOptions.RemoveAt(p);
                else if (validOptions[p].rotations.Count == 0)
                    validOptions.RemoveAt(p);


                Debug.Log($"TileOptions After: {validOptions.Count}");
            }
            else
            {
                Debug.Log($"TileOptions Before: {validOptions.Count}");
                for (int r = 0; r < validOptions[p].rotations.Count; r++)
                {
                    s += $" | {validOptions[p].prefab.GetComponentAtIndex(1).GetType()} -- Old Rotation: {validOptions[p].rotations[r].ToString()} + Ground Rotation: {groundComponents[x].transform.rotation.eulerAngles.y}";
                    //validOptions[p].rotations[r] = (int)(1 * (validOptions[p].rotations[r] + groundComponents[x].transform.rotation.eulerAngles.y) % 360);

                    validOptions[p].rotations = validOptions[p].originalRotations
                                    .Select(rot => ((int)(rot + groundComponents[x].transform.rotation.eulerAngles.y)) % 360)
                                    .ToList();

                    validOptions[p].rotationsChanged = true;
                    s += $" -- New Rotation: {validOptions[p].rotations[r].ToString()}";
                }
                Debug.Log($"TileOptions After: {validOptions.Count}");
            }
        }
        optionsNumOfUpdatedTimes++;
        Debug.Log(s);
        CheckValidity(ref options, validOptions);
    }

    void CheckValidity(ref List<RoadTypeDirection> optionList, List<RoadTypeDirection> validOptions)
    {
        if (optionList.Count == 0)
        {
            List<GameObject> validOptionsPrefabs = new List<GameObject>();
            for (int x = 0; x < validOptions.Count; x++)
            {
                if (allowedEnds <= 1 && (validOptions[x].prefab.GetComponent<RoadCulDeSac>() != null || validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() != null))
                    validOptions.Remove(validOptions[x]);
                else
                {
                    optionList.Add(validOptions[x]);
                    if (validOptions[x].prefab.GetComponent<RoadCulDeSac>() != null || validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() != null)
                        allowedEnds--;
                }   
            }
        }
        else
        {
            List<Type> validOptionsPrefabsTypes = new List<Type>();
            foreach (RoadTypeDirection prefab in validOptions)
            {
                validOptionsPrefabsTypes.Add(prefab.prefab.GetComponentAtIndex(1).GetType());
            }

            for (int x = optionList.Count - 1; x >= 0; x--)
            {
                if (!validOptionsPrefabsTypes.Contains(optionList[x].prefab.GetComponentAtIndex(1).GetType()))
                    optionList.RemoveAt(x);
                else
                {
                    foreach (RoadTypeDirection prefab in validOptions)
                    {
                        if (prefab.prefab.GetComponentAtIndex(1).GetType() == optionList[x].prefab.GetComponentAtIndex(1).GetType())
                        {

                            if (allowedEnds <= 1 && (prefab.prefab.GetComponent<RoadCulDeSac>() != null || prefab.prefab.GetComponent<RoadCulDeSacRail>() != null))
                                optionList.RemoveAt(x);
                            else
                                optionList[x].rotations = new List<int>(prefab.rotations);
                        }
                    }
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

        yield return new WaitForSeconds(0.125f);

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
        GameObject obj = Instantiate(foundRoad.prefab, cellToFill.transform.position, Quaternion.Euler(0, 0, 0));
        obj.transform.position = new Vector3(
                Mathf.Round(obj.transform.position.x / 45f) * 45f,
                obj.transform.position.y,
                Mathf.Round(obj.transform.position.z / 45f) * 45f
            );
        obj.transform.rotation = Quaternion.Euler(0, y, 0);
        GroundBase.lastTransform = obj.transform;

        //if (obj.TryGetComponent<RoadRamp>(out RoadRamp script))                                               Needs to be flushed in order to only account for pieces after ramps...
        //    GroundBase.currentHeightChange += 1;
        Debug.Log($"allowedEnds: {allowedEnds}");
        allowedEnds += ((GroundBase)foundRoad.prefab.GetComponentAtIndex(1)).extraEnds;
        Debug.Log($"New allowedEnds: {allowedEnds}");
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