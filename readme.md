# BACKUP FORTRESS
Backup Fortress es una aplicación diseñada para proteger datos sensibles, ofreciendo respaldo, control de versiones e integración con los contenedores de AWS. Su interfaz fácil de usar automatiza el respaldo, asegurando la seguridad de tus archivos. Destaca por el control de versiones detallado y la integración con AWS para una seguridad avanzada. Además, su arquitectura permite futuras expansiones. En resumen, brinda una solución completa y segura para la protección de datos críticos, ofreciendo tranquilidad contra pérdidas accidentales.

## <u>Objetivos marcados</u>
A continuación, se mostrarán las características que se han implementado en la 
aplicación, y las que se tenían previstas en un principio, y pueden ser implementadas 
más adelante.
* Realizados
  * Integración con AWS
  * Aplicación de escritorio 
  * Servicio de Windows
  * Archivo común de configuración
  * Gestión de versiones de cada archivo
  * Borrar o descargar las versiones individualmente
  * Compresión de las carpetas en un .zip
* Previstos para un futuro
  * Integración con el servicio de OVH u otros
  * Realizar cifrado de archivos al subir

## <u>Diseño</u>
La aplicación ha sido desarrollada con diferentes capas de abstracción, teniendo una 
librería en común entre la aplicación de servicio y la aplicación de Windows, el cual 
permite la escritura de archivos, compresión, trabajar con los servicios en la nube, etc.

![Spacenames](./Proyecto_final/Diagrama_spacenames.png "spacenames")

### <p style="text-align:center">Aplicación con interfaz de usuario</p>
La aplicación de Windows será la cara visible para el usuario, teniendo diferentes 
ventanas y funcionalidades los cuales se explicarán detalladamente más adelante.
Esta será la aplicación que brinda la posibilidad de acceder al archivo de configuración 
para modificarla, del cual depende el servicio de Windows. Dichas modificaciones 
incluyen por ejemplo establecer los tokens de acceso del usuario a los servicios de la 
nube, las carpetas a las que se les hace respaldo y cada cuanto tiempo, etc. 
Por supuesto se podría utilizar únicamente esta herramienta para realizar las copias 
de seguridad, sin depender del servicio de Windows, pero la aplicación en su conjunto 
perdería “la magia”.

### <p style="text-align:center">Aplicación sin interfaz (Servicio de Windows)</p>
Esta es una parte muy importante de la aplicación, ya que no queremos pasarnos la 
vida programando alarmas para realizar las copias de seguridad por nosotros mismos, 
de eso se va a encargar nuestro servicio, que según como hayamos configurado 
nuestra aplicación en la ventana visual, trabajará sobre los datos proporcionados allí, 
por tanto será necesario que indiquemos primero unas credenciales de acceso 
correctas para cada servicio de la nube del que disponemos, e indicar las carpetas a 
los que realizaremos las copias junto al resto de datos.

![visor_de_eventos](./Proyecto_final/capturas/visor_de_eventos.png "visor_de_eventos")

## <u>Planificación</u>
Las tecnologías usadas para crear la aplicación son las siguientes:
* Lenguaje C#
* .NET Framework 4.8
  * Servicio de Windows
  * Proyecto compartido
* .NET 6
  * Aplicación de escritorio (WPF)
* SDK AWS
* Git y Github
* Jira

## <u>Desarrollo</u>
### ***<p style="text-align:center">Proyecto compartido</p>***

En primer lugar, se ha creado el proyecto que contendrá la mayor parte de la lógica 
que se utilizará en toda la aplicación. 
Este proyecto no habría sido necesario en caso de crear únicamente una aplicación 
de escritorio o cualquier otra, pero como hemos dicho, se han creado 2 tipos diferentes 
de aplicaciones, uno de escritorio y otro un servicio de Windows, por ello, a falta de 
esta librería, se tendría que implementar la lógica en cada una de ellas por separado 
creando redundancia, que a la hora de mantenibilidad y escalabilidad supone un gran 
desafío y desperdicio de esfuerzo.

