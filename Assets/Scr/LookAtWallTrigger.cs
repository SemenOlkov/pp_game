using UnityEngine;

public class LookAtWallTrigger : MonoBehaviour
{
    public Camera mainCamera;
    public float maxDistance = 10f;
    public GameObject wallObject; // Стена с скриптом
    private WallTrigger wallTrigger;

    void Start()
    {
        wallTrigger = wallObject.GetComponent<WallTrigger>();
    }

    void Update()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("ScreamerWall"))
            {
                wallTrigger.StartScream();
            }
        }
    }
}