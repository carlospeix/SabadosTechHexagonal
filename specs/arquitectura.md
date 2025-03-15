# Conceptos de arquitectura

## Sobre la persistencia y estado del modelo

### Usamos Entity Framework code-first reduciendo al mínimo el acoplamiento entre modelo y persistencia

El unico elemento de persistencia presente en el modelo es el identificador (usualmente propiedad Id, public get, private set)

### Protección de estado e implementación del modelo (implementarion hidein)

#### Las colecciones no se modifican desde fuera del modelo que las contiene

```csharp
  public IReadOnlyCollection<Subject> Subjects => subjects.ToList().AsReadOnly();
  private readonly HashSet<Subject> subjects = [];

  public Subject AddSubject(Teacher teacher, string subjectName)
  {
    var subject = new Subject(this, subjectName, teacher);
    subjects.Add(subject);
    return subject;
  }
```

#### Usamos constructores parametrizados con todos los parametros necesarios y un constructor default privado para EF

```csharp
  private Grade() {}

  public Grade(string name)
  {
    Name = name;
  }
```

## Otros conceptos conversados sobre arquitectura, diseño y escalabilidad.

Queremos implementar APIs rest, gRPC, SignalR, WebSockets, etc.?

Donde ponemos la lógica de negocios? Servicios? En mi experiencia permite escalar mejor dijo alguien.

Usamos Repository o no? Dejamos la persistencia dentro del Hexágono?

Queremos desacoplar el modelo de la persistencia y usar servicios para la lógica (no implementar lógica en el dominio), o queremos que el modelo de dominio tenga la lógica?

Los queries van en los repositorios y en el dominio uso Spacifications.

Vamos por CQRS o no? Vamos a soportar GRPC o no? por qué?

Vamos por un modelo multi-tenant o no? De que tipo? base única o bases separadas por tenant?

Que arquitectura vamos a usar? (Microservicios, monolito, monolito modular, micro front-end, containers)

Para escalar, no usar transacciones y un diseño con acciones compensatorias (por audio)

Datos de volumetría:
- Cuántos colegios debe soportar? 1.000
- Cuántos alumnos y familias por colegio? 2.000
- Cuántos cambios a la estructura de familias por día, por colegio? Picos de 100 cambios por día?
- Cuántas notificaciones de cada tipo por día, por colegio? Una administrativa general, 20 disciplinarias, 20 administrativas individuales, una mensual con la facturación (todo por día, salvo la mensual)

El código que escribamos tiene que cumplir las leyes del código simple, en order de prioridad:
- La menor cantidad de elementos posible
- Revela la itención y no hay duplicación
- Pasa las pruebas

Mas criterios:
- Debe funcionar
- Debe ser fácil de leer
- Debe ser fácil de mantener
- Debe ser fácil de cambiar
- Debe ser fácil de probar
