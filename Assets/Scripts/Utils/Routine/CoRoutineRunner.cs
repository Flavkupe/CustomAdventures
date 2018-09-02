using UnityEngine;

public class CoRoutineRunner : MonoBehaviour
{
    private static CoRoutineRunner _instance;

    public static CoRoutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Utils.InstantiateOfType<CoRoutineRunner>();
            }

            return _instance;
        }
    }
}
