using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BlockLevel : MonoBehaviour
{
    Image  _image;
    Button _button;

    [Header("Sprite"), SerializeField]
    Sprite _originalSprite;

    [Space(7), Header("Prop's")]
    public bool _block = true;


    void Awake()
    {
        _image          = GetComponent<Image>();
        _button         = GetComponent<Button>();
        
    }

    public void Block(bool? nowBlock=null)
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if(_image==null)
            _image = GetComponent<Image>();

        _block = nowBlock == true ? true : false;

        _button.interactable = !_block;

        if(nowBlock == null)
            _button.gameObject.SetActive(false);
        else
        if (_block == true)
        {
            if (_originalSprite == null)
                _originalSprite = _image.sprite;

            _image.sprite = SelectLevel.Instante.BlockSprite;
        }
        else
            if (_block == false)
            _image.sprite = _originalSprite;
            


        //return _block;
    }
}
