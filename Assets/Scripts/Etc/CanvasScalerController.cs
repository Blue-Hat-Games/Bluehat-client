using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour
{
    public Canvas[] canvases;

    private void Start()
    {
        for (var i = 0; i < canvases.Length; i++)
            canvases[i].GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }
}