using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseDragPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected RectTransform m_RT;
    protected Vector3 startpos = Vector3.zero;
    protected Vector3 endpos = Vector3.zero;

    public virtual void Awake()
    {
        m_RT = gameObject.GetComponent<RectTransform>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startpos = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out startpos);
        m_RT.position = startpos;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out pos);
        m_RT.position = pos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        endpos = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RT, eventData.position, eventData.enterEventCamera, out endpos);
        m_RT.position = endpos;
    }
}