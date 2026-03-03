using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    #region Data
   
    MeshRenderer[] meshRenderers;
    float initialSpeed = -1f;
    float currentSpeed;
    [SerializeField] float duration;
    [SerializeField] float lastValue = 0.6f;
    [SerializeField] Material dissolveMaterial;
    Color baseColor;
    #endregion

    #region Mono
    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

    }
    private void Start()
    {
        currentSpeed = initialSpeed;
        foreach (Renderer renderer in meshRenderers)
        {
            if(renderer.material.HasProperty("_Color"))
            {
                baseColor = renderer.material.color;
                renderer.material = dissolveMaterial;
                renderer.material.SetColor("_Base_Color", baseColor);
            }
        }
    }
    #endregion 
    #region Methods
    public void StartDissolve()
    {
        StartCoroutine(ApplyDissolveMaterial());
    }
    public IEnumerator ApplyDissolveMaterial()
    {
        float timer = 0.0f;
        float startSpeed = currentSpeed;
        while(timer<duration)
        {
            currentSpeed = Mathf.Lerp(startSpeed, lastValue, timer / duration);
            foreach (Renderer renderer in meshRenderers)
            {
                renderer.material.SetFloat("_Speed", currentSpeed);
            }
            yield return null;
            timer+= Time.deltaTime; 
        }
        currentSpeed = lastValue;
        foreach (Renderer renderer in meshRenderers)
        {
            renderer.material.SetFloat("_Speed", currentSpeed);
        }
    }
    #endregion
}
