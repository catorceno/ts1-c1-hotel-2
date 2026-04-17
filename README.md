# Sistema de Gestión de Estadías de un Hotel

## 1. Alcance
El alcance del sistema está enfocado en el flujo principal de estadías en un hotel pequeño, que enfrenta varios problemas en su sistema actual como el solapamiento de las reservas e inconsistencia en los estados. El flujo abarca desde el registro de la estadía, asegurando la disponibilidad de la habitación y la correcta captura de datos del huésped, hasta la ejecución del check-in y el check-out asegurando la coherencia de los estados. El proceso finaliza con la persistencia del cálculo del costo total a pagar basado en la duración de la estadía y el tipo de habitación.

## 2. Historias de Usuario

### HU-01 &mdash; Registrar estadía
Como recepcionista, quiero registrar una nueva estadía asignando una habitación disponible y los datos del huésped, para formalizar la reserva en el sistema.

**Criterios de aceptación:**

**Escenario 1 — Registro exitoso**
- **Dado** que se ha seleccionado un rango de fechas válido (inicio anterior a fin) y una habitación disponible en ese rango, y se han completado los datos obligatorios del huésped principal (nombre y CI)
- **Cuando** el recepcionista confirma la estadía
- **Entonces** el sistema crea la estadía con estado *Pendiente*, registra el precio por noche correspondiente al tipo de habitación y asocia al huésped

**Escenario 2 — Habitación no disponible**
- **Dado** que la habitación seleccionada ya tiene una reserva que se solapa con el rango de fechas ingresado
- **Cuando** el recepcionista intenta confirmar la estadía
- **Entonces** el sistema rechaza la operación con el mensaje "Habitación ocupada"

**Escenario 3 — Fechas inválidas**
- **Dado** que la fecha de inicio es igual o posterior a la fecha de fin
- **Cuando** el recepcionista intenta confirmar la estadía
- **Entonces** el sistema rechaza la operación con el mensaje "La fecha de inicio debe ser anterior a la fecha fin"

---

### HU-02 &mdash; Registrar check-in
Como recepcionista, quiero registrar el check-in de una estadía, para activarla cuando el huésped se presenta en el hotel.

**Criterios de aceptación:**

**Escenario 1 — Check-in exitoso**
- **Dado** que la estadía se encuentra en estado *Pendiente* y la fecha actual está dentro del rango reservado (inicio ≤ hoy < fin)
- **Cuando** el recepcionista ejecuta el check-in
- **Entonces** el sistema cambia el estado a *Activa* y registra la fecha y hora exacta del check-in

**Escenario 2 — Estado incorrecto**
- **Dado** que la estadía no se encuentra en estado *Pendiente*
- **Cuando** el recepcionista intenta ejecutar el check-in
- **Entonces** el sistema rechaza la operación con el mensaje "El huésped no puede hacer check-in"

**Escenario 3 — Fuera del rango de fechas**
- **Dado** que la estadía está en estado *Pendiente* pero la fecha actual está fuera del rango reservado
- **Cuando** el recepcionista intenta ejecutar el check-in
- **Entonces** el sistema rechaza la operación con el mensaje "No se puede hacer check-in fuera del rango reservado"

---

### HU-03 &mdash; Registrar check-out
Como recepcionista, quiero registrar el check-out de una estadía, para finalizarla cuando el huésped abandona el hotel.

**Criterios de aceptación:**

**Escenario 1 — Check-out exitoso**
- **Dado** que la estadía se encuentra en estado *Activa* y la fecha actual está dentro del rango reservado (inicio ≤ hoy ≤ fin)
- **Cuando** el recepcionista ejecuta el check-out
- **Entonces** el sistema cambia el estado a *Finalizada*, registra la fecha y hora exacta del check-out y persiste el costo total calculado

**Escenario 2 — Estado incorrecto**
- **Dado** que la estadía no se encuentra en estado *Activa*
- **Cuando** el recepcionista intenta ejecutar el check-out
- **Entonces** el sistema rechaza la operación con el mensaje "El huésped no puede hacer check-out"

