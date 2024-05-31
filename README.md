# IAV - Proyecto Final

## Autores

- Miguel Ramírez Castrillo (Migram05)
- David Rivera Martínez (DavidRainder)

## Introducción y propuesta

Este proyecto se presenta como una práctica final para la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM.

La idea de la práctica es simular el comportamiento de varios personajes no jugadores en una aldea. Cada _PNJ_ ejecutará una serie de rutinas a lo largo del día: por la mañana hará el trabajo o tarea que tenga asignado, una vez terminada, tendrá el resto del día para dedicarse a sus tareas personales o de ocio y, al llegar la noche, todos los _PNJ_ se van a dormir y al día siguiente repetirán su rutina. 
Adicionalmente, queremos intentar añadir un sistema de eventos por el cual pueda ocurrir un evento prioritario en la aldea (una catástrofe u otro tipo de emergencia) que obligue a los _PNJ_ a abandonar su estado actual para poner fin a la emergencia.

La práctica se hará entre dos integrantes y consiste en programar un sistema de generación de terreno aleatorio usando un ruido de Perlin y programar una máquina de estados jerárquica para el comportamiento de los _PNJ_. Sobre el terreno creado se colocarán de manera coherente los edificios que componen la aldea y los _PNJ_, los cuales podrán navegar por los entornos generados para completar sus tareas personales. Los _PNJ_ serán omniscientes: cada personaje ya conocerá de antemano la disposición de la aldea y la posición de sus puntos de interés por lo que no necesitarán de un sistema de percepción para guiarse o tomar decisiones.

Cada alumno se centrará en uno de los dos pilares fundamentales de la práctica: generación aleatoria de la aldea y programación de la máquina de estados jerárquica y rutinas de los _PNJ_.

## Planteamiento del problema

El trabajo se ha dividido de la siguiente manera:

- Generación aleatoria del terreno usando el ruido de Perlin - Realizado por el estudiante A [***A***]
- Colocación de manera coherente de los edificios y puntos de interés de la aldea sobre el terreno generado - Realizado por el estudiante A [***B***]
- Implementación del comportamiento de los _PNJ_ a través de máquinas de estado jerárquicas - Realizado por el estudiante B [***C***]
- Programación de las rutinas de cada personaje pudiendo ser interrumpidas por eventos prioritarios - Realizado por el estudiante B [***D***]
- Implementación de eventos prioritarios o emergencias en la simulación - Realizado por los 2 estudiantes [***E***]

## Instalación y uso

En el repositorio están todos los archivos necesarios para ejecutar y probar la práctica.

Más adelante, se creará un ejecutable accesible desde el apartado de _releases_ del repositiorio de _GitHub_ con la última _build_ generada y con todos los recursos del proyecto. Solo se deberá corregir la _build_ marcada como la más reciente.

## Punto de partida
El proyecto de partida será un proyecto vacío.

## Diseño de la solución

### Generación procedural de contenido
Para la generación procedural de contenido usaremos un mapa de ruido de Perlin con el que luego modificaremos el terreno. 

- **Clase _Perlin Noise_**, que se encarga de asignarle un ruido de perlin a una textura a partir de unos parámetros

```
class PerlinNoise:
    #Tamaño de la textura que queremos crear
    width : int
    height : int

    #Escala de la textura 
    scale : float

    #Desplazamiento de la textura, estos parámetros podrían usarse como semilla de generación
    offsetX : float
    offsetY : float

    #Componente Renderer de Unity
    renderer : Renderer

    fuction Start():
        #Esto se podría hacer para que la generación fuese diferente en cada ejecución
        #Se puede quitar para hacer que la generación sea siempre la misma dados los offset
        offsetX = Random(0, 1000)
        offsetY = Random(0, 1000)

        renderer = GetComponent<Renderer>()
        renderer.material.mainTexture = GenerateTexture()

    #Función que devuelve el mapa de Perlin
    function GenerateTexture() -> Texture2D:
        texture: Texture2D = new Texture(width, height)
        for x in 0..(width)
            for y in 0..(height)
                color: Color = CalculateColor(x, y)
                texture.setPixel(x, y, color) 
        
        #función de Unity que guarda los cambios hechos a una textura
        texture.Apply()
        return texture

    #función que asigna el color de cada pixel según el mapa de Perlin
    function CalculateColor(x: int, y: int) -> Color:

        xCoord: float
        yCoord: float

        #Transformamos las coordenadas en valores entre 0 y 1
        xCoord = x / width * scale + offsetX
        yCoord = y / height * scale + offsetY

        #Consultamos el mapa de Perlin generado en un pixel concreto
        sample: pixel
        pixel = PerlinNoiseGenerator.GetPixel(xCoord,yCoord)
        color: Color
        color = new Color(pixel, pixel, pixel)
        return color

```

