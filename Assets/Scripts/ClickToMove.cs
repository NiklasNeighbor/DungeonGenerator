using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class ClickToMove : MonoBehaviour
{

    NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToMouse();
        }
    }

    public void MoveToMouse()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
        {
            Vector3 clickWorldPosition = hitInfo.point;
            Debug.Log(clickWorldPosition);
            agent.destination = clickWorldPosition;
        }
    }
}
