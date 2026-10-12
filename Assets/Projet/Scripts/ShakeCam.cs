using UnityEngine;
using Unity.Cinemachine;

public class ShakeCam : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource _impulseSource;

    public void Run()
    {
        _impulseSource.GenerateImpulse();
    }
}
