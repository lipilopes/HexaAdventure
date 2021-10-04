using UnityEngine;
using System.Collections.Generic;

public class CheckGrid : MonoBehaviour
{
    public static CheckGrid Instance;

    private void Awake()
    {
        if (Instance==null)
            Instance = this;
        else
            Destroy(this);
    }

    public bool[] floorFree;

    int X, Y;

    RespawMob listRespaw;

    public void SetUp()
    {
        X = (int)GridMap.Instance.Width;
        Y = (int)GridMap.Instance.Height;

        listRespaw = GetComponent<RespawMob>();

        floorFree = new bool[X * Y];

        Check();
    }

    public void Check()
    {
        foreach (var g in GridMap.Instance.hexManager)
        {
            floorFree[g.x * X + g.y] = g.free;
        }
    }

    public HexManager HexGround(int x, int y)
    {
        if (GridMap.Instance.hexManager.Count != 0)
            foreach (var hex in GridMap.Instance.hexManager)
            {
                if (hex.x == x && hex.y == y)
                {
                    return hex;
                }
            }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="range">Distance For Check</param>
    /// <param name="colore">Color Ground</param>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    public List<HexManager> RegisterRadioHex(int X, int Y, int range = 1, bool colore = true, int color = 3,bool onlyFree=false)
    {    
        List<HexManager> Rh = new List<HexManager>();

        if (HexGround((X) , Y))
        {
            Rh.Add(HexGround((X) , (Y)));
        }

        #region Range 1
        if (range >= 1)
        {
            #region Check Horizontal <->
            if (HexGround((X + 1), Y))
            {
                Rh.Add(HexGround((X + 1), (Y)));
            }

            if (HexGround((X - 1), Y))
            {
                Rh.Add(HexGround((X - 1), (Y)));
            }

            #endregion
            #region vertical^v         
            if (HexGround((X), (Y + 1)))
            {
                Rh.Add(HexGround((X), (Y + 1)));
            }

            if (HexGround((X), (Y - 1)))
            {
                Rh.Add(HexGround((X), (Y - 1)));
            }

            #endregion

            #region Check  Diagonal Left 
            if (Y % 2 == 1)//Caso impar
            {
                if (HexGround((X), (Y + 1)))
                {
                    Rh.Add(HexGround((X), (Y + 1)));
                }

                if (HexGround((X), (Y - 1)))
                {
                    Rh.Add(HexGround((X), (Y - 1)));
                }
            }
            if (Y % 2 == 0)//Caso casa par
            {
                if (HexGround((X - 1), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 1)));
                }

                if (HexGround((X - 1), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 1)));
                }
            }
            #endregion
            #region check Diagonal Right ->
            if (Y % 2 == 1) //Caso impar
            {

                if (HexGround((X + 1), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 1)));
                }


                if (HexGround((X + 1), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 1)));
                }

            }
            else  //Caso casa == 0
            {
                if (HexGround((X), (Y + 1)))
                {
                    Rh.Add(HexGround((X), (Y + 1)));
                }

                if (HexGround((X), (Y - 1)))
                {
                    Rh.Add(HexGround((X), (Y - 1)));
                }
            }
            #endregion
        }
        #endregion

        #region Range 2
        if (range >= 2)
        {
            if (Y % 2 == 0)//impar
            {
                #region Superior
                //Diagonal Esquerda
                if (HexGround((X - 1), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y + 2)));
                }
                //Cima
                if (HexGround((X), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X + 0), (Y + 2)));
                }
                //Diagonal Direita
                if (HexGround((X + 1), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y + 2)));
                }
                #endregion

                #region Lado Direito
                //Diagonal Direita lado 
                if (HexGround((X + 1), (Y + 1)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y + 1)));
                }
                //Diagonal Direita lado 
                if (HexGround((X + 2), (Y)) != null)
                {
                    Rh.Add(HexGround((X + 2), (Y + 0)));
                }
                //lado  Direito
                if (HexGround((X + 1), (Y - 1)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y - 1)));
                }
                #endregion

                #region Inferior
                //baixo direito
                if (HexGround((X + 1), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y - 2)));
                }

                //baixo Meio
                if (HexGround((X + 0), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X + 0), (Y - 2)));
                }

                //baixo esquerdo
                if (HexGround((X - 1), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y - 2)));
                }
                #endregion

                #region Lado Esquerdo
                //Diagonal Esquerda inferior 
                if (HexGround((X - 2), (Y - 1)) != null)
                {
                    Rh.Add(HexGround((X - 2), (Y - 1)));
                }
                //Diagonal Direita 
                if (HexGround((X - 2), (Y)) != null)
                {
                    Rh.Add(HexGround((X - 2), (Y + 0)));
                }
                //lado  Esquerdo superior
                if (HexGround((X - 2), (Y + 1)) != null)
                {
                    Rh.Add(HexGround((X - 2), (Y + 1)));
                }
                #endregion
            }
            else
            {
                #region Superior
                //Diagonal Esquerda
                if (HexGround((X - 1), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y + 2)));
                }
                //Cima
                if (HexGround((X), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X + 0), (Y + 2)));
                }
                //Diagonal Direita
                if (HexGround((X + 1), (Y + 2)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y + 2)));
                }
                #endregion

                #region Lado Direito
                //Diagonal Direita superior 
                if (HexGround((X + 2), (Y + 1)) != null)
                {
                    Rh.Add(HexGround((X + 2), (Y + 1)));
                }

                //Diagonal Direita meio 
                if (HexGround((X + 2), (Y + 0)) != null)
                {
                    Rh.Add(HexGround((X + 2), (Y + 0)));
                }

                //Diagonal Direita inferior 
                if (HexGround((X + 2), (Y - 1)) != null)
                {
                    Rh.Add(HexGround((X + 2), (Y - 1)));
                }
                #endregion

                #region Inferior
                //baixo direito
                if (HexGround((X + 1), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X + 1), (Y - 2)));
                }

                //baixo meio
                if (HexGround((X + 0), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X + 0), (Y - 2)));
                }

                //baixo esquerdo
                if (HexGround((X - 1), (Y - 2)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y - 2)));
                }
                #endregion

                #region Lado Esquerdo
                //Diagonal Esquerda inferior 
                if (HexGround((X - 1), (Y - 1)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y - 1)));
                }
                //Diagonal Esquerda meio 
                if (HexGround((X - 2), (Y + 0)) != null)
                {
                    Rh.Add(HexGround((X - 2), (Y + 0)));
                }
                //Diagonal Esquerda superior 
                if (HexGround((X - 1), (Y + 1)) != null)
                {
                    Rh.Add(HexGround((X - 1), (Y + 1)));
                }
                #endregion
            }
        }
        #endregion

        #region Range 3
        if (range >= 3)
        {
            #region Falhas 3 Par
            if (Y % 2 == 0)//Caso par
            {
                //lefts
                if (HexGround((X - 2), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 3)));
                }

                if (HexGround((X - 2), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 2)));
                }

                if (HexGround((X - 3), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 1)));
                }


                if (HexGround((X - 3), (Y)))
                {
                    Rh.Add(HexGround((X - 3), (Y)));
                }

                if (HexGround((X - 3), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 1)));
                }

                if (HexGround((X - 2), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 2)));
                }


                //rights
                if (HexGround((X + 2), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 2)));
                }

                if (HexGround((X + 2), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 1)));
                }

                if (HexGround((X + 3), (Y)))
                {
                    Rh.Add(HexGround((X + 3), (Y)));
                }


                if (HexGround((X + 2), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 1)));
                }

                if (HexGround((X + 2), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 2)));
                }


                //UP
                if (HexGround((X - 2), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 3)));
                }

                if (HexGround((X - 1), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 3)));
                }

                if (HexGround((X), (Y + 3)))
                {
                    Rh.Add(HexGround((X), (Y + 3)));
                }

                if (HexGround((X + 1), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 3)));
                }


                //downs
                if (HexGround((X + 1), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 3)));
                }

                if (HexGround((X), (Y - 3)))
                {
                    Rh.Add(HexGround((X), (Y - 3)));
                }

                if (HexGround((X - 1), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 3)));
                }

            }

            #endregion
            #region Falhar 3 impar
            if (Y % 2 == 1)//Caso Impar
            {
                //up
                if (HexGround((X - 1), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 3)));
                }

                if (HexGround((X), (Y + 3)))
                {
                    Rh.Add(HexGround((X), (Y + 3)));
                }

                if (HexGround((X + 1), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 3)));
                }

                if (HexGround((X + 2), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 3)));
                }


                //vertical right up
                if (HexGround((X + 2), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 2)));
                }

                if (HexGround((X + 3), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 1)));
                }


                //Right
                if (HexGround((X + 3), (Y)))
                {
                    Rh.Add(HexGround((X + 3), (Y)));
                }


                //vertical right down
                if (HexGround((X + 3), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 1)));
                }

                if (HexGround((X + 2), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 2)));
                }

                if (HexGround((X + 2), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 2)));
                }


                //down
                if (HexGround((X + 2), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 3)));
                }

                if (HexGround((X + 1), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 3)));
                }

                if (HexGround((X), (Y - 3)))
                {
                    Rh.Add(HexGround((X), (Y - 3)));
                }


                //vertical left Down
                if (HexGround((X - 1), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 3)));
                }

                if (HexGround((X - 2), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 2)));
                }

                if (HexGround((X - 2), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 1)));
                }


                //left
                if (HexGround((X - 3), (Y)))
                {
                    Rh.Add(HexGround((X - 3), (Y)));
                }


                //vertical left Up
                if (HexGround((X - 2), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 1)));
                }

                if (HexGround((X - 2), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 2)));
                }


            }
            #endregion
        }
        #endregion

        #region Range 4
        if (range >= 4)
        {
            #region Falhas 4 Par
            //lefts
            if (Y % 2 == 0)
            {
                if (HexGround((X - 3), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 3)));
                }

                if (HexGround((X - 3), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 2)));
                }

                if (HexGround((X - 4), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 1)));
                }


                if (HexGround((X - 1), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 4)));
                }

                if (HexGround((X - 2), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 4)));
                }

                if (HexGround((X - 4), (Y)))
                {
                    Rh.Add(HexGround((X - 4), (Y)));
                }

                if (HexGround((X - 4), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 1)));
                }

                if (HexGround((X - 3), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 2)));
                }


                //rights
                if (HexGround((X + 4), (Y)))
                {
                    Rh.Add(HexGround((X + 4), (Y)));
                }

                if (HexGround((X + 3), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 1)));
                }

                if (HexGround((X + 3), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 2)));
                }

                if (HexGround((X + 2), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 3)));
                }

                if (HexGround((X + 2), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 4)));
                }


                if (HexGround((X + 3), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 1)));
                }

                if (HexGround((X + 3), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 2)));
                }

                if (HexGround((X + 2), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 3)));
                }


                //UP
                if (HexGround((X - 3), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 3)));
                }

                if (HexGround((X - 2), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 3)));
                }

                if (HexGround((X), (Y + 4)))
                {
                    Rh.Add(HexGround((X), (Y + 4)));
                }

                if (HexGround((X + 1), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 4)));
                }


                //downs
                if (HexGround((X - 2), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 4)));
                }

                if (HexGround((X - 1), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 4)));
                }

                if (HexGround((X), (Y - 4)))
                {
                    Rh.Add(HexGround((X), (Y - 4)));
                }

                if (HexGround((X + 1), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 4)));
                }

                if (HexGround((X + 2), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 4)));
                }

            }
            #endregion
            #region Falhar 4 Impar
            if (Y % 2 == 1)//Caso Impar
            {
                //up
                if (HexGround((X - 2), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 4)));
                }

                if (HexGround((X - 1), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 4)));
                }

                if (HexGround((X), (Y + 4)))
                {
                    Rh.Add(HexGround((X), (Y + 4)));
                }

                if (HexGround((X + 1), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 4)));
                }

                if (HexGround((X + 2), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 4)));
                }


                //verical right up
                if (HexGround((X + 3), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 3)));
                }

                /*if (HexGround( (X + 3), (Y + 3)))
                {
                    Rh.Add(HexGround( (X + 3), (Y + 3)));
                }*/

                if (HexGround((X + 3), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 2)));
                }

                if (HexGround((X + 4), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 1)));
                }

                //right
                if (HexGround((X + 4), (Y)))
                {
                    Rh.Add(HexGround((X + 4), (Y)));
                }


                //vertical right down
                if (HexGround((X + 4), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 1)));
                }

                if (HexGround((X + 3), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 2)));
                }

                if (HexGround((X + 3), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 3)));
                }


                //down
                if (HexGround((X + 2), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 4)));
                }

                if (HexGround((X + 1), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 4)));
                }

                if (HexGround((X), (Y - 4)))
                {
                    Rh.Add(HexGround((X), (Y - 4)));
                }

                if (HexGround((X - 1), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 4)));
                }

                if (HexGround((X - 2), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 4)));
                }


                //vertical left down
                if (HexGround((X - 2), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 3)));
                }

                if (HexGround((X - 3), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 2)));
                }

                if (HexGround((X - 3), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 1)));
                }


                //left
                if (HexGround((X - 4), (Y)))
                {
                    Rh.Add(HexGround((X - 4), (Y)));
                }


                //vertical left up
                if (HexGround((X - 3), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 1)));
                }

                if (HexGround((X - 3), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 2)));
                }

                if (HexGround((X - 2), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 3)));
                }
            }
            #endregion
        }
        #endregion

        #region Range 5
        if (range >= 5)
        {
            #region Falhas 5 Par
            if (Y % 2 == 0)//Caso par
            {
                //Down
                if (HexGround((X + 1), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 5)));
                }

                if (HexGround((X), (Y - 5)))
                {
                    Rh.Add(HexGround((X), (Y - 5)));
                }

                if (HexGround((X - 1), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 5)));
                }

                if (HexGround((X - 2), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 5)));
                }

                if (HexGround((X - 3), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 5)));
                }


                //UP
                if (HexGround((X - 2), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 5)));
                }

                if (HexGround((X - 1), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 5)));
                }

                if (HexGround((X), (Y + 5)))
                {
                    Rh.Add(HexGround((X), (Y + 5)));
                }

                if (HexGround((X + 1), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 5)));
                }

                if (HexGround((X + 2), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 5)));
                }


                //vertical right up
                if (HexGround((X + 3), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 4)));
                }

                if (HexGround((X + 3), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 3)));
                }

                if (HexGround((X + 4), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 2)));
                }

                if (HexGround((X + 4), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 1)));
                }


                //right 
                if (HexGround((X + 5), (Y)))
                {
                    Rh.Add(HexGround((X + 5), (Y)));
                }


                //vertical right down
                if (HexGround((X + 4), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 1)));
                }

                if (HexGround((X + 4), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 2)));
                }

                if (HexGround((X + 3), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 2)));
                }

                if (HexGround((X + 3), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 3)));
                }

                if (HexGround((X + 3), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 4)));
                }

                if (HexGround((X + 2), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 5)));
                }


                //vertical left up
                if (HexGround((X - 3), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 5)));
                }

                if (HexGround((X - 3), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 4)));
                }

                if (HexGround((X - 4), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 3)));
                }

                if (HexGround((X - 4), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 2)));
                }

                if (HexGround((X - 5), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 5), (Y + 1)));
                }


                //left
                if (HexGround((X - 5), (Y)))
                {
                    Rh.Add(HexGround((X - 5), (Y)));
                }


                //vertical right down
                if (HexGround((X - 5), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 5), (Y - 1)));
                }

                if (HexGround((X - 4), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 2)));
                }

                if (HexGround((X - 4), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 3)));
                }

                if (HexGround((X - 3), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 4)));
                }

            }
            #endregion
            #region Falhas 5 Impar
            if (Y % 2 == 1)//Caso Impar
            {
                //up
                if (HexGround((X - 2), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 5)));
                }

                if (HexGround((X - 1), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 5)));
                }

                if (HexGround((X), (Y + 5)))
                {
                    Rh.Add(HexGround((X), (Y + 5)));
                }

                if (HexGround((X + 1), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 5)));
                }

                if (HexGround((X + 2), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 5)));
                }

                if (HexGround((X + 3), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 5)));
                }


                //vertical left up
                if (HexGround((X + 3), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 4)));
                }

                if (HexGround((X + 3), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 3)));
                }

                if (HexGround((X + 4), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 3)));
                }

                if (HexGround((X + 4), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 2)));
                }

                if (HexGround((X + 5), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 5), (Y + 1)));
                }


                //left
                if (HexGround((X + 5), (Y)))
                {
                    Rh.Add(HexGround((X + 5), (Y)));
                }


                //vertical left down
                if (HexGround((X + 5), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 5), (Y - 1)));
                }

                if (HexGround((X + 4), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 2)));
                }

                if (HexGround((X + 4), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 3)));
                }

                if (HexGround((X + 3), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 4)));
                }


                //down
                if (HexGround((X + 3), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 5)));
                }

                if (HexGround((X + 2), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 5)));
                }

                if (HexGround((X + 1), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 5)));
                }

                if (HexGround((X), (Y - 5)))
                {
                    Rh.Add(HexGround((X), (Y - 5)));
                }

                if (HexGround((X - 1), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 5)));
                }

                if (HexGround((X - 2), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 5)));
                }


                //vertical left Down
                if (HexGround((X - 3), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 4)));
                }

                if (HexGround((X - 3), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 3)));
                }

                if (HexGround((X - 4), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 2)));
                }

                if (HexGround((X - 4), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 1)));
                }


                //left
                if (HexGround((X - 5), (Y)))
                {
                    Rh.Add(HexGround((X - 5), (Y)));
                }


                //vertical Left Up
                if (HexGround((X - 4), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 1)));
                }

                if (HexGround((X - 4), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 2)));
                }

                if (HexGround((X - 3), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 3)));
                }

                if (HexGround((X - 3), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 4)));
                }
            }
            #endregion
        }
        #endregion

        #region Range 6
        if (range >= 6)
        {
            #region Falhas 6 Par
            if (Y % 2 == 0)//Caso par
            {
                //Down
                if (HexGround((X + 2), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 6)));
                }

                if (HexGround((X + 1), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 6)));
                }

                if (HexGround((X), (Y - 6)))
                {
                    Rh.Add(HexGround((X), (Y - 6)));
                }

                if (HexGround((X - 1), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 6)));
                }

                if (HexGround((X - 2), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 6)));
                }

                if (HexGround((X - 3), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 6)));
                }


                //up
                if (HexGround((X - 3), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 6)));
                }

                if (HexGround((X - 2), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 6)));
                }

                if (HexGround((X - 1), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 6)));
                }

                if (HexGround((X), (Y + 6)))
                {
                    Rh.Add(HexGround((X), (Y + 6)));
                }

                if (HexGround((X + 1), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 6)));
                }

                if (HexGround((X + 2), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 6)));
                }

                if (HexGround((X + 3), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 6)));
                }


                //vertical right up
                if (HexGround((X + 3), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 5)));
                }

                if (HexGround((X + 4), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 4)));
                }

                if (HexGround((X + 4), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 3)));
                }

                if (HexGround((X + 5), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 5), (Y + 2)));
                }

                if (HexGround((X + 5), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 5), (Y + 1)));
                }


                //right 
                if (HexGround((X + 6), (Y)))
                {
                    Rh.Add(HexGround((X + 6), (Y)));
                }


                //vertical right down
                if (HexGround((X + 5), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 5), (Y - 1)));
                }

                if (HexGround((X + 5), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 5), (Y - 2)));
                }

                if (HexGround((X + 4), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 3)));
                }

                if (HexGround((X + 4), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 4)));
                }

                if (HexGround((X + 3), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 5)));
                }

                if (HexGround((X + 3), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 6)));
                }


                //vertical left down
                if (HexGround((X - 4), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 5)));
                }

                if (HexGround((X - 4), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 4)));
                }

                if (HexGround((X - 5), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 5), (Y - 3)));
                }

                if (HexGround((X - 5), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 5), (Y - 2)));
                }

                if (HexGround((X - 6), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 6), (Y - 1)));
                }


                //left
                if (HexGround((X - 6), (Y)))
                {
                    Rh.Add(HexGround((X - 6), (Y)));
                }


                //vertical left Up
                if (HexGround((X - 6), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 6), (Y + 1)));
                }

                if (HexGround((X - 5), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 5), (Y + 2)));
                }

                if (HexGround((X - 5), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 5), (Y + 3)));
                }

                if (HexGround((X - 4), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 4)));
                }

                if (HexGround((X - 4), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 5)));
                }

            }
            #endregion
            #region Falhas 6 Impar
            if (Y % 2 == 1)//Caso Impar
            {
                //up
                if (HexGround((X - 2), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 2), (Y + 6)));
                }

                if (HexGround((X - 1), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 1), (Y + 6)));
                }

                if (HexGround((X), (Y + 6)))
                {
                    Rh.Add(HexGround((X), (Y + 6)));
                }

                //if (HexGround( (X), (Y + 6)))
                //{
                //    Rh.Add(HexGround( (X), (Y + 6)));
                //}

                if (HexGround((X + 1), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 1), (Y + 6)));
                }

                if (HexGround((X + 2), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 2), (Y + 6)));
                }


                //vertical right up
                if (HexGround((X + 3), (Y + 6)))
                {
                    Rh.Add(HexGround((X + 3), (Y + 6)));
                }

                if (HexGround((X + 4), (Y + 5)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 5)));
                }

                if (HexGround((X + 4), (Y + 4)))
                {
                    Rh.Add(HexGround((X + 4), (Y + 4)));
                }

                if (HexGround((X + 5), (Y + 3)))
                {
                    Rh.Add(HexGround((X + 5), (Y + 3)));
                }

                if (HexGround((X + 5), (Y + 2)))
                {
                    Rh.Add(HexGround((X + 5), (Y + 2)));
                }

                if (HexGround((X + 6), (Y + 1)))
                {
                    Rh.Add(HexGround((X + 6), (Y + 1)));
                }


                //right
                if (HexGround((X + 6), (Y)))
                {
                    Rh.Add(HexGround((X + 6), (Y)));
                }


                //vertical right down
                if (HexGround((X + 6), (Y - 1)))
                {
                    Rh.Add(HexGround((X + 6), (Y - 1)));
                }

                if (HexGround((X + 5), (Y - 2)))
                {
                    Rh.Add(HexGround((X + 5), (Y - 2)));
                }

                if (HexGround((X + 5), (Y - 3)))
                {
                    Rh.Add(HexGround((X + 5), (Y - 3)));
                }

                if (HexGround((X + 4), (Y - 4)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 4)));
                }

                if (HexGround((X + 4), (Y - 5)))
                {
                    Rh.Add(HexGround((X + 4), (Y - 5)));
                }


                //down
                if (HexGround((X + 3), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 3), (Y - 6)));
                }

                if (HexGround((X + 2), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 2), (Y - 6)));
                }

                if (HexGround((X + 1), (Y - 6)))
                {
                    Rh.Add(HexGround((X + 1), (Y - 6)));
                }

                if (HexGround((X), (Y - 6)))
                {
                    Rh.Add(HexGround((X), (Y - 6)));
                }

                if (HexGround((X - 1), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 1), (Y - 6)));
                }

                if (HexGround((X - 2), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 2), (Y - 6)));
                }

                if (HexGround((X - 3), (Y - 6)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 6)));
                }


                //vertical left down
                if (HexGround((X - 3), (Y - 5)))
                {
                    Rh.Add(HexGround((X - 3), (Y - 5)));
                }

                if (HexGround((X - 4), (Y - 4)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 4)));
                }

                if (HexGround((X - 4), (Y - 3)))
                {
                    Rh.Add(HexGround((X - 4), (Y - 3)));
                }

                if (HexGround((X - 5), (Y - 2)))
                {
                    Rh.Add(HexGround((X - 5), (Y - 2)));
                }

                if (HexGround((X - 5), (Y - 1)))
                {
                    Rh.Add(HexGround((X - 5), (Y - 1)));
                }


                //left
                if (HexGround((X - 6), (Y)))
                {
                    Rh.Add(HexGround((X - 6), (Y)));
                }


                //verical left up
                if (HexGround((X - 5), (Y + 1)))
                {
                    Rh.Add(HexGround((X - 5), (Y + 1)));
                }

                if (HexGround((X - 5), (Y + 2)))
                {
                    Rh.Add(HexGround((X - 5), (Y + 2)));
                }

                if (HexGround((X - 4), (Y + 3)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 3)));
                }

                if (HexGround((X - 4), (Y + 4)))
                {
                    Rh.Add(HexGround((X - 4), (Y + 4)));
                }

                if (HexGround((X - 3), (Y + 5)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 5)));
                }

                if (HexGround((X - 3), (Y + 6)))
                {
                    Rh.Add(HexGround((X - 3), (Y + 6)));
                }

            }
            #endregion
        }
        #endregion        

        if (onlyFree)
        {
            for (int i = 0; i < Rh.Count; i++)
                if (!Rh[i].free || Rh[i].currentMob==null)
                    Rh.Remove(Rh[i]);
        }

        if (colore)
        {
            for (int i = 0; i < Rh.Count; i++)
                ColorGrid(color, Rh[i].x, Rh[i].y);
        }

        return Rh;
    }

    public bool CheckAttack(int x, int y,GameObject checkObj,int skillCheck)
    {
        Check();

        bool targetFriend = false,
              targetMe    = false,
              needEnemy   = false;

        SkillManager skillManager = checkObj.GetComponent<SkillManager>();

        #region Need Target
        if (skillManager && (skillCheck - 1) > -1)
        {
            targetFriend = skillManager.Skills[skillCheck - 1].TargetFriend;

            targetMe     = skillManager.Skills[skillCheck - 1].TargetMe;

            needEnemy    = skillManager.Skills[skillCheck - 1].NeedTarget;
        }        
        #endregion

        bool canAttack    = false;

        GameObject objMob,objItem;

        HexManager grid = HexGround(x,y);

        if (grid != null && grid.GetComponent<MeshRenderer>() != null)
        {
            if (grid.currentMob)
                objMob = grid.currentMob;
            else
                objMob = null;

            if (grid.currentItem)
                objItem = grid.currentItem;
            else
                objItem = null;

            #region Target item
            if (objItem != null && checkObj.GetComponent<MobManager>().isPlayer)
            {
                if (skillManager != null)
                {
                    if (grid.puxeItem &&
                       skillManager.Skills[skillCheck - 1].GetComponent<MobSkillPull>() &&
                       skillManager.Skills[skillCheck - 1].GetComponent<MobSkillPull>().PullItem)
                    {
                        canAttack = true;

                        checkObj.GetComponent<EnemyAttack>().TargetAttack(true, objItem, skillCheck);

                        ColorGrid(3, x, y);
                    }
                    else
                    {
                        canAttack = false;
                        ColorGrid(2, x, y);
                    }
                }
                else
                {
                    if (grid.puxeItem)
                    {
                        canAttack = true;

                        checkObj.GetComponent<EnemyAttack>().TargetAttack(true, objItem, skillCheck);

                        ColorGrid(3, x, y);
                    }
                    else
                    {
                        canAttack = false;
                        ColorGrid(2, x, y);
                    }
                }

                if (objMob == null)
                    return canAttack;
            }
            else
                ColorGrid(2, x, y);
            #endregion

            #region Target Mob
            if (objMob != null/* && objMob!=checkObj*/)
            {
                if (objMob.GetComponent<MobManager>())
                {
                    if (objMob.GetComponent<MobManager>().Alive)
                    {
                        canAttack = false;

                        if (objMob.GetComponent<MobManager>().MesmoTime(checkObj.GetComponent<MobManager>().TimeMob))
                        {
                            if (targetFriend && objMob != checkObj ||
                                targetMe     && objMob == checkObj)
                            {
                                canAttack = true;

                                Debug.LogError(checkObj.name + " Add o objMob [<color=green>Amigo</color>] " + objMob.name + " na lista " + skillCheck);
                                checkObj.GetComponent<EnemyAttack>().TargetAttack(true, objMob, skillCheck);
                                ColorGrid(3, x, y);
                            }
                        }
                        else
                        if (needEnemy)
                        {
                            canAttack = true;

                            Debug.LogError(checkObj.name + " Add o objMob [<color=yellow>Inimigo</color>] " + objMob.name + " na lista " + skillCheck);
                            checkObj.GetComponent<EnemyAttack>().TargetAttack(true, objMob, skillCheck);

                            ColorGrid(1, x, y);
                        }

                        return canAttack;
                    }
                    else
                        ColorGrid(2, x, y);
                }
                else
                    ColorGrid(2, x, y);
            }
            else
                ColorGrid(2, x, y);
            #endregion
        }

        return canAttack;
    }

    public bool CheckWalk(int x, int y, bool color = true)
    {
        Check();

        HexManager grid = null;

        if (x >= 0 && x <= X && y >= 0 && y <= Y)
            grid = HexGround(x, y);


        if (grid != null && grid.GetComponent<MeshRenderer>() != null)
        {
            if (color)
            {
                if (floorFree[x * X + y])
                    ColorGrid(1, x, y);

                if (!floorFree[x * X + y])
                    ColorGrid(2, x, y);
            }

            return floorFree[x * X + y];
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    /// <param name="x">Position in X</param>
    /// <param name="y">Position in Y</param>
    /// <param name="clear">Clear Color</param>
    public void ColorGrid(int color=0, int x=0, int y=0, bool clear = false)
    {
        if (!clear)
        {
            HexManager grid = HexGround(x, y);

            if (grid != null && grid.GetComponent<MeshRenderer>() != null)
            {
                switch (color)
                {
                    case 0:
                        grid.GetComponent<MeshRenderer>().material.color = Color.white;
                        break;

                    case 1:
                        grid.GetComponent<MeshRenderer>().material.color = Color.blue;
                        break;

                    case 2:
                        grid.GetComponent<MeshRenderer>().material.color = Color.red;
                        break;

                    case 3:
                        grid.GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                }
                return;
            }
        }
        else
            {
            foreach (var g in GridMap.Instance.hex)
            {
                g.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: White, 1:Blue, 2:Red, 3:Green</param>
    /// <param name="_hex">Ground</param>
    /// <param name="clear">Clear Color</param>
    public void ColorGrid(int color = 0,HexManager _hex=null, bool clear = false)
    {
        if (_hex != null && _hex.GetComponent<MeshRenderer>() != null)
        {
            if (!clear)
            {
                switch (color)
                {
                    case 0:
                        _hex.GetComponent<MeshRenderer>().material.color = Color.white;
                        break;

                    case 1:
                        _hex.GetComponent<MeshRenderer>().material.color = Color.blue;
                        break;

                    case 2:
                        _hex.GetComponent<MeshRenderer>().material.color = Color.red;
                        break;

                    case 3:
                        _hex.GetComponent<MeshRenderer>().material.color = Color.green;
                        break;
                }
                return;
            }
            else
            {
                _hex.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefab">Prefab do mob</param>
    /// <param name="hexX">Posição no hex</param>
    /// <param name="hexY">Posição no hex</param>
    /// <param name="registrar">Registra nos turnos</param>
    public GameObject Respaw(GameObject prefab, int hexX, int hexY, bool spaceOccupy = true, bool record = true)
    {       
        if (prefab==null)
            return null;

        Debug.LogWarning("Respaw() " + prefab.name);

        GameObject grid = GameObject.Find("Hex" + hexX + "x" + hexY);

        if (grid != null)
            if (grid.GetComponent<HexManager>().free)
            {
                GameObject gameObj = Instantiate(prefab, grid.transform.position, grid.transform.rotation);

                MoveController test = gameObj.GetComponent<MoveController>();

                if (gameObj.GetComponent<PortalManager>()!=null)
                    if (RespawMob.Instance.Player!=null)
                        GameObject.FindObjectOfType<PortalManager>().LookAtPlayer(RespawMob.Instance.Player);


                //if(prefab.GetComponent<PortalManager>()== null)
                if (spaceOccupy)
                {
                    floorFree[hexX * X + hexY]                 = false;
                    grid.GetComponent<HexManager>().free       = false;
                    grid.GetComponent<HexManager>().currentMob = gameObj;
                }

                if (test)
                {
                    test.hexagonX = hexX;
                    test.hexagonY = hexY;

                    if (gameObj.GetComponent<MobManager>()!=null)
                    {
                        if (gameObj == RespawMob.Instance.Player)
                        {
                            if (GameObject.FindGameObjectWithTag("Portal") != null)
                            {
                                test.transform.LookAt(GameObject.FindGameObjectWithTag("Portal").transform.localPosition);

                                GameObject.FindGameObjectWithTag("Portal").GetComponent<PortalManager>().LookAtPlayer(gameObj);
                            }
                        }
                        else
                        {
                            if (RespawMob.Instance.Player != null)
                                test.transform.LookAt(RespawMob.Instance.Player.transform);
                            else
                            if (GameObject.FindGameObjectWithTag("Player") != null)
                                test.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
                        }
                    }

                    if (gameObj.GetComponent<PortalManager>()!=null)
                    {
                        if (grid.GetComponent<HexManager>().currentItem == null)
                        {
                            grid.GetComponent<HexManager>().currentItem = gameObj;
                        }
                    }

                    
                }

                Debug.Log(prefab.name + " criado, na " + grid.name);

                if (record)
                {
                    GameObject turn = GameObject.FindGameObjectWithTag("Manager");
                    print(gameObj.name + " Foi mandado para o registro!!");
                    turn.GetComponent<TurnSystem>().RegisterTurn(gameObj);
                }

                listRespaw.allRespaws.Add(gameObj);
                return gameObj;
            }

        Debug.LogError("Erro ao Respawnar "+prefab.name+" no Hex"+hexX+"x"+hexY+" , Pois a casa ja estava ocupada.");
        return null;
    }
}
