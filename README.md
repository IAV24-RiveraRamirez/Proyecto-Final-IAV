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

### Enemigos
Para el tratamiento de los enemigos se ha utlizado una _Máquina de Estados Finita_ (FSM), particularmente una FSM que usa _Herencia_ para definir los diferentes estados y transiciones entre los mismos. El pseudocódigo de las diferentes clases es el siguiente:

- **Máquina de Estados Finita por Herencia - (HSFM)**, que gestiona los diferentes estados y transiciones

```
class HFSM:
    #Es una clase que permite estar en un único estado a la vez
    #Para más información sobre la clase estado, mirar más abajo
    current : State

    gameObject : GameObject
    blackboard : Blackboard

    fsmStructure : Dictionary<State, Transition[]>

    function SetContext(object : GameObject):
        gameObject = object

    function StartMachine(initial : State):
        current = initial
        current.Enter()

        foreach(Transition t in fsmStructure[current]):
            t.Enter()
        

    function Update(dt : float):
        # Actualiza el estado actual
        current.Update(dt)
        #Comprueba las transiciones del estado actual y cambia de estado en caso de que se cumpla alguna
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
