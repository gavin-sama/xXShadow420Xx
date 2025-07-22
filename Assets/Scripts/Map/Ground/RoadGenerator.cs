using NUnit.Framework;
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
    [SerializeField] private Tile[] tileObjects;
    [SerializeField] private Cell cellObj;
    int cellSizeX = 45;
    int cellSizeZ = 45;

    private List<GameObject> gridComponents;
    private Stack<Cell> cellsToGenerate;
    private bool isComplete;


    private void Awake()
    {
        gridComponents = new List<Cell>();
        cellsToGenerate = new Stack<Cell>();
        isComplete = false;


        if (startingAreaPrefab.GetComponentCount() == 2)  // We are not going to use the 'transform' at index '0'
        {
            Instantiate(startingAreaPrefab, GroundBase.lastTransformPosition, Quaternion.identity);

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
            Cell newCellN = Instantiate(cellObj, groundScript.GetNextPosition(cellSizeX, cellSizeZ, Direction.North), Quaternion.identity);
            Cell newCellE = Instantiate(cellObj, groundScript.GetNextPosition(cellSizeX, cellSizeZ, Direction.East), Quaternion.identity);
            Cell newCellS = Instantiate(cellObj, groundScript.GetNextPosition(cellSizeX, cellSizeZ, Direction.South), Quaternion.identity);
            Cell newCellW = Instantiate(cellObj, groundScript.GetNextPosition(cellSizeX, cellSizeZ, Direction.West), Quaternion.identity);
            newCellN.CreateCell(false, tileObjects);
            newCellE.CreateCell(false, tileObjects);
            newCellS.CreateCell(false, tileObjects);
            newCellW.CreateCell(false, tileObjects);
            gridComponents.Add(newCellN);
            gridComponents.Add(newCellE);
            gridComponents.Add(newCellS);
            gridComponents.Add(newCellW);
        }
        else
        {

        }
    }

    IEnumerator CheckEntropy()
    {
        yield return null;
    }

    void CollapseCell(List<Cell> tempGrid)
    {

    }

    void UpdateGeneration(Cell cell)
    {
        foreach (Cell cell in gridComponents)
        {
            if (cell.transform )
        }
    }

    void CheckValidity()
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
