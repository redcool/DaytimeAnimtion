using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PowerUtilities
{
    /// <summary>
    /// DayTimeAnimation Update Weather
    /// </summary>
    public class DayTimePowerLitWeatherUpdate : MonoBehaviour
    {
        [Tooltip("Weather changed interval hours")]
        public int weatherChangeIntervalHour = 8;

        [Tooltip("current PowerLitWeatherControl")]
        public PowerLitWeatherControl weatherControl;

        [Tooltip("Weather list")]
        public List<PowerLitWeatherSettings> weatherSettingsList = new ();

        [Tooltip("current weather settings")]
        [EditorSettingSO]
        public PowerLitWeatherSettings curWeatherSettings;

        UnityEngine.Coroutine lastCoroutine;

        private void OnEnable()
        {
            DayTimeAnimationDriver.OnHourChanged -= OnHourChanged;
            DayTimeAnimationDriver.OnHourChanged += OnHourChanged;

            if(!weatherControl)
            {
                weatherControl = GetComponent<PowerLitWeatherControl>();
            }

            CalcWeatherSettingListProbabilityRange();
        }

        private void OnDisable()
        {
            DayTimeAnimationDriver.OnHourChanged -= OnHourChanged;
        }

        void OnHourChanged(int hour)
        {
            if(DayTimeAnimationDriver.Instance.totalHours % weatherChangeIntervalHour == 0)
            {
                var selectedWeather = SelectRandomWeather();
                Debug.Log($"change a weather type {selectedWeather}");
                if (selectedWeather == curWeatherSettings)
                {
                    // same weather, do nothing
                    return;
                }

                curWeatherSettings = selectedWeather;

                if (curWeatherSettings)
                    ChangeWeather();
            }
        }

        public PowerLitWeatherSettings SelectRandomWeather()
        {
            var sum = weatherSettingsList.Sum(setting => setting.probability);
            var randomValue = Random.Range(0, sum);
            Debug.Log(randomValue);

            foreach (var setting in weatherSettingsList)
            {
                if(setting.probabilityRange.x <= randomValue && setting.probabilityRange.y >= randomValue)
                {
                    return setting;
                }
            }
            return null;
            //return weatherSettingsList
            //    .Where(setting => setting.probabilityRange.x <= randomValue && setting.probabilityRange.y >= randomValue)
            //    .FirstOrDefault();
        }

        /// <summary>
        /// Calculate the probability range of weather settings,
        /// </summary>
        public void CalcWeatherSettingListProbabilityRange()
        {
            weatherSettingsList = weatherSettingsList.Where(setting => setting && setting.probability > 0).ToList();

            
            for (int i = 0; i < weatherSettingsList.Count-1; i++)
            {
                var setting = weatherSettingsList[i];
                var nextSetting = weatherSettingsList[i+1];
                
                if(i == 0)
                {
                    setting.probabilityRange.Set(0, setting.probability);
                }

                nextSetting.probabilityRange.Set(setting.probabilityRange.y, setting.probabilityRange.y + nextSetting.probability);
            }
        }

        public void ChangeWeather()
        {
            if (curWeatherSettings.fadingTime <= 0.01f)
            {
                ChangeWeatherDirect();
            }
            else
            {
                if(lastCoroutine != null)
                {
                    StopCoroutine(lastCoroutine);
                }

                lastCoroutine = StartCoroutine(WaitForChangeWeather());
            }
        }

        void ChangeWeatherDirect()
        {
            ReflectionTools.CopyFieldInfoValues(curWeatherSettings, weatherControl);
        }
        IEnumerator WaitForChangeWeather()
        {
            var startTime = Time.time;
            var rate = 0f;
            while (rate < 1)
            {
                rate = (Time.time - startTime) / curWeatherSettings.fadingTime;
                Debug.Log($"weather fading: {rate}");

                //yield return null;
                ReflectionTools.CopyFieldInfoValues(curWeatherSettings, weatherControl,onSetValue: (readValue, writeValue) =>
                {
                    if (readValue is float readFloat && writeValue is float writeFloat)
                    {
                        return Mathf.Lerp(writeFloat, readFloat, rate);
                    }else if (readValue is Color readColor && writeValue is Color writeColor)
                    {
                        return Color.Lerp(writeColor, readColor, rate);
                    }
                    else if (readValue is Vector4 readVector4 && writeValue is Vector4 writeVector4)
                    {
                        return Vector4.Lerp(writeVector4, readVector4, rate);
                    }
                    else if (readValue is Vector3 readVector3 && writeValue is Vector3 writeVector3)
                    {
                        return Vector3.Lerp(writeVector3, readVector3, rate);
                    }
                    else if (readValue is Vector2 readVector2 && writeValue is Vector2 writeVector2)
                    {
                        return Vector2.Lerp(writeVector2, readVector2, rate);
                    }

                    // default
                    return writeValue;
                });

                yield return 0;
            }

            lastCoroutine = null;
            Debug.Log($"weather fading finish");
        }
    }
}
