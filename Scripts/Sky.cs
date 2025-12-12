/* 
 *  Copyright © yiroth, all right reserved 
 *  Creation date: 2025-11-23
 *  Purpose: Sky component to manage time of day and visual updates
*/

using UnityEngine;
using System.Collections;
using LibYiroth;

namespace LibYiroth.Celestial
{
    public class Sky : MonoBehaviour
    {
        [Header("Time of Day")]
        
        [Min(0)]
        public int year;
        [Range(1, 13)]
        public int month;
        [Range(1, 28)]
        public int day;
        [Range(0, 59)]
        public int hour;
        [Range(0, 59)]
        public int minute;
        [Range(0, 59)]
        public int second;

        [Header("Settings")]
        
        public bool shouldUpdateVisuals = true;
        public bool shouldUpdateTime = true;
        public Helper.ETimeRate visualUpdateRate = Helper.ETimeRate.TimeRateFPS30;
        public Helper.ETimeRate timeUpdateRate = Helper.ETimeRate.TimeRateFPS30;
        public float realtimeMinutesPerCycle = 120.0f;

        private Data.Date _date;
        private Data.Time _time;
        private float _totalSeconds = 0;

        private void Start()
        {
            if(!CheckSkyValidation())
            {
                Debug.LogError("Sky Component: Sky validation failed. Disabling component.");
                this.enabled = false;
                return;
            }

            if (!(GetRealtimeMinutesPerCycle() > 0.0f))
            {
                Debug.LogError("Sky Component: RealtimeMinutesPerCycle must be greater than zero. Disabling component.");
                this.enabled = false;
                return;
            }

            StartCoroutine(UpdateVisuals());
            StartCoroutine(UpdateTime());
            
            _date = new Data.Date(year, month, day);
            _time = new Data.Time(hour, minute, second);
        }

        private IEnumerator UpdateVisuals()
        {
            while (shouldUpdateVisuals)
            {
                // Update visual elements here
                float mSolarHour = (float)_time.GetTotalSeconds() / 3600.0f;

                yield return new WaitForSeconds(Helper.TimeRate.GetTimeRateValue(visualUpdateRate));
            }
        }

        private IEnumerator UpdateTime()
        {
            while (shouldUpdateTime)
            {
                float mRealTimeSeconds = GetRealtimeMinutesPerCycle() * 60.0f;
	            float mAddition = (86400.0f / mRealTimeSeconds) / 24.0f;

	            _totalSeconds += mAddition;
	            if (_totalSeconds >= 86400.0f)
	            {
	            	_totalSeconds = 0;

	            	if (_date.GetDays() == 28)
	            	{
	            		_date.SetDays(1);

	            		if (_date.GetMonths() == 13)
	            		{
	            			_date.SetMonths(1);

	            			_date.AddYears();
	            		}
	            		else
	            		{
	            			_date.AddMonths();
	            		}
	            	}
	            	else
	            	{
	            		_date.AddDays();
	            	}
	            }

	            Data.Time tod = Helper.TimeRate.GetTimeFromSeconds((int)_totalSeconds);

                _time.SetHours(tod.GetHours());
                _time.SetMinutes(tod.GetMinutes());
                _time.SetSeconds(tod.GetSeconds());

                yield return new WaitForSeconds(Helper.TimeRate.GetTimeRateValue(timeUpdateRate));
            }
        }

        private bool CheckSkyValidation()
        {
            // TODO: Check sky components and values in here
            return true;
        }
        
	    public float GetRealtimeMinutesPerCycle()
        {
            return realtimeMinutesPerCycle;
        }
        
        public Data.Date GetDate()
        {
            return _date;
        }
        
        public Data.Time GetTime()
        {
            return _time;
        }
    }
}
