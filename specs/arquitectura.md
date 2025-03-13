Cosas que mencionamos sobre arquitectura, dise�o y escalabilidad.

Queremos implementar APIs rest, gRPC, SignalR, WebSockets, etc.?

Donde ponemos la logica de negocios? Servicios? En mi experiencia permite escalar mejor.

Usamos Repository o no? Dejamos la persistencia dentro del Hex�gono?

Queremos desacoplar el modelo de la persistencia y usar servicios para la l�gica (no implementar l�gica en el dominio), o queremos que el modelo de dominio tenga la l�gica?

Los queries van en los repositorios y en el dominio uso Spacifications.

Vamos por CQRS o no? Vamos a soportar GRPC o no? por qu�?

Vamos por un modelo multi-tenant o no? De que tipo?base �nica o bases separadas por tenant?

Que arquitectura vamos a usar? (Microservicios, monolito, monolito modular, micro front-end, containers)

Para escalar, no usar transacciones y un dise�o con acciones compensatorias (por audio)

Datos de volumetr�a:
- Cu�ntos colegios debe soportar? 1.000
- Cuantos alumnos y familias por colegio? 2.000
- Cu�ntos cambios a la estructura de familias por d�a, por colegio? Picos de 100 cambios por d�a?
- Cu�ntas notificaciones de cada tipo por d�a, por colegio? Una administrativa general, 20 disciplinarias, 20 administrativas individuales, una mensual con la facturaci�n (todo por d�a, salvo la mensual)

El c�digo que escribamos tiene que cumplir las leyes del c�digo simple, en order de prioridad:
- La menor cantidad de elementos posible
- Revela la itenci�n y no hay duplicaci�n
- Pasa las pruebas

Mas criterios:
- Debe funcionar
- Debe ser f�cil de leer
- Debe ser f�cil de mantener
- Debe ser f�cil de cambiar
- Debe ser f�cil de probar
