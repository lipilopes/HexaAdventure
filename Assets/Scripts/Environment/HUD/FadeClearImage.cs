using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class FadeClearImage : MonoBehaviour
{
    [SerializeField]
    bool fade=true;
    [SerializeField]
    float _wait,_fadeSpeed;

    [SerializeField]
    Image  _img;

    [SerializeField]
    Text _text;

    GameManagerScenes gms;

    float alpha;

    Color _textColor,_imgColor;

    private void Start()
    {
        if (_text != null)
            _textColor = _text.color;

        if (_img)
            _imgColor = _img.color;

        if (gms == null)
        {
            gms = GameManagerScenes._gms;

            if (_text != null && gms != null)
                ChangeText(gms.NameFase(gms.FaseAtual));
        }
    }

    void Update()
    {
        if (!fade)
            return;

        if (_wait > 0)
        {
            _wait -= Time.deltaTime;
            return;
        }          

        alpha = 1 * _fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        if (_img.color.a <= 0)
            this.gameObject.SetActive(false);

            CheckImage();
            CheckText();

    }

    public void ChangeText(string txt)
    {
        _text.text = txt;
    }

    void CheckImage()
    {
        if (_imgColor == null)
            return;

            _imgColor.a -= alpha;

            _img.color = _imgColor;

        _img.color = new Color(_img.color.r, _img.color.g, _img.color.b, _imgColor.a);
    }

    void CheckText()
    {
        if (_textColor == null)
            return;

            _textColor.a -= alpha;

            _text.color = new Color (_text.color.r, _text.color.g, _text.color.b, _textColor.a);
    }
}
