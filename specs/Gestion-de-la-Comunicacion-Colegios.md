# { } Gestión de la Comunicación

**Versión:** 0.0.1  
**Caso de estudio:** Arquitectura Hexagonal - Sábados Tech - Arquitectura

| Fecha       | Descripción cambio | Versión documento |
|-------------|--------------------|-------------------|
| 09/03/2025  | Alcance inicial    | 1.0.0             |

---

## Introducción

Derivada de las discusiones técnicas en el grupo Sábados Tech, surge la iniciativa de desarrollar una aplicación experimental que implemente los principios de la Arquitectura Hexagonal. Este enfoque nos permite separar el núcleo del dominio de sus interfaces, facilitando la integración de nuevos requerimientos y mejorando la mantenibilidad del sistema. Hemos elegido como dominio la gestión de la comunicación en una institución educativa, abarcando flujos que incluyen desde notificaciones disciplinarias y educativas hasta comunicaciones generales, con el objetivo de optimizar la interacción entre alumnos, docentes y responsables familiares.

## Objetivo

El proyecto busca no solo implementar una solución técnica, sino también extraer aprendizajes que respalden la adopción de la Arquitectura Hexagonal en escenarios reales. Para ello, se plantean objetivos que abordan tanto aspectos prácticos como teóricos.

### Objetivos Prácticos

- **Facilidad de Adaptación y Escalabilidad:**  
  Evaluar cómo la arquitectura permite incorporar nuevos requerimientos y adaptarse a cambios.

- **Implementación Modular:**  
  Validar que la separación entre el núcleo del dominio y los adaptadores facilita la integración y el reemplazo de componentes, contribuyendo a mantener la cohesión y simplificar futuras ampliaciones o refactorizaciones.

- **Pruebas de Integración y Desempeño:**  
  Identificar y superar desafíos en la ejecución de pruebas unitarias e integradas que garanticen el correcto funcionamiento del sistema, favoreciendo la realización de pruebas independientes y en conjunto.

### Objetivos Teóricos

- **Aplicación de Principios de Diseño:**  
  Demostrar la correcta aplicación de conceptos clave como la inversión de dependencias y la separación de responsabilidades.

- **Identificación de Desafíos Conceptuales:**  
  Reflexionar sobre las limitaciones y dificultades inherentes a la implementación de la Arquitectura Hexagonal en un entorno real, documentando aprendizajes y proponiendo mejoras basadas en la experiencia práctica.

- **Contribución al Conocimiento Técnico:**  
  Documentar la experiencia obtenida para que sirva de referencia en futuras implementaciones, aportando valor a la comunidad técnica al evidenciar los beneficios y retos del enfoque adoptado.

## Requerimientos

### Requerimientos Funcionales

La aplicación debe dar soporte a la solicitud del usuario, según el documento "Los pedidos del colegio", abordando aspectos de distinta naturaleza:

1. **Adecuación a los cambios sociales:**  
   Adaptarse a la conformación familiar actual, manteniendo una comunicación fluida con los responsables de los alumnos.

2. **Envío de comunicaciones:**  
   Implementar el envío de distintas comunicaciones vinculadas a la función educativa de la institución:  
   - Comunicaciones administrativas  
   - Comunicaciones disciplinarias  
   - Comunicaciones educativas  
   - Comunicaciones generales

### Requerimientos No Funcionales

- Aplicar la Arquitectura Hexagonal.  
- Plataforma de desarrollo: se implementará en .NET y se portará a Java.

## Modelo de Casos de Uso

### Diagrama de Casos de Uso

![Diagrama de Casos de Uso - Gestión de la Comunicación](../specs/estion_de_la_comunicación.drawio.png)

En el diagrama se puede apreciar cómo cada actor interactúa con el sistema a través de distintos casos de uso:

- **Comunicador** (Docente, Preceptor, Administrativo)  
  - Enviar Comunicación Administrativa  
  - Enviar Comunicación Disciplinaria  
  - Enviar Comunicación Educativa  
  - Enviar Comunicación General

- **Responsable**  
  - Gestión Suscripción Comunicaciones Administrativas

### Detalle de Casos de Uso

#### Enviar Comunicación Disciplinaria

- **Actor(es):** Docente o coordinador disciplinario (Comunicador).  
- **Objetivo:** Notificar a todos los responsables del alumno cuando se registra un incidente disciplinario.  
- **Flujo Principal:**
  1. Seleccionar el alumno implicado.
  2. Registrar el detalle del incidente disciplinario.
  3. El sistema identifica y recopila todos los responsables vinculados al alumno.
  4. Se envía la comunicación disciplinaria a cada responsable.
  5. Se notifica al Comunicador que la operación se ha realizado exitosamente.  
- **Condición de éxito:** Se verifica el paso 5 como verdadero.

#### Enviar Comunicación Educativa de Curso

- **Actor(es):** Docente, coordinador de curso o administrador (Comunicador).  
- **Objetivo:** Difundir información educativa relevante a todo el curso.  
- **Flujo Principal:**
  1. Seleccionar el curso de interés.
  2. Redactar el mensaje con la información educativa pertinente.
  3. El sistema reúne a todos los responsables de los alumnos inscritos en el curso y al docente asignado.
  4. Se envía la comunicación a todos los destinatarios.
  5. Se notifica al Comunicador que la operación se ha realizado exitosamente.  
- **Condición de éxito:** Se verifica el paso 5 como verdadero.

#### Enviar Comunicación General

- **Actor(es):** Administrador o coordinador de comunicaciones (Comunicador).  
- **Objetivo:** Difundir información de interés general a toda la comunidad educativa, incluyendo responsables de todos los cursos y docentes.  
- **Flujo Principal:**
  1. El actor accede al módulo de comunicaciones generales.
  2. Redacta el mensaje con la información a compartir.
  3. El sistema recopila la lista completa de todos los responsables y docentes de la institución.
  4. Se envía la comunicación general a todos los destinatarios de forma simultánea.
  5. Se notifica al Comunicador que la operación se ha realizado exitosamente.  
- **Condición de éxito:** Se verifica el paso 5 como verdadero.

#### Gestión de Suscripción a Comunicaciones Administrativas

- **Actor(es):** Responsable familiar (padre, tutor, etc.) y administrador del sistema.  
- **Objetivo:** Permitir que cada responsable configure si desea o no recibir comunicaciones administrativas.  
- **Flujo Principal:**
  1. El responsable accede a la configuración de preferencias en el sistema.
  2. Selecciona o deselecciona la opción para recibir comunicaciones administrativas.
  3. El sistema actualiza y almacena la preferencia del responsable para futuros envíos.  
- **Flujo Alternativo:**
  - El Administrador habilita al Responsable, reanudando el flujo normal desde el paso 1.  
- **Condición de éxito:** El caso de uso finaliza sin errores.

---


