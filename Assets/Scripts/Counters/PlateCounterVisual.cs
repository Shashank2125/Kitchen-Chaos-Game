using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter plateCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    //it is visaul object not the game object we are using
    private List<GameObject> plateVisualGameObjectList;
    private void Awake()
    {
        plateVisualGameObjectList = new List<GameObject>();
    }
    private void Start()
    {
        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }
    private void PlateCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        //plate is removed so in the list remove One plate and spawn new plate
        GameObject plateGameobject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        //grabbing the last element of the list
        plateVisualGameObjectList.Remove(plateGameobject);
        //removing from list and destroying it in the scene
        Destroy(plateGameobject);
    }
    private void PlateCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);
        float plateOffsetY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);
        //the plate will be spwaned on top of each other with the transform
        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);

    }
    
}
