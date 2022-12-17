using System;
using UnityEngine;

namespace Slipstream
{
	public class SlipstreamContentRunBehaviour : MonoBehaviour
	{
		public bool SetHook(ref bool active, bool condition = true)
		{
			if(!active && condition)
            {
				active = true;
				return true;
            }
			return false;
		}
		public bool UnsetHook(ref bool active)
		{
            if (active)
            {
				active = false;
				return true;
            }
			return false;
		}
	}
}