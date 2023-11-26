using System;
using UnityEngine;

public class TowerWizard : MonoBehaviour
{

    private bool isSlotTaken;

    private void Start()
    {
        isSlotTaken = false;
    }

    private void Update()
    {
        if (!isSlotTaken) {
            // Check if the left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Create a ray from the camera through the point where the mouse clicked
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                    // Check if the ray hits this GameObject
                    if (hit.collider.gameObject == gameObject)
                    {
                        Vector3 position = gameObject.transform.position;
                        Vector3 positionOffset = new Vector3(position.x, position.y + 3, position.z);
                        isSlotTaken = GameManager.Instance.TrySpawnTower(positionOffset);
                    }
            }
        }
    }
}