![configuracion](./Proyecto_final/capturas/estructura_config_json.png "configuracion")

Este proyecto contiene:
> - Archivo de configuración (JSON).
> - Librerías de acceso tanto de escritura como de lectura a dicho archivo.
> - Librería de compresión de carpetas en .zip
> - Librería de escritura de archivos (para cuando se comprime y se genera un nuevo archivo)
> - Clases y servicios que trabajan con la nube

### ***<p style="text-align:center">Aplicación de escritorio</p>***
* **Ventana principal**

![ventana_principal](./Proyecto_final/capturas/app_escritorio_princip.png "ventana_principal")

> - *Izquierda*: Se muestran los directorios y archivos locales
> - *Derecha*: Tras seleccionar el servicio en la nube, en este caso AWS, es el único 
que tenemos en este momento, nos muestra los contenedores en un 
desplegable, en este caso backupfortressbucket, veremos todos los archivos 
que contiene junto a sus versiones en la parte inferior, pudiendo descargar o 
borrar cualquier versión o la rama entera, mediante el submenú del click 
derecho.

* **Ventana para administrar cuentas**

![ventana_cuentas](./Proyecto_final/capturas/modificar_credenciales.png "ventana_cuentas")

En este caso, cada usuario, deberá introducir los tokens de acceso propios, que se 
obtienen en el apartado IAM de la página de AWS. Estos tokens deben pertenecer a 
un usuario que tenga los suficientes privilegios para acceder a S3, agregar y eliminar.
No se recomienda para nada utilizar el usuario root que te da AWS por defecto al 
registrarte.

> - *Probar conexión*: Permite comprobar si las credenciales introducidas son 
correctas y si se puede establecer la conexión
> - *Aceptar*: Prueba la conexión, y si esto es correcto, lo guarda en el archivo de 
configuración actualizando las credenciales antiguas o ingresándolos como 
nuevo.

* **Ventana para sincronizar carpetas**

![ventana_sincronizar](./Proyecto_final/capturas/sinc_carpetas.png "ventana_sincronizar")

La información que se ve en esta ventana es el mismo sobre el que trabajará el 
servicio de Windows, creando respaldos de las carpetas que aquí indiquemos con su 
frecuencia.

> - Local: Las carpetas a los que se les realiza la copia de seguridad automáticamente cada cierto tiempo. Únicamente podremos seleccionar carpetas para automatizar las copias de seguridad.
> - AWS / OVH: Los servicios de la nube donde se alojarán las copias, se podría marcar los 2 si así se desea, pero hay que asegurarse que todos los servicios que se marquen tengan un contenedor con el mismo nombre. (Esta versión todavía no cuenta con la compatibilidad con OVH, es únicamente un ejemplo de muestra).
> - Freq.: Cada cuanto tiempo se realizan las copias de seguridad, medido en horas.
> - Contenedor: Es necesario indicar el nombre del contenedor correctamente, ya que es donde se alojarán esas copias, y que todas las casillas marcadas de servicios de la nube, tengan ese contenedor ya creado previamente.

### ***<p style="text-align:center">Servicio de Windows</p>***

Los servicios son unos componentes bastante importantes y necesarios para los 
sistemas operativos, permiten automatizar procesos sin que el usuario esté pendiente 
de iniciarlo o pararlo, y para una aplicación que trata sobre realizar copias de 
seguridad a datos vitales de sus usuarios, no se podría evitar su implementación.

La manera de funcionar de nuestro servicio va a ser el siguiente:
>1. Se inicia automáticamente con el sistema
>1. Lee el archivo de configuración
>1. Obtiene tokens de acceso de cada servicio de nube y crea objetos que 
hagan conexión con ellos
>1. Obtiene la lista de archivos a los que hay que hacer copia de seguridad
>1. Establece un temporizador según la última copia realizada y la frecuencia 
establecida de cada una de ellas
>1. Al realizar una copia de seguridad y subirlo en su respectivo contenedor en 
la nube, modifica el archivo de configuración actualizando la fecha de la 
última copia realizada.
>1. Reconfigura el temporizador para la siguiente copia