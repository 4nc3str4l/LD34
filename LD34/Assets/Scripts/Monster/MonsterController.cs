using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : Entity
{
    private Image _healthOverlay;
    private bool _lerping = false;
    private float _lerpingStart = 0;
    private Color _lerpFrom;
    private Color _lerpTo;

    private const float LERPING_TIME = 5f;

    void Start()
    {
        _healthOverlay = GameObject.Find("GUI/HealthOverlay").GetComponent<Image>();
    }

    void Update()
    {
        if (_lerping)
        {
            if (Time.time - _lerpingStart < LERPING_TIME)
            {
                _healthOverlay.color = Color.Lerp(_lerpFrom, _lerpTo, LERPING_TIME);
            }
            else
            {
                _lerping = false;
            }
        }
    }

    protected override void OnDamaged()
    {
        _lerpFrom = _healthOverlay.color;
        _lerpTo = Color.red;
        _lerpTo.a = 1 - Health / 100f;
        _lerping = true;
        _lerpingStart = Time.time;
    }

    protected override void OnRestored()
    {
        _lerpFrom = _healthOverlay.color;
        _lerpTo = Color.clear;
        _lerpTo.a = 1 - Health / 100f;
        _lerping = true;
        _lerpingStart = Time.time;
    }
}
