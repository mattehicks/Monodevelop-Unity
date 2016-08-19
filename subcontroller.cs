using UnityEngine;
using System.Collections;

public class subcontroller : MonoBehaviour {
	
	//public float movespeed;
	public float turnSpeed;
	public float pitchSpeed;
	public float rollSpeed;
	public float horsepower;
	public GameObject Player;

    public AudioClip crash;
    public AudioClip powerup;
    public AudioClip tokens;
    public AudioClip other;

	public Texture sparks;

	public Font myfont;
	public Font smallfont;
	GUIStyle myStyle;
	GUIStyle centerStyle;

	int foundtargets = 0;
	int numpoints = 0;
	bool bpoints = false;
	int pcounter = 400;
    int endtimer = 700;//endscreen timeout
    bool rungame = true; //endgame flag
    bool flymode = false; //spectator mode
    GameObject camera1;

    bool showmenu = true;
    float yourtime = 0;
    float btnwaittimer = 0;//de-bounce the user input
    bool btnwaitbool =false;

	int count = 0;

	Vector2 textpos = new Vector2(20,40);
	Vector2 textsize = new Vector2(60,20);

	float mouseRoll;
	float mousePitch;
	bool applyroll;


	int yMinLimit = -180;
	int yMaxLimit = 180;

	private float x = 0.0f;
	private float y = 0.0f;

	bool mousehome = false;
	bool pause = false;

	// Use this for initialization
	void Start () {
        //numtargets = 10; //debug end game
        camera1 = GameObject.FindGameObjectWithTag("MainCamera");
        pausegame();

		centerStyle = new GUIStyle();
		centerStyle.alignment = TextAnchor.UpperCenter;
		centerStyle.font = myfont;
		centerStyle.fontSize = 40;

		myStyle = new GUIStyle();
		myStyle.font = myfont;
		myStyle.fontSize = 100;
		//Player.renderer.material.color = new Color32 (0, 0, 0, 0);

		Screen.lockCursor = true;
		if (horsepower==null){horsepower = 100;}

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		mouseRoll = rollSpeed * 0.05f;
		mousePitch = pitchSpeed * 0.05f;

        Debug.Log("target: " + GameObject.Find("Boid Controller/!Shark").transform.ToString());

        GameObject[] dumbfish = GameObject.FindGameObjectsWithTag("boid");

        foreach (GameObject fish in dumbfish)
        {
         //   fish.renderer.material.color = Color.red;
            fish.GetComponent<SmoothLookAt>().target = GameObject.Find("Boid Controller/!Shark").transform;
        }

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) { 
			rigidbody.AddForce (transform.forward * horsepower, ForceMode.Force); 
				}
		if (Input.GetKey (KeyCode.S)) {
			rigidbody.AddForce(-transform.forward * horsepower, ForceMode.Force);
			//transform.Translate(-Vector3.forward * movespeed * Time.deltaTime);
		}  
		if(Input.GetKey(KeyCode.D)){
			transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
		}    
		if(Input.GetKey(KeyCode.A)){
			transform.Rotate(Vector3.up,-turnSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			//angle down: Inverse
			transform.Rotate (Vector3.right, (1 * pitchSpeed) * Time.deltaTime);
		}
		if(Input.GetKey (KeyCode.DownArrow)){
			//angle up : Inverse
			transform.Rotate (Vector3.right, (-1 * pitchSpeed) * Time.deltaTime);
		}

		applyroll = false;

		if(Input.GetKey (KeyCode.LeftArrow)){
			//roll left
			transform.Rotate (Vector3.forward, (1 * rollSpeed) * Time.deltaTime); 
			applyroll = true;
		}
		if(Input.GetKey (KeyCode.RightArrow)){
			//roll right
			transform.Rotate (Vector3.forward, (-1 * rollSpeed) * Time.deltaTime); 
			applyroll = true;
		}
		if (applyroll == false) {
			//lerp to reset roll rotation
			Quaternion newRotation = transform.rotation;
			newRotation.z = 0.0f;
			//Quaternion rotation = Quaternion.Euler(currentX,currentY,0);
			transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, (.5f * Time.deltaTime));
				}
		//mouse input, point the ship up down, inverted
		float mouseX = Input.GetAxis ("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

			x += mouseX * mouseRoll;
		//Debug.Log ("roll@"+mouseX);
			y -= mouseY * mousePitch;
		
		//Debug.Log ("pitch@"+mouseY);
		
			y = ClampAngle (y, yMinLimit, yMaxLimit);
			//mouse X rotates ship on Z axis
			x = ClampAngle (x, -45, 45);
			transform.Rotate (Vector3.right, (-y * pitchSpeed) * Time.deltaTime); 
			transform.Rotate (Vector3.forward, (-x * rollSpeed) * Time.deltaTime); 
		x = 0;
		y = 0;

        if (btnwaitbool == false)
        {
            //////////////////////////////////////////////////////////////////
            //PAUSE AND QUIT GAME
            if (Input.GetKey(KeyCode.Q))
            {
                Application.Quit();
            }
            if (Input.GetKey(KeyCode.P))
            {
                pausegame();
                btnwaitbool = true;
            }
            if (Input.GetKey(KeyCode.E))
            {
                //Leave&Enter spectator mode
                SpectatorMode();
                btnwaitbool = true;
            }
            if (Input.GetKey(KeyCode.R))
            {
                //Restart
                Application.LoadLevel(0);
                btnwaitbool = true;
            }
            if (Input.GetKey(KeyCode.H))
            {
                showmenu = !showmenu;
                btnwaitbool = true;
                pausegame();
            }
            /////////////////////////////////////////////////////////////
         

        }
        if (btnwaitbool == true) 
        { 
            btnwaittimer++;
            if (btnwaittimer == 20) { btnwaittimer = 0; btnwaitbool = false; }
        }
	}

