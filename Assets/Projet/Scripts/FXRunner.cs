using UnityEngine;

public class FXRunner : MonoBehaviour
{
    [SerializeField] GameObject _entity;
    [SerializeField] GameObject _fxPrefab;
    [SerializeField] Vector3 _offset;

    public void RunFX()
    {
        if(_fxPrefab != null)
        {
            Vector3 position = _entity.transform.position + _entity.transform.TransformDirection(_offset);
            GameObject fxInstance = Instantiate(_fxPrefab, position, Quaternion.identity);
            ParticleSystem ps = fxInstance.GetComponent<ParticleSystem>();
            if(ps != null)
            {
                _fxPrefab.SetActive(true);
                ps.Play();
                Destroy(fxInstance, ps.main.duration);
            }
            else
            {
                Destroy(fxInstance, 2f);
            }
        }
    }
}
