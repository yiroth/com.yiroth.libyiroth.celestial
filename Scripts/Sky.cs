/* 
 *  Copyright © yiroth, all right reserved 
 *  Creation date: 2025-11-23
 *  Purpose: Sky component to manage time of day and visual updates
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibYiroth;

namespace LibYiroth.Celestial
{
    public class Sky : MonoBehaviour
    {
        [Header("Time of Day")]
        
        [Min(0)]
        public int year;
        [Range(1, 12)]
        public int month;
        [Range(1, 31)]
        public int day;
        [Range(0, 23)]
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

        private const float secondsPerDay = 86400.0f;
        private static readonly HashSet<int> SelectiveMonth = new() { 1, 3, 5, 7, 8, 10, 12 };

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
                float mSolarHour = (float)_time.GetTotalSeconds() / 3600.0f;

                // TODO: Curve based visual update here

                yield return new WaitForSeconds(Helper.TimeRate.GetTimeRateValue(visualUpdateRate));
            }
        }

        private IEnumerator UpdateTime()
        {
            while (shouldUpdateTime)
            {
                float mRealTimeSeconds = GetRealtimeMinutesPerCycle() * 60.0f;
	            float mAddition = (secondsPerDay / mRealTimeSeconds) / 24.0f;

	            _totalSeconds += mAddition;
	            if (_totalSeconds >= secondsPerDay)
	            {
	            	_totalSeconds = 0;
                    
                    if (_date.GetDays() == (SelectiveMonth.Contains(_date.GetMonths()) ? 31 : 30))
                    {
	            		_date.SetDays(1);

	            		if (_date.GetMonths() == 12)
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
