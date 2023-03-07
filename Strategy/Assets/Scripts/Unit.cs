using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool selected;
    GameMaster gm;

    public int tileSpeed;
    public bool isAlreadyMoved;

    public float moveSpeed;

    public int playerNumber;

    public int attackRange;
    List<Unit> enemiesInRange = new List<Unit>();
    public bool isAlreadyAttacked;

    public GameObject weaponIcon;

    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;


    private void Start()
    {
        gm = FindObjectOfType<GameMaster>();
    }

    private void OnMouseDown()
    {
        ResetWeaponIcons();

        if(selected)
        {
            selected = false;
            gm.selectedUnit = null;
            gm.ResetTiles();
        }
        else
        {
            if(playerNumber == gm.playerTurn)
            {
                if (gm.selectedUnit != null)
                    gm.selectedUnit.selected = false;

                selected = true;
                gm.selectedUnit = this;

                gm.ResetTiles();
                GetEnemies();
                GetWalkableTiles();
            }
        }

        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = col.GetComponent<Unit>();

        if(gm.selectedUnit != null)
        {
            if(gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.isAlreadyAttacked)
            {
                gm.selectedUnit.Attack(unit);
            }
        }
    }


    void Attack(Unit enemy)
    {
        isAlreadyAttacked = true;

        int enemyDamage = attackDamage - enemy.armor;
        int myDamage = enemy.defenseDamage - armor;

        if (enemyDamage >= 1)
            enemy.health -= enemyDamage;
        else if (myDamage >= 1)
            health -= myDamage;

        if (enemy.health <= 0)
        {
            Destroy(enemy.gameObject);
            GetWalkableTiles();
        }
        
        if(health <= 0)
        {
            gm.ResetTiles();
            Destroy(this.gameObject);
        }
    }

    void GetWalkableTiles()
    {
        if (isAlreadyMoved)
            return;

        ///Attack line
        /*foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if (tile.transform.position.y + 0.2f >= transform.position.y && tile.transform.position.y - 0.2f <= transform.position.y)
                tile.Highlights();
            if (tile.transform.position.x + 0.2f >= transform.position.x && tile.transform.position.x - 0.2f <= transform.position.x)
                tile.Highlights();
        }*/

            foreach(Tile tile in FindObjectsOfType<Tile>())
            {
                if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
                {
                    if (tile.IsClear())
                    {
                        tile.Highlights();
                    }
                }
            }

    }

    void GetEnemies()
    {
        enemiesInRange.Clear();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x) + Mathf.Abs(transform.position.y - unit.transform.position.y) <= tileSpeed)
            {
                if (unit.playerNumber != gm.playerTurn && !isAlreadyAttacked)
                {
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    public void ResetWeaponIcons()
    {
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(tilePos));
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        while(transform.position.x != tilePos.x)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(tilePos.x, transform.position.y, -1f), moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position.y != tilePos.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, tilePos.y, -1f), moveSpeed * Time.deltaTime);
            yield return null;
        }

        isAlreadyMoved = true;
        ResetWeaponIcons();
        GetEnemies();
    }
}
