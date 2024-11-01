using System.Collections.Generic;
using UnityEngine;

public class ToolsPanel : BaseDragPanel
{
    public int Sel = 0;

    public override void Awake()
    {
        base.Awake();
    }

    public static ToolsPanel Instance = null;
    public void OnEnable()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDefault();

        RepresentButtons();
    }

    public void LoadDefault()
    {
        StepGroup.Instance.HideAll();
        ShapeGroup.Instance.DestroyAll();
        ShapeGroup.Instance.Load("MH2JYUQKTEMTEAABAAAAAEI8_4");
        ViewPanelController.Instance.RestoreFromCameraViews();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RepresentButtons()
    {
        for(int i=0;i<transform.childCount;i++)
        {
            transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = Vector2.one * 60.0f;
        }
        transform.GetChild(Sel).GetComponent<RectTransform>().sizeDelta = Vector2.one * 72.0f;
    }




    public void OnToolsButton(int index)
    {
        Sel = index;

        RepresentButtons();

        //if (index==0)
        //{

        //}
        //else if (index == 1)
        //{
        //    ViewPanelController.Instance.RestoreFromCameraViews();
        //}
        //else if (index == 2)
        //{
        //    LoadDefault();
        //}
        //else if (index == 3)
        //{
        //    LoadDefault();
        //}

    }
}
