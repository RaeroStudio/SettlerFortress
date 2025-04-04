using UnityEngine;
using Unity.AI.Navigation;

public class RealTime_Navmesh : MonoBehaviour
{
    NavMeshSurface nav_mesh_surface;
    
    void Start ()
    {
        nav_mesh_surface = GetComponent<NavMeshSurface>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) nav_mesh_surface.BuildNavMesh();
    }
}
