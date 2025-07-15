using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class Road : MonoBehaviour
{
    [SerializeField] public List<GameObject> connectorRoadPrefabs;
    [SerializeField] public List<GameObject> connectorRoadRailPrefabs;
    [SerializeField] public List<GameObject> connectorBridgePrefabs;
    [SerializeField] public List<GameObject> housePrefabs;
    [SerializeField] public List<GameObject> grassPrefabs;
    [SerializeField] public List<GameObject> stonePrefabs;
    [SerializeField] public List<GameObject> treePrefabs;
    public List<GameObject> entries { get; private set; }

    private void Start()
    {
        GameObject entriesObject = transform.Find("Entries").gameObject;
        for (int i = 0; i < entriesObject.transform.childCount; i++)
        {
            entries.Add(entriesObject.transform.GetChild(i).gameObject);
        }
    }
}