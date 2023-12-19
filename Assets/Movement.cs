using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;
    private float yVel;

    public float jumpforce = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
    //public float timeBetweenShots = 0.1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public Guns[] allGuns;
    private int selectedGun;

    public float maxheat = 10f, heatPershot = 1f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        SwitchGun();
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);
        if(invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if(Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        } else
        {
            activeMoveSpeed = moveSpeed;
        }
        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if(charCon.isGrounded)
        {
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

        if(Input.GetButtonDown("Jump") && charCon.isGrounded)
        {
            movement.y = jumpforce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

        charCon.Move(movement * activeMoveSpeed * Time.deltaTime);

        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;

            if (muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }

        }
        if(!overHeated)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if(Input.GetMouseButton(0))
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }

            heatCounter -= coolRate * Time.deltaTime;
        } else
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;
            if(heatCounter <=0)
            {
                overHeated = false;
            }
        }

        if(heatCounter < 0)
        {
            heatCounter = 0f;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Shoot();

        }

        if(Input.GetMouseButton(0))
        {
            shotCounter -= Time.deltaTime;

            if(shotCounter <= 0)
            {
                Shoot();
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwitchGun();
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if(selectedGun <0)
            {
                selectedGun = allGuns.Length - 1;
            }
            SwitchGun();



        }

    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("We Hit" + hit.collider.gameObject.name);

            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point +(hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, 10f);
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += heatPershot;
        if(heatCounter >= maxheat)
        {
            heatCounter = maxheat;

            overHeated = true;
        }

        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;

    }
    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    void SwitchGun()
    {
        foreach(Guns gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
    }

}