- **Clase _Perlin Noise Generator_**, se encarga de crear el mapa de altura que luego se puede consultar para crear la textura

```
class PerinNoiseGenrator:
    octaves: PerlinOctave[]
    weights: float[]

    function PerlinNoiseGenerator(weights: float[]):
        this.weights = weights

        #Creamos octavas aleatoriamente de manera que cada octava es el doble de su anterior
        size = 1
        for _ in weights:
            octaves.push(PerlinOctave(size))
            size *= 2
    
    function getPixel(x:float, y: float) ->float:
        result = 0
        for i in 0..octaves.length():
            weight = weights[i]
            height = octaves[i].get(x,y)
            result += weight * height
        return result

```

- **Clase _PerlinOctave_**, representan las cuadrículas que componen el terreno

```
class PerlinOctave:
    gradient: float[][][2]
    size: int

    function PerlinOctave(size: int):
        this.size = size

        #Crea una malla de vectores unitarios aleatorios en cada esquina de la octava (llamados gradientes)
        gradient = float[size+1][size+1][2]
        for x in 0..(size + 1):
            for y in 0..(size + 1):
                gradient[x][y][0] = randomRange(-1,1)
                gradient[x][y][1] = randomRange(-1,1) 

    #Calculamos la altura para cada punto del terreno
    #Dicha altura dependerá de los 4 gradientes que estén en las coordenadas (ix, iy) de las esquinas de su celda
    function scaledHeight(ix: int, iy: int, x: float, y: float) -> float:
        dx: float = x - ix
        dy: float = y - iy

        return dx * gradient[ix][iy][0] + dy * gradient[ix][iy][1]

    #Para el cálculo de la altura se hace una interpolación entre los valores de las 4 esquinas de la octava
    function get(x: float, y: float) -> float:
        ix = int(x / size)
        iy = int(y / size)
        px = x - ix
        py = y - iy

        #Interpolación de valores
        tl = scaledHeight(ix, iy, x, y)
        tr = scaledHeight(ix + 1, iy, x, y)
        t = lerp(tl, tr, px)
        bl = scaledHeight(ix, iy + 1, x, y)
        br = scaledHeight(ix + 1, iy + 1, x, y)
        b = lerp(bl, br, px)
        return lerp(t, b, py)

```

### Comportamiento NPCs

Para la implementación del comportamientos de los NPCs utilizaremos Máquinas de Estado Finitas Jerárquicas (HFMS). Este tipo de máquinas de estados nos permitirá gestionar diferentes estados que pueden ser a su vez otras máquinas de estado. Es bastante útil para que los NPCs tengan partes claramente diferenciadas en su rutina: Trabajo, Ocio y Dormir. Estas pueden ser cuatro estados que son, a su vez, otras máquinas de estado con sus diferentes comportamientos y sub-estados, entre los que se cambiará por eventos sucededidos dentro de ese sub-estado. Los *estados padre* tendrán transiciones según los eventos del mundo, como la hora del día.

El pseudocódigo que nos sirve como base para la HFMS es el siguiente:

- **State** - Comportamiento a ejecutar. Cuenta con una serie de transiciones que comprueba constantemente
```
# Es una clase abstracta, pues sus hijos deben determinar qué sucede durante el estado
abstract class State:

    # Variables para settear el entorno donde se ejecuta este estado.
    gameObject : GameObject # Usado para acceder a funciones propias del objeto 
    fsm : HFSM # Usado para acceder a la Blackboard 

    function Init(_gO : GameObject, _fsm : HFSM):
        gameObject = _gO
        fsm = _fsm
        
    # Tiene funciones que se ejecutarán en su entrada (Enter), en su salida (Exit) y durante su tiempo de vida (Update)
    abstract function Enter()
    abstract function Exit()
    abstract function Update(dt : float)
```

- **Transition** - Condición a cumplir para que se pase del estado actual a un estado objetivo
```
abstract class Transition:
    # Estado al que la transición debe saltar en caso de cumplirse su condición
    nextState: State

    # Variables de contexto
    fsm : HFSM
    gameObject : GameObject

    # Constructora
    Transicion(_nextState: State) -> Transition:
        nextState = _nextState

    # Proporciona el contexto
    function Init(_gO : GameObject, _fsm : HFSM):
        gameObject = _gO
        fsm = _fsm

    # Función para devolver el estado al que la transición debe saltar. Virtual por si se quisiera cambiar, pero no debería hacer falta
    function virtual NextState() -> State:
        return nextState

    # Función abstracta que debe comprobar una condición concreta que definirá si se pasa o no de estado
    abstract function Check() -> bool
    
    # Funciones abstractas que definen qué pasa al entrar (Enter) y salir (Exit) de la transición
    abstract function Enter()
    abstrac function Exit()
```

