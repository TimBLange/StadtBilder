using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class InputManager : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] LayerMask[] placeableLayers;
    int currentLayer = 0;
    bool canBePlaced=true;


    Vector3 lastPosition = new Vector3(0, 0, 0);

    [SerializeField] Grid grid;

    [SerializeField] List<Vector3> occupiedFields;
    GameObject[] borderStuds;
    bool onFreeSpace = true;

    [SerializeField] GameObject[] houseArray;
    HouseLogic currentHouseLogic;
    GameObject currentHouseToBuild;
    GameObject mousePointer;

    private void Start()
    {
        AssignRandomHouse();

        borderStuds = GameObject.FindGameObjectsWithTag("stud");
        foreach (GameObject go in borderStuds)
        {
            occupiedFields.Add(go.transform.position);
        }

    }
    private void Update()
    {

        GetMouseSquare();
        mousePointer.transform.position = lastPosition;

        if (Input.GetMouseButtonDown(0)&& canBePlaced)
        {
            Debug.Log(lastPosition);
            foreach (Transform stud in currentHouseLogic.studs)
            {
                Vector3 newStud = RoundToNearestValue(stud.position);
                if (occupiedFields.Contains(newStud))
                {
                    onFreeSpace = false;
                }
            }

            if (onFreeSpace)
            {
                Debug.Log("build");


                foreach (Transform stud in currentHouseLogic.studs)
                {
                    Vector3 newStud = RoundToNearestValue(stud.position);
                    if (!occupiedFields.Contains(newStud))
                        occupiedFields.Add(newStud);
                    if (!occupiedFields.Contains(newStud + Vector3.forward))
                        occupiedFields.Add(newStud + Vector3.forward);
                    if (!occupiedFields.Contains(newStud - Vector3.forward))
                        occupiedFields.Add(newStud - Vector3.forward);
                    if (!occupiedFields.Contains(newStud + Vector3.right))
                        occupiedFields.Add(newStud + Vector3.right);
                    if (!occupiedFields.Contains(newStud - Vector3.right))
                        occupiedFields.Add(newStud - Vector3.right);
                }
                SwitchLayers();
                Instantiate(currentHouseToBuild, lastPosition, Quaternion.identity);
                AssignRandomHouse();
            }
            
            onFreeSpace = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            currentHouseLogic.rotationPoint.Rotate(Vector3.up, 90);
        }
    }
    public void GetMouseSquare()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.nearClipPlane;

        Ray ray = Camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, placeableLayers[currentLayer]))
        {
            mousePointer.SetActive(true);
            RoundToNearestValue(hit.point);
            lastPosition = hit.point;
            canBePlaced = true;
        }
        else
        {
            mousePointer.SetActive(false);
            canBePlaced = false;
        }

            lastPosition = grid.WorldToCell(lastPosition);

    }

    public Vector3 RoundToNearestValue(Vector3 number)
    {
        Vector3 newNumber = new Vector3(Mathf.FloorToInt(number.x) + 0.5f, number.y, Mathf.FloorToInt(number.z) + 0.5f);
        return newNumber;
    }


    public void AssignRandomHouse()
    {
        GameObject newHouse = Instantiate(houseArray[Random.Range(0, houseArray.Length)]);
        mousePointer = newHouse;
        mousePointer.SetActive(false);
        currentHouseToBuild = newHouse;
        currentHouseLogic = newHouse.GetComponent<HouseLogic>();
    }

    public void SwitchLayers()
    {
        if (currentLayer == 1)
        {
            currentLayer = 0;
        }
        else
        {
            currentLayer = 1;
        }
    }
   
}
