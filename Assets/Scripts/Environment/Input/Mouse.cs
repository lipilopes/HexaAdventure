using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour {
/*
	void Update ()
    {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit; 

		if (Physics.Raycast (ray, out hit)) 
		{
			GameObject hitObject = hit.collider.transform.gameObject;
			
			if (Input.GetMouseButtonDown (0)) 
			{
				CheckHome (hitObject,hitObject.GetComponentInChildren<HexManager>().x,hitObject.GetComponentInChildren<HexManager>().y);
			}
		}
	}

    public void CheckHome(GameObject grid, int x, int y, int casas = 1)
    {
        CheckGrid check = (CheckGrid)FindObjectOfType(typeof(CheckGrid));

        print(grid.name);

        check.CheckWalk(x, y);

     
          #region Check Horizontal <->
            if (grid = GameObject.Find("Hex" + (x + casas) + "x" + y))
                check.CheckWalk(x + casas, y);
            if (grid = GameObject.Find("Hex" + (x - casas) + "x" + y))
                check.CheckWalk(x - casas, y);
            #endregion

          #region Check  Vertical Left 
            if (y % 2 == 1)// Caso impar
            {
                if (grid = GameObject.Find("Hex" + x + "x" + (y + casas)))
                    check.CheckWalk(x, y + casas);
                if (grid = GameObject.Find("Hex" + x + "x" + (y - casas)))
                    check.CheckWalk(x, y - casas);
            }
            if (y % 2 == 0)// Caso casa == 0
            {
                if (grid = GameObject.Find("Hex" + (x - casas) + "x" + (y + casas)))
                    check.CheckWalk(x - casas, y + casas);
                if (grid = GameObject.Find("Hex" + (x - casas) + "x" + (y - casas)))
                    check.CheckWalk(x - casas, y - casas);
            }
            #endregion

          #region check Vertical Right ->
            if (y % 2 == 1)// Caso impar
            {
                if (grid = GameObject.Find("Hex" + (x + casas) + "x" + (y + casas)))
                    check.CheckWalk(x + casas, y + casas);
                if (grid = GameObject.Find("Hex" + (x + casas) + "x" + (y - casas)))
                    check.CheckWalk(x + casas, y - casas);
            }
            if (y % 2 == 0)// Caso casa == 0
            {
                if (grid = GameObject.Find("Hex" + (x) + "x" + (y + casas)))
                    check.CheckWalk(x, y + casas);
                if (grid = GameObject.Find("Hex" + (x) + "x" + (y - casas)))
                    check.CheckWalk(x, y - casas);
            }
        #endregion
    }
    */

}
