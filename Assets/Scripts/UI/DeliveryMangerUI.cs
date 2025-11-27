using UnityEngine;

public class DeliveryMangerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;
    private void Awake()
    {
        //hide the template 
        recipeTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        //listening to the event
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeSpawn += DeliveryManager_OnRecipeSapwn;
        UpdateVisual();
    }
    private void DeliveryManager_OnRecipeSapwn(object sender, System.EventArgs e)
    {
        //when recipe spawned update visual
        UpdateVisual();
    }
    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        //when recipe completed update visual remove the obj
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }
       foreach(RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryMangerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
