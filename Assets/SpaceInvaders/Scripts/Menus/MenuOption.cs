using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private Button _button;
    private GameObject _selectionIndicator;
    private RectTransform _buttonTransform;
    private RectTransform _selectionIndicatorTransform;

    [SerializeField] private float _indicatorSpacing = 20f;

    private void Start()
    {
        _button = GetComponent<Button>();
        _buttonTransform = GetComponent<RectTransform>();
        _selectionIndicator = GameObject.FindGameObjectWithTag("Selection Pointer");
        if (_selectionIndicator != null)
            _selectionIndicatorTransform = _selectionIndicator.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_selectionIndicator != null) {
            float buttonMinX = _buttonTransform.TransformPoint(_buttonTransform.rect.min).x;
            float indicatorWidth = _selectionIndicatorTransform.rect.width;
            _selectionIndicator.transform.position = new Vector3(buttonMinX - indicatorWidth / 2 - _indicatorSpacing, 
                _buttonTransform.position.y, _buttonTransform.position.z);
        }
    }
}
