# { } Historias de usuario
---

## Términos y conceptos
- Parent: Adulta/o responsable por el Student
- Student: Menor que cursa estudios en el colegio
- Grade: Un determinado año de estudio para un grupo de Students (por ejemplo, 3ro A o 5to)
- Subject: Cada una de las materias de un Grade, siempre a cargo de un Teacher
- Teacher: El profesor a cargo de esa manteria en ese curso. Puede ser Teacher de mas de una materia
- Recipient: Cada uno de los destinatarios a los que se envía una notificfación
- Notification: Un mensaje que se envía a uno o más destinatarios
- Secretary: La persona o área que asiste con las tareas de comunicación
- Registrar: Oficina del colegio a cargo de la base de datos de Parents, Students, Grades, Subjects, Teachers, etc

## Caso de uso

### Enviar Comunicación General

- **Actor(es):** Administrador o coordinador de comunicaciones (Comunicador).  
- **Objetivo:** Difundir información de interés general a toda la comunidad educativa, incluyendo responsables de todos los cursos y docentes.  
- **Flujo Principal:**
  1. El actor accede al módulo de comunicaciones generales.
  2. Redacta el mensaje con la información a compartir.
  3. El sistema recopila la lista completa de todos los responsables y docentes de la institución.
  4. Se envía la comunicación general a todos los destinatarios de forma simultánea.
  5. Se notifica al Comunicador que la operación se ha realizado exitosamente.  
- **Condición de éxito:** Se verifica el paso 5 como verdadero.

### Historias de usuario (priorizadas por el criterio de complejidad creciente)

Como administrador del colegio, deseo enviar un mensaje de inicio del año a toda la comunidad educativa.
  X El administrador envía un mensaje con una base de datos vacía, salen cero mensajes.
  X Enviamos una notificación cuando solo hay un Teacher, manda una sola notificación
  X Enviamos una notificación cuando hay un Teacher y un Parent, manda dos notificaciones
  X Si la notificación está vacía, reportamos un error

Como administrador del colegio, deseo enviar un una notificación a los Parents y Teachers de un Grade en particular.
  X Envío la notificación a los teachers del grade indicado
  - Si el grade no existe, reportamos un error
  - Envío la notificación a los teachers y parents del grade indicado

Como administrador del colegio, deseo enviar un una notificación disciplinaria a los Parents y Teachers de un Student en particular.
  - Si hay más de un student, el mensaje solo se envía a los teachers y parents del student indicado
  - Si el student no existe, reportamos un error

Como administrador del colegio, deseo enviar un una notificación administrativa a los Parents de un Student en particular.
  - Si hay más de un student, el mensaje solo se envía a los parents del student indicado
  - Si hay más de un parent para el student, el mensaje solo se envía al parent designado para contacto administrativo
  - Si el student no existe, reportamos un error

Como administrador del colegio, deseo enviar un una notificación para una fecha y hora futura.
  - Si envío una notificación futura, no se envía inmediatamente
  - Si envío una notificación futura, se envía en el momento indicado, con una tolerancia de 10 minutos.
  - Si el momento no es en el futuro, reportamos un error.

Como administrador del colegio, en la medida de lo posible, quiero saber si las notificaciones llegan y son leidas.
  - Si envío una notificación y el destinatario no la ha leído aún, aparece como no recibida/leída
  - Si envío una notificación y el destinatario la ha leído, aparece como recibida/leída


Pending refactorings
  X Global vs General notifications
  X Boolean return value de secretary.SendNotification
  X Reusing GradeRecipients in secretary.Recipients
  X ApiTests
  - Domain exceptions

  - Reusing same notificationRequest object or not?
  - Registrar, persistence, etc.
  - Separate endpoint configuration
