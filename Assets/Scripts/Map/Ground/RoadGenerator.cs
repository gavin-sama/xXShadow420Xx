using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> roadPrefabs;
    [SerializeField] private GameObject bossAreaPrefab;
    [SerializeField] private GameObject miniBossAreaPrefab;
    [SerializeField] private GameObject startingAreaPrefab;

    //private List<GameObject> possibleGroundTiles;
    [SerializeField] private GameObject cellObj;
    int cellSizeX = 45;
    int cellSizeZ = 45;

    private List<GameObject> gridComponents;
    private List<GameObject> groundComponents;
    private List<Cell> cellsToGenerate;
    private bool isComplete;


    private void Awake()
    {
        gridComponents = new List<GameObject>();
        groundComponents = new List<GameObject>();
        cellsToGenerate = new List<Cell>();
        isComplete = false;


        if (startingAreaPrefab.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
        {
            gridComponents.Add(Instantiate(startingAreaPrefab, GroundBase.lastTransformPosition, Quaternion.identity));
            InitializeNextGrid(gridComponents[0]);
        }
    }

    void InitializeNextGrid(GameObject groundToGenerateAround)
    {
        if (cellsToGenerate.Count > 0)
        {
            GroundBase.lastTransformPosition = groundToGenerateAround.transform.position;

        }
        else if (cellsToGenerate.Count == 0 && isComplete == false)
        {
            GroundBase.lastTransformPosition = groundToGenerateAround.transform.position;
            GroundBase groundScript = groundToGenerateAround.GetComponentAtIndex(1) as GroundBase;
            Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction direction in allDirections)
            {
                GameObject newCell = Instantiate(cellObj, groundScript.GetNextPosition(direction), Quaternion.identity);
                Cell cellScript = newCell.GetComponentAtIndex(newCell.GetComponentCount() - 1) as Cell;
                cellScript.CreateCell(false, roadPrefabs); 
                ValidateCellPosition(newCell);
                
                if (newCell)
                    gridComponents.Add(newCell);
                if (groundScript.GetAvailablePositions().Contains(newCell.transform.position))
                    cellsToGenerate.Add(cellScript);
            }
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

        yield return new WaitForSeconds(0.125f);

        FillRandomCell(tempGrid);
    }

    void FillRandomCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToFill = tempGrid[randIndex];

        cellToFill.filled = true;
        int random = UnityEngine.Random.Range(0, cellToFill.tileOptions.Count);
        RoadTypeDirection selectedRoad = cellToFill.tileOptions[random];
        cellToFill.tileOptions = new List<RoadTypeDirection> { selectedRoad };

        RoadTypeDirection foundRoad = cellToFill.tileOptions[0];
        Quaternion rotation = Quaternion.identity;
        rotation.y = foundRoad.rotations[UnityEngine.Random.Range(0, foundRoad.rotations.Count)];
        groundComponents.Add(Instantiate(foundRoad.prefab, cellToFill.transform.position, rotation));
        gridComponents.Add(groundComponents[groundComponents.Count - 1]);
    }

    void ValidateCellPosition(GameObject cell)
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
        List<Cell> newGenerationCell = new List<Cell>(cellsToGenerate);

        for (int i = 0; i < cellsToGenerate.Count; i++)
        {
            if (cellsToGenerate[i].filled)
            {
                newGenerationCell[i] = cellsToGenerate[i];
            }
            else
            {
                List<GameObject> options = new List<GameObject>();
                foreach (GameObject go in roadPrefabs)
                {
                    options.Add(go);
                }

                if ()
            }
        }
    }

    void CheckValidity(List<GameObject> optionList, List<GameObject> validOption)
    {

    }


    //private int rotation = 0;
    //private GameObject recentCreation;

    //public void Test()
    //{
    //    if (Input.anyKeyDown)
    //    {
    //        if (rotation == 0)
    //        {
    //            RoadTypeDirection mapToGenerate = NorthRoadPrefabs[Random.Range(0, NorthRoadPrefabs.Count - 1)];
    //            GameObject prefabToGenerate = mapToGenerate.prefab;

    //            if (prefabToGenerate.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
    //            {
    //                GroundBase prefabScript = prefabToGenerate.GetComponentAtIndex(1) as GroundBase;
    //                Vector3 newPosition = GetNextPosition(prefabScript.sizeX, prefabScript.sizeZ, Direction.North);
    //                Quaternion rotate = Quaternion.identity;
    //                rotate.y = mapToGenerate.rotations[Random.Range(0, mapToGenerate.rotations.Count - 1)];

    //                recentCreation = Instantiate(prefabToGenerate, newPosition, rotate);
    //                rotation++;
    //            }
    //        }
    //        else
    //        {
    //            Destroy(recentCreation);
    //            rotation--;
    //        }
    //    }
    //}
}
