using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurbidCurrent
{
    public class DirectorManager : Single<DirectorManager>
    {
        DirectorBase m_currentDirecor;

        public void ChangeDirector(DirectorBase curDiret)
        {
            if (m_currentDirecor != null)
            {
                m_currentDirecor.ExitDirector();

            }
            m_currentDirecor = curDiret;
            m_currentDirecor.EnterDierctor();
        }
    }
}
