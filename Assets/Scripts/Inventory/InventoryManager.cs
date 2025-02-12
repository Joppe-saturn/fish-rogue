using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> startingItems = new List<Item>();
    [SerializeField] private int inventorySize;
    [SerializeField] private Vector2 inventoryTankSize;

    public class InventoryItem
    {
        public Item item;
        public string itemName;
        public string description;
        public Sprite icon;
        public GameObject itemInstance;
        public int itemSpot;
        public UiManager.JUiAction button;
        public List<Algie> placedAlgie;

        public InventoryItem(Item item, string itemName, string description, Sprite icon, GameObject itemInstance, int itemSpot, UiManager.JUiAction button, List<Algie> placedAlgie)
        {
            this.item = item;
            this.itemName = itemName;
            this.description = description;
            this.icon = icon;
            this.itemInstance = itemInstance;
            this.itemSpot = itemSpot;
            this.button = button;
            this.placedAlgie = placedAlgie;
        }
    }

    public class Algie
    {
        public float maxHealth;
        public float health;
        public Item.Effect effect;
        public Sprite icon;
        public float spreadChance;
        public Item item;
        public int itemSpot;

        public Algie(float maxHealth, float health, Item.Effect effect, Sprite icon, float spreadChance, Item item, int itemSpot)
        {
            this.maxHealth = maxHealth; 
            this.health = health;
            this.effect = effect;
            this.icon = icon;
            this.spreadChance = spreadChance;
            this.item = item;
            this.itemSpot = itemSpot;
        }
    }

    private InventoryItem[] items;
    private Algie[,] algies;

    private UiManager uiManager;

    private InventoryItem selectedItem;

    public class Int2
    {
        public int x;
        public int y;
    }

    private void Awake()
    {
        items = new InventoryItem[inventorySize];
        algies = new Algie[(int)inventoryTankSize.x, (int)inventoryTankSize.y];
    }

    private void Start()
    {
        uiManager = FindFirstObjectByType<UiManager>();

        for (int i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                InventoryItem newInventoryItem = new InventoryItem(item, item.name, item.description, item.icon, item.item, i, uiManager.buttons[i + 13], new List<Algie>());
                items[i] = newInventoryItem;

                Transform itemIcon = uiManager.transform.GetChild(1).GetChild(2).GetChild(i);
                itemIcon.GetComponent<Image>().sprite = item.icon;

                break;
            }
        }
    }

    public void SelectItem()
    {
        if (selectedItem == null)
        {
            int currentButton = GetInventoryTile();

            InventoryItem currentItem = items[currentButton];
            
            if(currentItem != null)
            {
                selectedItem = currentItem;
            }
        } 
        else
        {
            selectedItem = null;
        }
    }

    public void RemoveItem()
    {
        int currentButton = GetInventoryTile();

        for (int i = 0; i < items[currentButton].placedAlgie.Count; i++)
        {
            RemoveAlgie(items[currentButton].placedAlgie[i]);
        }

        items[currentButton] = null;

        Transform itemIcon = uiManager.transform.GetChild(1).GetChild(2).GetChild(currentButton);
        itemIcon.GetComponent<Image>().sprite = null;

        
    }

    public void PlaceAlgie()
    {
        Int2 currentButton = new Int2();

        if (true)
        {
            currentButton = GetTankTile();
        } 

        if (algies[currentButton.x, currentButton.y] == null && selectedItem != null)
        {
            if (selectedItem.placedAlgie.Count == 0)
            {
                Algie newAlgie = ConvertItemToAlgie(selectedItem);                
                algies[currentButton.x, currentButton.y] = newAlgie;
                items[newAlgie.itemSpot].placedAlgie.Add(newAlgie);

                selectedItem.placedAlgie.Add(newAlgie);

                selectedItem = null;

                Transform itemIcon = uiManager.transform.GetChild(1).GetChild(4).GetChild(currentButton.x * (int)inventoryTankSize.y + currentButton.y);
                itemIcon.GetComponent<Image>().sprite = algies[currentButton.x, currentButton.y].icon;
            }
        }
    }

    private int GetInventoryTile()
    {
        UiManager.JUiAction buttonPressed = uiManager.mostRecentAction;

        for (int i = 0; i < inventorySize; i++)
        {
            if (uiManager.buttons[i + 13].position == buttonPressed.position)
            {
                return i;
            }
        }
        return 0;
    }

    private Int2 GetTankTile()
    {
        UiManager.JUiAction buttonPressed = uiManager.mostRecentAction;
        Int2 buttonReturn = new Int2();

        for (int i = 0; i < inventoryTankSize.x; i++)
        {
            for (int j = 0; j < inventoryTankSize.y; j++)
            {
                if (uiManager.buttons[j + i * (int)inventoryTankSize.y + 13 + inventorySize].position == buttonPressed.position)
                {
                    buttonReturn.x = i;
                    buttonReturn.y = j;
                    return buttonReturn;
                }
            }
        }

        buttonReturn.x = 0;
        buttonReturn.y = 0;
        return buttonReturn;
    }

    public float RequestEffects(Item.Effect effect)
    {
        float effectStrength = 0;
        for (int i = 0; i < inventoryTankSize.x;i++)
        {
            for (int j = 0; j < inventoryTankSize.y; j++)
            {
                if (algies[i, j] != null)
                {
                    if (algies[i, j].effect == effect)
                    {
                        effectStrength += algies[i, j].item.power;
                    }
                }
            }
        }
        return effectStrength;
    }

    private void RemoveAlgie(Algie algie)
    {
        for(int i = 0; i < inventoryTankSize.x; i++)
        {
            for (int j = 0; j < inventoryTankSize.y; j++)
            {
                if (algies[i, j] == algie)
                {
                    algies[i, j] = null;
                    Transform itemIcon = uiManager.transform.GetChild(1).GetChild(4).GetChild(i * (int)inventoryTankSize.y + j);
                    itemIcon.GetComponent<Image>().sprite = null;
                }
            }
        }
    }

    public IEnumerator SpreadAlgie(float time)
    {
        for (int i = 0; i < time * 50f; i++)
        {
            List<Algie> activeAlgie = new List<Algie>();
            List<Int2> positions = new List<Int2>();
            for (int j = 0; j < inventoryTankSize.x; j++)
            {
                for (int k = 0; k < inventoryTankSize.y; k++)
                {
                    if (algies[j, k] != null)
                    {
                        activeAlgie.Add(algies[j, k]);

                        Int2 activeAlgiePosition = new Int2();
                        activeAlgiePosition.x = j;
                        activeAlgiePosition.y = k;

                        positions.Add(activeAlgiePosition);
                    }
                }
            }

            for (int j = 0; j < activeAlgie.Count; j++)
            {
                float spreadChance = 50f / activeAlgie[j].spreadChance;
                float r = Random.Range(0, spreadChance);

                activeAlgie[j].health -= 0.2f;

                if(r < 1)
                {
                    activeAlgie[j].health -= 0.2f;

                    Int2 direction = new Int2();
                    direction.x = Random.Range(-1, 2);
                    direction.y = Random.Range(-1, 2);
                    
                    Int2 algiePosition = positions[j];
                    algiePosition.x += direction.x;
                    algiePosition.y += direction.y;
                    
                    if(algiePosition.x >= 0 && algiePosition.y >= 0)
                    {
                        if (algies[algiePosition.x, algiePosition.y] == null)
                        {
                            Algie newAlgie = new Algie(activeAlgie[j].maxHealth, activeAlgie[j].maxHealth, activeAlgie[j].effect, activeAlgie[j].icon, activeAlgie[j].spreadChance, activeAlgie[j].item, activeAlgie[j].itemSpot);
                            algies[algiePosition.x, algiePosition.y] = newAlgie;
                            items[newAlgie.itemSpot].placedAlgie.Add(newAlgie);

                            Transform itemIcon = uiManager.transform.GetChild(1).GetChild(4).GetChild(algiePosition.x * (int)inventoryTankSize.y + algiePosition.y);
                            itemIcon.GetComponent<Image>().sprite = newAlgie.icon;
                        }
                    } 
                    else
                    {
                        activeAlgie[j].health -= 0.5f;
                    }
                }

                if (activeAlgie[j].health < 0)
                {
                    RemoveAlgie(activeAlgie[j]);
                }
            }
            
            yield return new WaitForSeconds(0.02f);
        }
    }

    private Int2 GetAlgiePosition(Algie algie)
    {
        Int2 position = new Int2();
        position.x = 0;
        position.y = 0;

        for (int i = 0; i < inventoryTankSize.x; i++)
        {
            for (int j = 0; j < inventoryTankSize.y; j++)
            {
                if (algies[i, j] == algie)
                {
                    position.x = i;
                    position.y = j;
                    return position;
                }
            }
        }

        return position;
    }

    private Algie ConvertItemToAlgie(InventoryItem item)
    {
        Item convertItem = item.item;
        Algie algie = new Algie(convertItem.maxHealth, convertItem.maxHealth, convertItem.effect, convertItem.algieIcon, convertItem.algieSpreadChance, convertItem, item.itemSpot);

        return algie;
    }
}