**Escenario 3 — Fuera del rango de fechas**
- **Dado** que la estadía está en estado *Activa* pero la fecha actual está fuera del rango reservado
- **Cuando** el recepcionista intenta ejecutar el check-out
- **Entonces** el sistema rechaza la operación con el mensaje "No se puede hacer check-out fuera del rango reservado"

---

### HU-04 &mdash; Consultar estadías
Como recepcionista, quiero consultar el listado de todas las estadías del hotel con sus datos principales, para tener visibilidad del estado operativo actual.

**Criterios de aceptación:**

**Escenario 1 — Listado completo**
- **Dado** que existen estadías registradas en el sistema
- **Cuando** el recepcionista accede al panel de estadías
- **Entonces** el sistema muestra todas las estadías con: ID, nombre del huésped principal, número y tipo de habitación, fecha de inicio, fecha de fin, estado y costo total

**Escenario 2 — Filtrado por estado**
- **Dado** que el recepcionista selecciona un estado del filtro (Pendiente, Activa o Finalizada)
- **Cuando** aplica el filtro
- **Entonces** el sistema muestra únicamente las estadías que coinciden con ese estado

**Escenario 3 — Sin estadías registradas**
- **Dado** que no existen estadías en el sistema o ninguna coincide con el filtro aplicado
- **Cuando** el recepcionista accede al panel
- **Entonces** el sistema muestra el mensaje "Sin estadías"

---

### HU-05 &mdash; Consultar disponibilidad de habitaciones
Como recepcionista, quiero consultar qué habitaciones están disponibles para un rango de fechas dado, para ofrecer opciones al huésped antes de registrar la estadía.

**Criterios de aceptación:**

**Escenario 1 — Habitaciones encontradas**
- **Dado** que se ingresa un rango de fechas válido (inicio anterior a fin)
- **Cuando** el recepcionista ejecuta la búsqueda
- **Entonces** el sistema muestra las habitaciones disponibles agrupadas por tipo, indicando cuántas hay de cada tipo y permitiendo seleccionar el número de habitación

**Escenario 2 — Sin disponibilidad**
- **Dado** que todas las habitaciones tienen reservas que se solapan con el rango ingresado
- **Cuando** el recepcionista ejecuta la búsqueda
- **Entonces** el sistema muestra el mensaje "No hay habitaciones disponibles en ese período"

**Escenario 3 — Fechas inválidas o incompletas**
- **Dado** que el recepcionista no ha ingresado ambas fechas o la fecha de inicio no es anterior a la fecha de fin
- **Cuando** intenta ejecutar la búsqueda
- **Entonces** el sistema muestra un mensaje de validación sin realizar la consulta

---

### HU-06 &mdash; Calcular costo total de la estadía
Como recepcionista, quiero que el sistema calcule automáticamente el costo total de la estadía al momento del check-out, para cobrar al huésped el monto correcto sin cálculos manuales.

**Criterios de aceptación:**

**Escenario 1 — Cálculo correcto al finalizar**
- **Dado** que una estadía en estado *Activa* tiene registrado el precio por noche de su tipo de habitación y un rango de fechas válido
- **Cuando** el recepcionista ejecuta el check-out
- **Entonces** el sistema calcula el total como `precio por noche × cantidad de noches` y lo persiste en el registro de pago de la estadía

**Escenario 2 — Total visible en el listado**
- **Dado** que una estadía se encuentra en estado *Finalizada* y tiene un total calculado
- **Cuando** el recepcionista consulta el listado de estadías
- **Entonces** el sistema muestra el costo total en la columna "Total" de esa estadía

**Escenario 3 — Total pendiente en estadías no finalizadas**
- **Dado** que una estadía se encuentra en estado *Pendiente* o *Activa*
- **Cuando** el recepcionista consulta el listado de estadías
- **Entonces** el sistema muestra "—" en la columna "Total", indicando que el monto aún no fue calculado

---

## 3. Diagrama Arquitectura: Stack Tecnológico
![diagrama-stack-tecnologico](./diagrama-stack-tecnologico.png)

## 4. Diagrama de Base de Datos
![diagrama-base-de-datos](./diagrama-db.drawio.png)
