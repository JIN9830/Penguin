using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static readonly Dictionary<float, WaitForSeconds> m_WaitForSecondsDic = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitForSecond(float waitTime)
    {
        WaitForSeconds wfs;

        if(m_WaitForSecondsDic.TryGetValue(waitTime, out wfs))
        {
            return wfs;
        }
        else
        {
            wfs = new WaitForSeconds(waitTime);
            m_WaitForSecondsDic.Add(waitTime, wfs);
            return wfs;
        }
    }
}
