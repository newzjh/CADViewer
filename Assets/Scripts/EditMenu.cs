using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEditMenu()
    {
        int sel = GetComponent<Dropdown>().value;
        Debug.Log("OnEditMenu:"+sel);

        if (sel==1)
        {
            StepGroup.Instance.HideAll();
            ShapeGroup.Instance.DestroyAll();
            ShapeGroup.Instance.Load("MH2JYUQKTEMTEAABAAAAAEI8_4");
            ViewPanelController.Instance.RestoreFromCameraViews();
        }
        else if (sel==2)
        {
        }
    }

}
