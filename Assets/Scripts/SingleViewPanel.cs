using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SingleViewPanel))]
class SingleViewPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SingleViewPanel com = (SingleViewPanel)target;

        if (GUILayout.Button("AddToList"))
        {
            string postfix = "_s";
            if (com.name.StartsWith("Left"))
                postfix = "_s";
            else if (com.name.StartsWith("Top"))
                postfix = "_t";
            else if (com.name.StartsWith("Front"))
                postfix = "_f";
            com.AddSelectList(postfix);
        }
    }
}
#endif

public class SingleViewPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform m_RT;
    private Vector2 startpos = Vector2.zero;
    private Vector2 endpos = Vector2.zero;

    // Start is called before the first frame update
    void Awake()
    {
        m_RT = gameObject.GetComponent<RectTransform>();
        //var img = GetComponentInChildren<SVGImage>(true);
        //img.gameObject.AddComponent<SVGImageHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //print("IBeginDragHandler.OnBeginDrag");
        //gameObject.GetComponent<Transform>().position = Input.mousePosition;
        startpos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out startpos);
        //m_RT.position = pos;
        //print("这是实现的拖拽开始接口");
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //print("IDragHandler.OnDrag");
        //虽然用Input.mousePosition可以得到一个2D坐标，不过我们现在需要的是3D坐标，看下面
        //gameObject.GetComponent<Transform>().position = Input.mousePosition;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out pos);

        //m_RT.position = pos;
        //print("拖拽中……");
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //print("IEndDragHandler.OnEndDrag");
        endpos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out endpos);

        Vector2 startpt = startpos / m_RT.sizeDelta.y;
        startpt.y *= -1;
        Vector2 endpt = endpos / m_RT.sizeDelta.y;
        endpt.y *= -1;

        var sceneNode = new SceneNode();

        //ParseID(node, sceneNode);
        //ParseOpacity(sceneNode);
        //sceneNode.Transform = SVGAttribParser.ParseTransform(node);
        PathCorner strokeCorner = PathCorner.Tipped;
        PathEnding strokeEnding = PathEnding.Chop;
        //var stroke = ParseStrokeAttributeSet(node, out strokeCorner, out strokeEnding);

        var path = new Shape();
        path.PathProps = new PathProperties() { Stroke = new Stroke(), Head = strokeEnding, Tail = strokeEnding };
        path.PathProps.Stroke.Color = Color.black;
        path.PathProps.Stroke.HalfThickness = 0.0025f;
        path.Contours = new BezierContour[] {
                new BezierContour() { Segments = VectorUtils.BezierSegmentToPath(VectorUtils.MakeLine(startpt, endpt)) }
            };
        sceneNode.Shapes = new List<Shape>(1);
        sceneNode.Shapes.Add(path);
        currentsvg.Root.Children.Add(sceneNode);

        RepresentSVG();

        ShapeGroup.Instance.DestroyAll();
        ShapeGroup.Instance.GenerateFromSVGs();
        ViewPanelController.Instance.RenderOrthView();

        //m_RT.position = pos;
        //print("实现的拖拽结束接口");
    }

    public void ToggleSelectList()
    {
        ScrollRect sr = GetComponentInChildren<ScrollRect>(true);
        sr.gameObject.SetActive(!sr.gameObject.activeSelf);
    }

    [System.NonSerialized]
    public Unity.VectorGraphics.Scene currentsvg;

    public void SetSVG(Unity.VectorGraphics.Scene svg)
    {
        currentsvg = svg;
        RepresentSVG();
    }

    public void RepresentSVG()
    {
        if (currentsvg == null)
            return;

        var tessOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100.0f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        var geoms = VectorUtils.TessellateScene(currentsvg, tessOptions);

        // Build a sprite with the tessellated geometry.

        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(-float.MaxValue, -float.MaxValue);
        foreach (var geo in geoms)
        {
            foreach (var v in geo.Vertices)
            {
                Vector2 vv = geo.WorldTransform * v;
                min = Vector2.Min(min, vv);
                max = Vector2.Max(max, vv);
            }
        }

        Rect rc = new Rect(min.x, min.y, max.x-min.x, max.y-min.y);
        var sprite = VectorUtils.BuildSprite(geoms, 10.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        var img = GetComponentInChildren<SVGImage>();
        var rt = img.GetComponent<RectTransform>();
        rt.sizeDelta = rc.size * 320 * m_RT.sizeDelta.y / 320.0f;
        Vector2 center = rc.center;
        center.y = -center.y;
        rt.localPosition = center * 320 * m_RT.sizeDelta.y / 320.0f;
        img.sprite = sprite;

    }

#if UNITY_EDITOR

    public void AddSelectList(string postfix)
    {
        GameObject SelectListButton = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UI/SelectListButton.prefab");
        if (SelectListButton == null)
            return;

        ScrollRect sr = GetComponentInChildren<ScrollRect>(true);
        string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Resources/svgs");
        if (files != null)
        {
            int index = 0;
            foreach (string file in files)
            {
                var filename = System.IO.Path.GetFileNameWithoutExtension(file);
                if (!filename.EndsWith(postfix))
                    continue;
                GameObject subgo = GameObject.Instantiate(SelectListButton);
                subgo.name = filename;
                RectTransform rt = subgo.GetComponent<RectTransform>();
                subgo.transform.parent = sr.content;
                rt.localScale = Vector3.one;
                Vector2 pos = Vector2.zero;
                pos.x = index * 100 + 50;
                rt.anchoredPosition = pos;
                subgo.AddComponent<SVGSelectButton>();
                index++;
            }
            sr.content.sizeDelta = new Vector2(index * 100, 100);
        }
        
    }
#endif
}