- **StateMachine** - Máquina de estados que ejecuta una serie de estados dados.
```
# Un tipo de hijo especial de la clase State, que permite ejecutar una serie de estados guardados en lugar de solamente 1
class StateMachine overrides State:
    #Es una clase que permite estar en un único estado a la vez
    # Gracias a que State es una clase abstracta, esto también nos permite meter StateMachines como estados dentro de esta máquina de estados.
    current : State
    # Guardamos una referencia a la máquina de estados que está ejecutando esta. Nulo en caso de que sea la más alta en la jerarquía
    parent : StateMachine

    gameObject : GameObject
    blackboard : Blackboard

    fsmStructure : Dictionary<State, Transition[]>

    # Vacía, necesaria para Override
    functioon Enter()
    # Vacía, neceseria para Override
    function Exit()

    function SetContext(object : GameObject, _parent : StateMachine):
        gameObject = object
        parent = _parent

    function StartMachine(initial : State):
        current = initial
        current.Enter()

        foreach(Transition t in fsmStructure[current]):
            t.Enter()

    function Update(dt : float):
        # Llama a la actualización del comportamiento actual
        current.Update()
        #Comprueba las transiciones y cambia de estado en caso de que se cumpla alguna
        bool changeState = false;
        foreach(Transition t in fsmStructure[current]):
            changeState = ChangeState(t)

    function ChangeState(t : Transition):
        changeState : bool = t.Check()
        if(changeState):
            t.Exit()
            current.Exit()
            current = t.NextState()
            current.Enter()
            foreach(Transition t in fsmStructure[current]):
                t.Enter()
        return changeState

    function AddState(State s, Transition[] transitions):
        s.Init(gameObject, this)
        foreach(Transition t in transitions):
                t.Init(gameObject, this)
        fsmStructure.Add(s. transitions)
```

Gracias a esta implementación podremos tener una máquina de estados 'padre', con diferentes estados que pueden ser a su vez otra máquina de estados, lo que permite una abstracción interesante de cada estado y una mayor modularidad. A continuación un ejemplo:

<image src="gdd_assets/HFSM_Example.png"/>

## Producción

Asumiendo que cada punto equivale a 30 minutos:

| Estado | Tarea                                        | Puntos estimados | Puntos finales |   Fecha    |
| :----: | :------------------------------------------- | :--------------: | :------------: | :--------: |
|   -    | Generación de terreno mediante Ruido Perlin  |        20        |       -        | ---------- |
|   -    | Colocación de Aldea en terreno               |        30        |       -        | ---------- |
|   -    | Comportamiento básico de NPCs                |        33        |       -        | ---------- |

[Link](https://github.com/orgs/IAV24-RiveraRamirez/projects/1/views/1) al proyecto de GitHub con información más detallada y ampliada

## Ampliaciones

En caso de disponer del tiempo suficiente, nos gustaría poder implementar las siguientes ampliaciones al proyecto, para dejar una experiencia más cerrada, en la que se explora más de un tipo de comportamiento:

- **Especializaciones de NPC** (Realizada): Un NPC podría no tener un trabajo genérico, sino un trabajo específico que aporte algo a la aldea. Algunos ejemplos: Granjero, Herrero, Ganadero, Comerciante, etc. Esto afectaría tremendamente a la hora de generación procedural de la aldea, ya que la colocación de edificios debería poder adaptarse al número de trabajadores de cada tipo, colocando los lugares de trabajo necesarios en lugar de un lugar de trabajo genérico. También, obviamente, afectaría al comportamiento de los NPCs

Se han implementado: Trabajador en Aserredro, Carpintero, Mercader de Compra de recursos.

- **Eventos de Peligro** (No implementado): Un evento de consecuencias catastróficas puede tener lugar en la aldea. Para ello, los ciudadanos dejarían de lado su ciclo de actuación convencial para desviarse y conseguir para esta amenaza. Algunos ejemplos: Incendio, Ventisca, Ataque enemigo, etc. La máquina de estados de los NPCs cambiaría drásticamente para poder reflejar el impacto de este evento en el mundo.


## Pruebas y métricas

Para poner a prueba la práctica y demostrar el correcto funcionamiento de todo el contenido implementado se pueden hacer las siguientes pruebas en la escena _Test_ _Level_ y posteriormente en la escena _Level_ _1_:

- A: Probar que el cambio de valores desde el menú afecta directamente a la generación de la aldea y del terreno.
- B: Comprobar el comportamiento de los diferentes NPCs con los edificios en los que están

## Conclusiones



## Documental con las pruebas y características



## Licencia



## Referencias

Los recursos de terceros utilizados son de uso público.

- Obstáculos del entorno: [Link](https://assetstore.unity.com/packages/3d/environments/fantasy/fantasy-forest-set-free-70568)

- _AI for Games_, Ian Millington.
