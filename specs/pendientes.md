# Temas pendientes para revisar

## Refactorings

Mantengamos acá una lista de los refáctorings pendientes para que cualquier pueda hacerlos. Todas las propuestas sobn tentativas, para debatirlas.

  - Renombrar el port **INotificationSender** a **INotificator**, creo que el sufijo sender no es necesario. Me parece natural decir que la Secretary, para responder a un pedido de notificación, colabora con el IRegistrar y con un INotificator

  - La firma del método Send del Notificator podría cambiar de
```csharp
  public void Send(Recipient recipient, string message);
```
  a 
```csharp
  public void Send(Recipient[] recipient, string message);
  
  o

  public void Send(IEnumerable<Recipient>, string message);
```

  - Quizás podriamos mover los ports IRegistrar e INotificationSender (o INotificator si lo cambiamos) a una carpeta (y namespace) /Ports, para clarificar.

  - La clase Recipient podría convertirse a un record

  - Ocultamos el Id de la clase Subject? No se usa más alla de la persistencia, nadie debería referenciar directo a un Subject ya que es parte de un aggregate. De hecho lo encontré porque el coverage lo marca como no utilizado. Para ocultarlo habría que hacer esto:

```csharp
  // En ApplicationContext
  modelBuilder.Entity<Subject>(x =>
  {
    x.ToTable("Subjects");
    x.HasKey("id");
    x.Property("id").UseIdentityColumn();
    ...
  });
```
```csharp
  // En la clase Subject
  protected int? id;
```

## Agregados

  - Podríamos agregar una fachada (en el proyecto Application u otro) que interacture con una eventual API. En esa fachada estaría todo el "cableado" de las dependencias, etc. Esa sería la "protection layer" o *facade*. Incluso podrían ser varias. En este modelo quizás una para notificaciones y otra para mantenimiendo de la base de datos de Students y Parents y también de Grades, Teachers, Subjects, etc.
  

## Temas para la próxima reunión

  - Persistencia dentro o fuera del hexágono (InMemory, SqlServer, etc.)
  - Refactorings pendientes
  - Hacemos una API? App?
  - Hacemos pruebas de carga?
  - Multitenancy
  - Transacciones (UnitOfWork) vs acciones compensatorias.
