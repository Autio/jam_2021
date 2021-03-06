using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{

    [SerializeField]
    private Image foregroundImage;
    [SerializeField] 
    private float updateSpeedSecongs = 0.5f;

    private void Awake()
    {
        GetComponentInParent<EnemyBase>().OnHealthPctChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator  ChangeToPct(float pct){
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed <updateSpeedSeconds)
        {
            elapsed += Time.deltaDime;
        }
    }


    // [SerializeField]
    // private int maxHealth = 100;
    // private int currentHealth;

    // public event Action<float> OnHealthPctChanged = delegate {};
    // // Start is called before the first frame update
    
    // void OnEnable()
    // {
    //     currentHealth = maxHealth;
    // }

    // public void ModifyHealth(int amount)
    // {
    //     currentHealth += amount;
    //     float currentHealthPct = (float)currentHealth / (float)maxHealth; 
    //     OnHealthPctChanged(currentHealthPct);
    // }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
