using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Tooltip("将子Transform拖到场景片段的末端")]
    public List<Transform> spawnPointsList = new List<Transform>();
    public EnvironmentController environmentController2;
    public EnvironmentController environmentController3;

    private void Start()
    {
        environmentController2.gameObject.SetActive(false);
        environmentController3.gameObject.SetActive(false);
    }
}
