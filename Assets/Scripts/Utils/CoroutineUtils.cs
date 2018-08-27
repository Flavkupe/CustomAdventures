using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class CoroutineUtils
    {
        public static IEnumerator WaitUntil(Func<bool> evaluation)
        {
            while (!evaluation())
            {
                yield return null;
            }
        }
    }
}
