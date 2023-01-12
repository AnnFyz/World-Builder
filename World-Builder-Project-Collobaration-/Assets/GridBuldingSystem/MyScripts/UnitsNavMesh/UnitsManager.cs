using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance { get; private set; }
    public List<Unit> selectedUnits = new List<Unit>();
    //List<GameObject> units = new List<GameObject>();
    public LayerMask unitMask;
    public LayerMask groundMask;
    public List<Transform> waypoints; // to make them for each type of building and unit
    public Action TimeToGo;
    private void Awake()
    {
        Instance = this;
    }
        void Update()
    {
        if(waypoints.Count >= 2)
        {
            TimeToGo?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
            {

                Unit unit = hit.collider.transform.GetComponentInParent<Unit>();
                if (!selectedUnits.Contains(unit))
                {

                    if (unit != null)
                    {
                        selectedUnits.Add(unit);
                        unit.OnSelected();
                    }

                }
            }

            else
            {
                foreach (var unit in selectedUnits)
                {
                    if (unit != null)
                        unit.OnDeselected();
                }

                selectedUnits.Clear();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                foreach (Unit unit in selectedUnits)
                {
                    if (unit != null)
                        unit.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);
                }
            }
        }
    }
}
