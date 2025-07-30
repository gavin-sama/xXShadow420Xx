using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private GameObject bossAreaPrefab;
    [SerializeField] private GameObject miniBossAreaPrefab;
    [SerializeField] private GameObject startingAreaPrefab;

    //private List<GameObject> possibleGroundTiles;
    [SerializeField] private GameObject cellObj;

    private List<GameObject> gridComponents;
    private List<GameObject> groundComponents;
    private List<Cell> cellsToGenerate;
    private List<GameObject> cells;
    private bool isComplete;


    private void Awake()
    {
        gridComponents = new List<GameObject>();
        groundComponents = new List<GameObject>();
        cellsToGenerate = new List<Cell>();
        cells = new List<GameObject>();
        isComplete = false;


        if (startingAreaPrefab.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
        {
            gridComponents.Add(Instantiate(startingAreaPrefab, GroundBase.lastTransformPosition, Quaternion.identity));
            groundComponents.Add(gridComponents[0]);
            InitializeNextGrid(gridComponents[0]);
        }
    }

    void InitializeNextGrid(GameObject groundToGenerateAround)
    {
        if (cellsToGenerate.Count > 0)
        {
            GroundBase groundScript = (GroundBase)groundToGenerateAround.GetComponentAtIndex(1);
            Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction direction in allDirections)
            {
                GameObject newCell = Instantiate(cellObj, groundScript.GetNextPosition(direction), Quaternion.identity);
                Cell cellScript = newCell.GetComponentAtIndex(newCell.GetComponentCount() - 1) as Cell;
                cellScript.CreateCell(false, roadPrefabs);
                ValidateCellPosition(ref newCell);

                if (newCell.activeSelf)
                {
                    gridComponents.Add(newCell);
                    cells.Add(newCell);
                    cellsToGenerate.Add(cellScript);
                }
            }

            UpdateGeneration();
        }
        else if (cellsToGenerate.Count == 0 && isComplete == false)
        {
            GroundBase groundScript = (GroundBase)groundToGenerateAround.GetComponentAtIndex(1);
            Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction direction in allDirections)
            {
                GameObject newCell = Instantiate(cellObj, groundScript.GetNextPosition(direction), Quaternion.identity);
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
        else
        {

        }
    }

    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(cellsToGenerate);
        tempGrid.RemoveAll(c => c.filled);
        tempGrid.Sort((a, b) => a.tileOptions.Count - b.tileOptions.Count);
        tempGrid.RemoveAll(a => a.tileOptions.Count != tempGrid[0].tileOptions.Count);

        yield return new WaitForSeconds(10.125f);

        FillRandomCell(ref tempGrid);
    }

    void FillRandomCell(ref List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        Cell cellToFill = tempGrid[randIndex];
        cellToFill.CreateCell(false, roadPrefabs);

        cellToFill.filled = true;
        int random = UnityEngine.Random.Range(0, cellToFill.tileOptions.Count);
        RoadTypeDirection selectedRoad = cellToFill.tileOptions[random];
        cellToFill.tileOptions = new List<RoadTypeDirection> { selectedRoad };

        RoadTypeDirection foundRoad = cellToFill.tileOptions[0];
        Quaternion rotation = Quaternion.identity;
        rotation.y = foundRoad.rotations[UnityEngine.Random.Range(0, foundRoad.rotations.Count)];
        GameObject obj = Instantiate(foundRoad.prefab, cellToFill.transform.position, rotation);
        GroundBase.lastTransformPosition = obj.transform.position;
        Destroy(cellToFill);
        groundComponents.Add(obj);
        gridComponents.Add(obj);

        InitializeNextGrid(obj);
    }

    void ValidateCellPosition(ref GameObject cell)
    {
        foreach (GameObject component in gridComponents)
        {
            if (cell.transform == component.transform)
            {
                Destroy(cell);
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
            if (cellsToGenerate[i].filled)
            {
                Destroy(cells[i]);
            }
            else
            {
                List<RoadTypeDirection> options = new List<RoadTypeDirection>();
                
                foreach (GameObject groundComponent in groundComponents)
                {
                    GroundBase groundScript = groundComponent.GetComponentAtIndex(1) as GroundBase;
                    List<Vector3> componentAvailablePositions = groundScript.GetAvailablePositions();
                    int n = 0;
                    foreach (Vector3 transform in componentAvailablePositions)
                        if (groundTransforms.Contains(transform))
                            n++;
                    if (n == 4)
                        groundComponents.Remove(groundComponent);
                    else
                    {
                        if (componentAvailablePositions.Contains(cellsToGenerate[i].transform.position))
                        {
                            // Is current groundComponent the cell's South object
                            if (groundScript.GetNextPosition(Direction.North) == cellsToGenerate[i].transform.position)
                            {
                                CheckValidity(ref options, groundScript.NorthRoadPrefabs);
                            }

                            // Is current groundComponent the cell's West object
                            else if (groundScript.GetNextPosition(Direction.East) == cellsToGenerate[i].transform.position)
                            {
                                CheckValidity(ref options, groundScript.EastRoadPrefabs);
                            }

                            // Is current groundComponent the cell's North object
                            else if (groundScript.GetNextPosition(Direction.South) == cellsToGenerate[i].transform.position)
                            {
                                CheckValidity(ref options, groundScript.SouthRoadPrefabs);
                            }

                            // Current groundComponent is the cell's East object
                            else
                            {
                                CheckValidity(ref options, groundScript.WestRoadPrefabs);
                            }
                        }
                    }
                }

                cellsToGenerate[i].tileOptions = options;
            }
        }

        StartCoroutine(CheckEntropy());
    }

    void CheckValidity(ref List<RoadTypeDirection> optionList, List<RoadTypeDirection> validOptions)
    {
        if (optionList.Count == 0)
        {
            List<GameObject> validOptionsPrefabs = new List<GameObject>();
            foreach (RoadTypeDirection validOption in validOptions)
            {
                validOptionsPrefabs.Add(validOption.prefab);
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
}
