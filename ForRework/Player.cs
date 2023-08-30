using System.Collections;
using TOAFL.UserInput;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 13f,
                  jumpForce = 4f,
                  friction = 7f;
    public Animator animator;
    private Rigidbody rgb;
    public Transform legs;

    [Header("Camera Info")] 
    [SerializeField] private Transform cameraFollowTransform;
    [SerializeField] private Transform cameraLookAtTransform;

    private bool grounded = false, airGravity = false, down;
    private float h, v, lastHeight, dist;
    private Vector3 currentSpeed;
    private bool fix = false, fixHelp = false, jumpHelp = false;
    private Quaternion fixAngleP = Quaternion.Euler(0f, 90f,0f);
    private Quaternion fixAngleN = Quaternion.Euler(0f,-90f,0f);
    private Vector3 _normal, dirP, dirN, fixDir, moveDir, helpDir, sMoveDir, sMoveDirH;
    private Transform obj;
    
    private Controls _controls;

    public Transform CameraFollowTransform => cameraFollowTransform;
    public Transform CameraLookAtTransform => cameraLookAtTransform;

    [Inject]
    private void Construct(Controls controls)
    {
        _controls = controls;
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    public void Start()
    {
        rgb = GetComponent<Rigidbody>();
        lastHeight = transform.position.y;
        //Нижня граница коллайдера
        dist = GetComponent<Collider>().bounds.extents.y;
    }

    public void Update()
    {
        // Движение по осям ХY
        var xyInput = _controls.PlayerControls.Movement.ReadValue<Vector2>();
        h = xyInput.x;
        v = xyInput.y;
        /*moveDir = new Vector3(h,0,v);
        Quaternion rot = Quaternion.Euler(0,Vector3.Angle(transform.forward,moveDir),0);
        transform.rotation*=rot;*/
        /*h = h != 0 ? h * h / h : 0;
        v = v != 0 ? v * v / v : 0;//*/
        /**/h = h != 0 ? h / Mathf.Abs(h) : 0;
        v = v != 0 ? v / Mathf.Abs(v) : 0;//*/
        moveDir = new Vector3(h,0,v).normalized;
        // При подаче силы учитывается состояние игрока (в прыжке или на земле он)
        //rgb.AddForce(((fix ? fixDir : transform.right) * h + (fix ? fixDir : transform.forward) * v) * (grounded ? speed : speed * 0.2f) * Time.deltaTime, ForceMode.VelocityChange);
        if (moveDir!=Vector3.zero)
        if (airGravity||jumpHelp)
        rgb.velocity = new Vector3(0f,rgb.velocity.y,0f); // по y можно добавить множитель для замедленного опускания или наоборот
        else
        rgb.AddForce((fix||fixHelp ? fixDir : moveDir) * (grounded ? speed : speed * 0.2f) * Time.deltaTime, ForceMode.VelocityChange);
        

        // Сопротивление (на таких поверхностях как лёд можно наоборот сбавить для эффекта скольжения)
        currentSpeed = rgb.velocity;
        currentSpeed.y = 0;
        rgb.AddForce(-currentSpeed * Time.deltaTime * (grounded ? friction : friction * 0.25f), ForceMode.VelocityChange);

        // Прыжок
        if (_controls.PlayerControls.Jump.IsPressed() && grounded)
        {
            rgb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }

        // Проверка направления по Y
        down = (transform.position.y - lastHeight<0f) ? true : false;
        lastHeight = transform.position.y;

        // Animator
        animator.SetFloat("Move",(jumpHelp? 0 : moveDir.sqrMagnitude));
        animator.SetBool("Grounded",grounded);
        animator.SetFloat("Jump",rgb.velocity.y);

        //Debug отрисовка
        Debug.DrawRay(transform.position,moveDir,Color.red,1f);
        Debug.DrawRay(legs.position,Vector3.down*(0.2f),Color.gray,1f);
        
    }

    public void FixedUpdate()
    {
        
        RaycastHit hit;
        // Проверка приземления
        if (Physics.SphereCast(legs.position,0.15f, Vector3.down, out hit, /*dist + 0.05f-0.4f*/0.1f))
        {
            if (!hit.transform.GetComponent<Collider>().isTrigger)
            {
                grounded = true;
                airGravity = false;
            }
        }
        else 
        { 
            grounded = false;
            if(timerAirOff&&down)
            {
                //Debug.Log("Aiaiaiaia!");
                StartCoroutine(timerAir());   
            } 
        }

        // Автоматический обход препятсвий под углом, во избежания залипания в стенке
        if (Physics.Raycast(legs.position, moveDir, out hit, 0.5f + 0.5f))
        {
            if (!hit.collider.isTrigger&&hit.collider.tag=="Static") //&&hit.collider.tag=="Static" как возможная проверка
            {
                //Debug.Log(Vector3.Dot(transform.forward,hit.normal.normalized));
                // Выбор направления обхода
                _normal = hit.normal;
                _normal.y = 0;
                dirP = fixAngleP*_normal;
                dirN = fixAngleN*_normal;
                if(Vector3.Dot(moveDir,hit.normal.normalized)<0f&&Vector3.Dot(moveDir,hit.normal.normalized)>-0.95f) 
                if(Vector3.Dot(moveDir,(dirP).normalized)>
                   Vector3.Dot(moveDir,(dirN).normalized))
                {
                    jumpHelp = false;
                    sMoveDir = moveDir;
                    fixDir = dirP;
                    fix = true;
                }
                else
                {
                    jumpHelp = false;
                    sMoveDir = moveDir;
                    fixDir = dirN;
                    fix = true;
                }
                else if (obj) jumpHelp = true;
                Debug.DrawRay(transform.position,fixDir,Color.green,1f);
            }
            
        }
        else if(fix&&timerOff&&moveDir==sMoveDir)
        {
            Debug.Log("Fix");
            StartCoroutine(timer());
        }
        // Вспомогательная проверка обхода при замедленном движении
        else if (timerOff)
        {
            fix = false;
            jumpHelp = false;
            if(moveDir!=Vector3.zero)
            {
                if(fixHelp&&sMoveDirH!=moveDir) fixHelp=false;

                if((rgb.velocity.sqrMagnitude<5f||fixHelp)&&obj)
                {
                    helpDir = new Vector3(obj.transform.position.x - legs.position.x,0,obj.transform.position.z - legs.position.z);
                    Debug.DrawRay(legs.position,helpDir,Color.magenta,1.5f);
                    if (Physics.Raycast(legs.position, helpDir, out hit, 0.5f + 0.2f))
                    {
                        if (!hit.transform.GetComponent<Collider>().isTrigger&&hit.collider.tag=="Static")
                        {
                            //Debug.Log(Vector3.Dot(transform.forward,hit.normal.normalized));
                            // Выбор направления обхода 
                            //Debug.Log("Help!");
                            _normal = hit.normal;
                            _normal.y = 0;
                            dirP = fixAngleP*_normal;
                            dirN = fixAngleN*_normal;
                            if(Vector3.Dot(moveDir,hit.normal.normalized)<0f) // Проверка на напрвление векторов во избежание ненужного обхода
                            if(Vector3.Dot(moveDir,(dirP).normalized)>
                            Vector3.Dot(moveDir,(dirN).normalized))
                            {
                                sMoveDirH = moveDir;
                                fixDir = dirP;
                                fixHelp = true;
                            }
                            else
                            {
                                sMoveDirH = moveDir;
                                fixDir = dirN;
                                fixHelp = true;
                            }
                            //Debug.DrawRay(transform.position,fixDir,Color.blue,1.5f);
                        }
                        
                    }
                    else if(fixHelp&&timerOff)
                    { 
                        //Debug.Log("HELPfix!");
                        StartCoroutine(timer());
                    }
                }
                else fixHelp = false;
            }
            else fixHelp = false; 
            
        }
        // Поворот вдоль оси движения
        if (moveDir.sqrMagnitude!=0&&grounded)
        {
            Vector3 characterPos = transform.position;
            characterPos.y = 0;
            Vector3 newDir = (characterPos + (fix||fixHelp ? fixDir : moveDir)) - characterPos;
            Quaternion dirQ = Quaternion.LookRotation(newDir);
            Quaternion slerp = Quaternion.Slerp(transform.rotation,dirQ,5f*Time.deltaTime);
            rgb.MoveRotation(slerp);
        }
        //DEBUG
        //Debug.Log(rgb.velocity.sqrMagnitude);

    }

    private Vector3 timerMoveDir;
    private bool timerOff = true;
    IEnumerator timer() // Остаточное движение в обход (для исключения залипания на углах)
	{
        timerOff = false;
		float t = 0f;
        timerMoveDir = new Vector3(h,0,v);
		//while(t<0.05f)
        while(t<0.1f)
		{
            if(moveDir!=timerMoveDir)
            { 
                Debug.Log("arara");
                fix = false;
            }
            //Debug.Log("arara");
			t += Time.deltaTime;
			yield return null;
		}
        fix = false;
        fixHelp = false;
        timerOff = true;
	}

    private bool timerAirOff = true;
    IEnumerator timerAir()
    {
        timerAirOff = false;
        Vector3 lastHeight = transform.position;
        float t = 0.1f;
        //Debug.Log("Start");
		while(t>0f)
		{
			t -= Time.deltaTime;
			yield return null;
		}
        /*float dY = transform.position.y - lastHeight.y;*/
        //if (d > -0.1f && d < 0f) airGravity = true;
        float d = (lastHeight - transform.position).sqrMagnitude;
        //Debug.Log(d);
        if (/*dY < 0f && */d<0.001f) airGravity = true;
        timerAirOff = true;
    }

    // Для вспомагательной проверки обхода
    void OnCollisionStay(Collision col)
    {
        // Проверка на то, что объект сбоку, а не внизу
        // НЕОБХОДИМО ПРОВЕРИТЬ при неровной поверхности террейна, т.к. могут быть баги
        //Debug.DrawRay(legs.position,new Vector3(col.transform.position.x - legs.position.x,0,col.transform.position.z - legs.position.z)/*.normalized*1f*/,Color.black,1.5f);
        RaycastHit hit;
        float l = (col.transform.position-legs.position).magnitude;
        if(Physics.Raycast(legs.position,new Vector3(col.transform.position.x - legs.position.x,0,col.transform.position.z - legs.position.z),out hit,l) )
        {
            if (!hit.transform.GetComponent<Collider>().isTrigger&&hit.collider.tag=="Static"&&hit.transform.gameObject==col.gameObject)
            {
                obj = col.transform;
            }
        }
        //else Debug.Log(":(");
        
        //Debug.Log("Yappy!");
    }
    void OnCollisionExit(Collision col)
    {
        obj = null;
    }
}