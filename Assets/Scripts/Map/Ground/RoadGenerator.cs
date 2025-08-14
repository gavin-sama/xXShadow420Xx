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
using static UnityEngine.Rendering.DebugUI.Table;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private GameObject bossAreaPrefab;
    [SerializeField] private GameObject miniBossAreaPrefab;
    [SerializeField] private GameObject startingAreaPrefab;

    [SerializeField] private GameObject cellObj;

    private int allowedEnds;
    [SerializeField] private int totalAllowedComponents = 25;

    public static List<GameObject> gridComponents;
    [SerializeField] private List<GameObject> groundComponents;
    private List<Cell> cellsToGenerate;
    private List<GameObject> cells;

    private bool isComplete;
    private bool isReady;

    private List<GameObject> tempGrid;
    private const float waitTime = 0.125f;


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
        GroundBase groundScript = (GroundBase)groundToGenerateAround.GetComponentAtIndex(1);

        if (!isComplete || cellsToGenerate.Count > 1)
        {
            foreach (Direction direction in groundScript.PlaceableDirections)
            {
                GameObject newCell = Instantiate(cellObj, groundScript.GetNextPosition(direction), new Quaternion(0, 0, 0, 0));
                float y = 0f;
                if (groundToGenerateAround.GetComponent<RoadRamp>() != null)
                {
                    if (groundScript.GetNextPosition(Direction.North) == newCell.transform.position)
                        y = newCell.transform.position.y + GroundBase.heightAdjustment;
                    else if (groundScript.GetNextPosition(Direction.South) == newCell.transform.position)
                        y = newCell.transform.position.y - GroundBase.heightAdjustment;
                }
                else
                {
                    y = newCell.transform.position.y;
                }
                newCell.transform.position = new Vector3(
                    Mathf.Round(newCell.transform.position.x / 45f) * 45f,
                    y,
                    Mathf.Round(newCell.transform.position.z / 45f) * 45f
                );


                Cell cellScript = newCell.GetComponentAtIndex(newCell.GetComponentCount() - 1) as Cell;
                cellScript.CreateCell(false, roadPrefabs);
                ValidateCellPosition(ref newCell, groundToGenerateAround);

                if (newCell)
                {
                    gridComponents.Add(newCell);
                    cells.Add(newCell);
                    cellsToGenerate.Add(cellScript);
                }
            }

            UpdateGeneration();
        }
        else
        {
            GameObject bossGround = Instantiate(bossAreaPrefab, cellsToGenerate[0].gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            RoadCulDeSac bossGroundScript = bossGround.GetComponent<RoadCulDeSac>();

            DestroyImmediate(cellsToGenerate[0]);
            DestroyImmediate(cells[0]);

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))                                 // Change the groundScript to derive from the attached ground based on the connected fog wall
            {
                Debug.Log("Starting loop");
                Vector3 testDirection = bossGroundScript.GetNextPosition(direction);
                (float x, float z) testTuple = (testDirection.x, testDirection.z);

                foreach (GameObject component in gridComponents)
                {
                    Debug.Log("Starting loop");
                    if (((GroundBase)component.GetComponentAtIndex(1)).GetAvailablePositions().Contains(testDirection))
                    {
                        Debug.Log("Starting loop/available position found");
                        foreach (Vector3 position in ((GroundBase)component.GetComponentAtIndex(1)).GetAvailablePositions())
                        {
                            (float x, float z) positionTuple = (position.x, position.z);
                            if (positionTuple == testTuple)
                            {
                                switch (direction)
                                {
                                    case Direction.North:
                                        bossGround.transform.rotation = Quaternion.Euler(0, 90f, 0);
                                        break;
                                    case Direction.East:
                                        bossGround.transform.rotation = Quaternion.Euler(0, 180f, 0);
                                        break;
                                    case Direction.South:
                                        bossGround.transform.rotation = Quaternion.Euler(0, 270f, 0);
                                        break;
                                    case Direction.West:
                                        bossGround.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        break;
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    void ValidateCellPosition(ref GameObject cell, GameObject objectGeneratedAround)
    {
        int invalidCells = 0;
        for (int i = 0; i < gridComponents.Count; i++)
        {
            if (cell.transform.position.x == gridComponents[i].transform.position.x &&
                cell.transform.position.z == gridComponents[i].transform.position.z &&
                cell.transform.position.y > gridComponents[i].transform.position.y - GroundBase.heightAdjustment * 4 &&
                cell.transform.position.y < gridComponents[i].transform.position.y + GroundBase.heightAdjustment * 4)
            {
                foreach (GameObject component in gridComponents)
                {
                    if (cell.transform.position.x == component.transform.position.x &&
                    cell.transform.position.z == component.transform.position.z &&
                    cell.transform.position.y > component.transform.position.y - GroundBase.heightAdjustment * 4 &&
                    cell.transform.position.y < component.transform.position.y + GroundBase.heightAdjustment * 4)
                    {
                        allowedEnds--;
                        if (component.GetComponent<Cell>() != null)
                            component.GetComponent<Cell>().connections++;
                    }
                }

                invalidCells++;

                gridComponents.Remove(cell);
                cells.Remove(cell);
                cellsToGenerate.Remove((Cell)cell.GetComponentAtIndex(cell.GetComponentCount() - 1));
                DestroyImmediate(cell);
                break;
            }
        }

        //if (invalidCells == ((GroundBase)objectGeneratedAround.GetComponentAtIndex(1)).extraEnds + 2)
        //    allowedEnds--;
    }

    void UpdateGeneration()
    {
        //if (allowedEnds < cellsToGenerate.Count)
        //    allowedEnds--;


        List<RoadTypeDirection> validOptions;
        List<(float x, float z)> groundTransforms = new List<(float x, float z)>();
        foreach (GameObject component in groundComponents)
            groundTransforms.Add((component.transform.position.x, component.transform.position.z));

        for (int i = 0; i < cellsToGenerate.Count; i++)
        {
            List<RoadTypeDirection> options = cellsToGenerate[i].tileOptions;

            int optionsNumOfUpdatedTimes = 0;
            for (int x = 0; x < groundComponents.Count; x++)
            {
                GroundBase groundScript = groundComponents[x].GetComponentAtIndex(1) as GroundBase;
                List<(float x, float z)> componentAvailablePositions = new List<(float x, float z)>();
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    Vector3 position = groundScript.GetNextPosition(direction);
                    componentAvailablePositions.Add((position.x, position.z));
                }

                (float x, float z) cellPosition = (cellsToGenerate[i].transform.position.x, cellsToGenerate[i].transform.position.z);
                if (componentAvailablePositions.Contains(cellPosition))
                {
                    // Is current cell North of groundComponent
                    if (componentAvailablePositions[0] == cellPosition)
                    {
                        validOptions = groundScript.NorthRoadPrefabs.Select(original => original.Clone()).ToList();
                        UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, i);
                    }
                    
                    // Is current cell East of groundComponent
                    else if (componentAvailablePositions[1] == cellPosition)
                    {
                        validOptions = groundScript.EastRoadPrefabs.Select(original => original.Clone()).ToList();
                        UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, i);
                    }

                    // Is current cell South of groundComponent
                    else if (componentAvailablePositions[2] == cellPosition)
                    {
                        validOptions = groundScript.SouthRoadPrefabs.Select(original => original.Clone()).ToList();
                        UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, i);
                    }

                    //Current cell is West of groundComponent
                    else
                    {
                        validOptions = groundScript.WestRoadPrefabs.Select(original => original.Clone()).ToList();
                        UpdateRules(ref optionsNumOfUpdatedTimes, options, ref validOptions, x, i);
                    }
                }
            }
        }

        if (gridComponents.Count >= totalAllowedComponents)
            isComplete = true;

        StartCoroutine(CheckEntropy());
    }

    void UpdateRules(ref int optionsNumOfUpdatedTimes, List<RoadTypeDirection> options, ref List<RoadTypeDirection> validOptions, int x, int i)
    {
        for (int p = validOptions.Count - 1; p >= 0; p--)
        {
            if (optionsNumOfUpdatedTimes > 0)
            {
                List<Type> optionsPrefabsTypes = new List<Type>();
                foreach (RoadTypeDirection obj in options)
                {
                    if (obj.prefab.GetComponentAtIndex(1).GetType() == validOptions[p].prefab.GetComponentAtIndex(1).GetType())
                    {
                        for (int r = validOptions[p].rotations.Count - 1; r >= 0; r--)
                        {
                            if (validOptions[p].rotationsChanged == false)
                            {
                                validOptions[p].rotations = validOptions[p].originalRotations
                                    .Select(rot => ((int)(rot + groundComponents[x].transform.rotation.eulerAngles.y)) % 360)
                                    .ToList();

                                validOptions[p].rotationsChanged = true;
                            }

                            if (!obj.rotations.Contains(validOptions[p].rotations[r]))
                            {
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
            }
            else
            {
                for (int r = 0; r < validOptions[p].rotations.Count; r++)
                {
                    validOptions[p].rotations = validOptions[p].originalRotations
                                    .Select(rot => ((int)(rot + groundComponents[x].transform.rotation.eulerAngles.y)) % 360)
                                    .ToList();

                    validOptions[p].rotationsChanged = true;
                }
            }
        }
        optionsNumOfUpdatedTimes++;
        CheckValidity(ref options, validOptions, i);
    }

    void CheckValidity(ref List<RoadTypeDirection> optionList, List<RoadTypeDirection> validOptions, int i)
    {
        if (optionList.Count == 0)
        {
            List<GameObject> validOptionsPrefabs = new List<GameObject>();
            for (int x = 0; x < validOptions.Count; x++)
            {
                if (allowedEnds == 1 && (validOptions[x].prefab.GetComponent<RoadCulDeSac>() != null || validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() != null))
                    validOptions.Remove(validOptions[x]);
                else if (groundComponents.Count >= (totalAllowedComponents - allowedEnds) && validOptions[x].prefab.GetComponent<RoadCulDeSac>() == null && validOptions[x].prefab.GetComponent<RoadCulDeSacRail>() == null)
                    validOptions.Remove(validOptions[x]);
                else
                {
                    optionList.Add(validOptions[x]);
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
                            // If one allowed end, cannot be culdesac
                            if (allowedEnds <= 1 && (prefab.prefab.GetComponent<RoadCulDeSac>() != null || prefab.prefab.GetComponent<RoadCulDeSacRail>() != null))
                            {
                                optionList.RemoveAt(x);
                                break;
                            }
                            else if (groundComponents.Count >= (totalAllowedComponents - allowedEnds)
                                && cellsToGenerate[i].connections > 0)
                            {
                                if (cellsToGenerate[i].connections == 1
                                    && prefab.prefab.GetComponent<RoadCorner>() == null
                                    && prefab.prefab.GetComponent<RoadCornerRail>() == null)
                                    optionList.RemoveAt(x);
                                else if (cellsToGenerate[i].connections == 2
                                    && prefab.prefab.GetComponent<RoadThreeWay>() == null
                                    && prefab.prefab.GetComponent<RoadThreeWayRail>() == null)
                                    optionList.RemoveAt(x);
                                else if (cellsToGenerate[i].connections == 3
                                    && prefab.prefab.GetComponent<RoadIntersection>() == null
                                    && prefab.prefab.GetComponent<RoadIntersectionRail>() == null)
                                    optionList.RemoveAt(x);
                                break;
                            }
                            // If remaining cells up to total allowed == allowed ends, must be culdesac
                            else if (groundComponents.Count >= (totalAllowedComponents - allowedEnds) && prefab.prefab.GetComponent<RoadCulDeSac>() == null && prefab.prefab.GetComponent<RoadCulDeSacRail>() == null)
                            {
                                optionList.RemoveAt(x);
                                break;
                            }
                            // If one available cell that is not last, it must have extra ends
                            else if (cellsToGenerate.Count == 1 && (!validOptionsPrefabsTypes.Contains(typeof(RoadStraight)) || !validOptionsPrefabsTypes.Contains(typeof(RoadStraightRail))))
                            {
                                if (((GroundBase)prefab.prefab.GetComponentAtIndex(1)).extraEnds < 1)
                                {
                                    optionList.RemoveAt(x);
                                    break;
                                }
                            }
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

        yield return new WaitForSeconds(waitTime);

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