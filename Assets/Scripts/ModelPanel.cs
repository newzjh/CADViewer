using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPanel : MonoBehaviour
{
    private GameObject stepgroup = null;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnModelButton(int index)
    {
        ShapeGroup.Instance.DestroyAll();
        StepGroup.Instance.HideAll();
        StepGroup.Instance.Show(index);
        ViewPanelController.Instance.ShowSVG(false);
        ViewPanelController.Instance.RestoreFromCameraViews();
    }
}
