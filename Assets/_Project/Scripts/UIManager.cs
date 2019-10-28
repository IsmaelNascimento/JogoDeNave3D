using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    private static UIManager m_Instance;
    private static UIManager Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(UIManager)) as UIManager;
            }

            return m_Instance;
        }
    }

    [SerializeField] private CanvasGroup cgCanvasFadeScreen;
    [SerializeField] private GameObject goCanvasStart;

    public void CallFade(bool fadeIn, float time, TweenCallback callback = null)
    {
        if (fadeIn)
        {
            if(callback != null)
            {
                DOTween.To(() => cgCanvasFadeScreen.alpha, x => cgCanvasFadeScreen.alpha = x, 1f, time).OnComplete(() => callback());
            }
            else
            {
                DOTween.To(() => cgCanvasFadeScreen.alpha, x => cgCanvasFadeScreen.alpha = x, 1f, time);
            }
        }
        else
        {
            if (callback != null)
            {
                DOTween.To(() => cgCanvasFadeScreen.alpha, x => cgCanvasFadeScreen.alpha = x, 0, time).OnComplete(() => callback());
            }
            else
            {
                DOTween.To(() => cgCanvasFadeScreen.alpha, x => cgCanvasFadeScreen.alpha = x, 0, time);
            }
        }
    }

    public void OnClickButtonStart()
    {
        CallFade(true, .5f, () => 
        {
            goCanvasStart.SetActive(false);
            CallFade(false, 1f, PlayerControl.Instance.AnimationStart);
        });
    }
}