    void pausegame()
    {
        if (pause == false)
        {
            Time.timeScale = 0.0f;
            pause = true;
        }
        else if (pause == true)
        {
            Time.timeScale = 1.0f;
            pause = false;
        }
    }
    void SpectatorMode()
    {
        if ((rungame == true)&(flymode == false))
        {
                //set camera location to floating
                GameObject floatingobject = new GameObject();
                floatingobject.name = "cam_free";
                floatingobject.transform.position = camera1.transform.position;
                camera1.transform.parent = floatingobject.transform;
                flymode = true;
                return;
            }
        else if((rungame==true)&(flymode==true))
            {
                GameObject cam_plot = GameObject.Find("SubEffects/cam_plot");
                Debug.Log("camplot: " + cam_plot);
                camera1.transform.parent = cam_plot.transform;
                camera1.transform.position = cam_plot.transform.position;
                flymode = false;
                Destroy(GameObject.Find("cam_free"));
                return;
            }
    }

	void OnTriggerEnter(Collider other){
		if (other.collider.gameObject.name == "ring") {
			other.collider.enabled = false;
			other.collider.renderer.material.color = Color.red;
			Destroy (other.gameObject, 1);
			//Debug.Log ("name1: " + other.collider.name);
			foundtargets+=1;
            bpoints = true;

            audio.clip = powerup;
            audio.Play();

		}
		if (other.collider.gameObject.name == "tresure_box") {
			numpoints+=100;
			//other.collider.renderer.material.color = Color.red;
			//play sound & animation
			bpoints = true;
			Destroy (other.gameObject);
			//Debug.Log ("points: " + other.collider.name);

            audio.clip = tokens;
            audio.Play();

		}

		}
	
