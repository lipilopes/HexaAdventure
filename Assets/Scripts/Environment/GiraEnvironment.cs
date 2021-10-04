using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiraEnvironment : MonoBehaviour
{
    [SerializeField] int Randomspin;


	void Start ()
    {
        transform.Rotate(0, (Random.Range(Randomspin / -1, Randomspin) + Time.deltaTime), 0);
    }
	
}
