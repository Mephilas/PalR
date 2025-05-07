using Mathd;
using ToolSelf;
using UnityEngine;
using UnityEngine.UI;

public class UIPos : MonoBehaviour
{
    public Text test;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3d mousePosition = Input.mousePosition;
        mousePosition.z = 1f;
        Vector3d mousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePoint.y = 0;
        mousePoint = ToolM.WorldToGridVec(mousePoint);
        test.text = mousePoint.ToString();
        test.transform.parent.position = Input.mousePosition;
    }
}
