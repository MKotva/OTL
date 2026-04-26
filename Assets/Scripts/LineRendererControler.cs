using UnityEngine;

public class LineRendererControler : MonoBehaviour
{
    public LineRenderer line;
    public float length = 20f;

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * length;

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
