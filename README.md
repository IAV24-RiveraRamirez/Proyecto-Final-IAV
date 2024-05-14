# IAV - Proyecto Final

## Autores

- Miguel Ramírez Castrillo (Migram05)
- David Rivera Martínez (DavidRainder)

## Introducción y propuesta

Este proyecto es una práctica de la asignatura de Inteligencia Artificial para Videojuegos del Grado en Desarrollo de Videojuegos de la UCM, cuyo enunciado original se puede encontrar en el siguiente enlace: [Robot a la Fuga](https://narratech.com/es/inteligencia-artificial-para-videojuegos/decision/robot-a-la-fuga/).

La práctica basa su planteamiento alrededor del clásico relato de ciencia ficción "Pequeño Robot Perdido" publicado en 1947 por Isaac Asimov. En este texto nos encontramos con _Néstor_, uno de los muchos robots fabricado en un complejo militar. A pesar de ser físicamente idéntico a los demás robots de su modelo, él tiene una diferencia: han modificado una de sus leyes de la robótica y, después de verse ofendido por uno de los investigadores, decide escapar del complejo lo más rápido posible.

Deberemos programar la inteligencia artificial que permita a _Néstor_ huir del complejo militar. Para ello debemos usar Behaviour Bricks, una herramienta de Unity que permite diseñar comportamientos de entidades en un entorno. Los guardias del complejo también estarán tratadas mediante IA, solo que usando una máquina de estados finita (FSM) para su implementación y sus diferentes comportamientos.

Además, deberemos abordar el problema teniendo en cuenta que _Néstor_ no conozca el mapa del complejo ni la ruta más rápida y segura para salir, si no que debe ir viajando de una habitación a otra recabando información para tomar decisiones en función de la misma.

## Planteamiento del problema

Como se ha mencionado, deberemos desarrollar los comportamientos de dos tipos de entidades (_Néstor_ y _Guardias_) desde dos enfoques diferentes (Behaviour Trees y FTS) para dar con la solución a esta práctica.

Así, hemos dividido el trabajo requerido de la siguiente forma:

- Implementación de Zoom mediante input de teclado - Realizado por un integrante [***A***]
- Impedir que los guardias te persigan al salir de la sala donde tienen su base - Realizado por un integrante [***B***]
- Implementación del comportamiento actual de los _Guardias_ a través de máquinas de estado de los enemigos - Realizado por una pareja de integrantes del grupo [***C***]
- Desarrollo de una IA a través de Behaviour Trees capaz de sacar a _Néstor_ del laberinto teniendo en cuenta que conoce la información del mapa - Realizado por una pareja [***D***]
- Realización de cambios al mapa y al diseño de la IA de _Néstor_ para poder adaptarse de forma genérica a los diferentes mapas - Realizado por la misma pareja que haya realizado la IA de _Néstor_ [***E***]

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
|  Done  | Planificación inicial: Repartición de tareas |        1         |       1        | 11-04-2024 |
|  Done  | Zoom Cámara                                  |        1         |       1        | 11-04-2024 |
|  Done   | Máquina estados Enemigos                     |        10        |       14        | 27-04-2024 |
|  Done   | Comportamientos del Jugador                  |        10        |       18        | 28-04-2024 |

## Ampliaciones

Hemos conseguido hacer todo lo obligatorio de la práctica, no obstante, no hemos realizado ninguna ampliación.

## Pruebas y métricas

Para poner a prueba la práctica y demostrar el correcto funcionamiento de todo el contenido implementado se pueden hacer las siguientes pruebas en la escena _Test_ _Level_ y posteriormente en la escena _Level_ _1_:

- A: Para empezar, vamos a hacer _scroll_ con el ratón para comprobar que el zoom está hecho. Comprobar que está limitado, tanto al acercar como al alejar la cámara.
- B - 1: Entrar en la sala del botón azul. Meterse entre los robots y esperar a que el robot mire en tu dirección, para verificar que no detectan al jugador.
- C: Salir del escondite y colocarse en frente al robot, para comprobar que te detecta y que dispara hasta quedarse sin munición. Verificar que al quedarse sin munición, vuelve a su base a recargar. Comprobar que el jugador muere al quedarse sin vida. Tras morir, cargar la escena de _test_ otra vez y acercarse otra vez al robot, asegurarse de que te percibe y alejarse para comprobar si te persigue.
- B - 2: Pulsar el botón azul, pasar por la puerta azul y acercarse a la puerta blanca que está cerrada. Verificar que el robot, al mirar a la puerta, no te detectan.
- D y E: . Por último, completar el nivel o dejar que los robots derroten al jugador para cargar la escena _Level_ _1_. Para comprobar el funcionamiento de la pizarra, basta con esperar a que resuelva el nivel, ya que el jugador se moverá entre varios puntos del mapa almacenados en la pizarra para conocer que salas tiene que visitar. Verificar que el resultado es correcto, es decir, que Nestor llega a la salida.

## Conclusiones

Se puede probar la práctica en el directorio Build, lanzando el ejecutable "Liquid Snake.exe".
Todo lo propuesto ha sido llevado a cabo. Aun así, hemos tenido un problema, ya que las animaciones del jugador no funcionan correctamente en la _build_, pero en el editor de _Unity_ si.


## Documental con las pruebas y características

https://youtu.be/wEc0SvyqwH8

## Licencia

Alejandro Massó Martínez, Roi Quintas Diz, Miguel Ramírez Castrillo y David Rivera Martínez, autores de la documentación, código y recursos de este trabajo, concedemos permiso permanente a los profesores de la Facultad de Informática de la Universidad Complutense de Madrid para utilizar nuestro material, con sus comentarios y evaluaciones, con fines educativos o de investigación; ya sea para obtener datos agregados de forma anónima como para utilizarlo total o parcialmente reconociendo expresamente nuestra autoría.

Una vez superada con éxito la asignatura se prevee publicar todo en abierto (la documentación con licencia Creative Commons Attribution 4.0 International (CC BY 4.0) y el código con licencia GNU Lesser General Public License 3.0).

## Referencias

Los recursos de terceros utilizados son de uso público.

- _AI for Games_, Ian Millington.
- [Unity 2018 Artificial Intelligence Cookbook, Second Edition](https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition)
- [Unity Artificial Intelligence Programming, 5th Edition](https://github.com/PacktPublishing/Unity-Artificial-Intelligence-Programming-Fifth-Edition)