	void OnCollisionEnter(Collision coll) {
		
		ContactPoint contact = coll.contacts[0];
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;

		GameObject firesphere = GameObject.CreatePrimitive(PrimitiveType.Quad);
		firesphere.collider.enabled = false;
		firesphere.renderer.material.color = Color.yellow;
		sparks = (Texture)Resources.Load ("sparks");
		firesphere.renderer.material.mainTexture = sparks;
		
		firesphere.transform.position = pos;
		firesphere.transform.rotation = rot;
		firesphere.transform.localScale = new Vector3 (2, 2, 2);
		// Destroy the projectile
		Destroy (firesphere,0.5f);

        audio.clip = crash;
        audio.Play();

	}

	
	static float ClampAngle(float angle,float min, float max) {
		if (angle < -90)
			angle += 90;
		if (angle > 90)
			angle -= 90;
		return Mathf.Clamp (angle, min, max);
	}


	
	void OnGUI () {
        if (showmenu == true)
        {
           
            myStyle.fontSize = 20;
           
            GUI.Box(new Rect(180, Screen.height / 2 - 100, 100, 50), "<color=white><b>\nControls: </b></color>", myStyle);
            GUI.Box(new Rect(150, Screen.height / 2 - 50, 100, 50), "<color=white>Use WASD + Mouse or Arrow Keys</color>", myStyle);  
        GUI.Box(new Rect(150, Screen.height / 2 - 25, 100, 50),"<color=white>'E'</color> <color=silver>for free-fly mode </color>",myStyle);
        GUI.Box(new Rect(150, Screen.height / 2 - 0, 100, 50), "<color=white>'R' </color><color=silver>to restart</color>", myStyle);
        GUI.Box(new Rect(150, Screen.height / 2 +25, 100, 50), "<color=white>'H' </color><color=silver>for help</color>", myStyle);
        GUI.Box(new Rect(150, Screen.height / 2 + 50, 100, 50), "<color=white>'P/Q' </color><color=silver>pause/quit</color>", myStyle);
            
            
            GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 60, 100, 50), "<color=white><OK></color>", myStyle);
            if (Input.GetKey(KeyCode.Return)| Input.GetKey(KeyCode.KeypadEnter))
            {
                showmenu = !showmenu;
                pausegame();
            }

        }
		if(count == pcounter){
            //wait to clear the gui
			bpoints = false;
			count = 0;
		}	
		if (bpoints == true) {
            //display points on screen
			centerStyle.fontSize = 40;
			GUI.Box (new Rect (Screen.width/2-50, Screen.height/2-25, 100, 50), "<color=white><b>+100</b></color>", centerStyle);
            count ++;
		}
        if (foundtargets >= 10)
        {
            if (endtimer > 0)
            {
                endtimer--;
                myStyle.fontSize = 100;
                //myStyle.alignment = TextAnchor.MiddleCenter;  
                if (yourtime == 0) { yourtime = Mathf.Round(Time.realtimeSinceStartup); }
                GUI.Box(new Rect(20, 150, 755, 240), "");
                GUI.Box(new Rect(35, 200, 100, 80), "<color=white><b>You W in!</b></color>", myStyle);
                myStyle.fontSize = 40;
                GUI.Box(new Rect(30, 300, 100, 80), "<color=white>Points: " + numpoints + "</color>", myStyle);
                GUI.Box(new Rect(30, 330, 100, 80), "<color=white>Time: " + yourtime + "</color>", myStyle);
                myStyle.fontSize = 30;
                GUI.Box(new Rect(100, 360, 100, 80), "<color=silver>Q to Quit</color>", myStyle);

            }
            else
            {
                Debug.Log("GAME OVER");
                //Debug.Log("bool1" + rungame.ToString());
                //Debug.Log("counter" + endtimer.ToString());
            }
            if( (rungame == true)&(endtimer <=0))
            {
                SpectatorMode();
                rungame = false;
            }

        }
        else
        {
            GUI.Box(new Rect(20, 40, 200, 50), "");
            myStyle.fontSize = 20;
            myStyle.alignment = TextAnchor.UpperLeft;
            // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            GUI.Box(new Rect(25, 45, 125, 40), "<color=white>Rings: " + foundtargets + "/10</color>", myStyle);
        }
		
	

	}



}
