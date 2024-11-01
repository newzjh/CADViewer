using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

public class ViewPanelController : MonoBehaviour
{
    public GameObject leftView;
    public GameObject topView;
    public GameObject frontView;
    public GameObject orthView;

    public Camera leftCam;
    public Camera topCam;
    public Camera frontCam;
    public Camera orthCam;

    public static ViewPanelController Instance = null;

    public void Awake()
    {
        Instance = this;
        Reset();
    }

    public void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reset()
    {
        RepresentOrthView();
        RepresentLeftView();
        RepresentTopView();
        RepresentFrontView();
    }

    bool bToggleOrth = false;
    public void OnToggleOrthView()
    {
        bToggleOrth = !bToggleOrth;
        RepresentOrthView();
    }
    private void RepresentOrthView()
    { 
        RectTransform rt = orthView.GetComponent<RectTransform>();
        if (bToggleOrth)
        {
            frontView.SetActive(false);
            topView.SetActive(false);
            leftView.SetActive(false);
            rt.sizeDelta = new Vector2(1280, 640);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            frontView.SetActive(true);
            topView.SetActive(true);
            leftView.SetActive(true);
            rt.sizeDelta = new Vector2(640, 320);
            rt.anchoredPosition = new Vector2(320, -160);
        }
    }

    public void ShowSVG(bool show)
    {
        leftView.GetComponentInChildren<SVGImage>(true).enabled = show;
        topView.GetComponentInChildren<SVGImage>(true).enabled = show;
        frontView.GetComponentInChildren<SVGImage>(true).enabled = show;
        leftView.GetComponentInChildren<RawImage>(true).enabled = !show;
        topView.GetComponentInChildren<RawImage>(true).enabled = !show;
        frontView.GetComponentInChildren<RawImage>(true).enabled = !show;
    }

    bool bToggleLeft = false;
    public void OnToggleLeftView()
    {
        bToggleLeft = !bToggleLeft;
        RepresentLeftView();
    }
    private void RepresentLeftView()
    { 
        RectTransform rt = leftView.GetComponent<RectTransform>();
        if (bToggleLeft)
        {
            frontView.SetActive(false);
            topView.SetActive(false);
            orthView.SetActive(false);
            rt.sizeDelta = new Vector2(1280, 640);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            frontView.SetActive(true);
            topView.SetActive(true);
            orthView.SetActive(true);
            rt.sizeDelta = new Vector2(640, 320);
            rt.anchoredPosition = new Vector2(-320, -160);
        }
        leftView.GetComponent<SingleViewPanel>().RepresentSVG();
    }

    bool bToggleFront = false;
    public void OnToggleFrontView()
    {
        bToggleFront = !bToggleFront;
        RepresentFrontView();
    }
    private void RepresentFrontView()
    { 
        RectTransform rt = frontView.GetComponent<RectTransform>();
        if (bToggleFront)
        {
            leftView.SetActive(false);
            topView.SetActive(false);
            orthView.SetActive(false);
            rt.sizeDelta = new Vector2(1280, 640);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            leftView.SetActive(true);
            topView.SetActive(true);
            orthView.SetActive(true);
            rt.sizeDelta = new Vector2(640, 320);
            rt.anchoredPosition = new Vector2(320, 160);
        }
        frontView.GetComponent<SingleViewPanel>().RepresentSVG();
    }

    bool bToggleTop = false;
    public void OnToggleTopView()
    {
        bToggleTop = !bToggleTop;
        RepresentTopView();
    }
    private void RepresentTopView()
    { 
        RectTransform rt = topView.GetComponent<RectTransform>();
        if (bToggleTop)
        {
            leftView.SetActive(false);
            frontView.SetActive(false);
            orthView.SetActive(false);
            rt.sizeDelta = new Vector2(1280, 640);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            leftView.SetActive(true);
            frontView.SetActive(true);
            orthView.SetActive(true);
            rt.sizeDelta = new Vector2(640, 320);
            rt.anchoredPosition = new Vector2(-320, 160);
        }
        topView.GetComponent<SingleViewPanel>().RepresentSVG();
    }


    public void RestoreFromCameraViews()
    {
        leftCam.GetComponentInParent<SingleView>(true).RenderSingleView();
        topCam.GetComponentInParent<SingleView>(true).RenderSingleView();
        frontCam.GetComponentInParent<SingleView>(true).RenderSingleView();
        orthCam.GetComponentInParent<SingleView>(true).RenderSingleView();
    }

    public void RenderOrthView()
    {
        orthCam.GetComponentInParent<SingleView>(true).RenderSingleView();
    }

    
}
