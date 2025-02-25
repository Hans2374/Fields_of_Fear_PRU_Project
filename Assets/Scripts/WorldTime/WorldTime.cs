using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChanged;

    [SerializeField]
    private float _dayLength;

    [SerializeField]
    private TimeSpan _startTime = new TimeSpan(6, 0, 0);

    private TimeSpan _currentTime;
    private float _minuteLength => _dayLength / WorldTimeConstants.MinutesInDay;

    private void Start()
    {
        _currentTime = _startTime;
        StartCoroutine(AddMinute());
    }
    private IEnumerator AddMinute()
    {
        _currentTime += TimeSpan.FromMinutes(1);
        WorldTimeChanged?.Invoke(this, _currentTime);
        yield return new WaitForSeconds(_minuteLength);
        StartCoroutine(AddMinute());
    }
}
